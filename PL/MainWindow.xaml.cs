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

        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        // Method called when the window is loaded
        public void Loaded_Screen()
        {
            CurrentTime = s_bl.Admin.GetSystemClock();
            TimeRisk = s_bl.Admin.GetRiskTimeSpan();
            s_bl.Admin.AddClockObserver(clockObserver);
            s_bl.Admin.AddConfigObserver(configObserver);
        }
        public void Closed_Screen()
        {
            s_bl.Admin.RemoveClockObserver(clockObserver);
            s_bl.Admin.RemoveConfigObserver(configObserver);
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
            s_bl.Admin.AddClockObserver(() => s_bl.Admin.AdvanceSystemClock(BO.TimeUnit.Minute));
        }

        // Button click event to add one hour to the system clock
        private void btnAddOneHour_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.AddClockObserver(() => s_bl.Admin.AdvanceSystemClock(BO.TimeUnit.Hour));
        }

        // Button click event to add one day to the system clock
        private void btnAddOneDay_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.AddClockObserver(() => s_bl.Admin.AdvanceSystemClock(BO.TimeUnit.Day));
        }

        // Button click event to add one month to the system clock
        private void btnAddOneMonth_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.AddClockObserver(() => s_bl.Admin.AdvanceSystemClock(BO.TimeUnit.Month));
        }

        // Button click event to add one year to the system clock
        private void btnAddOneYear_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.AddClockObserver(() => s_bl.Admin.AdvanceSystemClock(BO.TimeUnit.Year));
        }

        // Button click event to update the risk time span
        private void btnUpdateTimeSpan_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.AddConfigObserver(() => s_bl.Admin.SetRiskTimeSpan(TimeRisk));
        }
        private void btnListVolunteers_Click(object sender, RoutedEventArgs e)
        { new VolunteerInList().Show(); }
        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult mbResult = MessageBox.Show("Do you Sure that you want to continue?", "Reset",
                MessageBoxButton.YesNoCancel, MessageBoxImage.Question,
                MessageBoxResult.Cancel);

            switch (mbResult)
            {
                case MessageBoxResult.Yes:
                    s_bl.Admin.ResetDatabase();
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
                    s_bl.Admin.InitializeDatabase();
                    break;
                case MessageBoxResult.No:
                    break;
            }
        }
    }
}