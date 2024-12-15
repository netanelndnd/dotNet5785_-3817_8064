using DalApi;
using BO;
namespace Helpers;

internal static class VolunteerManager
{
    private static IDal s_dal = Factory.Get; //stage 4

    /// <summary>
    /// Get the number of calls that a volunteer has handled
    /// </summary>
    /// <param name="volunteerId"></param>
    /// <returns></returns
    public static int GetCompletedAssignmentsCount(int volunteerId)
    {
        var assignments = s_dal.Assignment.ReadAll().Where(a => a.VolunteerId == volunteerId);
        return assignments.Count(a => a.CompletionStatus.ToString() == "Treated");
    }

    /// <summary>
    /// Get the number of calls that a volunteer has canceled
    /// </summary>
    /// <param name="volunteerId"></param>
    /// <returns></returns>
    public static int GetTotalCallsCancelled(int volunteerId)
    {
        var assignments = s_dal.Assignment.ReadAll().Where(a => a.VolunteerId == volunteerId);
        return assignments.Count(a => a.CompletionStatus.ToString() == "SelfCancellation");
    }

    /// <summary>
    /// Get the number of assignments that have expired for a volunteer
    /// </summary>
    /// <param name="volunteerId"></param>
    /// <returns></returns>
    public static int GetExpiredAssignmentsCount(int volunteerId)
    {
        var assignments = s_dal.Assignment.ReadAll().Where(a => a.VolunteerId == volunteerId);
        return assignments.Count(a => a.CompletionStatus.ToString() == "Expired");
    }

    /// <summary>
    /// <summary>
    /// Check if there is an assignment for the volunteer that has not been completed yet and return the assignmentID
    /// </summary>
    /// <param name="volunteerId"></param>
    /// <returns></returns>
    public static int? GetPendingAssignmentId(int volunteerId)
    {
        var assignment = s_dal.Assignment.ReadAll().FirstOrDefault(a => a.VolunteerId == volunteerId && a.CompletionTime == null);
        return assignment?.Id;
    }

    /// <summary>
    /// Get the call ID of the pending assignment for a volunteer
    /// </summary>
    /// <param name="volunteerId"></param>
    /// <returns></returns>
    public static int? GetPendingAssignmentCallId(int volunteerId)
    {
        var assignment = s_dal.Assignment.ReadAll().FirstOrDefault(a => a.VolunteerId == volunteerId && a.CompletionTime == null);
        return assignment != null ? assignment.CallId : (int?)null;
    }

    /// <summary>
    /// Get the call type of the pending assignment for a volunteer
    /// </summary>
    /// <param name="volunteerId"></param>
    /// <returns></returns>
    public static BO.CallType? GetPendingAssignmentCallType(int volunteerId)
    {
        int? callId = GetPendingAssignmentCallId(volunteerId);
        if (callId.HasValue)
        {
            int callType = CallManager.GetCallTypeById(callId.Value);
            return (BO.CallType)callType;
        }
        return BO.CallType.None;
    }

    /// <summary>
    /// Sort a list of volunteers by a specific field
    /// </summary>
    /// <param name="volunteerList"></param>
    /// <param name="sortField"></param>
    /// <returns></returns>
    public static IEnumerable<BO.VolunteerInList> SortVolunteers(IEnumerable<BO.VolunteerInList> volunteerList, BO.VolunteerFields? sortField)
    {
        switch (sortField)
        {
            case BO.VolunteerFields.FullName:
                return volunteerList.OrderBy(v => v.FullName);
            case BO.VolunteerFields.TotalCallsHandled:
                return volunteerList.OrderBy(v => v.TotalCallsHandled);
            case BO.VolunteerFields.TotalCallsCancelled:
                return volunteerList.OrderBy(v => v.TotalCallsCancelled);
            case BO.VolunteerFields.TotalExpiredCalls:
                return volunteerList.OrderBy(v => v.TotalExpiredCalls);
            case BO.VolunteerFields.CurrentCallId:
                return volunteerList.OrderBy(v => v.CurrentCallId);
            case BO.VolunteerFields.CurrentCallType:
                return volunteerList.OrderBy(v => v.CurrentCallType);
            default:
                return volunteerList.OrderBy(v => v.Id);
        }
    }

    /// <summary>
    /// Converts a volunteer ID to a BO.Volunteer object
    /// </summary>
    /// <param name="volunteerId">The ID of the volunteer</param>
    /// <returns>A BO.Volunteer object</returns>
    public static BO.Volunteer ConvertVolunteerIdToBO(int volunteerId)
    {
        var volunteer = s_dal.Volunteer.Read(volunteerId);
        return new BO.Volunteer
        {
            Id = volunteerId,
            FullName = volunteer.FullName,
            Email = volunteer.Email,
            PhoneNumber = volunteer.PhoneNumber,
            IsActive = volunteer.IsActive,
            MaxDistance = volunteer.MaxDistance,
            DistanceType = (BO.DistanceType)volunteer.DistanceType,
            Latitude = volunteer.Latitude,
            Longitude = volunteer.Longitude,
            Role = (BO.VolunteerRole)volunteer.VolunteerRole,
            TotalCallsHandled = GetCompletedAssignmentsCount(volunteerId),
            TotalCallsCancelled = GetTotalCallsCancelled(volunteerId),
            TotalExpiredCalls = GetExpiredAssignmentsCount(volunteerId),
            CurrentCall = GetPendingAssignmentCallId(volunteerId) != null ?
                          CallManager.ConvertCallIdToCallInProgress((int)GetPendingAssignmentCallId(volunteerId), volunteerId) :
                          null
        };
    }
}





    











