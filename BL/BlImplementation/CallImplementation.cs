using BlApi;
using BO;
using Helpers;
using System.Linq.Expressions;

namespace BlImplementation
{
    internal class CallImplementation : BlApi.ICall
    {
        private readonly DalApi.IDal _dal = DalApi.Factory.Get;

        #region Stage 5

        public void AddObserver(Action listObserver) =>
          CallManager.Observers.AddListObserver(listObserver); //stage 5
        public void AddObserver(int id, Action observer) =>
          CallManager.Observers.AddObserver(id, observer); //stage 5
        public void RemoveObserver(Action listObserver) =>
          CallManager.Observers.RemoveListObserver(listObserver); //stage 5
        public void RemoveObserver(int id, Action observer) =>
          CallManager.Observers.RemoveObserver(id, observer); //stage 5

        #endregion Stage 5


        /// <summary>
        /// Adds a new call to the system.
        /// </summary>
        /// <param name="call">The call object to add.</param>
        /// <exception cref="BlInvalidDateOrderException">Thrown when the maximum completion time is less than or equal to the opening time.</exception>
        /// <exception cref="BlInvalidAddressException">Thrown when the address is null, empty, or invalid.</exception>
        /// <exception cref="BlOperationException">Thrown when the call cannot be added due to an internal error.</exception>
        public void AddCall(Call call)
        {
            AdminManager.ThrowOnSimulatorIsRunning(); //stage 7
                                                      // Validate the maximum completion time is greater than the opening time
            if (call.MaxCompletionTime.HasValue && call.MaxCompletionTime <= call.OpenedAt)
            {
                throw new BlInvalidDateOrderException("Maximum completion time must be greater than the opening time.");
            }

            // Validate the address
            if (string.IsNullOrWhiteSpace(call.FullAddress))
            {
                throw new BlInvalidAddressException("Address cannot be null or empty.");
            }

            // Convert BO.Call to DO.Call
            var doCall = CallManager.ConvertBOCallToDOCall(call);

            try
            {
                // Attempt to add the call in the data layer
                lock (AdminManager.BlMutex) //stage 7
                    _dal.Call.Create(doCall);
                CallManager.Observers.NotifyListUpdated();
            }
            catch (Exception ex)
            {
                // Handle any exceptions that may occur during the creation process
                throw new BlOperationException("Failed to add the call.", ex);
            }

            // Compute the coordinates asynchronously without waiting for the results
            _ = CallManager.UpdateCoordinatesForCallAsync(doCall);
        }


        /// <summary>
        /// Assigns a call to a volunteer for handling.
        /// </summary>
        /// <param name="volunteerId">The identifier of the volunteer.</param>
        /// <param name="callId">The identifier of the call.</param>
        /// <exception cref="BlOperationException">Thrown when the call cannot be assigned due to invalid status or expiration.</exception>
        public void AssignCallToVolunteer(int volunteerId, int callId)
        {
            //AdminManager.ThrowOnSimulatorIsRunning(); //stage 7
            DO.Call? call;
            lock (AdminManager.BlMutex) //stage 7
            {
                call = _dal.Call.Read(callId);
            }

            var callStatus = CallManager.GetCallStatus(callId);

            // Check if the call is open or open in risk and the maximum completion time has not passed
            if ((callStatus == CallStatus.Open || callStatus == CallStatus.OpenInRisk) && call.MaxCompletionTime > AdminManager.Now)
            {
                DO.Assignment newAssignment = new DO.Assignment
                {
                    CallId = callId,
                    VolunteerId = volunteerId,
                    EntryTime = AdminManager.Now
                };
                lock (AdminManager.BlMutex) //stage 7
                {
                    _dal.Assignment.Create(newAssignment);
                }
                VolunteerManager.Observers.NotifyItemUpdated(newAssignment.VolunteerId);
                CallManager.Observers.NotifyListUpdated();
            }
            else
            {
                throw new BlOperationException("Cannot assign the call to the volunteer. The call is not open or is expired.");
            }
        }

