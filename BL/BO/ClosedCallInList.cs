using Helpers;

namespace BO;

/// <summary>
/// Entity for viewing only
/// </summary>
public class ClosedCallInList
{
    public int Id { get; init; }
    // Unique ID of the call (retrieved from DO.Call)

    public CallType CallType { get; init; }
    // The type of the call (retrieved from DO.Call)

    public string FullAddress { get; set; }
    // The full address of the call (retrieved from DO.Call)

    public DateTime OpenedAt { get; set; }
    // The time when the call was opened (retrieved from DO.Call)

    public DateTime StartedAt { get; set; }
    // The time when the call was assigned to a volunteer for handling (retrieved from DO.Assignment)

    public DateTime? CompletedAt { get; set; }
    // The actual time when the call was completed (null if not applicable, retrieved from DO.Assignment)

    public CompletionType? CompletionStatus { get; init; }
    // The type of completion for the call (null if not applicable, retrieved from DO.Assignment)
    public override string ToString() => this.ToStringProperty();
}


