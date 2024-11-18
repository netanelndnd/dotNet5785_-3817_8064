namespace DO;



public record Call
{
    public int Id { get; set; }  // Unique identifier for the call, auto-incremented
    public CallType CallType { get; set; }  // Type of the call - ENUM with specific values for different types of calls
    public string? Description { get; set; }  // Description of the call - additional details about the call, can be null

    public string Address { get; set; }  // Full address of the call - must be a valid format, cannot be null
    public double Latitude { get; set; }  // Latitude - calculated by logic layer based on address, used for distance calculations
    public double Longitude { get; set; }  // Longitude - calculated by logic layer based on address, used for distance calculations

    public DateTime OpenTime { get; set; }  // Opening time - the date and time when the call was created
    public DateTime? MaxCompletionTime { get; set; }  // Maximum completion time - deadline for the call to be completed, can be null

    public Call()
    {
        OpenTime = DateTime.Now;  // Set default opening time to current system time
    }
}
