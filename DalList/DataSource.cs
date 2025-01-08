namespace Dal;
internal static class DataSource
{
    internal static IEnumerable<DO.Assignment> Assignments { get; } 
    internal static IEnumerable<DO.Call> Calls { get; } 
    internal static IEnumerable<DO.Volunteer> Volunteers { get; } 
}
