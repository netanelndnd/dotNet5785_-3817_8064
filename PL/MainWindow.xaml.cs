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

namespace PL
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        // Observer method to update the current time
        private void clockObserver()
        {
            CurrentTime = s_bl.Admin.GetSystemClock();
        }

        // Observer method to update the risk time span
        private void configObserver()
        {
            TimeRisk = s_bl.Admin.GetRiskTimeSpan();
        }

        // Dependency property for the current time
        public DateTime CurrentTime
        {
            get { return (DateTime)GetValue(CurrentTimeProperty); }
            set { SetValue(CurrentTimeProperty, value); }
        }
        public static readonly DependencyProperty CurrentTimeProperty =
            DependencyProperty.Register("CurrentTime", typeof(DateTime), typeof(MainWindow));

        // Dependency property for the risk time span
        public TimeSpan TimeRisk
        {
            get { return (TimeSpan)GetValue(TimeRiskProperty); }
            set { SetValue(TimeRiskProperty, value); }
        }
        public static readonly DependencyProperty TimeRiskProperty =
            DependencyProperty.Register("TimeRisk", typeof(TimeSpan), typeof(MainWindow));

        // Button click event to add one minute to the system clock
        private void btnAddOneMinute_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.AdvanceSystemClock(BO.TimeUnit.Minute);
        }

        // Button click event to add one hour to the system clock
        private void btnAddOneHour_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.AdvanceSystemClock(BO.TimeUnit.Hour);
        }

        // Button click event to add one day to the system clock
        private void btnAddOneDay_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.AdvanceSystemClock(BO.TimeUnit.Day);
        }

        // Button click event to add one month to the system clock
        private void btnAddOneMonth_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.AdvanceSystemClock(BO.TimeUnit.Month);
        }

        // Button click event to add one year to the system clock
        private void btnAddOneYear_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.AdvanceSystemClock(BO.TimeUnit.Year);
        }

        // Button click event to update the risk time span
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
                new CallInListWindow().Show();
            }
        }

        // Button click event to reset the database
        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            // Show a confirmation message box to the user
            MessageBoxResult mbResult = MessageBox.Show("Do you Sure that you want to continue?", "Reset",
                MessageBoxButton.YesNoCancel, MessageBoxImage.Question,
                MessageBoxResult.Cancel);

            // Handle the user's response
            switch (mbResult)
            {
                case MessageBoxResult.Yes:
                    // Change the mouse cursor to a wait cursor
                    Mouse.OverrideCursor = Cursors.Wait;
                    try
                    {
                        // Close all open windows except the main window
                        foreach (Window window in Application.Current.Windows)
                        {
                            if (window != this)
                            {
                                window.Close();
                            }
                        }

                        // Reset the database
                        s_bl.Admin.ResetDatabase();
                    }
                    finally
                    {
                        // Restore the mouse cursor to its default state
                        Mouse.OverrideCursor = null;
                    }
                    break;
                case MessageBoxResult.No:
                    // Do nothing if the user selects "No"
                    break;
            }
        }

        // Button click event to initialize the database
        private void btnInitialize_Click(object sender, RoutedEventArgs e)
        {
            // Show a confirmation message box to the user
            MessageBoxResult mbResult = MessageBox.Show("Do you Sure that you want to continue?", "Initialize",
                MessageBoxButton.YesNoCancel, MessageBoxImage.Question,
                MessageBoxResult.Cancel);

            // Handle the user's response
            switch (mbResult)
            {
                case MessageBoxResult.Yes:
                    // Change the mouse cursor to a wait cursor
                    Mouse.OverrideCursor = Cursors.Wait;
                    try
                    {
                        // Close all open windows except the main window and the login window
                        foreach (Window window in Application.Current.Windows)
                        {
                            if (window != this && !(window is LoginWindow))
                            {
                                window.Close();
                            }
                        }

                        // Initialize the database
                        s_bl.Admin.InitializeDatabase();
                    }
                    finally
                    {
                        // Restore the mouse cursor to its default state
                        Mouse.OverrideCursor = null;
                    }
                    break;
                case MessageBoxResult.No:
                    // Do nothing if the user selects "No"
                    break;
            }
        }

        // Method called when the window is loaded
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CurrentTime = s_bl.Admin.GetSystemClock();
            TimeRisk = s_bl.Admin.GetRiskTimeSpan();
            UpdateCallCounts();
            s_bl.Admin.AddClockObserver(clockObserver);
            s_bl.Admin.AddConfigObserver(configObserver);
            s_bl.Call.AddObserver(UpdateCallCounts);
        }

        // Method called when the window is closed
        private void Window_Closed(object sender, EventArgs e)
        {
            s_bl.Admin.RemoveClockObserver(clockObserver);
            s_bl.Admin.RemoveConfigObserver(configObserver);
            s_bl.Call.RemoveObserver(UpdateCallCounts);
        }

        private void btnOpenCalls_Click(object sender, RoutedEventArgs e)
        {
            // Navigate to the filtered call list screen for Open Calls
            CallListWindow callListWindow = new CallListWindow("Open");
            callListWindow.Show();
        }

        private void btnInProgressCalls_Click(object sender, RoutedEventArgs e)
        {
            // Navigate to the filtered call list screen for In Progress Calls
            CallListWindow callListWindow = new CallListWindow("In Progress");
            callListWindow.Show();
        }

        private void btnOpenInRiskCalls_Click(object sender, RoutedEventArgs e)
        {
            // Navigate to the filtered call list screen for Open In Risk Calls
            CallListWindow callListWindow = new CallListWindow("OpenInRisk");
            callListWindow.Show();
        }

        private void btnInProgressInRiskCalls_Click(object sender, RoutedEventArgs e)
        {
            // Navigate to the filtered call list screen for In Progress In Risk Calls
            CallListWindow callListWindow = new CallListWindow("InProgressInRisk");
            callListWindow.Show();
        }

        private void btnTreatedCalls_Click(object sender, RoutedEventArgs e)
        {
            // Navigate to the filtered call list screen for Treated Calls
            CallListWindow callListWindow = new CallListWindow("Treated");
            callListWindow.Show();
        }

        private void btnExpiredCalls_Click(object sender, RoutedEventArgs e)
        {
            // Navigate to the filtered call list screen for Expired Calls
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

            private void UpdateCallCounts()
            {
                // Get the call quantities by status
                int[] quantities = s_bl.Call.GetCallQuantitiesByStatus();
                OpenCallsCount = quantities[(int)BO.CallStatus.Open];
                InProgressCallsCount = quantities[(int)BO.CallStatus.InProgress];
                OpenInRiskCallsCount = quantities[(int)BO.CallStatus.OpenInRisk];
                InProgressInRiskCallsCount = quantities[(int)BO.CallStatus.InProgressInRisk];
                TreatedCallsCount = quantities[(int)BO.CallStatus.Treated];
                ExpiredCallsCount = quantities[(int)BO.CallStatus.Expired];
            }
        }
   
}