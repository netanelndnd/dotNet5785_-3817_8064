
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
            return AdminManager.Now;
        }

        /// <summary>
        /// Advances the system clock by a specified time unit.
        /// מקדם את שעון המערכת ביחידת זמן מסוימת.
        /// </summary>
        
        public void AdvanceSystemClock(TimeUnit timeUnit)
        {
            AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
            if (timeUnit == TimeUnit.Minute)
            {
                AdminManager.UpdateClock(AdminManager.Now.AddMinutes(1));
            }
            else if (timeUnit == TimeUnit.Hour)
            {
                AdminManager.UpdateClock(AdminManager.Now.AddHours(1));
            }
            else if (timeUnit == TimeUnit.Day)
            {
                AdminManager.UpdateClock(AdminManager.Now.AddDays(1));
            }
            else if (timeUnit == TimeUnit.Month)
            {
                AdminManager.UpdateClock(AdminManager.Now.AddMonths(1));
            }
            else if (timeUnit == TimeUnit.Year)
            {
                AdminManager.UpdateClock(AdminManager.Now.AddYears(1));
            }
        }

        /// <summary>
        /// Gets the risk time span.
        /// מקבל את משך הזמן של הסיכון.
        /// </summary>
        public TimeSpan GetRiskTimeSpan()
        {
            return AdminManager.RiskRange;
        }

        /// <summary>
        /// Sets the risk time span.
        /// מגדיר את משך הזמן של הסיכון.
        /// </summary>
        public void SetRiskTimeSpan(TimeSpan riskTimeSpan)
        {
            AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
            AdminManager.RiskRange = riskTimeSpan;

            // Notify observers of the configuration update
            //נועד לעדכן את רשימות הקריאות והמתנדבים על פי הסיכון החדש
            VolunteerManager.Observers.NotifyListUpdated();
            CallManager.Observers.NotifyListUpdated();

        }

        /// <summary>
        ///resets the database to its initial state.
        /// מאפס את מסד הנתונים למצבו ההתחלתי.
        /// </summary>
        public void ResetDatabase()
        {
            AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
            AdminManager.ResetDB(); //stage 7
        }

        /// <summary>
        /// Initializes the database with initial data.
        /// מאתחל את מסד הנתונים עם נתונים ראשוניים.
        /// </summary>
        public void InitializeDatabase()
        {
            // Reset the database to its initial state

            AdminManager.ThrowOnSimulatorIsRunning();  //stage 7

            ResetDatabase();
            AdminManager.InitializeDB(); //stage 7
        }

        #region Stage 5

        /// <summary>
        /// Adds an observer for clock updates.
        /// </summary>
        /// <param name="clockObserver">The observer to add.</param>
        public void AddClockObserver(Action clockObserver) =>
           AdminManager.ClockUpdatedObservers += clockObserver;

        /// <summary>
        /// Removes an observer for clock updates.
        /// </summary>
        /// <param name="clockObserver">The observer to remove.</param>
        public void RemoveClockObserver(Action clockObserver) =>
           AdminManager.ClockUpdatedObservers -= clockObserver;

        /// <summary>
        /// Adds an observer for configuration updates.
        /// </summary>
        /// <param name="configObserver">The observer to add.</param>
        public void AddConfigObserver(Action configObserver) =>
           AdminManager.ConfigUpdatedObservers += configObserver;

        /// <summary>
        /// Removes an observer for configuration updates.
        /// </summary>
        /// <param name="configObserver">The observer to remove.</param>
        public void RemoveConfigObserver(Action configObserver) =>
           AdminManager.ConfigUpdatedObservers -= configObserver;

        #endregion Stage 5

    }
}
