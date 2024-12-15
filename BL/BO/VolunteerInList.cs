using Helpers;

namespace BO;

/// <summary>
/// This is a read-only entity, therefore no validation is performed.
/// </summary>
public class VolunteerInList
{
    public int Id { get; init; } // Volunteer ID (retrieved from DO.Volunteer)

    public string FullName { get; set; } // Full name (first and last name). retrieved from DO.Volunteer

    public bool IsActive { get; init; } // Indicates if the volunteer is active (retrieved from DO.Volunteer)

    public int TotalCallsHandled { get; set; } // Total number of calls handled by the volunteer. 
    // Query for all assignment entities belonging to the volunteer where the end type is "Treated"

    public int TotalCallsCancelled { get; set; } // Total number of calls canceled by the volunteer.
    // Query for all assignment entities belonging to the volunteer where the end type is "SelfCancellation"

    public int TotalExpiredCalls { get; set; } // Total number of calls that expired under the volunteer's responsibility
    // Query for all assignment entities belonging to the volunteer where the end type is "Expired"

    public int? CurrentCallId { get; set; }
    // Check if there is an assignment entity for the volunteer where the actual end time is still null
    public CallType CurrentCallType { get; init; }
    // The type of the current call being handled by the volunteer (None if no call is in progress)
    public override string ToString() => this.ToStringProperty();
}



