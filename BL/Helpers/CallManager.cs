using DalApi;
using System.Text.RegularExpressions;
using BlImplementation;
namespace Helpers;

public static class CallManager
{
    private static IDal s_dal = Factory.Get; //stage 4

    internal static ObserverManager Observers = new(); //stage 5

    public static int GetCallTypeById(int callId)
    {
        lock (AdminManager.BlMutex) //stage 7
        {
            var callDetails = s_dal.Call.Read(a => a.Id == callId);
            if (callDetails == null)
            {
                throw new InvalidOperationException($"Call with ID {callId} not found.");
            }
            return (int)callDetails.CallType;
        }
    }

    /// <summary>
    /// Convert a call ID to a CallInProgress object
    /// </summary>
    /// <param name="callId">The identifier of the call.</param>
    /// <param name="volunteerId">The identifier of the volunteer handling the call.</param>
    /// <returns>A CallInProgress object containing details of the call and its progress.</returns>
    public static BO.CallInProgress ConvertCallIdToCallInProgress(int callId, int volunteerId)
    {
        BO.CallInProgress callInProgress;
        lock (AdminManager.BlMutex) //stage 7
        {
            var callDetails = s_dal.Call.Read(a => a.Id == callId);
            if (callDetails == null)
            {
                throw new InvalidOperationException($"Call with ID {callId} not found.");
            }

            var assignment = s_dal.Assignment.Read(a => a.VolunteerId == volunteerId && a.CompletionStatus == null && a.CallId == callId);
            if (assignment == null)
            {
                throw new InvalidOperationException($"Assignment for volunteer ID {volunteerId} and call ID {callId} not found.");
            }

            var volunteer = s_dal.Volunteer.Read(volunteerId);
            if (volunteer == null)
            {
                throw new InvalidOperationException($"Volunteer with ID {volunteerId} not found.");
            }

            callInProgress = new BO.CallInProgress
            {
                Id = assignment.Id,
                CallId = callDetails.Id,
                CallType = (BO.CallType)callDetails.CallType,
                Description = callDetails.Description,
                FullAddress = callDetails.Address,
                OpenedAt = callDetails.OpenTime,
                StartedAt = assignment.EntryTime,
                MaxCompletionTime = callDetails.MaxCompletionTime,
                DistanceFromVolunteer = Tools.CalculateDistance(volunteer.Latitude ?? throw new InvalidOperationException("Volunteer latitude is null."),
                                                      volunteer.Longitude ?? throw new InvalidOperationException("Volunteer longitude is null."),
                                                      callDetails.Latitude, callDetails.Longitude),
                Status = GetCallStatus(callId)
            };
        }
        return callInProgress;
    }

    // פונקציה להמיר מעלות לרדיאנים
    private static double DegreesToRadians(double degrees)
    {
        return degrees * (Math.PI / 180);
    }

    /// <summary>
    /// Retrieves all calls from the data layer as a list of CallInList.
    /// </summary>
    /// <returns>A list of CallInList objects containing details of all calls.</returns>
    /// 
    public static IEnumerable<BO.CallInList> GetAllCalls()
    {
        lock (AdminManager.BlMutex) //stage 7
        {
            var callEntities = s_dal.Call.ReadAll();
            return callEntities.Select(call => new BO.CallInList
            {
                AssignmentId = AssignmentManager.GetAssignmentIdByCallId(call.Id),
                CallId = call.Id,
                CallType = (BO.CallType)call.CallType,
                OpeningTime = call.OpenTime,
                RemainingTime = GetRemainingTimeIfOpenOrInRisk(call.Id),
                LastVolunteerName = AssignmentManager.GetVolunteerNameByCallId(call.Id),
                CompletionDuration = AssignmentManager.GetTimeDifferenceForLastAssignment(call.Id),
                Status = GetCallStatus(call.Id),
                TotalAssignments = AssignmentManager.CountAssignmentsByCallId(call.Id)
            });
        }
    }

    /// <summary>
    /// Checks if the status of a specific call is either Open or OpenInRisk and returns the remaining time.
    /// </summary>
    /// <param name="callId">The identifier of the call.</param>
    /// <returns>The remaining time if the call status is Open or OpenInRisk, otherwise null.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the call with the specified ID is not found.</exception>
    public static TimeSpan? GetRemainingTimeIfOpenOrInRisk(int callId)
    {
        lock (AdminManager.BlMutex) //stage 7
        {
            var callDetails = s_dal.Call.Read(a => a.Id == callId);
            if (callDetails == null)
            {
                throw new InvalidOperationException($"Call with ID {callId} not found.");
            }

            var callStatus = GetCallStatus(callId);
            if (callStatus == BO.CallStatus.Open || callStatus == BO.CallStatus.OpenInRisk || callStatus == BO.CallStatus.InProgressInRisk || callStatus == BO.CallStatus.InProgress)
            {
                return callDetails.MaxCompletionTime.HasValue ? callDetails.MaxCompletionTime.Value - AdminManager.Now : (TimeSpan?)null;
            }

            return (TimeSpan?)null;
        }
    }

