using DalApi;

namespace Helpers;

internal static class AssignmentManager
{
    private static IDal s_dal = Factory.Get; //stage 4

    /// <summary>
    /// Get the assignment id by call id
    /// </summary>
    /// <param name="callId">The identifier of the call.</param>
    /// <returns>The identifier of the assignment or null if not found.</returns>
    public static int? GetAssignmentIdByCallId(int callId)
    {
        var assignments = s_dal.Assignment.ReadAll();
        // Return the last assignment created for the call
        var assignment = assignments
            .Where(a => a.CallId == callId)
            .OrderByDescending(a => a.EntryTime)
            .FirstOrDefault();
        return assignment?.Id;
    }



    /// <summary>
    /// Get all assignments sorted by entry time
    /// </summary>
    /// <returns>A collection of assignments sorted by entry time in ascending order.</returns>
    public static IEnumerable<DO.Assignment> GetAssignmentsSortedByEntryTime()
    {
        var assignments = s_dal.Assignment.ReadAll();
        // Sorted by ascending order of entry time
        return assignments.OrderBy(a => a.EntryTime);
    }

    /// <summary>
    /// Count the number of assignments for a given call id
    /// </summary>
    /// <param name="callId">The identifier of the call.</param>
    /// <returns>The number of assignments for the given call id.</returns>
    public static int CountAssignmentsByCallId(int callId)
    {
        var assignments = s_dal.Assignment.ReadAll();
        return assignments.Count(a => a.CallId == callId);
    }

    /// <summary>
    /// Get the volunteer name by call id
    /// </summary>
    /// <param name="callId">The identifier of the call.</param>
    /// <returns>The name of the volunteer or null if not found.</returns>
    public static string? GetVolunteerNameByCallId(int callId)
    {
        var assignments = s_dal.Assignment.ReadAll();
        // Return the last assignment created for the call
        var assignment = assignments
            .Where(a => a.CallId == callId)
            .OrderByDescending(a => a.EntryTime)
            .FirstOrDefault();
        if (assignment == null)
        {
            return null;
        }
        var volunteer = s_dal.Volunteer.Read(assignment.VolunteerId);
        return volunteer?.FullName;
    }

    /// <summary>
    /// Get the time difference between the completion time and entry time of the last assignment for a given call id.
    /// </summary>
    /// <param name="callId">The identifier of the call.</param>
    /// <returns>The time difference as TimeSpan if the call was treated, otherwise null.</returns>
    public static TimeSpan? GetTimeDifferenceForLastAssignment(int callId)
    {
        var assignments = s_dal.Assignment.ReadAll();
        // Get the last assignment created for the call
        var assignment = assignments
            .Where(a => a.CallId == callId)
            .OrderByDescending(a => a.EntryTime)
            .FirstOrDefault();
        // Return null if the assignment is null or the call was not treated
        if (assignment == null || assignment.CompletionTime == null)
        {
            return null;
        }
        // Get the call associated with the assignment
        var call = s_dal.Call.Read(callId);
        if (call == null)
        {
            return null;
        }
        // Return the time difference between the completion time and entry time
        return call.OpenTime - assignment.CompletionTime;
    }

    /// <summary>
    /// Get a list of call assignments for a given call id.
    /// </summary>
    /// <param name="callId">The identifier of the call.</param>
    /// <returns>A list of call assignments or null if none found.</returns>
    public static List<BO.CallAssignInList>? GetCallAssignmentsByCallId(int callId)
    {
        var assignments = s_dal.Assignment.ReadAll()
            .Where(a => a.CallId == callId)
            .ToList();

        if (!assignments.Any())
        {
            return null;
        }

        var callAssignInList = assignments.Select(a => new BO.CallAssignInList
        {
            VolunteerId = a.VolunteerId,
            VolunteerName = s_dal.Volunteer.Read(a.VolunteerId)?.FullName,
            StartTime = a.EntryTime,
            EndTime = a.CompletionTime,
            CompletionType = (BO.CompletionType?)a.CompletionStatus
        }).ToList();

        return callAssignInList;
    }
    
    
}
