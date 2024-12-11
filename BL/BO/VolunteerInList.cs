using Helpers;

namespace BO;
public class VolunteerInList
{
    public int Id { get; init; } // Volunteer ID (retrieved from DO.Volunteer)

    public string FullName { get; set; } // Full name (first and last name)

    public bool IsActive { get; init; } // Indicates if the volunteer is active (retrieved from DO.Volunteer)

    public int TotalCallsHandled { get; set; } // Total number of calls handled by the volunteer

    public int TotalCallsCancelled { get; set; } // Total number of calls canceled by the volunteer

    public int TotalExpiredCalls { get; set; } // Total number of calls that expired under the volunteer's responsibility

    public int? CurrentCallId { get; set; }
    // The ID of the current call being handled by the volunteer (null if no call is in progress)

    public CallType CurrentCallType { get; init; }
    // The type of the current call being handled by the volunteer (None if no call is in progress)
    public override string ToString() => this.ToStringProperty();
}
