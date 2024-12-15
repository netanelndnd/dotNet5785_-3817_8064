using Helpers;

namespace BO;

/// <summary>
/// Entity for viewing only
/// </summary>
public class CallAssignInList
{
    public int? VolunteerId { get; set; }
    // Volunteer ID associated with the assignment.
    // Retrieved from DO.Assignment.
    // Can be null in cases where the assignment was artificially created for calls 
    // that were opened but never handled, and were marked with a completion type of "Expired Cancellation".

    public string? VolunteerName { get; set; }
    // Name of the volunteer (first and last name).
    // Retrieved from DO.Volunteer.
    // Can be null if no volunteer is assigned to this assignment.

    public DateTime StartTime { get; set; }
    // The timestamp when the assignment started.
    // Retrieved from DO.Assignment.

    public DateTime? EndTime { get; set; }
    // The actual end time of the assignment.
    // Retrieved from DO.Assignment.
    // Can be null if the assignment is still ongoing.

    public CompletionType? CompletionType { get; init; }
    // Type of assignment completion.
    // Retrieved from DO.Assignment.
    // Can be null for assignments that are still ongoing or have no specified completion type.
    public override string ToString() => this.ToStringProperty();
}
