
using BlApi;
using DalApi;
using Helpers;
//using BO;

namespace BlImplementation
{

    internal class VolunteerImplementation : BlApi.IVolunteer
    {

        private readonly DalApi.IDal _dal = DalApi.Factory.Get;

        /// <summary>
        /// Adds a new volunteer to the system after validating the provided details.
        /// </summary>
        /// <param name="volunteerB">The volunteer object containing the details to be added.</param>
        /// <exception cref="InvalidOperationException">Thrown when the volunteer cannot be added due to validation failure or other issues.</exception>
        public void AddVolunteer(BO.Volunteer volunteerB)
        {
            try
            {
                // Validate the email format
                bool isEmailValid = VolunteerManager.IsValidEmail(volunteerB.Email);

                // Validate the ID format
                bool isIdValid = VolunteerManager.IsValidID(volunteerB.Id);

                // Validate the phone number format
                bool isPhoneNumberValid = VolunteerManager.IsValidPhoneNumber(volunteerB.PhoneNumber);

                // Check if the location is within Israel
                bool isLocationValid = VolunteerManager.IsLocationInIsrael(volunteerB.Latitude, volunteerB.Longitude);

                // If all validations pass
                if (isEmailValid && isPhoneNumberValid && isLocationValid && isIdValid)
                {
                    // Create a new DO.Volunteer object with the validated details
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
                        VolunteerRole = (DO.Role)volunteerB.Role,
                        DistanceType = (DO.DistanceType)volunteerB.DistanceType,
                    };

                    // Add the new volunteer to the database
                    _dal.Volunteer.Create(volunteerD);
                }
                else
                {
                    throw new InvalidOperationException("Validation failed for the volunteer details.");
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Can't add volunteer because of an error.", ex);
            }
        }

        /// <summary>
        /// Deletes a volunteer from the system.
        /// </summary>
        /// <param name="id">The ID of the volunteer to delete.</param>
        /// <exception cref="InvalidOperationException">Thrown when the volunteer cannot be deleted.</exception>
        /// <exception cref="KeyNotFoundException">Thrown when the volunteer does not exist.</exception>
        public void DeleteVolunteer(int id)
        {
            try
            {
                var volunteer = _dal.Volunteer.Read(id);
                if (volunteer == null)
                {
                    throw new KeyNotFoundException($"Volunteer with ID {id} not found.");
                }

                if (volunteer.IsActive == false)
                {
                    _dal.Volunteer.Delete(id);
                }
                else
                {
                    throw new InvalidOperationException("The volunteer is active");
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

        /// <summary>
        /// Retrieves a list of volunteers based on their active status and sorts them by the specified field.
        /// </summary>
        /// <param name="isActive">Nullable boolean to filter active/inactive volunteers. If null, all volunteers are included.</param>
        /// <param name="sortField">Nullable enum to sort the list by a specific field. If null, the list is sorted by ID.</param>
        /// <returns>A sorted and filtered list of volunteers.</returns>
        public IEnumerable<BO.VolunteerInList> GetVolunteers(bool? isActive, BO.VolunteerInListFields? sortField)
        {
            // Get all volunteers from the database
            var volunteers = _dal.Volunteer.ReadAll();

            // If the active status is null, include all volunteers in the list
            // If the active status is not null, filter the list of volunteers by the active status
            if (isActive.HasValue)
            {
                volunteers = volunteers.Where(v => v.IsActive == isActive.Value);
            }

            // Convert the list of DO.Volunteer objects to a list of BO.VolunteerInList objects
            var volunteerList = volunteers.Select(v => VolunteerManager.ConvertVolunteerIdToVolunteerInList(v.Id));
            // If the sort field is not specified, sort by ID
            if (!sortField.HasValue)
            {
                sortField = BO.VolunteerInListFields.Id;
            }
            // Sort the list of volunteers by the specified field
            var sortedVolunteerList = VolunteerManager.SortVolunteers(volunteerList, sortField);
            return sortedVolunteerList;
        }

        /// <summary>
        /// Logs in a volunteer to the system - Email is the username.
        /// </summary>
        /// <param name="username">The username of the volunteer. Email is the username.</param>
        /// <param name="password">The password of the volunteer.</param>
        /// <returns>The role of the user.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the user does not exist or the password is incorrect.</exception>
        public string Login(string username, string password)
        {
            try
            {
                var volunteer = _dal.Volunteer.Read(v => v.Email == username);
                if (volunteer == null)
                {
                    throw new InvalidOperationException("Invalid username");
                }

                if (volunteer.Password != password)
                {
                    throw new InvalidOperationException("Invalid password");
                }

                return volunteer.VolunteerRole.ToString();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to login.", ex);
            }
        }

        /// <summary>
        /// Update the details of a volunteer
        /// </summary>
        /// <param name="id">The ID of the user executing the function.</param>
        /// <param name="volunteer">The volunteer object with updated details.</param>
        /// <exception cref="InvalidOperationException">Thrown when the volunteer cannot be updated due to validation failure or other issues.</exception>
        public void UpdateVolunteer(int id, BO.Volunteer volunteer)
        {
            try
            {
                // Check if the user is a manager
                bool isManager = VolunteerManager.IsManager(id);

                // Validate the email format
                bool isEmailValid = VolunteerManager.IsValidEmail(volunteer.Email);

                // Validate the ID format
                bool isIdValid = VolunteerManager.IsValidID(volunteer.Id);

                // Validate the phone number format
                bool isPhoneNumberValid = VolunteerManager.IsValidPhoneNumber(volunteer.PhoneNumber);

                // Check if the location is within Israel
                bool isLocationValid = VolunteerManager.IsLocationInIsrael(volunteer.Latitude, volunteer.Longitude);

                // If all validations pass
                if (isEmailValid && isPhoneNumberValid && isLocationValid && isIdValid)
                {
                    // If the user is not a manager, update as a volunteer
                    if (!isManager)
                    {
                        VolunteerManager.UpdateVolunteerDetails(volunteer);
                    }
                    // If the user is a manager, update as a manager
                    else
                    {
                        VolunteerManager.UpdateManagerDetails(volunteer);
                    }
                }
                else
                {
                    throw new InvalidOperationException("Validation failed for the volunteer details.");
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Can't update volunteer details. because:", ex);
            }
        }

    }
}


