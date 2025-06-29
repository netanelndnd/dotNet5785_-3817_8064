﻿using BO;
using DalApi;

namespace Helpers;

public static class AssignmentManager
{
    private static IDal s_dal = Factory.Get; //stage 4

    internal static ObserverManager Observers = new(); //stage 5

    /// <summary>
    /// Get the assignment id by call id
    /// </summary>
    /// <param name="callId">The identifier of the call.</param>
    /// <returns>The identifier of the assignment or null if not found.</returns>
    public static int? GetAssignmentIdByCallId(int callId)
    {
        IEnumerable<DO.Assignment> assignments;

        lock (AdminManager.BlMutex) //stage 7
        {
            assignments = s_dal.Assignment.ReadAll();
        }

        var assignment = assignments
            .Where(a => a.CallId == callId)
            .OrderByDescending(a => a.EntryTime)
            .FirstOrDefault();

        return assignment?.Id;
    }

    /// <summary>
    /// Count the number of assignments for a given call id
    /// </summary>
    /// <param name="callId">The identifier of the call.</param>
    /// <returns>The number of assignments for the given call id.</returns>
    public static int CountAssignmentsByCallId(int callId)
    {
        IEnumerable<DO.Assignment> assignments;

        lock (AdminManager.BlMutex) //stage 7
        {
            assignments = s_dal.Assignment.ReadAll();
        }

        return assignments.Count(a => a.CallId == callId);
    }

    /// <summary>
    /// Get the volunteer name by call id
    /// </summary>
    /// <param name="callId">The identifier of the call.</param>
    /// <returns>The name of the volunteer or null if not found.</returns>
    public static string? GetVolunteerNameByCallId(int callId)
    {
        DO.Assignment? assignment;
        DO.Volunteer? volunteer = null;

        lock (AdminManager.BlMutex) //stage 7
        {
            var assignments = s_dal.Assignment.ReadAll();
            // Return the last assignment created for the call
            assignment = assignments
                .Where(a => a.CallId == callId)
                .OrderByDescending(a => a.EntryTime)
                .FirstOrDefault();
            if (assignment != null)
            {
                volunteer = s_dal.Volunteer.Read(assignment.VolunteerId);
            }
        }

        return volunteer?.FullName;
    }

    /// <summary>
    /// Get the time difference between the completion time and entry time of the last assignment for a given call id.
    /// </summary>
    /// <param name="callId">The identifier of the call.</param>
    /// <returns>The time difference as TimeSpan if the call was treated, otherwise null.</returns>
    public static TimeSpan? GetTimeDifferenceForLastAssignment(int callId)
    {
        DO.Assignment? assignment;
        DO.Call? call = null;
        BO.CallStatus? callStatus = null;

        lock (AdminManager.BlMutex) //stage 7
        {
            var assignments = s_dal.Assignment.ReadAll();
            // Get the last assignment created for the call
            assignment = assignments
                .Where(a => a.CallId == callId)
                .OrderByDescending(a => a.EntryTime)
                .FirstOrDefault();
            if (assignment != null && assignment.CompletionTime != null)
            {
                call = s_dal.Call.Read(callId);
                if (call != null)
                {
                    callStatus = CallManager.GetCallStatus(callId);
                }
            }
        }

        // Return null if the assignment is null, the call is null, or the call was not treated
        if (assignment == null || assignment.CompletionTime == null || call == null ||
            (callStatus != BO.CallStatus.Expired && callStatus != BO.CallStatus.Treated))
        {
            return null;
        }

        // Return the time difference between the completion time and entry time
        return (call.OpenTime - assignment.CompletionTime) * -1;
    }

    /// <summary>
    /// Get a list of call assignments for a given call id.
    /// </summary>
    /// <param name="callId">The identifier of the call.</param>
    /// <returns>A list of call assignments or null if none found.</returns>
    public static IEnumerable<BO.CallAssignInList>? GetCallAssignmentsByCallId(int callId)
    {
        IEnumerable<DO.Assignment> assignments;
        IEnumerable<CallAssignInList>? callAssignInList;

        lock (AdminManager.BlMutex) //stage 7
        {
            assignments = s_dal.Assignment.ReadAll()
                .Where(a => a.CallId == callId);
        }

        if (!assignments.Any())
        {
            return null;
        }
        lock (AdminManager.BlMutex) //stage 7
        {
            callAssignInList = assignments.Select(a => new BO.CallAssignInList
            {
                VolunteerId = a.VolunteerId,
                VolunteerName = s_dal.Volunteer.Read(a.VolunteerId)?.FullName,
                StartTime = a.EntryTime,
                EndTime = a.CompletionTime,
                CompletionType = (BO.CompletionType?)a.CompletionStatus
            });
        }

        return callAssignInList;
    }


    private static int s_periodicCounter = 0;

    // פונקציה לבדיקת הקצאות שפג תוקפן
    public static void CheckAndExpireAssignments()
    {
        List<DO.Assignment> assignments;
        DateTime now;

        Thread.CurrentThread.Name = $"Periodic{++s_periodicCounter}"; //stage 7 (optional)

        lock (AdminManager.BlMutex) //stage 7
        {
            assignments = s_dal.Assignment.ReadAll().ToList();
            now = AdminManager.Now;
        }
        lock (AdminManager.BlMutex) //stage 7
        {
            foreach (var assignment in assignments)
            {
                var call = s_dal.Call.Read(assignment.CallId);
                if (call == null)
                {
                    continue;
                }

                if (assignment.CompletionStatus == null && now > call.MaxCompletionTime)
                {
                    var updatedAssignment = assignment with
                    {
                        CompletionStatus = DO.CompletionType.Expired,
                        CompletionTime = now
                    };

                    s_dal.Assignment.Update(updatedAssignment);
                }
            }
        }

        CallManager.Observers.NotifyListUpdated();
        VolunteerManager.Observers.NotifyListUpdated();
    }
}
