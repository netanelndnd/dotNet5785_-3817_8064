using DalApi;
using System.Text.RegularExpressions;
using BlImplementation;
namespace Helpers;

internal static class CallManager 
{
    private static IDal s_dal = Factory.Get; //stage 4

    internal static ObserverManager Observers = new(); //stage 5

    public static int GetCallTypeById(int callId)
    {
        var callDetails = s_dal.Call.Read(a => a.Id == callId);
        if (callDetails == null)
        {
            throw new InvalidOperationException($"Call with ID {callId} not found.");
        }
        return (int)callDetails.CallType;
    }

    /// <summary>
    /// Convert a call ID to a CallInProgress object
    /// </summary>
    /// <param name="callId">The identifier of the call.</param>
    /// <param name="volunteerId">The identifier of the volunteer handling the call.</param>
    /// <returns>A CallInProgress object containing details of the call and its progress.</returns>
    public static BO.CallInProgress ConvertCallIdToCallInProgress(int callId, int volunteerId)
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


        var callInProgress = new BO.CallInProgress
        {
            Id = callDetails.Id,
            CallType = (BO.CallType)callDetails.CallType,
            Description = callDetails.Description,
            FullAddress = callDetails.Address,
            OpenedAt = callDetails.OpenTime,
            StartedAt = assignment.EntryTime,
            MaxCompletionTime = callDetails.MaxCompletionTime,
            DistanceFromVolunteer = CalculateDistance(volunteer.Latitude ?? throw new InvalidOperationException("Volunteer latitude is null."),
                                                  volunteer.Longitude ?? throw new InvalidOperationException("Volunteer longitude is null."),
                                                  callDetails.Latitude, callDetails.Longitude),
            Status = GetCallStatus(callId)
        };

