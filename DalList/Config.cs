using System.Runtime.CompilerServices;

namespace Dal;

internal static class Config
{
    /// <summary>
    /// Starting value for call IDs
    /// </summary>
    internal const int startCallId = 1;

    /// <summary>
    /// Next call ID to be assigned
    /// </summary>
    private static int nextCallId = startCallId;

    /// <summary>
    /// Property to get the next call ID and increment it
    /// </summary>
    internal static int NextCallId
    {
        [MethodImpl(MethodImplOptions.Synchronized)]
        get => nextCallId++;
    }

    /// <summary>
    /// Starting value for assignment IDs
    /// </summary>
    internal const int startAssignmentId = 1;

    /// <summary>
    /// Next assignment ID to be assigned
    /// </summary>
    private static int nextAssignmentId = startAssignmentId;

    /// <summary>
    /// Property to get the next assignment ID and increment it
    /// </summary>
    internal static int NextAssignmentId
    {
        [MethodImpl(MethodImplOptions.Synchronized)]
        get => nextAssignmentId++;
    }

    /// <summary>
    /// Simulated clock, initialized to the current system time
    /// </summary>
    internal static DateTime Clock
    {
        [MethodImpl(MethodImplOptions.Synchronized)]
        get;
        [MethodImpl(MethodImplOptions.Synchronized)]
        set;
    } = DateTime.Now;

    /// <summary>
    /// Time span representing the risk range, set to 2 hours
    /// </summary>
    internal static TimeSpan RiskRange
    {
        [MethodImpl(MethodImplOptions.Synchronized)]
        get;
        [MethodImpl(MethodImplOptions.Synchronized)]
        set;
    } = TimeSpan.FromHours(2);

    /// <summary>
    /// Method to reset the configuration to its initial state
    /// </summary>
    [MethodImpl(MethodImplOptions.Synchronized)]
    internal static void Reset()
    {
        // Reset the simulated clock to the current system time
        Clock = DateTime.Now;
        // Reset the next call ID to the starting value
        nextCallId = startCallId;
        // Reset the next assignment ID to the starting value
        nextAssignmentId = startAssignmentId;
    }
}
