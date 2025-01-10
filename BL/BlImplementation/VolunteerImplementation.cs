
using BlApi;
using DalApi;
using Helpers;
//using BO;

namespace BlImplementation
{
    internal class VolunteerImplementation : BlApi.IVolunteer
    {
        private readonly DalApi.IDal _dal = DalApi.Factory.Get;

        #region Stage 5

        public void AddObserver(Action listObserver) =>
          VolunteerManager.Observers.AddListObserver(listObserver); //stage 5
        public void AddObserver(int id, Action observer) =>
          VolunteerManager.Observers.AddObserver(id, observer); //stage 5
        public void RemoveObserver(Action listObserver) =>
          VolunteerManager.Observers.RemoveListObserver(listObserver); //stage 5
        public void RemoveObserver(int id, Action observer) =>
          VolunteerManager.Observers.RemoveObserver(id, observer); //stage 5

        #endregion Stage 5


        /// <summary>
        /// Adds a new volunteer to the system after validating the provided details.
        /// </summary>
        /// <param name="volunteerB">The volunteer object containing the details to be added.</param>
        /// <exception cref="BO.BlValidationException">
        /// Thrown when validation of the volunteer details fails. The exception message includes a list of invalid details: Email, ID, Phone Number, Location.
        /// </exception>
        /// <exception cref="BO.BlAlreadyExistsException">Thrown when a volunteer with the same ID already exists.</exception>
        /// <exception cref="BO.BlSystemException">Thrown when an unexpected        
        public void AddVolunteer(BO.Volunteer volunteerB)
        {
            try
            {
                // Validate the email format
                if (string.IsNullOrWhiteSpace(volunteerB.Email))
                    throw new BO.BlValidationException("Email cannot be null or empty.");
                bool isEmailValid = VolunteerManager.IsValidEmail(volunteerB.Email);

                // Validate the ID format
                if (volunteerB.Id == 0)
                    throw new BO.BlValidationException("ID cannot be zero.");
                bool isIdValid = VolunteerManager.IsValidID(volunteerB.Id);

                // Validate the phone number format
                if (string.IsNullOrWhiteSpace(volunteerB.PhoneNumber))
                    throw new BO.BlValidationException("Phone number cannot be null or empty.");
                bool isPhoneNumberValid = VolunteerManager.IsValidPhoneNumber(volunteerB.PhoneNumber);

                // Validate the address
                if (string.IsNullOrWhiteSpace(volunteerB.CurrentAddress))
                    throw new BO.BlValidationException("Current address cannot be null or empty.");
                var coordinates = Tools.GetCoordinates(volunteerB.CurrentAddress);

        // Check if the location is within Israel
        bool isLocationValid = coordinates.IsInIsrael;

        // Collect invalid details
        List<string> invalidDetails = new();
                if (!isEmailValid) invalidDetails.Add("Email");
                if (!isIdValid) invalidDetails.Add("ID");
                if (!isPhoneNumberValid) invalidDetails.Add("Phone Number");
                if (!isLocationValid) invalidDetails.Add("Location");

                // If all validations pass
                if (isEmailValid && isPhoneNumberValid && isLocationValid && isIdValid)
                {
                    DO.Volunteer volunteerD = new()
                    {
                        Id = volunteerB.Id,
                        Email = volunteerB.Email,
                        Password = volunteerB.Password,
                        PhoneNumber = volunteerB.PhoneNumber,
                        CurrentAddress = volunteerB.CurrentAddress,
                        FullName = volunteerB.FullName,
                        Latitude = coordinates.Latitude,
                        Longitude = coordinates.Longitude,
                        MaxDistance = volunteerB.MaxDistance,
                        IsActive = true,
                        VolunteerRole = (DO.Role)volunteerB.Role,
                        DistanceType = (DO.DistanceType)volunteerB.DistanceType,
                    };

        _dal.Volunteer.Create(volunteerD);
                    VolunteerManager.Observers.NotifyListUpdated();
                }
                else
                {
                    throw new BO.BlValidationException($"Validation failed for the following details: {string.Join(", ", invalidDetails)}");
                }
            }
            catch (BO.BlValidationException ex)
            {
                throw;
            }
            catch (DO.DalAlreadyExistsException ex)
            {
                throw new BO.BlAlreadyExistsException($"Volunteer with ID={volunteerB.Id} already exists.", ex);
            }
            catch (Exception ex)
            {
                throw new BO.BlSystemException("An error occurred while adding the volunteer.", ex);
            }
        }

        /// <summary>
        /// Deletes a volunteer from the system.
        /// </summary>
        /// <param name="id">The ID of the volunteer to delete.</param>
        public void DeleteVolunteer(int id)
        {
            try
            {
                var volunteer = _dal.Volunteer.Read(id) ??
                    throw new BO.BlDoesNotExistException($"Volunteer with ID={id} does not exist.");

                if (volunteer.IsActive == false)
                {
                    _dal.Volunteer.Delete(id);
                    VolunteerManager.Observers.NotifyListUpdated();
                }
                else
                {
                    throw new BO.BlDeletionImpossible("Cannot delete an active volunteer.");
                }
            }
            catch (DO.DalDeletionImpossible ex)
            {
                throw new BO.BlSystemException("An error occurred while deleting the volunteer.", ex);
            }
        }

