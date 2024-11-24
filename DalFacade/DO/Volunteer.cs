namespace DO;

/// <summary>
/// Represents a volunteer with various attributes such as ID, full name, phone number, email, etc.
/// </summary>
/// <param name="Id">ID - must be numeric and unique</param>
/// <param name="FullName">Full name - cannot be null or empty</param>
/// <param name="PhoneNumber">Mobile phone - must be 10 digits and start with 0</param>
/// <param name="Email">Email - must be in a valid format</param>
/// <param name="Password">Password - can be null, will be validated in the logic layer</param>
/// <param name="CurrentAddress">Current full address - can be null</param>
/// <param name="Latitude">Latitude - updated by the logic layer based on the volunteer's address</param>
/// <param name="Longitude">Longitude - updated by the logic layer based on the volunteer's address</param>
/// <param name="MaxDistance">Maximum distance to accept a call - can be null (no distance limit)</param>
/// <param name="VolunteerRole">Role - ENUM with values: "Manager" or "Volunteer"</param>
/// <param name="IsActive">Active status - indicates if the volunteer is active or retired</param>
/// <param name="DistanceType">Distance type - ENUM with default: Air distance</param>
public record Volunteer(
  int Id,
  string FullName,
  string PhoneNumber,
  string Email,
  string? Password,
  string? CurrentAddress,
  double? Latitude,
  double? Longitude,
  double? MaxDistance,
  Role VolunteerRole = Role.Volunteer,
  bool IsActive = true,
  DistanceType DistanceType = DistanceType.AirDistance
)
{
    public Volunteer() : this(0, string.Empty, "0000000000", string.Empty, null, null, null, null, null, Role.Volunteer, true, DistanceType.AirDistance) { }
}

