namespace DO;
using DalApi;
/// <summary>
/// Represents an assignment of a volunteer to a call.
/// </summary>
/// <param name="Id">Unique identifier for the assignment, auto-incremented</param>
/// <param name="CallId">ID of the call the volunteer chose to handle</param>
/// <param name="VolunteerId">ID of the volunteer handling the call</param>
/// <param name="EntryTime">Entry time - date and time when the volunteer started handling the call</param>
/// <param name="CompletionTime">Actual completion time - date and time when the volunteer finished handling the call, can be null if not completed</param>
/// <param name="CompletionStatus">Type of completion - ENUM, can be null if the call is still active</param>
public record Assignment(
    int Id,
    int CallId,
    int VolunteerId,
    DateTime EntryTime,
    DateTime? CompletionTime,
    CompletionType? CompletionStatus
)
{
    /// <summary>
    /// Default constructor for the assignment record.
    /// </summary>
    //לשנות את זמן הכניסה
    public Assignment() : this(0, 0, 0, DateTime.Now, null, null) { }

}