        /// <summary>
        /// Cancels the handling of a call by the volunteer or a manager.
        /// </summary>
        /// <param name="requesterId">The identifier of the requester (volunteer or manager).</param>
        /// <param name="assignmentId">The identifier of the assignment to be canceled.</param>
        /// <exception cref="BlUnauthorizedAccessException">Thrown when the requester does not have permission to cancel the call handling or the assignment has already been completed.</exception>
        public void CancelCallHandling(int requesterId, int assignmentId)
        {
            //AdminManager.ThrowOnSimulatorIsRunning(); //stage 7
            DO.Assignment? assignment;
            DO.Volunteer? requester;
            // Read the requester and assignment details from the data layer
            lock (AdminManager.BlMutex) //stage 7
            {
                requester = _dal.Volunteer.Read(requesterId);
                assignment = _dal.Assignment.Read(assignmentId);
            }

            // Check if the CompletionStatus is open(null) or not
            if (assignment.CompletionStatus == null)
            {
                // If the requester is the volunteer assigned to the call
                if (requester.Id == assignment.VolunteerId)
                {
                    // Create a new assignment with self-cancellation status
                    DO.Assignment newAssignment = new DO.Assignment
                    {
                        Id = assignment.Id,
                        CallId = assignment.CallId,
                        VolunteerId = assignment.VolunteerId,
                        EntryTime = assignment.EntryTime,
                        CompletionStatus = (DO.CompletionType)CompletionType.SelfCancellation,
                        CompletionTime = AdminManager.Now
                    };
                    lock (AdminManager.BlMutex) //stage 7
                    {
                        _dal.Assignment.Update(newAssignment);
                    }
                    VolunteerManager.Observers.NotifyItemUpdated(assignment.VolunteerId);
                    CallManager.Observers.NotifyListUpdated();
                }
                // If the requester is a manager
                else if (requester.VolunteerRole == DO.Role.Manager)
                {
                    // Create a new assignment with manager-cancellation status
                    DO.Assignment newAssignment = new DO.Assignment
                    {
                        Id = assignment.Id,
                        CallId = assignment.CallId,
                        VolunteerId = assignment.VolunteerId,
                        EntryTime = assignment.EntryTime,
                        CompletionStatus = (DO.CompletionType)CompletionType.ManagerCancellation,
                        CompletionTime = AdminManager.Now
                    };
                    lock (AdminManager.BlMutex) //stage 7
                    {
                        _dal.Assignment.Update(newAssignment);
                    }
                    VolunteerManager.Observers.NotifyItemUpdated(assignment.VolunteerId);
                    CallManager.Observers.NotifyListUpdated();
                }
            }
            else
            {
                // Throw an exception if the requester does not have permission to cancel the call handling
                throw new BlUnauthorizedAccessException("Requester does not have permission to cancel this call handling.");
            }
        }

        /// <summary>
        /// Completes the handling of a call by a volunteer.
        /// </summary>
        /// <param name="volunteerId">The identifier of the volunteer completing the call.</param>
        /// <param name="assignmentId">The identifier of the assignment related to the call.</param>
        /// <exception cref="BlUnauthorizedAccessException">Thrown when the call handling cannot be completed due to invalid status or permissions.</exception>
        /// <exception cref="BlDoesNotExistException">Thrown when the assignment or volunteer is not found.</exception>
        public void CompleteCallHandling(int volunteerId, int assignmentId)
        {
            //AdminManager.ThrowOnSimulatorIsRunning(); //stage 7
            try
            {
                // Read the requester and assignment details from the data layer
                DO.Volunteer? volunteer;
                DO.Assignment? assignment;
                lock (AdminManager.BlMutex) //stage 7
                {
                    volunteer = _dal.Volunteer.Read(volunteerId);
                    assignment = _dal.Assignment.Read(assignmentId);
                }

                // Check if the CompletionStatus is open(null) or not
                if (assignment.CompletionStatus == null)
                {
                    // If the requester is the volunteer assigned to the call
                    if (volunteerId == assignment.VolunteerId)
                    {
                        // Create a new assignment with self-cancellation status
                        DO.Assignment newAssignment = new DO.Assignment
                        {
                            Id = assignment.Id,
                            CallId = assignment.CallId,
                            VolunteerId = assignment.VolunteerId,
                            EntryTime = assignment.EntryTime,
                            CompletionStatus = (DO.CompletionType)CompletionType.Treated,
                            CompletionTime = AdminManager.Now
                        };
                        lock (AdminManager.BlMutex) //stage 7
                        {
                            _dal.Assignment.Update(newAssignment);
                        }
                        VolunteerManager.Observers.NotifyItemUpdated(assignment.VolunteerId);
                        CallManager.Observers.NotifyListUpdated();
                    }
                    else
                    {
                        throw new BlUnauthorizedAccessException("Requester does not have permission to cancel this call handling.");
                    }
                }
                else
                {
                    throw new BlUnauthorizedAccessException("The assignment has already been handled.");
                }
            }
            catch (Exception ex)
            {
                // Rethrow the exception with a more appropriate message for the presentation layer
                throw new BlDoesNotExistException($"Assignment with ID {assignmentId} not found.", ex);
            }
        }

