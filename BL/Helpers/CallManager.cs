using DalApi;
using System;

namespace Helpers;

internal static class CallManager
{
    private static IDal s_dal = Factory.Get; //stage 4


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

        var assignment = s_dal.Assignment.Read(a => a.VolunteerId == volunteerId && a.CompletionTime == null && a.CallId == callId);
        if (assignment == null)
        {
            throw new InvalidOperationException($"Assignment for volunteer ID {volunteerId} and call ID {callId} not found.");
        }

        var volunteer = s_dal.Volunteer.Read(volunteerId);
        if (volunteer == null)
        {
            throw new InvalidOperationException($"Volunteer with ID {volunteerId} not found.");
        }

        return new BO.CallInProgress
        {
            Id = callDetails.Id,
            CallType = (BO.CallType)callDetails.CallType,
            Description = callDetails.Description,
            FullAddress = callDetails.Address,
            OpenedAt = callDetails.OpenTime,
            MaxCompletionTime = callDetails.MaxCompletionTime,
            StartedAt = (DateTime)assignment.EntryTime,
            DistanceFromVolunteer = CalculateDistance((double)volunteer.Latitude, (double)volunteer.Longitude, callDetails.Latitude, callDetails.Longitude),
            Status = GetCallStatus(callId)
        };
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
            RemainingTime = call.MaxCompletionTime.HasValue ? call.MaxCompletionTime.Value - ClockManager.Now : (TimeSpan?)null,
            LastVolunteerName = AssignmentManager.GetVolunteerNameByCallId(call.Id),
            CompletionDuration = AssignmentManager.GetTimeDifferenceForLastAssignment(call.Id),
            Status = GetCallStatus(call.Id),
            TotalAssignments = AssignmentManager.CountAssignmentsByCallId(call.Id)
        }).ToList();
    }


    /// <summary>
    /// Updates the status of a call based on its current state and assignment details.
    /// </summary>
    /// <param name="callId">The identifier of the call.</param>
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
        var now = ClockManager.Now;
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
    public static BO.Call ConvertCallIdToCall(int callId)
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




}
