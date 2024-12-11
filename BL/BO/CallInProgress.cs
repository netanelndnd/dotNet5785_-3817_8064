using Helpers;

namespace BO;

public class CallInProgress
{
    public int Id { get; set; } // Unique identifier for the allocation entity (not displayed in the view)

    public int CallId { get; init; } // Unique identifier for the call entity

    public CallType CallType { get; init; } // Type of the call (ENUM)

    public string? Description { get; set; } // Verbal description (can be null)

    public string FullAddress { get; set; } // Full address of the call

    public DateTime OpenedAt { get; set; } // Opening time

    public DateTime? MaxCompletionTime { get; set; } // Maximum time to complete the call (can be null)

    public DateTime StartedAt { get; set; } // Time when the call started being handled

    public double DistanceFromVolunteer { get; set; } // Distance of the call from the volunteer handling it

    public CallStatus Status { get; init; } // Status of the call (ENUM)
    public override string ToString() => this.ToStringProperty();
}