        /// <summary>
        /// Deletes a call by its identifier.
        /// </summary>
        /// <param name="id">The identifier of the call to delete.</param>
        /// <exception cref="BlDoesNotExistException">Thrown when the call with the specified ID is not found.</exception>
        /// <exception cref="BlOperationException">Thrown when the call cannot be deleted because it is not open or has been assigned.</exception>
        public void DeleteCall(int id)
        {
            AdminManager.ThrowOnSimulatorIsRunning(); //stage 7
            // Retrieve the call details from the data layer
            DO.Call? callEntity;
            lock (AdminManager.BlMutex) //stage 7
            {
                callEntity = _dal.Call.Read(id);
            }

            if (callEntity == null)
            {
                throw new BlDoesNotExistException($"Call with ID {id} not found.");
            }

            // Check if the call is in an open status and has never been assigned
            if (AssignmentManager.GetAssignmentIdByCallId(id) != null || (CallManager.GetCallStatus(id) != CallStatus.OpenInRisk && CallManager.GetCallStatus(id) != CallStatus.Open))
            {
                throw new BlDeletionImpossible("Cannot delete the call. Only open calls that have never been assigned can be deleted.");
            }

            try
            {
                // Attempt to delete the call in the data layer
                lock (AdminManager.BlMutex) //stage 7
                {
                    _dal.Call.Delete(id);
                }

                CallManager.Observers.NotifyListUpdated();
            }
            catch (Exception ex)
            {
                // Rethrow the exception with a more appropriate message for the presentation layer
                throw new BlDoesNotExistException($"Call with ID {id} not found.", ex);
            }
        }

        /// <summary>
        /// Retrieves the details of a specific call.
        /// </summary>
        /// <param name="id">The identifier of the call.</param>
        /// <returns>The details of the call.</returns>
        /// <exception cref="BlDoesNotExistException">Thrown when the call does not exist.</exception>
        public Call GetCallDetails(int id)
        {
            try
            {
                // Retrieve the call details from the data layer
                DO.Call? callEntity;
                lock (AdminManager.BlMutex) //stage 7
                {
                    callEntity = _dal.Call.Read(id);
                }

                if (callEntity == null)
                {
                    throw new BlDoesNotExistException($"Call with ID {id} not found.");
                }

                // Create the BO.Call object
                var call = CallManager.ConvertDOCallToBOCall(callEntity.Id);

                return call;
            }
            catch (Exception ex)
            {
                // Rethrow the exception with a more appropriate message for the presentation layer
                throw new BlDoesNotExistException($"Call with ID {id} not found.", ex);
            }
        }

        /// <summary>
        /// Requests a list of calls for management.
        /// </summary>
        /// <param name="filterField">Field to filter by (nullable).</param>
        /// <param name="filterValue">Value to filter by (nullable).</param>
        /// <param name="sortField">Field to sort by (nullable).</param>
        /// <returns>A sorted and filtered collection of calls.</returns>
        public IEnumerable<CallInList> GetCallList(CallInListFields? filterField, object? filterValue, CallInListFields? sortField)
        {
            // Retrieve all calls from CallManager
            var callList = CallManager.GetAllCalls();

            // Filter the list if filterField and filterValue are provided
            if (filterField.HasValue && filterValue != null)
            {
                callList = callList.Where(call =>
                {
                    var property = typeof(CallInList).GetProperty(filterField.Value.ToString());
                    return property != null && property.GetValue(call)?.Equals(filterValue) == true;
                });
            }

            // Sort the call list based on the provided sortField, or by CallId if no sortField is provided.
            if (sortField.HasValue) // Check if a sortField is specified.
            {
                callList = callList.OrderBy(call => // Use OrderBy to sort the list.
                {
                    // Use reflection to get the property info for the sortField.
                    // The sortField's value is converted to a string, which matches the property name.
                    var property = typeof(CallInList).GetProperty(sortField.Value.ToString());

                    // Get the value of the property for the current 'call' object.
                    // If the property is found, return its value for sorting.
                    // If the property is null (not found), return null.
                    return property?.GetValue(call);
                });
            }
            else
            {
                callList = callList.OrderBy(call => call.CallId);
            }

            // Return the filtered and sorted list
            return callList;
        }

        /// <summary>
        /// Retrieves the quantities of calls grouped by their status.
        /// </summary>
        /// <returns>An array of integers representing the quantities of calls for each status.</returns>
        public int[] GetCallQuantitiesByStatus()
        {
            // Retrieve all calls from CallManager
            var callInList = CallManager.GetAllCalls();

            // Check if there are no calls in the list
            if (!callInList.Any())
            {
                // Return an empty array with zeros if there are no calls
                return new int[Enum.GetValues<CallStatus>().Length];
            }

            // Group calls by status and count them
            var groupedCalls = callInList
            .GroupBy(call => (int)call.Status)
            .Select(group => new { Status = group.Key, Count = group.Count() });

            // Create an array of call quantities by status
            int[] quantities = new int[Enum.GetValues<CallStatus>().Length];
            foreach (var group in groupedCalls)
            {
                quantities[group.Status] = group.Count;
            }

            return quantities;
        }

