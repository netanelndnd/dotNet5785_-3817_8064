namespace BO;

using Helpers;
using System;

/// <summary>
/// Represents a call in the system with various details such as assignment, type, status, and timing information.
/// Entity for viewing only
/// </summary>
public class CallInList
{
    public int? AssignmentId { get; init; }
    // Assignment ID associated with the call.
    // Retrieved from DO.Assignment.
    // Null if no assignment has been made yet.
    // Not displayed in the UI.

    public int CallId { get; init; }
    // Unique identifier of the call.
    // Retrieved from DO.Assignment.

    public CallType CallType { get; init; }
    // The type of the call, represented as an ENUM.
    // Retrieved from DO.Call.

    public DateTime OpeningTime { get; init; }
    // The time when the call was created.
    // Retrieved from DO.Call.

    public TimeSpan? RemainingTime { get; init; }
    // The remaining time until the call deadline.
    // Calculated based on the maximum completion time from DO.Call 
    // and the current system time. Null if no maximum time is set.

    public string? LastVolunteerName { get; set; }
    // The name of the last volunteer associated with the call.
    // Retrieved from a combination of DO.Assignment and DO.Volunteer.
    // Null if no volunteer has handled the call.

    public TimeSpan? CompletionDuration { get; set; }
    // The time it took to complete the call.
    // Calculated as the difference between the completion time and opening time.
    // Relevant only for calls that have been completed. Null otherwise.

    public CallStatus Status { get; init; }
    // The status of the call, represented as an ENUM.
    // Computed based on:
    // - Assignment completion type in DO.Assignment.
    // - Maximum completion time in DO.Call.
    // - Current system time.
    // Possible statuses:
    // Open - Not currently assigned to any volunteer
    // InProgress - Currently being handled by a volunteer
    // Treated - Completed by a volunteer
    // Expired - Not handled or not completed in time
    // OpenInRisk - Open and approaching MaxCompletionTime
    // InProgressInRisk - InProgress and approaching MaxCompletionTime

    public int TotalAssignments { get; set; }
    // The total number of assignments associated with the current call.
    // Retrieved from DO.Assignment.
    // Represents how many times the call was taken, canceled, or handled up to its current status.
    public override string ToString() => this.ToStringProperty();
}



