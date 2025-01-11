using Helpers;

namespace BO;

/// <summary>
/// This is a read-only entity, therefore no validation is performed.
/// </summary>
public class CallInProgress
{
    //ההקצאה
    public int Id { get; set; } // Will not appear in the view.
                                // Found by searching DO.Assignment by volunteer Id and appropriate call status (call that has not been completed in any way)

    public int CallId { get; init; } // Taken from DO.Assignment entity

    public CallType CallType { get; init; } // Taken from DO.Call entity

    public string? Description { get; set; } // Taken from DO.Call entity

    public string FullAddress { get; set; } // Taken from DO.Call entity

    public DateTime OpenedAt { get; set; } // Opening time. Taken from DO.Call entity

    public DateTime? MaxCompletionTime { get; set; } // Maximum time to complete the call (can be null).
                                                     // Taken from DO.Call entity

    public DateTime StartedAt { get; set; } // Time when the call started being handled.
                                            // Taken from DO.Call entity
    public double DistanceFromVolunteer { get; set; } // Distance of the call from the volunteer handling it

    public CallStatus Status { get; init; } // Status of the call (ENUM)
    public override string ToString() => this.ToStringProperty();
}

