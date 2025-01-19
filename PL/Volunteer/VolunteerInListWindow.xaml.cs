using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PL.Volunteer
{
    /// <summary>
    /// Interaction logic for VolunteerInListWindow.xaml
    /// </summary>
    public partial class VolunteerInListWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        public VolunteerInListWindow()
        {
            InitializeComponent();
        }

        // Property to store and retrieve the list of volunteers.
        // This property uses a DependencyProperty to enable data binding in WPF.
        public IEnumerable<BO.VolunteerInList> VolunteerList
        {
            // Getter method to retrieve the value of the VolunteerListProperty.
            get { return (IEnumerable<BO.VolunteerInList>)GetValue(VolunteerListProperty); }

            // Setter method to set the value of the VolunteerListProperty.
            set { SetValue(VolunteerListProperty, value); }
        }

        // DependencyProperty to enable data binding for the VolunteerList property in WPF.
        // This allows the VolunteerList property to be used in XAML for data binding.
        public static readonly DependencyProperty VolunteerListProperty =
            DependencyProperty.Register("VolunteerList", typeof(IEnumerable<BO.VolunteerInList>), typeof(VolunteerInListWindow), new PropertyMetadata(null));

        // Property to store the type of call. It is initialized to 'None' by default.
        public BO.CallType callType { get; set; } = BO.CallType.None;

        // Property to store the selected volunteer in the list.
        public BO.VolunteerInList? SelectedVolunteer { get; set; }

        // Method to handle the selection of a call type in the ComboBox.
        // This method is triggered when the selection in the ComboBox changes.
        private void CallTyps_CB(object sender, SelectionChangedEventArgs e)
        {
            // Update the VolunteerList based on the selected call type.
            // If the call type is 'None', retrieve all volunteers.
            // Otherwise, filter the volunteers by the selected call type.
            VolunteerList = (callType == BO.CallType.None) ?
                s_bl?.Volunteer.GetCallTypsOfVolunteers(BO.CallType.None)! :
                s_bl?.Volunteer.GetCallTypsOfVolunteers(callType)!;
        }

        // Method to query the list of volunteers based on the current call type.
        // If the call type is 'None', it retrieves all volunteers.
        // Otherwise, it filters the volunteers by the selected call type.
        private void queryVolunteerList()
            => VolunteerList = (callType == BO.CallType.None) ?
               s_bl?.Volunteer.GetCallTypsOfVolunteers(BO.CallType.None)! : s_bl?.Volunteer.GetCallTypsOfVolunteers(callType)!;

        // Observer method to update the volunteer list when changes occur.
        private void volunteerListObserver()
            => queryVolunteerList();

        // Event handler for when the window is loaded.
        // Adds the volunteerListObserver as an observer to the Volunteer service.
        private void Window_Loaded(object sender, RoutedEventArgs e)
            => s_bl.Volunteer.AddObserver(volunteerListObserver);

        // Event handler for when the window is closed.
        // Removes the volunteerListObserver from the Volunteer service.
        private void Window_Closed(object sender, EventArgs e)
            => s_bl.Volunteer.RemoveObserver(volunteerListObserver);


        // Event handler for double-clicking on a volunteer in the list.
        private void lsvVolunteersList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (SelectedVolunteer != null)
                new WindowVolunteer(SelectedVolunteer.Id).Show();
        }

        // Event handler for clicking the Add button.
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            new WindowVolunteer().Show();
        }

        
    }
}
