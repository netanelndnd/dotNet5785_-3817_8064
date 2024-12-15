
using BlApi;
using BO;

namespace BlImplementation
{
    internal class AdminImplementation : IAdmin
    {
        private readonly DalApi.IDal _dal = DalApi.Factory.Get;

        public void AdvanceSystemClock(TimeUnit timeUnit)
        {
            throw new NotImplementedException();
        }

        public TimeSpan GetRiskTimeSpan()
        {
            throw new NotImplementedException();
        }

        public DateTime GetSystemClock()
        {
            throw new NotImplementedException();
        }

        public void InitializeDatabase()
        {
            throw new NotImplementedException();
        }

        public void ResetDatabase()
        {
            throw new NotImplementedException();
        }

        public void SetRiskTimeSpan(TimeSpan riskTimeSpan)
        {
            throw new NotImplementedException();
        }
    }
}
