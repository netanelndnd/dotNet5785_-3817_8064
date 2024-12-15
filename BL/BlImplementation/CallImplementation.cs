using BlApi;
using BO;

namespace BlImplementation
{
    internal class CallImplementation : ICall
    {
        private readonly DalApi.IDal _dal = DalApi.Factory.Get;

        public void AddCall(Call call)
        {
            throw new NotImplementedException();
        }

        public void AssignCallToVolunteer(int volunteerId, int callId)
        {
            throw new NotImplementedException();
        }

        public void CancelCallHandling(int requesterId, int assignmentId)
        {
            throw new NotImplementedException();
        }

        public void CompleteCallHandling(int volunteerId, int assignmentId)
        {
            throw new NotImplementedException();
        }

        public void DeleteCall(int id)
        {
            throw new NotImplementedException();
        }

        public Call GetCallDetails(int id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<CallInList> GetCallList(CallInListFields? filterField, object? filterValue, CallInListFields? sortField)
        {
            throw new NotImplementedException();
        }

        public int[] GetCallQuantitiesByStatus()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ClosedCallInList> GetClosedCallsByVolunteer(int volunteerId, CallType? callType, ClosedCallInListFields? sortField)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<OpenCallInList> GetOpenCallsForVolunteer(int volunteerId, CallType? callType, OpenCallInListFields? sortField)
        {
            throw new NotImplementedException();
        }

        public void UpdateCall(Call call)
        {
            throw new NotImplementedException();
        }
    }
}
