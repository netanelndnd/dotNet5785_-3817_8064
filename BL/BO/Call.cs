namespace BO;

using Helpers;
using System;
using System.Collections.Generic;

public class Call
{
    public int Id { get; init; }
    // Unique ID for the call, retrieved from DO.Call

    public CallType CallType { get; init; }
    // Type of the call, defined in an ENUM, retrieved from DO.Call

    public string? Description { get; set; }
    // Optional textual description, retrieved from DO.Call

    public string FullAddress { get; set; }
    // Full address of the call. During viewing, retrieved from DO.Call.
    // During creation or update, its validity must be checked in the logical layer.
    // If invalid, an exception should be thrown.
    // Latitude and Longitude are updated based on this address.

    public double Latitude { get; set; }
    // Latitude of the call location. Updated whenever the address changes.
    // Calculated using geolocation functions. This value is used for distance calculations and not displayed.

    public double Longitude { get; set; }
    // Longitude of the call location. Updated whenever the address changes.
    // Calculated using geolocation functions. This value is used for distance calculations and not displayed.

    public DateTime OpenedAt { get; set; }
    // The timestamp when the call was opened. 
    // During viewing, retrieved from DO.Call. 
    // During creation, assigned using the system clock.

    public DateTime? MaxCompletionTime { get; set; }
    // Maximum allowed time to complete the call. 
    // Must be later than OpenedAt and the current time, validated in the logical layer.

    public CallStatus Status { get; init; }
    // Status of the call. Computed based on the assignment's completion type, MaxCompletionTime, and the current system clock:
    // Open - Not currently assigned to any volunteer
    // InProgress - Currently being handled by a volunteer
    // Treated - Completed by a volunteer
    // Expired - Not handled or not completed in time
    // OpenInRisk - Open and approaching MaxCompletionTime
    // InProgressInRisk - InProgress and approaching MaxCompletionTime

    public List<CallAssignInList>? Assignments { get; set; }
    // A list of assignments related to this call. Each assignment is of type BO.CallAssignInList.
    // If no assignments exist yet, this will be null.
    public override string ToString() => this.ToStringProperty();
}
