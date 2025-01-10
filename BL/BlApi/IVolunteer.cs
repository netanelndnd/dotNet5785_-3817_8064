
using BO;

namespace BlApi
{
    public interface IVolunteer : IObservable //stage 5
    {
        IEnumerable<VolunteerInList> GetCallTypsOfVolunteers(CallType? callType);

        IEnumerable<VolunteerInList> GetVolunteers(bool? isActive, VolunteerInListFields? sortField);

        /// <summary>
        /// Logs in a volunteer to the system.
        /// </summary>
        /// <param name="username">The username of the volunteer.</param>
        /// <param name="password">The password of the volunteer.</param>
        /// <returns>The role of the user.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the user does not exist or the password is incorrect.</exception>
        string Login(string username, string password);

        /// <summary>
        /// Gets the details of a volunteer.
        /// </summary>
        /// <param name="id">The ID of the volunteer.</param>
        /// <returns>The volunteer details including the call in progress.</returns>
        /// <exception cref="KeyNotFoundException">Thrown when the volunteer does not exist.</exception>
        BO.Volunteer GetVolunteerDetails(int id);

        /// <summary>
        /// Updates the details of a volunteer.
        /// </summary>
        /// <param name="id">The ID of the user executing the function.</param>
        /// <param name="volunteer">The volunteer object with updated details.</param>
        /// <exception cref="KeyNotFoundException">Thrown when the volunteer does not exist.</exception>
        void UpdateVolunteer(int id, BO.Volunteer volunteer);

        /// <summary>
        /// Deletes a volunteer.
        /// </summary>
        /// <param name="id">The ID of the volunteer to delete.</param>
        /// <exception cref="InvalidOperationException">Thrown when the volunteer cannot be deleted.</exception>
        /// <exception cref="KeyNotFoundException">Thrown when the volunteer does not exist.</exception>
        void DeleteVolunteer(int id);

        /// <summary>
        /// Adds a new volunteer.
        /// </summary>
        /// <param name="volunteer">The volunteer object to add.</param>
        /// <exception cref="InvalidOperationException">Thrown when the volunteer already exists.</exception>
        void AddVolunteer(BO.Volunteer volunteer);
    }
}
