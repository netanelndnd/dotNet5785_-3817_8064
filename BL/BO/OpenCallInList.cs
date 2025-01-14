using Helpers;

namespace BO;

/// <summary>
/// Entity for viewing only
/// </summary>
public class OpenCallInList
{
    public int Id { get; init; }
    // Unique ID of the call (retrieved from DO.Call)

    public CallType CallType { get; init; }
    // The type of the call (retrieved from DO.Call)

    public string? Description { get; set; }
    // A textual description of the call (nullable, retrieved from DO.Call)

    public string FullAddress { get; set; }
    // The full address of the call (retrieved from DO.Call)

    public DateTime OpenedAt { get; set; }
    // The time when the call was opened (retrieved from DO.Call)

    public DateTime? MaxCompletionTime { get; set; }
    // The maximum allowed time to complete the call (nullable, retrieved from DO.Call)

    public double? DistanceFromVolunteer { get; set; }
    // The distance from the volunteer to the call, calculated in the logical layer
    public override string ToString() => this.ToStringProperty();
}

