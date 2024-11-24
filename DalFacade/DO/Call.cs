namespace DO;

/// <summary>
/// Represents a call record with details such as type, address, coordinates, and timestamps.
/// </summary>
/// <param name="Id">Unique identifier for the call, auto-incremented</param>
/// <param name="CallType">Type of the call - ENUM with specific values for different types of calls</param>
/// <param name="Address">Full address of the call - must be a valid format, cannot be null</param>
/// <param name="Latitude">Latitude - calculated by logic layer based on address, used for distance calculations</param>
/// <param name="Longitude">Longitude - calculated by logic layer based on address, used for distance calculations</param>
/// <param name="OpenTime">Opening time - the date and time when the call was created</param>
/// <param name="Description">Description of the call - additional details about the call, can be null</param>
/// <param name="MaxCompletionTime">Maximum completion time - deadline for the call to be completed, can be null</param>
public record Call(
    int Id,
    CallType CallType,
    string Address,
    double Latitude,
    double Longitude,
    DateTime OpenTime,
    string? Description = null,
    DateTime? MaxCompletionTime = null
)
{
    public Call() : this(0, default, string.Empty, 0.0, 0.0, DateTime.Now) { }
}
