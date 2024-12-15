using Helpers;

namespace BO;

/// <summary>
/// Represents a volunteer in the system.
/// </summary>
public class Volunteer
{
    public int Id { get; set; } // Volunteer ID - required, cannot be updated after addition

    public string FullName { get; set; } // Full name - first and last

    public string PhoneNumber { get; set; } // Mobile phone

    public string Email { get; set; } // Email

    public string? Password { get; set; } // Password - can be null, must be encrypted

    public string? CurrentAddress { get; set; } // Current full address

    public double? Latitude { get; set; } // Latitude - updated according to address

    public double? Longitude { get; set; } // Longitude - updated according to address

    public VolunteerRole Role { get; init; } // Role (ENUM: "Manager" or "Volunteer")

    public bool IsActive { get; init; } // Is active

    public double? MaxDistance { get; set; } // Maximum distance to receive a call (can be null)

    public DistanceType DistanceType { get; init; } // Distance type (ENUM)

    public int TotalCallsHandled { get; set; } // Total calls handled (view only)

    public int TotalCallsCancelled { get; set; } // Total calls cancelled (view only)
    
    public int TotalExpiredCalls { get; set; } // Total expired calls (view only)

    /// <summary>
    /// Current call in progress, if any.
    /// </summary>
    public CallInProgress? CurrentCall { get; set; } 
    public override string ToString() => this.ToStringProperty();
}
