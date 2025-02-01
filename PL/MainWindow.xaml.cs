using PL.call;
using PL.Volunteer;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace PL
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow(int userId = 0)
        {
            Interval = 2;
            CurrentManager = userId;
            //initial total calls
            TotalInitialCalls = 30;
            InitializeComponent();
        }

        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        private volatile DispatcherOperation? _observerOperation = null;

        private void clockObserver()
        {
            if (_observerOperation is null || _observerOperation.Status == DispatcherOperationStatus.Completed)
                _observerOperation = Dispatcher.BeginInvoke(() =>
                {
                    CurrentTime = s_bl.Admin.GetSystemClock();
                });
        }

        int CurrentManager = 0;

        private void configObserver()
        {
            if (_observerOperation is null || _observerOperation.Status == DispatcherOperationStatus.Completed)
                _observerOperation = Dispatcher.BeginInvoke(() =>
                {
                    TimeRisk = s_bl.Admin.GetRiskTimeSpan();
                });
        }

        public DateTime CurrentTime
        {
            get { return (DateTime)GetValue(CurrentTimeProperty); }
            set { SetValue(CurrentTimeProperty, value); }
        }
        public static readonly DependencyProperty CurrentTimeProperty =
            DependencyProperty.Register("CurrentTime", typeof(DateTime), typeof(MainWindow));

        public TimeSpan TimeRisk
        {
            get { return (TimeSpan)GetValue(TimeRiskProperty); }
            set { SetValue(TimeRiskProperty, value); }
        }
        public static readonly DependencyProperty TimeRiskProperty =
            DependencyProperty.Register("TimeRisk", typeof(TimeSpan), typeof(MainWindow));

        public int Interval
        {
            get { return (int)GetValue(IntervalProperty); }
            set { SetValue(IntervalProperty, value); }
        }
        public static readonly DependencyProperty IntervalProperty =
            DependencyProperty.Register("Interval", typeof(int), typeof(MainWindow));

        public bool IsSimulatorRunning
        {
            get { return (bool)GetValue(IsSimulatorRunningProperty); }
            set { SetValue(IsSimulatorRunningProperty, value); }
        }
        public static readonly DependencyProperty IsSimulatorRunningProperty =
            DependencyProperty.Register("IsSimulatorRunning", typeof(bool), typeof(MainWindow));

        public double SimulationProgress
        {
            get { return (double)GetValue(SimulationProgressProperty); }
            set { SetValue(SimulationProgressProperty, value); }
        }
        public static readonly DependencyProperty SimulationProgressProperty =
            DependencyProperty.Register("SimulationProgress", typeof(double), typeof(MainWindow));

        private int TotalInitialCalls { get; set; }



        private void btnAddOneMinute_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.AdvanceSystemClock(BO.TimeUnit.Minute);
        }

        private void btnAddOneHour_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.AdvanceSystemClock(BO.TimeUnit.Hour);
        }

        private void btnAddOneDay_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.AdvanceSystemClock(BO.TimeUnit.Day);
        }

        private void btnAddOneMonth_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.AdvanceSystemClock(BO.TimeUnit.Month);
        }

        private void btnAddOneYear_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.AdvanceSystemClock(BO.TimeUnit.Year);
        }

        private void btnUpdateTimeSpan_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.SetRiskTimeSpan(TimeRisk);
        }

        private void btnListVolunteers_Click(object sender, RoutedEventArgs e)
        {
            var existingVolunteerWindow = Application.Current.Windows.OfType<VolunteerInListWindow>().FirstOrDefault();
            if (existingVolunteerWindow != null)
            {
                MessageBox.Show("A volunteer list window is already open.", "Window Already Open", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                new VolunteerInListWindow().Show();
            }
        }

        private void btnListCalls_Click(object sender, RoutedEventArgs e)
        {
            var existingCallWindow = Application.Current.Windows.OfType<CallInListWindow>().FirstOrDefault();
            if (existingCallWindow != null)
            {
                MessageBox.Show("A call list window is already open.", "Window Already Open", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                new CallInListWindow(CurrentManager).Show();
            }
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult mbResult = MessageBox.Show("Do you Sure that you want to continue?", "Reset",
                MessageBoxButton.YesNoCancel, MessageBoxImage.Question,
                MessageBoxResult.Cancel);

            switch (mbResult)
            {
                case MessageBoxResult.Yes:
                    Mouse.OverrideCursor = Cursors.Wait;
                    try
                    {
                        foreach (Window window in Application.Current.Windows)
                        {
                            if (window != this)
                            {
                                window.Close();
                            }
                        }

                        s_bl.Admin.ResetDatabase();
                    }
                    finally
                    {
                        Mouse.OverrideCursor = null;
                    }
                    break;
                case MessageBoxResult.No:
                    break;
            }
        }

        private void btnInitialize_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult mbResult = MessageBox.Show("Do you Sure that you want to continue?", "Initialize",
                MessageBoxButton.YesNoCancel, MessageBoxImage.Question,
                MessageBoxResult.Cancel);

            switch (mbResult)
            {
                case MessageBoxResult.Yes:
                    Mouse.OverrideCursor = Cursors.Wait;
                    try
                    {
                        foreach (Window window in Application.Current.Windows)
                        {
                            if (window != this && !(window is LoginWindow))
                            {
                                window.Close();
                            }
                        }

                        s_bl.Admin.InitializeDatabase();
                    }
                    finally
                    {
                        Mouse.OverrideCursor = null;
                    }
                    break;
                case MessageBoxResult.No:
                    break;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CurrentTime = s_bl.Admin.GetSystemClock();
            TimeRisk = s_bl.Admin.GetRiskTimeSpan();
            UpdateCallCountsObserver();
            s_bl.Admin.AddClockObserver(clockObserver);
            s_bl.Admin.AddConfigObserver(configObserver);
            s_bl.Call.AddObserver(UpdateCallCountsObserver);

        }

        private void Window_Closed(object sender, EventArgs e)
        {
            s_bl.Admin.RemoveClockObserver(clockObserver);
            s_bl.Admin.RemoveConfigObserver(configObserver);
            s_bl.Call.RemoveObserver(UpdateCallCountsObserver);
            if (IsSimulatorRunning)
            {
                s_bl.Admin.StopSimulator();
            }
        }

        private void btnOpenCalls_Click(object sender, RoutedEventArgs e)
        {
            CallListWindow callListWindow = new CallListWindow("Open");
            callListWindow.Show();
        }

        private void btnInProgressCalls_Click(object sender, RoutedEventArgs e)
        {
            CallListWindow callListWindow = new CallListWindow("In Progress");
            callListWindow.Show();
        }

        private void btnOpenInRiskCalls_Click(object sender, RoutedEventArgs e)
        {
            CallListWindow callListWindow = new CallListWindow("OpenInRisk");
            callListWindow.Show();
        }

        private void btnInProgressInRiskCalls_Click(object sender, RoutedEventArgs e)
        {
            CallListWindow callListWindow = new CallListWindow("InProgressInRisk");
            callListWindow.Show();
        }

        private void btnTreatedCalls_Click(object sender, RoutedEventArgs e)
        {
            CallListWindow callListWindow = new CallListWindow("Treated");
            callListWindow.Show();
        }

        private void btnExpiredCalls_Click(object sender, RoutedEventArgs e)
        {
            CallListWindow callListWindow = new CallListWindow("Expired");
            callListWindow.Show();
        }

        public int OpenCallsCount
        {
            get { return (int)GetValue(OpenCallsCountProperty); }
            set { SetValue(OpenCallsCountProperty, value); }
        }
        public static readonly DependencyProperty OpenCallsCountProperty =
            DependencyProperty.Register("OpenCallsCount", typeof(int), typeof(MainWindow));

        public int InProgressCallsCount
        {
            get { return (int)GetValue(InProgressCallsCountProperty); }
            set { SetValue(InProgressCallsCountProperty, value); }
        }
        public static readonly DependencyProperty InProgressCallsCountProperty =
            DependencyProperty.Register("InProgressCallsCount", typeof(int), typeof(MainWindow));

        public int OpenInRiskCallsCount
        {
            get { return (int)GetValue(OpenInRiskCallsCountProperty); }
            set { SetValue(OpenInRiskCallsCountProperty, value); }
        }
        public static readonly DependencyProperty OpenInRiskCallsCountProperty =
            DependencyProperty.Register("OpenInRiskCallsCount", typeof(int), typeof(MainWindow));

        public int InProgressInRiskCallsCount
        {
            get { return (int)GetValue(InProgressInRiskCallsCountProperty); }
            set { SetValue(InProgressInRiskCallsCountProperty, value); }
        }
        public static readonly DependencyProperty InProgressInRiskCallsCountProperty =
            DependencyProperty.Register("InProgressInRiskCallsCount", typeof(int), typeof(MainWindow));

        public int TreatedCallsCount
        {
            get { return (int)GetValue(TreatedCallsCountProperty); }
            set { SetValue(TreatedCallsCountProperty, value); }
        }
        public static readonly DependencyProperty TreatedCallsCountProperty =
            DependencyProperty.Register("TreatedCallsCount", typeof(int), typeof(MainWindow));

        public int ExpiredCallsCount
        {
            get { return (int)GetValue(ExpiredCallsCountProperty); }
            set { SetValue(ExpiredCallsCountProperty, value); }
        }
        public static readonly DependencyProperty ExpiredCallsCountProperty =
            DependencyProperty.Register("ExpiredCallsCount", typeof(int), typeof(MainWindow));

        private void UpdateCallCountsObserver()
        {
            if (_observerOperation is null || _observerOperation.Status == DispatcherOperationStatus.Completed)
                _observerOperation = Dispatcher.BeginInvoke(() =>
                {
                    int[] quantities = s_bl.Call.GetCallQuantitiesByStatus();
                    OpenCallsCount = quantities[(int)BO.CallStatus.Open];
                    InProgressCallsCount = quantities[(int)BO.CallStatus.InProgress];
                    OpenInRiskCallsCount = quantities[(int)BO.CallStatus.OpenInRisk];
                    InProgressInRiskCallsCount = quantities[(int)BO.CallStatus.InProgressInRisk];
                    TreatedCallsCount = quantities[(int)BO.CallStatus.Treated];
                    ExpiredCallsCount = quantities[(int)BO.CallStatus.Expired];

                    UpdateSimulationProgress();
                });
        }

        private void btnToggleSimulator_Click(object sender, RoutedEventArgs e)
        {
            if (IsSimulatorRunning)
            {
                s_bl.Admin.StopSimulator();
                IsSimulatorRunning = false;
            }
            else
            {
                s_bl.Admin.StartSimulator(Interval);
                IsSimulatorRunning = true;
            }
        }

        private void UpdateSimulationProgress()
        {
            int remainingCalls = OpenCallsCount + InProgressCallsCount + OpenInRiskCallsCount + InProgressInRiskCallsCount;

            if (TotalInitialCalls > 0)
            {
                SimulationProgress = ((double)(TotalInitialCalls - remainingCalls) / TotalInitialCalls) * 100;
            }
            else
            {
                SimulationProgress = 0;
            }
        }
    }
   
}