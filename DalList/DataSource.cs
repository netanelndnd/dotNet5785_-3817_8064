namespace Dal;
internal static class DataSource
{
    internal static IEnumerable<DO.Assignment> Assignments { get; } = Enumerable.Empty<DO.Assignment>(); 
    internal static IEnumerable<DO.Call> Calls { get; } = Enumerable.Empty<DO.Call>();
    internal static IEnumerable<DO.Volunteer> Volunteers { get; } = Enumerable.Empty<DO.Volunteer>();
}
