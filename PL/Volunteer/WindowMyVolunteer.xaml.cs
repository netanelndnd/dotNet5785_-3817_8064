using PL.call;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows.Threading;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;


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

        private async void UpdateMapImage(int _volunteerId)
        {
            IsMapLoading = true; // הצג ProgressBar

            try
            {
                // שליפת פרטי המתנדב
                var volunteerDetails = s_bl.Volunteer.GetVolunteerDetails(_volunteerId);
                string apiKey = "AIzaSyBnuV561P8tA08Y7DQDH0GAu5AhQ86m5xs";

                // בדיקה אם אין קריאה פעילה
                if (volunteerDetails.CurrentCall == null)
                {
                    VolunteerImageMap = null;
                    return;
                }

                // קבלת קואורדינטות של הקריאה
                var call = Helpers.Tools.GetCoordinates(volunteerDetails.CurrentCall.FullAddress);

                // חישוב גבולות, מרכז המפה, והבדלים גיאוגרפיים
                double minLatitude = Math.Min(volunteerDetails.Latitude.Value, call.Latitude);
                double maxLatitude = Math.Max(volunteerDetails.Latitude.Value, call.Latitude);
                double minLongitude = Math.Min(volunteerDetails.Longitude.Value, call.Longitude);
                double maxLongitude = Math.Max(volunteerDetails.Longitude.Value, call.Longitude);
                double centerLatitude = (minLatitude + maxLatitude) / 2;
                double centerLongitude = (minLongitude + maxLongitude) / 2;
                double latDiff = maxLatitude - minLatitude;
                double lngDiff = maxLongitude - minLongitude;

                // חישוב זום (מוגדל יותר)
                double maxDiff = Math.Max(latDiff, lngDiff);
                int zoom = (int)(Math.Log(360 / maxDiff) / Math.Log(2)) + 1; // זום מוגדל יותר
                zoom = Math.Clamp(zoom, 1, 20);

                // חישוב גודל התמונה (סטטי בגבולות פשוטים)
                double distance = Helpers.Tools.CalculateDistance(
                    volunteerDetails.Latitude.Value,
                    volunteerDetails.Longitude.Value,
                    call.Latitude,
                    call.Longitude
                );
                int size = (int)Math.Clamp(distance * 20, 800, 1200); // גודל סטטי יותר

                // יצירת פרמטרים למפה
                string markerParams = $"markers=color:blue|label:Call|{call.Latitude},{call.Longitude}";
                string specialMarkerParams = $"markers=color:red|label:★|{volunteerDetails.Latitude},{volunteerDetails.Longitude}";
                string pathParams = $"path=color:green|weight:3|{call.Latitude},{call.Longitude}|{volunteerDetails.Latitude},{volunteerDetails.Longitude}";

                // בניית URL
                string mapUrl = $"https://maps.googleapis.com/maps/api/staticmap?center={centerLatitude},{centerLongitude}&zoom={zoom}&size={size}x{size}&maptype=roadmap&{markerParams}&{specialMarkerParams}&{pathParams}&key={apiKey}";

                await Task.Run(async () =>
                {
                    // טען את הנתונים כ-bytes
                    using (var client = new WebClient())
                    {
                        var imageBytes = await client.DownloadDataTaskAsync(mapUrl);
                        return imageBytes;
                    }
                }).ContinueWith(task =>
                {
                    if (task.IsCompletedSuccessfully)
                    {
                        var bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.StreamSource = new MemoryStream(task.Result);
                        bitmap.CacheOption = BitmapCacheOption.OnLoad;
                        bitmap.EndInit();
                        VolunteerImageMap = bitmap;
                    }
                }, TaskScheduler.FromCurrentSynchronizationContext());
            }
            finally
            {
                IsMapLoading = false; // הסתר ProgressBar
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

        // CLR wrapper for the Dependency Property
        public ImageSource VolunteerImageMap
        {
            get { return (ImageSource)GetValue(VolunteerImageMapProperty); }
            set { SetValue(VolunteerImageMapProperty, value); }
        }

        // Define the Dependency Property
        public static readonly DependencyProperty VolunteerImageMapProperty =
            DependencyProperty.Register("VolunteerImageMap", typeof(ImageSource), typeof(WindowMyVolunteer), new PropertyMetadata(null));

        // DependencyProperty for IsMapLoading
        public static readonly DependencyProperty IsMapLoadingProperty =
            DependencyProperty.Register("IsMapLoading", typeof(bool), typeof(WindowMyVolunteer), new PropertyMetadata(false));

        public bool IsMapLoading
        {
            get { return (bool)GetValue(IsMapLoadingProperty); }
            set { SetValue(IsMapLoadingProperty, value); }
        }

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
                    MessageBox.Show("המתנדב עודכן בהצלחה!", "הצלחה", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (BO.BlDoesNotExistException ex)
                {
                    MessageBox.Show("המתנדב לא נמצא: " + ex.Message, "שגיאה", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (BO.BlOperationException ex)
                {
                    MessageBox.Show("הפעולה נכשלה: " + ex.Message, "שגיאה", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("אירעה שגיאה בלתי צפויה: " + ex.Message, "שגיאה", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        // Method to validate input data
        private bool ValidateInput()
        {
            // Validate email format
            if (string.IsNullOrWhiteSpace(CurrentVolunteer?.Email) || !Helpers.VolunteerManager.IsValidEmail(CurrentVolunteer.Email))
            {
                MessageBox.Show("פורמט אימייל לא תקין.", "שגיאה", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            // Validate phone number format
            if (string.IsNullOrWhiteSpace(CurrentVolunteer?.PhoneNumber) || !Helpers.VolunteerManager.IsValidPhoneNumber(CurrentVolunteer.PhoneNumber))
            {
                MessageBox.Show("פורמט מספר טלפון לא תקין.", "שגיאה", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            // Validate full name is not null or empty
            if (string.IsNullOrWhiteSpace(CurrentVolunteer?.FullName))
            {
                MessageBox.Show("שם מלא לא יכול להיות ריק.", "שגיאה", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            // Validate current address is not null or empty
            if (string.IsNullOrWhiteSpace(CurrentVolunteer?.CurrentAddress))
            {
                MessageBox.Show("כתובת נוכחית לא יכולה להיות ריקה.", "שגיאה", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            // Validate max distance is not null and is a number
            if (CurrentVolunteer?.MaxDistance == null || !double.TryParse(CurrentVolunteer.MaxDistance.ToString(), out _))
            {
                MessageBox.Show("מרחק מקסימלי חייב להיות מספר תקין.", "שגיאה", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            return true;
        }


        // Observer method to refresh the volunteer details
        private volatile DispatcherOperation? _observerOperation = null; //stage 7

        private void VolunteerObserver() //stage 7
        {
            if (_observerOperation is null || _observerOperation.Status == DispatcherOperationStatus.Completed)
                _observerOperation = Dispatcher.BeginInvoke(() =>
                {
                    int id = CurrentVolunteer!.Id;
                    CurrentVolunteer = null;
                    CurrentVolunteer = s_bl.Volunteer.GetVolunteerDetails(id);
                    UpdateMapImage(id);
                });
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


        private void BtnUpdatePassword_Click(object sender, RoutedEventArgs e)
        {
            PasswordInputWindow passwordInputWindow = new PasswordInputWindow();
            if (passwordInputWindow.ShowDialog() == true)
            {
                string newPassword = passwordInputWindow.Password;

                if (ValidatePassword(newPassword))
                {
                    CurrentVolunteer.Password = newPassword;
                    SaveVolunteerData();
                    MessageBox.Show("Password updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private bool ValidatePassword(string password)
        {
            if (!HasLowerCase(password))
            {
                MessageBox.Show("הסיסמה חייבת לכלול אותיות קטנות.", "סיסמה חלשה", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (!HasUpperCase(password))
            {
                MessageBox.Show("הסיסמה חייבת לכלול אותיות גדולות.", "סיסמה חלשה", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (!HasDigit(password))
            {
                MessageBox.Show("הסיסמה חייבת לכלול מספרים.", "סיסמה חלשה", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (!HasSpecialCharacter(password))
            {
                MessageBox.Show("הסיסמה חייבת לכלול תווים מיוחדים (כמו @ או #).", "סיסמה חלשה", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (!HasNumPas(password))
            {
                MessageBox.Show("הסיסמה חייבת להיות באורך של בין 8 ל-15 תווים.", "סיסמה חלשה", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            return true;
        }


         
        public static bool HasLowerCase(string password)
        {
            return Regex.IsMatch(password, @"[a-z]");
        }

        public static bool HasUpperCase(string password)
        {
            return Regex.IsMatch(password, @"[A-Z]");
        }

        public static bool HasDigit(string password)
        {
            return Regex.IsMatch(password, @"\d");
        }

        public static bool HasSpecialCharacter(string password)
        {
            return Regex.IsMatch(password, @"[^\da-zA-Z]");
        }
        public static bool HasNumPas(string password)
        {
            return password.Length >= 8 && password.Length <= 15;
        }


    }
}
