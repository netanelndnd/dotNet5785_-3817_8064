namespace BO;

using Helpers;
using System;

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
    // - Open: No volunteer is currently handling the call.
    // - InProgress: A volunteer is currently handling the call.
    // - Closed: A volunteer has completed handling the call.
    // - Expired: The call was not handled or not completed within the deadline.
    // - OpenRisk: The call is open and approaching its deadline.
    // - InProgressRisk: The call is being handled and approaching its deadline.

    public int TotalAssignments { get; set; }
    // The total number of assignments associated with the current call.
    // Retrieved from DO.Assignment.
    // Represents how many times the call was taken, canceled, or handled up to its current status.
    public override string ToString() => this.ToStringProperty();
}
