
namespace BlApi
{
    /// <summary>
    /// Interface for Admin operations.
    /// </summary>
    public interface IAdmin
    {

        void StartSimulator(int interval); //stage 7
        void StopSimulator(); //stage 7

        /// <summary>
        /// Gets the current system clock time.
        /// </summary>
        /// <returns>The current DateTime of the system clock.</returns>
        DateTime GetSystemClock();

        /// <summary>
        /// Advances the system clock by a specified time unit.
        /// </summary>
        /// <param name="timeUnit">The time unit to advance the clock by.</param>
        void AdvanceSystemClock(BO.TimeUnit timeUnit);
            
        /// <summary>
        /// Gets the risk time span.
        /// </summary>
        /// <returns>The current risk time span.</returns>
        TimeSpan GetRiskTimeSpan();

        /// <summary>
        /// Sets the risk time span.
        /// </summary>
        /// <param name="riskTimeSpan">The new risk time span.</param>
        void SetRiskTimeSpan(TimeSpan riskTimeSpan);

        /// <summary>
        /// Resets the database to its initial state.
        /// </summary>
        void ResetDatabase();

        /// <summary>
        /// Initializes the database with initial data.
        /// </summary>
        void InitializeDatabase();

        #region Stage 5

        void AddConfigObserver(Action configObserver);
        void RemoveConfigObserver(Action configObserver);
        void AddClockObserver(Action clockObserver);
        void RemoveClockObserver(Action clockObserver);

        #endregion Stage 5
    }
}
