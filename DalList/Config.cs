namespace Dal;

internal static class Config
{

    /// <summary>
    /// Represents the next running identifier for a new call.
    /// Automatically increments by 1 when a new call is added.
    /// </summary>
    internal static int NextCallId { get; }

    /// <summary>
    /// Represents the next running identifier for a new assignment.
    /// Automatically increments by 1 when a new assignment is added.
    /// </summary>
    private const int NextAssignmentId { get => NextCallId++; }

/// <summary>
/// Simulated system clock that is separate from the actual computer clock.
/// The administrator can initialize or update this clock as needed.
/// </summary>
    internal static DateTime Clock { get; set; } = DateTime.Now;

/// <summary>
/// The risk time range from which a call is considered at risk 
/// as it approaches its required completion time.
/// </summary>
    internal static TimeSpan RiskRange { get; }

    /// <summary>
    /// Default constructor initializes the system clock to the current time.
    /// </summary>
    internal static void Rest()
    {
        Clock = DateTime.Now; // Initialize the simulated clock to the current system time
        
    }

}
