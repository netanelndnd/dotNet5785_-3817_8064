
using BlApi;
using BO;
using Helpers;

namespace BlImplementation
{
    internal class AdminImplementation : IAdmin
    {
        private readonly DalApi.IDal _dal = DalApi.Factory.Get;

        /// <summary>
        /// Gets the current system clock time.
        /// מקבל את הזמן הנוכחי של שעון המערכת.
        /// </summary>
        public DateTime GetSystemClock()
        {
            return DateTime.Now;
        }

        /// <summary>
        /// Advances the system clock by a specified time unit.
        /// מקדם את שעון המערכת ביחידת זמן מסוימת.
        /// </summary>
        public void AdvanceSystemClock(TimeUnit timeUnit)
        {
            if(timeUnit == TimeUnit.Minute)
            {
                ClockManager.UpdateClock(ClockManager.Now.AddMinutes(1));
            }
            else if (timeUnit == TimeUnit.Hour)
            {
                ClockManager.UpdateClock(ClockManager.Now.AddHours(1));
            }
            else if (timeUnit == TimeUnit.Day)
            {
                ClockManager.UpdateClock(ClockManager.Now.AddDays(1));
            }
            else if (timeUnit == TimeUnit.Month)
            {
                ClockManager.UpdateClock(ClockManager.Now.AddMonths(1));
            }
            else if (timeUnit == TimeUnit.Year)
            {
                ClockManager.UpdateClock(ClockManager.Now.AddYears(1));
            }
        }

        /// <summary>
        /// Gets the risk time span.
        /// מקבל את משך הזמן של הסיכון.
        /// </summary>
        public TimeSpan GetRiskTimeSpan()
        {
            return _dal.Config.RiskRange;
        }

        /// <summary>
        /// Sets the risk time span.
        /// מגדיר את משך הזמן של הסיכון.
        /// </summary>
        public void SetRiskTimeSpan(TimeSpan riskTimeSpan)
        {
            _dal.Config.RiskRange = riskTimeSpan;
        }

        /// <summary>
        /// Resets the database to its initial state.
        /// מאפס את מסד הנתונים למצבו ההתחלתי.
        /// </summary>

        public void ResetDatabase()
        {
            // Reset configuration data to initial values
            _dal.Config.Reset();
            ClockManager.UpdateClock(ClockManager.Now);

            // Clear data for all entities
            _dal.Assignment.DeleteAll();
            _dal.Call.DeleteAll();
            _dal.Volunteer.DeleteAll();
        }

        /// <summary>
        /// Initializes the database with initial data.
        /// מאתחל את מסד הנתונים עם נתונים ראשוניים.
        /// </summary>
        public void InitializeDatabase()
        {
            // Reset the database to its initial state
            DalTest.Initialization.Do();
            ClockManager.UpdateClock(ClockManager.Now);

          
        }
    }
}
