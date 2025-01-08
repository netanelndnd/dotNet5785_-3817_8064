using PL.Volunteer;
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
        { new VolunteerInListWindow().Show(); }



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
                        // Close all open windows except the main window
                        foreach (Window window in Application.Current.Windows)
                        {
                            if (window != this)
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
            s_bl.Admin.AddClockObserver(clockObserver);
            s_bl.Admin.AddConfigObserver(configObserver);
        }

        // Method called when the window is closed
        private void Window_Closed(object sender, EventArgs e)
        {
            s_bl.Admin.RemoveClockObserver(clockObserver);
            s_bl.Admin.RemoveConfigObserver(configObserver);
        }
    }
}