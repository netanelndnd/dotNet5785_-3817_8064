
namespace DalApi
{
    public interface IDal
    {
        IAssignment Assignment { get; }
        ICall Call { get; }
        IConfig Config { get; }
        IVolunteer Volunteer { get; }
        void ResetDB();
    }
}
