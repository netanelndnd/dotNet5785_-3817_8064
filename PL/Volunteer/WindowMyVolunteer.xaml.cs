using PL.call;
using System.Windows;
using System.Windows.Media.Imaging;

namespace PL.Volunteer
{
    public partial class WindowMyVolunteer : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        public WindowMyVolunteer(int id)
        {
            CurrentVolunteer = s_bl.Volunteer.GetVolunteerDetails(id);
            InitializeComponent();
            UpdateMapImage(id);
            // Register event handlers for loading and closing the window
            this.Loaded += Window_Loaded;
            this.Closed += Window_Closed;
        }
        private void UpdateMapImage(int _volunteerId)
        {
            var volunteerDetails = s_bl.Volunteer.GetVolunteerDetails(_volunteerId);
            string apiKey = "AIzaSyBnuV561P8tA08Y7DQDH0GAu5AhQ86m5xs";
            if (volunteerDetails.CurrentCall == null)
            {
                VolunteerImageMap.Source = null;
                return;
            }
            // Create a list to store the coordinates of the calls
            var call = Helpers.Tools.GetCoordinates(volunteerDetails.CurrentCall!.FullAddress);

            // Create the marker parameters for the map URL
            // Create the marker parameters for the call (blue marker)
            string markerParams = $"markers=color:blue|label:Call|{call.Latitude},{call.Longitude}";

            // Create the special marker parameter for the volunteer's location (red marker)
            string specialMarkerParams = $"markers=color:red|label:★|{volunteerDetails.Latitude},{volunteerDetails.Longitude}";

            // Create the path parameter for the line between the call and the volunteer (green path)
            string pathParams = $"path=color:green|weight:2|{call.Latitude},{call.Longitude}|{volunteerDetails.Latitude},{volunteerDetails.Longitude}";

            // Construct the map URL with all the parameters
            string mapUrl = $"https://maps.googleapis.com/maps/api/staticmap?center={volunteerDetails.Latitude},{volunteerDetails.Longitude}&zoom=9&size=600x400&maptype=roadmap&{markerParams}&{specialMarkerParams}&{pathParams}&key={apiKey}";

            // Check if the URL is valid and set the map image source
            if (Uri.TryCreate(mapUrl, UriKind.Absolute, out Uri uriResult) &&
               (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps))
            {
                VolunteerImageMap.Source = new BitmapImage(uriResult);
            }
        }
        // CLR wrapper for the Dependency Property
        public BO.Volunteer? CurrentVolunteer
        {
            get { return (BO.Volunteer?)GetValue(CurrentVolunteerProperty); }
            set { SetValue(CurrentVolunteerProperty, value); }
        }

        // Define the Dependency Property
        public static readonly DependencyProperty CurrentVolunteerProperty =
            DependencyProperty.Register("CurrentVolunteer", typeof(BO.Volunteer), typeof(WindowMyVolunteer), new PropertyMetadata(null));

        // Event handler for window loaded event
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (CurrentVolunteer!.Id != 0)
                s_bl.Volunteer.AddObserver(CurrentVolunteer!.Id, VolunteerObserver);
        }

        // Event handler for button click event
        private void BtnAddUpdate_Click(object sender, RoutedEventArgs e)
        {
            SaveVolunteerData();
        }

        // Method to save volunteer data
        private void SaveVolunteerData()
        {
            if (ValidateInput())
            {
                try
                {
                    s_bl.Volunteer.UpdateVolunteer(CurrentVolunteer!.Id, CurrentVolunteer);
                    UpdateMapImage(CurrentVolunteer!.Id);
                    MessageBox.Show("Volunteer updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (BO.BlDoesNotExistException ex)
                {
                    MessageBox.Show("Volunteer not found: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (BO.BlOperationException ex)
                {
                    MessageBox.Show("Operation failed: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An unexpected error occurred: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        // Method to validate input data
        private bool ValidateInput()
        {
            // Validate email format
            if (string.IsNullOrWhiteSpace(CurrentVolunteer?.Email) || !Helpers.VolunteerManager.IsValidEmail(CurrentVolunteer.Email))
            {
                MessageBox.Show("Invalid email format.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            // Validate phone number format
            if (string.IsNullOrWhiteSpace(CurrentVolunteer?.PhoneNumber) || !Helpers.VolunteerManager.IsValidPhoneNumber(CurrentVolunteer.PhoneNumber))
            {
                MessageBox.Show("Invalid phone number format.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            // Validate full name is not null or empty
            if (string.IsNullOrWhiteSpace(CurrentVolunteer?.FullName))
            {
                MessageBox.Show("Full name cannot be empty.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            // Validate current address is not null or empty
            if (string.IsNullOrWhiteSpace(CurrentVolunteer?.CurrentAddress))
            {
                MessageBox.Show("Current address cannot be empty.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            // Validate max distance is not null and is a number
            if (CurrentVolunteer?.MaxDistance == null || !double.TryParse(CurrentVolunteer.MaxDistance.ToString(), out _))
            {
                MessageBox.Show("Max distance must be a valid number.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            return true;
        }

        // Observer method to refresh the volunteer details
        private void VolunteerObserver()
        {
            int id =CurrentVolunteer!.Id;
            CurrentVolunteer = null;
            CurrentVolunteer = s_bl.Volunteer.GetVolunteerDetails(id);
        }

        // Event handler for when the window is closed
        private void Window_Closed(object sender, EventArgs e)
        {
            if (CurrentVolunteer!.Id != 0)
                s_bl.Volunteer.RemoveObserver(CurrentVolunteer!.Id, VolunteerObserver);
        }

        // Event handler for completing a call
        private void btnCompleteCall_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentVolunteer?.CurrentCall != null)
            {
                try
                {
                    s_bl.Call.CompleteCallHandling(CurrentVolunteer.Id, CurrentVolunteer.CurrentCall.Id);
                    MessageBox.Show("Call completed successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    UpdateMapImage(CurrentVolunteer!.Id);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred while completing the call: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

        }

        // Event handler for canceling a call
        private void btnCancelCall_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentVolunteer?.CurrentCall != null)
            {
                try
                {
                    s_bl.Call.CancelCallHandling(CurrentVolunteer.Id, CurrentVolunteer.CurrentCall.Id);
                    UpdateMapImage(CurrentVolunteer!.Id);

                    MessageBox.Show("Call canceled successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred while canceling the call: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

        }

        // Event handler for selecting a call
        private void btnSelectCall_Click(object sender, RoutedEventArgs e)
        {
            // Logic to navigate to the "Select Call for Handling" screen
            if (CurrentVolunteer?.CurrentCall == null)
            {
                SelectCallWindow selectCallWindow = new SelectCallWindow(CurrentVolunteer!.Id);
                selectCallWindow.ShowDialog();
                UpdateMapImage(CurrentVolunteer!.Id);
            }
        }

        // Event handler for viewing call history
        private void btnCallHistory_Click(object sender, RoutedEventArgs e)
        {
            // Logic to view call history
            CallHistoryOfVolunteer callHistory = new CallHistoryOfVolunteer(CurrentVolunteer!.Id);
            callHistory.ShowDialog();
        }
    }
}
