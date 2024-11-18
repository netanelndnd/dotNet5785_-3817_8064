namespace Dal;

internal static class Config
{

    internal const int startCallId = 1;
    private static int nextCallId = startCallId;
    internal static int NextCallId { get => nextCallId++; }

    internal const int startAssigmentId = 1;
    private static int nextAssigmentId = startAssigmentId;
    internal static int NextAssigmentId { get => nextAssigmentId++; }

    internal static DateTime Clock { get; set; } = DateTime.Now;
    internal static TimeSpan RiskRange { get; set; } = TimeSpan.FromHours(2);


    internal static void Rest()
    {
        Clock = DateTime.Now; // Initialize the simulated clock to the current system time
        nextCallId = startCallId;
        nextAssigmentId = startAssigmentId;
    }

}
