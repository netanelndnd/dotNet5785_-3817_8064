using BlApi;
using BO;
using Helpers;

namespace BlImplementation
{
    internal class CallImplementation : ICall
    {
        private readonly DalApi.IDal _dal = DalApi.Factory.Get;

        public void AddCall(Call call)
        {
            throw new NotImplementedException();
        }

        public void AssignCallToVolunteer(int volunteerId, int callId)
        {
            throw new NotImplementedException();
        }

        public void CancelCallHandling(int requesterId, int assignmentId)
        {
            throw new NotImplementedException();
        }

        public void CompleteCallHandling(int volunteerId, int assignmentId)
        {
            throw new NotImplementedException();
        }

        public void DeleteCall(int id)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Retrieves the details of a specific call.
        /// </summary>
        /// <param name="id">The identifier of the call.</param>
        /// <returns>The details of the call.</returns>
        /// <exception cref="KeyNotFoundException">Thrown when the call does not exist.</exception>
        public Call GetCallDetails(int id)
        {
            // Retrieve the call details from the data layer
            var callEntity = _dal.Call.Read(id);
            if (callEntity == null)
            {
                throw new KeyNotFoundException($"Call with ID {id} not found.");
            }

            // Create the BO.Call object
            var call = CallManager.ConvertCallIdToCall(callEntity.Id);

            return call;
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

            // Sort the list if sortField is provided, otherwise sort by CallId
            if (sortField.HasValue)
            {
                callList = callList.OrderBy(call =>
                {
                    var property = typeof(CallInList).GetProperty(sortField.Value.ToString());
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


        public IEnumerable<ClosedCallInList> GetClosedCallsByVolunteer(int volunteerId, CallType? callType, ClosedCallInListFields? sortField)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<OpenCallInList> GetOpenCallsForVolunteer(int volunteerId, CallType? callType, OpenCallInListFields? sortField)
        {
            throw new NotImplementedException();
        }

        public void UpdateCall(Call call)
        {
            throw new NotImplementedException();
        }
    }
}
