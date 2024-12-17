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
            var calls = _dal.Call.ReadAll();
            var groupedCalls = calls.GroupBy(call => (int)call.CallType)
                                    .Select(group => new { Status = group.Key, Count = group.Count() })
                                    .ToList();

            int maxStatus = Enum.GetValues(typeof(CallStatus)).Cast<int>().Max();
            int[] quantities = new int[maxStatus + 1];

            foreach (var group in groupedCalls)
            {
                quantities[group.Status] = group.Count;
            }

            return quantities;
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
