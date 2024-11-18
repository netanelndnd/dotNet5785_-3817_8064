namespace DO;


public record Volunteer
{
    public int Id { get; set; }  // ID - must be numeric and unique
    public string FullName { get; set; }  // Full name - cannot be null or empty
    public string PhoneNumber { get; set; }  // Mobile phone - must be 10 digits and start with 0
    public string Email { get; set; }  // Email - must be in a valid format
    public string? Password { get; set; }  // Password - can be null, will be validated in the logic layer

    public string? CurrentAddress { get; set; }  // Current full address - can be null
    public double? Latitude { get; set; }  // Latitude - updated by the logic layer based on the volunteer's address
    public double? Longitude { get; set; }  // Longitude - updated by the logic layer based on the volunteer's address

    public Role VolunteerRole { get; set; }  // Role - ENUM with values: "Manager" or "Volunteer"
    public bool IsActive { get; set; }  // Active status - indicates if the volunteer is active or retired
    public double? MaxDistance { get; set; }  // Maximum distance to accept a call - can be null (no distance limit)
    public DistanceType DistanceType { get; set; }  // Distance type - ENUM with default: Air distance

    public Volunteer()
    {
        VolunteerRole = Role.Volunteer;  // Default role: Volunteer
        IsActive = true;  // Default: volunteer is active
        DistanceType = DistanceType.AirDistance;  // Default: Air distance
    }
}
