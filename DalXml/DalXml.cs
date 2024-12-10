namespace Dal;
using DalApi;



sealed internal class DalXml : IDal
{
    public static IDal Instance { get; } = new DalXml(); //stage 4
    private DalXml() { } // constructor ,stage 4

    public IAssignment Assignment { get; } = new AssignmentImplementation();

    public ICall Call { get; } = new CallImplementation();

    public IConfig Config { get; } = new ConfigImplementation();

    public IVolunteer Volunteer { get; } = new VolunteerImplementation();

    public void ResetDB()
    {
        Assignment.DeleteAll();
        Call.DeleteAll();
        Volunteer.DeleteAll();
        Config.Reset();
    }
}
