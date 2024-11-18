namespace DO;


public record Assignment
{
    public int Id { get; set; }  // Unique identifier for the assignment, auto-incremented
    public int CallId { get; set; }  // ID of the call the volunteer chose to handle
    public int VolunteerId { get; set; }  // ID of the volunteer handling the call

    public DateTime EntryTime { get; set; }  // Entry time - date and time when the volunteer started handling the call
    public DateTime? CompletionTime { get; set; }  // Actual completion time - date and time when the volunteer finished handling the call, can be null if not completed

    public CompletionType? CompletionStatus { get; set; }  // Type of completion - ENUM, can be null if the call is still active

    public Assignment()
    {
        EntryTime = DateTime.Now;  // Set the entry time to the current system time
    }
}

