
namespace Dal;
using DalApi;


sealed public class DalList : IDal
{
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


