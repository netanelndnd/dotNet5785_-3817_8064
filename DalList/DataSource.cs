namespace Dal;
internal static class DataSource
{
    internal static IEnumerable<DO.Assignment> Assignments { get; set; } = Enumerable.Empty<DO.Assignment>();
    internal static IEnumerable<DO.Call> Calls { get; set; } = Enumerable.Empty<DO.Call>();
    internal static IEnumerable<DO.Volunteer> Volunteers { get; set; } = Enumerable.Empty<DO.Volunteer>();
}
