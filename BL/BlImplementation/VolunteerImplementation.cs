
using BlApi;
using BO;
using DalApi;
using DO;
using Helpers;
//using BO;

namespace BlImplementation
{

    internal class VolunteerImplementation : BlApi.IVolunteer
    {

        private readonly DalApi.IDal _dal = DalApi.Factory.Get;

        public void AddVolunteer(BO.Volunteer volunteerB)
        {
            try
            {

                // Validate the email format
                bool chekemail = VolunteerManager.IsValidEmail(volunteerB.Email);

                // Validate the ID format
                bool chekid = VolunteerManager.IsValidID(volunteerB.Id);

                // Validate the phone number format
                bool chekphone = VolunteerManager.IsValidPhoneNumber(volunteerB.PhoneNumber);

                // Check if the location is within Israel
                bool chekLatitudeandLongitude = VolunteerManager.IsLocationInIsrael(volunteerB.Latitude, volunteerB.Longitude);

                // If all validations pass
                if (chekemail == true && chekphone == true && chekLatitudeandLongitude == true && chekid == true)
                {
                    DO.Volunteer volunteerD = new()
                    {
                        Id = volunteerB.Id,
                        Email = volunteerB.Email,
                        Password = volunteerB.Password,
                        PhoneNumber = volunteerB.PhoneNumber,
                        CurrentAddress = volunteerB.CurrentAddress,
                        FullName = volunteerB.FullName,
                        Latitude = volunteerB.Latitude,
                        Longitude = volunteerB.Longitude,
                        MaxDistance = volunteerB.MaxDistance,
                        IsActive = true,
                        VolunteerRole = (Role)volunteerB.Role,
                        DistanceType = (DO.DistanceType)volunteerB.DistanceType,
                    };
                    _dal.Volunteer.Create(volunteerD);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Can't add voluenteer because ", ex);
            }

        }
            

        public void DeleteVolunteer(int id)
        {

            try
            {
                var volunteer = _dal.Volunteer.Read(id);
                if (volunteer.IsActive==false)
                {
                    _dal.Volunteer.Delete(id);
                }
                else
                {
                    throw new InvalidOperationException("The volunterr is active");
                }
            }
            
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to delete volunteer.", ex);
            }
          
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
            if (!sortField.HasValue)
            {
                sortField = BO.VolunteerFields.Id;
            }
            var sortedVolunteerList = VolunteerManager.SortVolunteers(volunteerList, sortField);
            return sortedVolunteerList;
        }


        public string Login(string username, string password)
        {
            var volunteer = _dal.Volunteer.Read(v => v.Email == username);//השם משתמש הוא האימייל של המתנדב
            if (volunteer.Email != username)
                throw new InvalidOperationException("Invalid username");

            if (volunteer.Password != password)
                throw new InvalidOperationException("Invalid password");

            return volunteer.VolunteerRole.ToString();
        }

        public void UpdateVolunteer(int id, BO.Volunteer volunteer)
        {
            try
            {
                // Check if the user is a manager
                bool chekmanger = VolunteerManager.IsManager(id);

                // Validate the email format
                bool chekemail = VolunteerManager.IsValidEmail(volunteer.Email);

                // Validate the ID format
                bool chekid = VolunteerManager.IsValidID(volunteer.Id);

                // Validate the phone number format
                bool chekphone = VolunteerManager.IsValidPhoneNumber(volunteer.PhoneNumber);

                // Check if the location is within Israel
                bool chekLatitudeandLongitude = VolunteerManager.IsLocationInIsrael(volunteer.Latitude, volunteer.Longitude);

                // If all validations pass
                if (chekemail == true && chekphone == true && chekLatitudeandLongitude == true && chekid == true)
                {
                    // If the user is not a manager, update as a volunteer
                    if (chekmanger == false)
                    {
                        VolunteerManager.Chengeforvolunteer(volunteer);
                    }
                    // If the user is a manager, update as a manager
                    else
                    {
                        VolunteerManager.Chengeformaneger(volunteer);
                    }
                }


            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Can't Update because ", ex);
            }

        }

    }
}
