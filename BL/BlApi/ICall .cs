
namespace BlApi
{
    public interface ICall
    {
        /// <summary>
        /// Requests the quantities of calls by status.
        /// </summary>
        /// <returns>An array of quantities by call status.</returns>
        int[] GetCallQuantitiesByStatus();

        /// <summary>
        /// Requests a list of calls for management.
        /// </summary>
        /// <param name="filterField">Field to filter by (nullable).</param>
        /// <param name="filterValue">Value to filter by (nullable).</param>
        /// <param name="sortField">Field to sort by (nullable).</param>
        /// <returns>A sorted and filtered collection of calls.</returns>
        IEnumerable<BO.CallInList> GetCallList(BO.CallInListFields? filterField, object? filterValue, BO.CallInListFields? sortField);

        /// <summary>
        /// Requests the details of a specific call.
        /// </summary>
        /// <param name="id">The identifier of the call.</param>
        /// <returns>The details of the call.</returns>
        BO.Call GetCallDetails(int id);

        /// <summary>
        /// Updates the details of a specific call.
        /// </summary>
        /// <param name="call">The call object with updated details.</param>
        void UpdateCall(BO.Call call);

        /// <summary>
        /// Deletes a specific call.
        /// </summary>
        /// <param name="id">The identifier of the call.</param>
        void DeleteCall(int id);

        /// <summary>
        /// Adds a new call.
        /// </summary>
        /// <param name="call">The call object to add.</param>
        void AddCall(BO.Call call);

        /// <summary>
        /// Requests a list of closed calls handled by a volunteer.
        /// </summary>
        /// <param name="volunteerId">The identifier of the volunteer.</param>
        /// <param name="callType">Type of call to filter by (nullable).</param>
        /// <param name="sortField">Field to sort by (nullable).</param>
        /// <returns>A sorted collection of closed calls handled by the volunteer.</returns>
        IEnumerable<BO.ClosedCallInList> GetClosedCallsByVolunteer(int volunteerId, BO.CallType? callType, BO.ClosedCallInListFields? sortField);

        /// <summary>
        /// Requests a list of open calls available for a volunteer to choose from.
        /// </summary>
        /// <param name="volunteerId">The identifier of the volunteer.</param>
        /// <param name="callType">Type of call to filter by (nullable).</param>
        /// <param name="sortField">Field to sort by (nullable).</param>
        /// <returns>A sorted collection of open calls available for the volunteer.</returns>
        IEnumerable<BO.OpenCallInList> GetOpenCallsForVolunteer(int volunteerId, BO.CallType? callType, BO.OpenCallInListFields? sortField);

        /// <summary>
        /// Updates the status of a call to "Treated" by a volunteer.
        /// </summary>
        /// <param name="volunteerId">The identifier of the volunteer.</param>
        /// <param name="assignmentId">The identifier of the assignment.</param>
        void CompleteCallHandling(int volunteerId, int assignmentId);

        /// <summary>
        /// Cancels the handling of a call by a volunteer.
        /// </summary>
        /// <param name="requesterId">The identifier of the requester.</param>
        /// <param name="assignmentId">The identifier of the assignment.</param>
        void CancelCallHandling(int requesterId, int assignmentId);

        /// <summary>
        /// Assigns a call to a volunteer for handling.
        /// </summary>
        /// <param name="volunteerId">The identifier of the volunteer.</param>
        /// <param name="callId">The identifier of the call.</param>
        void AssignCallToVolunteer(int volunteerId, int callId);
    }
}