        return callInProgress;
    }

    // פונקציה לחישוב המרחק בין שתי נקודות על פי קו רוחב וקו אורך
    public static double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
    {

        // קבועים עבור חישוב המרחק
        double R = 6371; // רדיוס כדור הארץ בקילומטרים
        double dLat = DegreesToRadians(lat2 - lat1); // שינוי בקו הרוחב
        double dLon = DegreesToRadians(lon2 - lon1); // שינוי בקו האורך

        // חישוב המרחק באמצעות נוסחת Haversine
        double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                   Math.Cos(DegreesToRadians(lat1)) * Math.Cos(DegreesToRadians(lat2)) *
                   Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        double distance = R * c; // המרחק בקילומטרים

        return distance;
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
        var callEntities = s_dal.Call.ReadAll();
        return callEntities.Select(call => new BO.CallInList
        {
            AssignmentId = AssignmentManager.GetAssignmentIdByCallId(call.Id),
            CallId = call.Id,
            CallType = (BO.CallType)call.CallType,
            OpeningTime = call.OpenTime,
            RemainingTime = call.MaxCompletionTime.HasValue ? call.MaxCompletionTime.Value - AdminManager.Now : (TimeSpan?)null,
            LastVolunteerName = AssignmentManager.GetVolunteerNameByCallId(call.Id),
            CompletionDuration = AssignmentManager.GetTimeDifferenceForLastAssignment(call.Id),
            Status = GetCallStatus(call.Id),
            TotalAssignments = AssignmentManager.CountAssignmentsByCallId(call.Id)
        }).ToList();
    }

    /// <summary>
    /// Retrieves the status of a specific call, optionally filtered by volunteer ID.
    /// </summary>
    /// <param name="callId">The identifier of the call.</param>
    /// <param name="volunteerId">The identifier of the volunteer (optional).</param>
    /// <returns>The updated status of the call.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the call with the specified ID is not found.</exception>
    public static BO.CallStatus GetCallStatus(int callId, int? volunteerId = null)
    {
        // Get the call details
        var callDetails = s_dal.Call.Read(a => a.Id == callId);
        // Check if the call exists
        if (callDetails == null)
        {
            throw new InvalidOperationException($"Call with ID {callId} not found.");
        }
        // Get the assignment details
        var assignment = s_dal.Assignment.Read(a => a.CallId == callId && a.CompletionTime == null);
        var now = AdminManager.Now;
        var riskRange = s_dal.Config.RiskRange;

        if (volunteerId.HasValue)
        {
            // Check if the call is assigned to the given volunteer
            if (assignment != null && assignment.VolunteerId == volunteerId.Value)
            {
                // Check if the call is in progress and in risk of expiring
                if (callDetails.MaxCompletionTime.HasValue && now > callDetails.MaxCompletionTime.Value - riskRange)
                {
                    return BO.CallStatus.InProgressInRisk;
                }
                // The call is in progress
                return BO.CallStatus.InProgress;
            }
        }
        //מדבור בקיראה שעדיין לא הוקצאה למתנדב
        if (assignment == null)
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
        //מדובר בקריאה שהוקצתה למתנדב
        else
        {
            // Check if the call has expired
            if (callDetails.MaxCompletionTime.HasValue && now > callDetails.MaxCompletionTime.Value)
            {
                return BO.CallStatus.Expired;
            }
            // Check if the call is in progress and in risk of expiring
            if (callDetails.MaxCompletionTime.HasValue && now > callDetails.MaxCompletionTime.Value - riskRange)
            {
                return BO.CallStatus.InProgressInRisk;
            }
            // Check if the call has been treated within the maximum completion time
            if (assignment.CompletionTime.HasValue && callDetails.MaxCompletionTime.HasValue && assignment.CompletionTime.Value <= callDetails.MaxCompletionTime.Value)
            {
                return BO.CallStatus.Treated;
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
        var callDetails = s_dal.Call.Read(a => a.Id == callId);
        if (callDetails == null)
        {
            throw new InvalidOperationException($"Call with ID {callId} not found.");
        }

        var assignments = AssignmentManager.GetCallAssignmentsByCallId(callId);

        return new BO.Call
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
            Assignments = assignments.Any() ? assignments : null
        };
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
        // Get all closed calls
        var closedCalls = s_dal.Call.ReadAll().Where(c => c.MaxCompletionTime != null);
        if (closedCalls == null || !closedCalls.Any())
        {
            return null;
        }
        // Return a list of closed calls handled by the volunteer
        return closedCalls.Select(call => new BO.ClosedCallInList
        {
            Id = call.Id,
            CallType = (BO.CallType)call.CallType,
            FullAddress = call.Address,
            OpenedAt = call.OpenTime,
            StartedAt = s_dal.Assignment.Read(a => a.CallId == call.Id && a.VolunteerId == volunteerId)?.EntryTime ?? DateTime.MinValue,
            CompletedAt = s_dal.Assignment.Read(a => a.CallId == call.Id && a.VolunteerId == volunteerId)?.CompletionTime,
            CompletionStatus = (BO.CompletionType)s_dal.Assignment.Read(a => a.CallId == call.Id && a.VolunteerId == volunteerId)?.CompletionStatus
        }).ToList();
    }

    /// <summary>
    /// Retrieves a list of open calls available for a volunteer to choose from.
    /// </summary>
    /// <param name="volunteerId">The identifier of the volunteer.</param>
    /// <returns>A list of OpenCallInList objects containing details of open calls available for the volunteer.</returns>
    public static IEnumerable<BO.OpenCallInList>? GetOpenCallsForVolunteer(int volunteerId)
    {
        var openCalls = s_dal.Call.ReadAll().Where(c => c.MaxCompletionTime == null || c.MaxCompletionTime > AdminManager.Now);
        if (openCalls == null || !openCalls.Any())
        {
            return null;
        }



        return openCalls.Select(call => new BO.OpenCallInList
        {
            Id = call.Id,
            CallType = (BO.CallType)call.CallType,
            FullAddress = call.Address,
            OpenedAt = call.OpenTime,
            DistanceFromVolunteer = CalculateDistance((double)s_dal.Volunteer.Read(volunteerId).Latitude
            , (double)s_dal.Volunteer.Read(volunteerId).Longitude
            , call.Latitude, call.Longitude),
        }).ToList();
    }


    





}