        /// <summary>
        /// Retrieves the details of a volunteer by ID.
        /// </summary>
        /// <param name="id">The ID of the volunteer.</param>
        /// <returns>The BO.Volunteer object with the requested details.</returns>
        public BO.Volunteer GetVolunteerDetails(int id)
        {
            try
            {
                var volunteer = _dal.Volunteer.Read(id) ??
                    throw new BO.BlDoesNotExistException($"Volunteer with ID={id} does not exist.");

                return VolunteerManager.ConvertVolunteerIdToBO(id);
            }
            catch (Exception ex)
            {
                throw new BO.BlSystemException("An error occurred while retrieving volunteer details.", ex);
            }
        }

        /// <summary>
        /// Retrieves a list of volunteers filtered and sorted based on the provided criteria.
        /// </summary>
        /// <param name="isActive">Nullable boolean to filter active/inactive volunteers. Null for all.</param>
        /// <param name="sortField">Nullable field to sort the list. Null for default sorting by ID.</param>
        /// <returns>A sorted and filtered list of volunteers.</returns>
        public IEnumerable<BO.VolunteerInList> GetVolunteers(bool? isActive, BO.VolunteerInListFields? sortField)
        {
            try
            {
                var volunteers = _dal.Volunteer.ReadAll();

                if (isActive.HasValue)
                {
                    volunteers = volunteers.Where(v => v.IsActive == isActive.Value);
                }

                IEnumerable<BO.VolunteerInList> volunteerList = volunteers.Select(v => VolunteerManager.ConvertVolunteerIdToVolunteerInList(v.Id));
                sortField ??= BO.VolunteerInListFields.Id;

                return VolunteerManager.SortVolunteers(volunteerList, sortField);
            }
            catch (Exception ex)
            {
                throw new BO.BlSystemException("An error occurred while retrieving the list of volunteers.", ex);
            }
        }
        public IEnumerable<BO.VolunteerInList> GetCallTypsOfVolunteers(BO.CallType? callType)
        {
            try
            {
                var volunteers = GetVolunteers(null, null);
                if (callType == null || callType == BO.CallType.None)
                {
                    return volunteers;
                }
                else
                {
                    return volunteers.Where(v => v.CurrentCallType == callType);
                }
            }
            catch (Exception ex)
            {
                throw new BO.BlSystemException("An error occurred while retrieving the list of volunteers.", ex);
            }
        }
        /// <summary>
        /// Logs in a volunteer to the system.
        /// </summary>
        /// <param name="username">The username (email) of the volunteer.</param>
        /// <param name="password">The password of the volunteer.</param>
        /// <returns>The role of the user as a string.</returns>
        public string Login(string username, string password)
        {
            try
            {
                var volunteer = _dal.Volunteer.Read(v => v.Email == username) ??
                    throw new BO.BlLoginException("Invalid username.");

                if (volunteer.Password != password)
                {
                    throw new BO.BlLoginException("Invalid password.");
                }

                return volunteer.VolunteerRole.ToString();
            }
            catch (Exception ex)
            {
                throw new BO.BlSystemException("An error occurred during login.", ex);
            }
        }

        /// <summary>
        /// Updates the details of an existing volunteer.
        /// </summary>
        /// <param name="id">The ID of the user executing the update.</param>
        /// <param name="volunteer">The volunteer object with updated details.</param>
        public void UpdateVolunteer(int id, BO.Volunteer volunteer)
        {
            try
            {
                bool isManager = VolunteerManager.IsManager(id);

                bool isEmailValid = VolunteerManager.IsValidEmail(volunteer.Email);
                bool isIdValid = VolunteerManager.IsValidID(volunteer.Id);
                bool isPhoneNumberValid = VolunteerManager.IsValidPhoneNumber(volunteer.PhoneNumber);

                var coordinates = Tools.GetCoordinates(volunteer.CurrentAddress);

                bool isLocationValid = coordinates.IsInIsrael;

                // Collect invalid details
                List<string> invalidDetails = new();
                if (!isEmailValid) invalidDetails.Add("Email");
                if (!isIdValid) invalidDetails.Add("ID");
                if (!isPhoneNumberValid) invalidDetails.Add("Phone Number");
                if (!isLocationValid) invalidDetails.Add("Location");

                if (isEmailValid && isPhoneNumberValid && isLocationValid && isIdValid)
                {
                    BO.Volunteer volunteerB = new()
                    {
                        Id = volunteer.Id,
                        Email = volunteer.Email,
                        Password = volunteer.Password,
                        PhoneNumber = volunteer.PhoneNumber,
                        CurrentAddress = volunteer.CurrentAddress,
                        FullName = volunteer.FullName,
                        Latitude = coordinates.Latitude,
                        Longitude = coordinates.Longitude,
                        MaxDistance = volunteer.MaxDistance,
                        IsActive = true,
                        Role = (BO.VolunteerRole)volunteer.Role,
                        DistanceType = (BO.DistanceType)volunteer.DistanceType,
                    };

                    if (!isManager)
                    {
                        VolunteerManager.UpdateVolunteerDetails(volunteerB);
                        VolunteerManager.Observers.NotifyItemUpdated(volunteerB.Id);
                        VolunteerManager.Observers.NotifyListUpdated();
                    }
                    else
                    {
                        VolunteerManager.UpdateManagerDetails(volunteerB);
                        VolunteerManager.Observers.NotifyItemUpdated(volunteerB.Id);
                        VolunteerManager.Observers.NotifyListUpdated();
                    }
                }
                else
                {
                    throw new BO.BlValidationException($"Validation failed for the following details: {string.Join(", ", invalidDetails)}");
                }
            }
            catch (Exception ex)
            {
                throw new BO.BlSystemException("An error occurred while updating the volunteer details.", ex);
            }
        }
    }
}



