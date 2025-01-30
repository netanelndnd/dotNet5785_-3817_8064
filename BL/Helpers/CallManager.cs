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
        DO.Call? callDetails;
        lock (AdminManager.BlMutex) //stage 7
        {
            callDetails = s_dal.Call.Read(a => a.Id == callId);
        }

        if (callDetails == null)
        {
            throw new InvalidOperationException($"Call with ID {callId} not found.");
        }

        int callType = (int)callDetails.CallType;
        return callType;
    }

    /// <summary>
    /// Convert a call ID to a CallInProgress object
    /// </summary>
    /// <param name="callId">The identifier of the call.</param>
    /// <param name="volunteerId">The identifier of the volunteer handling the call.</param>
    /// <returns>A CallInProgress object containing details of the call and its progress.</returns>
    public static BO.CallInProgress ConvertCallIdToCallInProgress(int callId, int volunteerId)
    {
        DO.Call? callDetails;
        DO.Assignment? assignment;
        DO.Volunteer? volunteer;

        lock (AdminManager.BlMutex) //stage 7
        {
            callDetails = s_dal.Call.Read(a => a.Id == callId);
            assignment = s_dal.Assignment.Read(a => a.VolunteerId == volunteerId && a.CompletionStatus == null && a.CallId == callId);
            volunteer = s_dal.Volunteer.Read(volunteerId);
        }

        if (callDetails == null)
        {
            throw new InvalidOperationException($"Call with ID {callId} not found.");
        }

        if (assignment == null)
        {
            throw new InvalidOperationException($"Assignment for volunteer ID {volunteerId} and call ID {callId} not found.");
        }

        if (volunteer == null)
        {
            throw new InvalidOperationException($"Volunteer with ID {volunteerId} not found.");
        }

        var callInProgress = new BO.CallInProgress
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
        IEnumerable<DO.Call> callEntities;

        lock (AdminManager.BlMutex) //stage 7
        {
            callEntities = s_dal.Call.ReadAll();
        }

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

    /// <summary>
    /// Checks if the status of a specific call is either Open or OpenInRisk and returns the remaining time.
    /// </summary>
    /// <param name="callId">The identifier of the call.</param>
    /// <returns>The remaining time if the call status is Open or OpenInRisk, otherwise null.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the call with the specified ID is not found.</exception>
    public static TimeSpan? GetRemainingTimeIfOpenOrInRisk(int callId)
    {
        DO.Call? callDetails;
        BO.CallStatus callStatus;

        lock (AdminManager.BlMutex) //stage 7
        {
            callDetails = s_dal.Call.Read(a => a.Id == callId);
            callStatus = GetCallStatus(callId);
        }

        if (callDetails == null)
        {
            throw new InvalidOperationException($"Call with ID {callId} not found.");
        }

        if (callStatus == BO.CallStatus.Open || callStatus == BO.CallStatus.OpenInRisk || callStatus == BO.CallStatus.InProgressInRisk || callStatus == BO.CallStatus.InProgress)
        {
            return callDetails.MaxCompletionTime.HasValue ? callDetails.MaxCompletionTime.Value - AdminManager.Now : (TimeSpan?)null;
        }

        return (TimeSpan?)null;
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
        DO.Call? callDetails;
        IEnumerable<DO.Assignment> assignments;
        DateTime now;
        TimeSpan riskRange;

        lock (AdminManager.BlMutex) //stage 7
        {
            callDetails = s_dal.Call.Read(a => a.Id == callId);
            assignments = s_dal.Assignment.ReadAll().Where(a => a.CallId == callId).OrderByDescending(a => a.EntryTime);
            now = AdminManager.Now;
            riskRange = s_dal.Config.RiskRange;
        }

        if (callDetails == null)
        {
            throw new InvalidOperationException($"Call with ID {callId} not found.");
        }

        var latestAssignment = assignments.FirstOrDefault();
        if (latestAssignment == null)
        {
            if (callDetails.MaxCompletionTime.HasValue && now > callDetails.MaxCompletionTime.Value)
            {
                return BO.CallStatus.Expired;
            }
            if (callDetails.MaxCompletionTime.HasValue && now > callDetails.MaxCompletionTime.Value - riskRange)
            {
                return BO.CallStatus.OpenInRisk;
            }
            return BO.CallStatus.Open;
        }

        if (latestAssignment.CompletionStatus == DO.CompletionType.Expired)
        {
            return BO.CallStatus.Expired;
        }
        if (latestAssignment.CompletionStatus == DO.CompletionType.Treated)
        {
            return BO.CallStatus.Treated;
        }

        if (latestAssignment.CompletionStatus == DO.CompletionType.ManagerCancellation || latestAssignment.CompletionStatus == DO.CompletionType.SelfCancellation)
        {
            var otherAssignments = assignments.Skip(1).FirstOrDefault(a => a.CompletionStatus == null);
            if (otherAssignments != null)
            {
                if (callDetails.MaxCompletionTime.HasValue && now > callDetails.MaxCompletionTime.Value - riskRange)
                {
                    return BO.CallStatus.InProgressInRisk;
                }
                return BO.CallStatus.InProgress;
            }
            else
            {
                if (callDetails.MaxCompletionTime.HasValue && now > callDetails.MaxCompletionTime.Value)
                {
                    return BO.CallStatus.Expired;
                }
                if (callDetails.MaxCompletionTime.HasValue && now > callDetails.MaxCompletionTime.Value - riskRange)
                {
                    return BO.CallStatus.OpenInRisk;
                }
                return BO.CallStatus.Open;
            }
        }

        if (callDetails.MaxCompletionTime.HasValue && now > callDetails.MaxCompletionTime.Value - riskRange)
        {
            return BO.CallStatus.InProgressInRisk;
        }
        return BO.CallStatus.InProgress;
    }

    /// <summary>
    /// Retrieves the details of a call and its assignments.
    /// </summary>
    /// <param name="callId">The identifier of the call.</param>
    /// <returns>A Call object BO containing details of the call and its assignments.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the call with the specified ID is not found.</exception>
    public static BO.Call ConvertDOCallToBOCall(int callId)
    {
        DO.Call? callDetails;
        IEnumerable<BO.CallAssignInList>? assignments;

        lock (AdminManager.BlMutex) //stage 7
        {
            callDetails = s_dal.Call.Read(a => a.Id == callId);
            assignments = AssignmentManager.GetCallAssignmentsByCallId(callId);
        }

        if (callDetails == null)
        {
            throw new InvalidOperationException($"Call with ID {callId} not found.");
        }

        var boCall = new BO.Call
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

        return new DO.Call
        {
            Id = boCall.Id,
            CallType = (DO.CallType)boCall.CallType,
            Address = boCall.FullAddress,
            Latitude = boCall.Latitude,
            Longitude = boCall.Longitude,
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
        IEnumerable<DO.Call>? closedCallsEntities;

        lock (AdminManager.BlMutex) //stage 7
        {
            closedCallsEntities = s_dal.Call.ReadAll()
                .Where(c => c.MaxCompletionTime != null && s_dal.Assignment.Read(a => a.CallId == c.Id && a.VolunteerId == volunteerId) != null);
        }

        if (closedCallsEntities == null || !closedCallsEntities.Any())
        {
            return null;
        }

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

    /// <summary>
    /// Retrieves a list of open calls available for a volunteer to choose from.
    /// </summary>
    /// <param name="volunteerId">The identifier of the volunteer.</param>
    /// <returns>A list of OpenCallInList objects containing details of open calls available for the volunteer.</returns>
    public static IEnumerable<BO.OpenCallInList>? GetOpenCallsForVolunteer(int volunteerId)
    {
        IEnumerable<DO.Call>? openCallsEntities;
        DO.Volunteer? volunteer;

        lock (AdminManager.BlMutex) //stage 7
        {
            var Calls = s_dal.Call.ReadAll();
            volunteer = s_dal.Volunteer.Read(volunteerId);
            openCallsEntities = Calls.Where(c => (GetCallStatus(c.Id) == BO.CallStatus.OpenInRisk || GetCallStatus(c.Id) == BO.CallStatus.Open) && (volunteer.MaxDistance >= Tools.CalculateDistance((double)volunteer.Latitude
                , (double)volunteer.Longitude
                , c.Latitude, c.Longitude)));
        }

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

    public static async Task UpdateCoordinatesForCallAsync(DO.Call doCall)
    {
        if (!string.IsNullOrWhiteSpace(doCall.Address))
        {
            var coordinates = await Tools.GetCoordinatesAsync(doCall.Address);
            if (coordinates.IsInIsrael)
            {
                doCall = doCall with { Latitude = coordinates.Latitude, Longitude = coordinates.Longitude };
                lock (AdminManager.BlMutex)
                    s_dal.Call.Update(doCall);
                CallManager.Observers.NotifyListUpdated();
                CallManager.Observers.NotifyItemUpdated(doCall.Id);
            }
        }
    }



}