    /// <summary>
    /// Retrieves the status of a specific call, optionally filtered by volunteer ID.
    /// </summary>
    /// <param name="callId">The identifier of the call.</param>
    /// <param name="volunteerId">The identifier of the volunteer (optional).</param>
    /// <returns>The updated status of the call.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the call with the specified ID is not found.</exception>
    public static BO.CallStatus GetCallStatus(int callId)
    {
        lock (AdminManager.BlMutex) //stage 7
        {
            // Get the call details
            var callDetails = s_dal.Call.Read(a => a.Id == callId);
            // Check if the call exists
            if (callDetails == null)
            {
                throw new InvalidOperationException($"Call with ID {callId} not found.");
            }
            // Get the assignment details
            var assignments = s_dal.Assignment.ReadAll()
                .Where(a => a.CallId == callId)
                .OrderByDescending(a => a.EntryTime);

            var now = AdminManager.Now;
            var riskRange = s_dal.Config.RiskRange;

            // Check if there are no assignments
            var latestAssignment = assignments.FirstOrDefault();
            if (latestAssignment == null)
            {
                // Check if the call has expired
                if (callDetails.MaxCompletionTime.HasValue && now > callDetails.MaxCompletionTime.Value)
                {
                    return BO.CallStatus.Expired;
                }
                // Check if the call is open and in risk of expiring
                if (callDetails.MaxCompletionTime.HasValue && now > callDetails.MaxCompletionTime.Value - riskRange)
                {
                    return BO.CallStatus.OpenInRisk;
                }
                // The call is open and not in risk
                return BO.CallStatus.Open;
            }

            // Check if the latest assignment has a completion status of Expired or Treated
            if (latestAssignment.CompletionStatus == DO.CompletionType.Expired)
            {
                return BO.CallStatus.Expired;
            }
            if (latestAssignment.CompletionStatus == DO.CompletionType.Treated)
            {
                return BO.CallStatus.Treated;
            }

            // Check if the latest assignment has a completion status of ManagerCancellation or SelfCancellation
            if (latestAssignment.CompletionStatus == DO.CompletionType.ManagerCancellation || latestAssignment.CompletionStatus == DO.CompletionType.SelfCancellation)
            {
                // Check if there are other assignments that are still open or in progress
                var otherAssignments = assignments.Skip(1).FirstOrDefault(a => a.CompletionStatus == null);
                if (otherAssignments != null)
                {
                    // Check if the call is in progress and in risk of expiring
                    if (callDetails.MaxCompletionTime.HasValue && now > callDetails.MaxCompletionTime.Value - riskRange)
                    {
                        return BO.CallStatus.InProgressInRisk;
                    }
                    // The call is in progress
                    return BO.CallStatus.InProgress;
                }
                else
                {
                    // Check if the call has expired
                    if (callDetails.MaxCompletionTime.HasValue && now > callDetails.MaxCompletionTime.Value)
                    {
                        return BO.CallStatus.Expired;
                    }
                    // Check if the call is open and in risk of expiring
                    if (callDetails.MaxCompletionTime.HasValue && now > callDetails.MaxCompletionTime.Value - riskRange)
                    {
                        return BO.CallStatus.OpenInRisk;
                    }
                    // The call is open and not in risk
                    return BO.CallStatus.Open;
                }
            }

            // Check if the call is in progress and in risk of expiring
            if (callDetails.MaxCompletionTime.HasValue && now > callDetails.MaxCompletionTime.Value - riskRange)
            {
                return BO.CallStatus.InProgressInRisk;
            }
            // The call is in progress
            return BO.CallStatus.InProgress;
        }
    }

    /// <summary>
    /// Retrieves the details of a call and its assignments.
    /// </summary>
    /// <param name="callId">The identifier of the call.</param>
    /// <returns>A Call object BO containing details of the call and its assignments.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the call with the specified ID is not found.</exception>
    public static BO.Call ConvertDOCallToBOCall(int callId)
    {
        BO.Call boCall;
        lock (AdminManager.BlMutex) //stage 7
        {
            var callDetails = s_dal.Call.Read(a => a.Id == callId);
            if (callDetails == null)
            {
                throw new InvalidOperationException($"Call with ID {callId} not found.");
            }

            var assignments = AssignmentManager.GetCallAssignmentsByCallId(callId);

            boCall = new BO.Call
            {
                Id = callDetails.Id,
                CallType = (BO.CallType)callDetails.CallType,
                Description = callDetails.Description,
                FullAddress = callDetails.Address,
                Latitude = callDetails.Latitude,
                Longitude = callDetails.Longitude,
                OpenedAt = callDetails.OpenTime,
                MaxCompletionTime = callDetails.MaxCompletionTime,
                Status = GetCallStatus(callId),
                Assignments = assignments != null && assignments.Any() ? assignments : null
            };
        }
        return boCall;
    }

