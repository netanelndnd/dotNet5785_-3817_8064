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
            Status = (BO.CallStatus)callDetails.CallType,/////לתקןןןןן זה לא נכון
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
            Status = (BO.CallStatus)call.CallType,///לתקןןןן
            TotalAssignments = AssignmentManager.CountAssignmentsByCallId(call.Id)
        }).ToList();
    }

}