        /// <summary>
        /// Retrieves a list of closed calls handled by a volunteer.
        /// </summary>
        /// <param name="volunteerId">The identifier of the volunteer.</param>
        /// <param name="callType">The type of call to filter by (nullable).</param>
        /// <param name="sortField">The field to sort by (nullable).</param>
        /// <returns>A sorted collection of closed calls handled by the volunteer.</returns>
        public IEnumerable<ClosedCallInList> GetClosedCallsByVolunteer(int volunteerId, CallType? callType, ClosedCallInListFields? sortField)
        {
            // Retrieve all closed calls from CallManager Filter the list by volunteerId
            var filteredCalls = CallManager.GetClosedCallsByVolunteer(volunteerId);

            // Further filter the list by callType if provided
            if (callType.HasValue)
            {
                filteredCalls = filteredCalls.Where(call => call.CallType == callType.Value);
            }

            // Sort the list by the specified sortField, or by Id if no sortField is provided
            if (sortField.HasValue)
            {
                filteredCalls = filteredCalls.OrderBy(call =>
                {
                    var property = typeof(ClosedCallInList).GetProperty(sortField.Value.ToString());
                    return property?.GetValue(call);
                });
            }
            else
            {
                filteredCalls = filteredCalls.OrderBy(call => call.Id);
            }

            // Return the filtered and sorted list
            return filteredCalls;
        }

        /// <summary>
        /// Retrieves a list of open calls available for a volunteer to choose from.
        /// </summary>
        /// <param name="volunteerId">The identifier of the volunteer.</param>
        /// <param name="callType">The type of call to filter by (nullable).</param>
        /// <param name="sortField">The field to sort by (nullable).</param>
        /// <returns>A sorted collection of open calls available for the volunteer.</returns>
        public IEnumerable<OpenCallInList> GetOpenCallsForVolunteer(int volunteerId, CallType? callType, OpenCallInListFields? sortField)
        {
            // Retrieve all open calls from CallManager
            var openCalls = CallManager.GetOpenCallsForVolunteer(volunteerId);
            if (openCalls == null)
                return null;
            // Filter the list by callType if provided
            if (callType.HasValue)
            {
                openCalls = openCalls.Where(call => call.CallType == callType.Value);
            }

            // Sort the list by the specified sortField, or by Id if no sortField is provided
            if (sortField.HasValue)
            {
                openCalls = openCalls.OrderBy(call =>
                {
                    var property = typeof(OpenCallInList).GetProperty(sortField.Value.ToString());
                    return property?.GetValue(call);
                });
            }
            else
            {
                openCalls = openCalls.OrderBy(call => call.Id);
            }

            // Return the filtered and sorted list
            return openCalls;
        }

        /// <summary>
        /// Updates the details of a call.
        /// </summary>
        /// <param name="call"></param>
        /// <exception cref="BlInvalidDateOrderException"></exception>
        /// <exception cref="BlInvalidAddressException"></exception>
        /// <exception cref="BlDoesNotExistException"></exception>
        public void UpdateCall(Call call)
        {
            AdminManager.ThrowOnSimulatorIsRunning(); //stage 7

            // Validate the maximum completion time is greater than the opening time
            if (call.MaxCompletionTime.HasValue && call.MaxCompletionTime <= call.OpenedAt)
            {
                throw new BlInvalidDateOrderException("Maximum completion time must be greater than the opening time.");
            }

            // Validate the address and update latitude and longitude
            if (string.IsNullOrWhiteSpace(call.FullAddress))
            {
                throw new BlInvalidAddressException("Address cannot be null or empty.");
            }

            // Convert BO.Call to DO.Call
            var doCall = CallManager.ConvertBOCallToDOCall(call);

            try
            {
                // Attempt to update the call in the data layer
                lock (AdminManager.BlMutex) //stage 7
                    _dal.Call.Update(doCall);
                CallManager.Observers.NotifyItemUpdated(doCall.Id);
                CallManager.Observers.NotifyListUpdated();
            }
            catch (DO.DalDoesNotExistException ex)
            {
                // Rethrow the exception with a more appropriate message for the presentation layer
                throw new BlDoesNotExistException($"Call with ID {call.Id} not found.", ex);
            }

            // Compute the coordinates asynchronously without waiting for the results
            _ = CallManager.UpdateCoordinatesForCallAsync(doCall);
        }
    }
}