    /// <summary>
    /// Converts a BO.Call object to a DO.Call object.
    /// </summary>
    /// <param name="boCall">The BO.Call object to convert.</param>
    /// <returns>A DO.Call object containing the same details as the BO.Call object.</returns>
    public static DO.Call ConvertBOCallToDOCall(BO.Call boCall)
    {
        if (boCall == null)
        {
            throw new ArgumentNullException(nameof(boCall), "The BO.Call object cannot be null.");
        }

        // Get the coordinates of the address
        var coordinates = Tools.GetCoordinates(boCall.FullAddress);

        return new DO.Call
        {
            Id = boCall.Id,
            CallType = (DO.CallType)boCall.CallType,
            Address = boCall.FullAddress,
            Latitude = coordinates.Latitude,
            Longitude = coordinates.Longitude,
            OpenTime = boCall.OpenedAt,
            Description = boCall.Description,
            MaxCompletionTime = boCall.MaxCompletionTime
        };
    }

    /// <summary>
    /// Retrieves a list of closed calls handled by a specific volunteer.
    /// </summary>
    /// <param name="volunteerId">The identifier of the volunteer.</param>
    /// <returns>A list of ClosedCallInList objects containing details of closed calls handled by the volunteer.</returns>
    public static IEnumerable<BO.ClosedCallInList>? GetClosedCallsByVolunteer(int volunteerId)
    {
        lock (AdminManager.BlMutex) //stage 7
        {
            // Get all closed calls assigned to the specific volunteer
            var closedCallsEntities = s_dal.Call.ReadAll()
                .Where(c => c.MaxCompletionTime != null && s_dal.Assignment.Read(a => a.CallId == c.Id && a.VolunteerId == volunteerId) != null);
            if (closedCallsEntities == null || !closedCallsEntities.Any())
            {
                return null;
            }

            // Return a list of closed calls handled by the volunteer
            return closedCallsEntities.Select(call => new BO.ClosedCallInList
            {
                Id = call.Id,
                CallType = (BO.CallType)call.CallType,
                FullAddress = call.Address,
                OpenedAt = call.OpenTime,
                StartedAt = s_dal.Assignment.Read(a => a.CallId == call.Id && a.VolunteerId == volunteerId)?.EntryTime ?? DateTime.MinValue,
                CompletedAt = s_dal.Assignment.Read(a => a.CallId == call.Id && a.VolunteerId == volunteerId)?.CompletionTime,
                CompletionStatus = (BO.CompletionType?)s_dal.Assignment.Read(a => a.CallId == call.Id && a.VolunteerId == volunteerId)?.CompletionStatus,
            });
        }
    }

    /// <summary>
    /// Retrieves a list of open calls available for a volunteer to choose from.
    /// </summary>
    /// <param name="volunteerId">The identifier of the volunteer.</param>
    /// <returns>A list of OpenCallInList objects containing details of open calls available for the volunteer.</returns>
    public static IEnumerable<BO.OpenCallInList>? GetOpenCallsForVolunteer(int volunteerId)
    {
        lock (AdminManager.BlMutex) //stage 7
        {
            var Calls = s_dal.Call.ReadAll();
            var volunteer = s_dal.Volunteer.Read(volunteerId);
            var openCallsEntities = Calls.Where(c => (GetCallStatus(c.Id) == BO.CallStatus.OpenInRisk || GetCallStatus(c.Id) == BO.CallStatus.Open) && (volunteer.MaxDistance >= Tools.CalculateDistance((double)volunteer.Latitude
                , (double)volunteer.Longitude
                , c.Latitude, c.Longitude)));

            if (openCallsEntities == null || !openCallsEntities.Any())
            {
                return null;
            }

            return openCallsEntities.Select(call => new BO.OpenCallInList
            {
                Id = call.Id,
                CallType = (BO.CallType)call.CallType,
                Description = call.Description,
                FullAddress = call.Address,
                OpenedAt = call.OpenTime,
                MaxCompletionTime = call.MaxCompletionTime,
                DistanceFromVolunteer = Tools.CalculateDistance((double)volunteer.Latitude
                , (double)volunteer.Longitude
                , call.Latitude, call.Longitude),
            });
        }
    }
}
