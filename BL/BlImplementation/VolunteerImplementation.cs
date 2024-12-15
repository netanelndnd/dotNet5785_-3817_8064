
using BlApi;
using Helpers;
//using BO;

namespace BlImplementation
{

    internal class VolunteerImplementation : IVolunteer
    {

        private readonly DalApi.IDal _dal = DalApi.Factory.Get;

        public void AddVolunteer(BO.Volunteer volunteer)
        {

            throw new NotImplementedException();
        }

        public void DeleteVolunteer(int id)
        {

            throw new NotImplementedException();
        }

        /// <summary>
        /// Get the details of a volunteer
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="KeyNotFoundException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public BO.Volunteer GetVolunteerDetails(int id)
        {
            try
            {
                var volunteer = _dal.Volunteer.Read(id);
                if (volunteer == null)
                {
                    throw new KeyNotFoundException($"Volunteer with ID {id} not found.");
                }

                return VolunteerManager.ConvertVolunteerIdToBO(id);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to retrieve volunteer details.", ex);
            }
        }


        public IEnumerable<BO.VolunteerInList> GetVolunteers(bool? isActive, BO.VolunteerFields? sortField)
        {
            var volunteers = _dal.Volunteer.ReadAll();

            if (isActive.HasValue)
            {
                volunteers = volunteers.Where(v => v.IsActive == isActive.Value);
            }

            var volunteerList = volunteers.Select(v => new BO.VolunteerInList
            {
                Id = v.Id,
                FullName = v.FullName,
                IsActive = v.IsActive,
                TotalCallsHandled = VolunteerManager.GetCompletedAssignmentsCount(v.Id),
                TotalCallsCancelled = VolunteerManager.GetTotalCallsCancelled(v.Id),
                TotalExpiredCalls = VolunteerManager.GetExpiredAssignmentsCount(v.Id),
                CurrentCallId = VolunteerManager.GetPendingAssignmentCallId(v.Id),
                CurrentCallType = VolunteerManager.GetPendingAssignmentCallType(v.Id) ?? BO.CallType.None,


            });
            var sortedVolunteerList = VolunteerManager.SortVolunteers(volunteerList, sortField);
            return sortedVolunteerList;
        }


        public string Login(string username, string password)
        {
            var volunteer = _dal.Volunteer.Read(v => v.Email == username);//השם משתמש הוא האימייל של המתנדב
            if (volunteer == null || volunteer.Password != password)
            {
                throw new InvalidOperationException("Invalid username or password.");
            }
            return volunteer.VolunteerRole.ToString();
        }

        public void UpdateVolunteer(int id, BO.Volunteer volunteer)
        {
            throw new NotImplementedException();
        }

    }
}
