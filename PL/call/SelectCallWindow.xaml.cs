using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Policy;
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
using System.Windows.Threading;

namespace PL.Volunteer
{
    public partial class SelectCallWindow : Window
    {
        private readonly int _volunteerId;
        private readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        private IEnumerable<BO.OpenCallInList> _openCalls;

        public SelectCallWindow(int volunteerId)
        {
            _volunteerId = volunteerId;
            InitializeComponent();
            UpdateMapImage(_volunteerId);

            // Register event handlers for loading and closing the window
            this.Loaded += Window_Loaded;
            this.Closed += Window_Closed;
        }

        private async void UpdateMapImage(int _volunteerId)
        {
            IsMapLoading = true; // הצג ProgressBar

            try
            {
                var volunteerDetails = s_bl.Volunteer.GetVolunteerDetails(_volunteerId);
                string apiKey = "AIzaSyBnuV561P8tA08Y7DQDH0GAu5AhQ86m5xs";

                if (SelectedCall == null)
                {
                    MapImageSource = null;
                    return;
                }

                var call = s_bl.Call.GetCallDetails(SelectedCall.Id);

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
                        MapImageSource = bitmap;
                    }
                }, TaskScheduler.FromCurrentSynchronizationContext());
            }
            finally
            {
                IsMapLoading = false; // הסתר ProgressBar
            }
        }

        // Property to store and retrieve the list of open calls.
        // This property uses a DependencyProperty to enable data binding in WPF.
        public IEnumerable<BO.OpenCallInList> OpenCalls
        {
            // Getter method to retrieve the value of the OpenCallsProperty.
            get { return (IEnumerable<BO.OpenCallInList>)GetValue(OpenCallsProperty); }

            // Setter method to set the value of the OpenCallsProperty.
            set { SetValue(OpenCallsProperty, value); }
        }

        // DependencyProperty to enable data binding for the OpenCalls property in WPF.
        // This allows the OpenCalls property to be used in XAML for data binding.
        public static readonly DependencyProperty OpenCallsProperty =
            DependencyProperty.Register("OpenCalls", typeof(IEnumerable<BO.OpenCallInList>), typeof(SelectCallWindow), new PropertyMetadata(null));

        // Method to query the list of open calls for the volunteer.
        private void queryOpenCalls()
            => OpenCalls = (callType == BO.CallType.None) ?
            s_bl.Call.GetOpenCallsForVolunteer(_volunteerId, null, null)! : s_bl?.Call.GetOpenCallsForVolunteer(_volunteerId, callType, null)!;

        private volatile DispatcherOperation? _observerOperation = null; //stage 7


        // Observer method to update the open calls list when changes occur.
        private void openCallsObserver() //stage 7
        {
            if (_observerOperation is null || _observerOperation.Status == DispatcherOperationStatus.Completed)
                _observerOperation = Dispatcher.BeginInvoke(() =>
                {
                    queryOpenCalls();
                });
        }

        // Event handler for when the window is loaded.
        // Adds the openCallsObserver as an observer to the Call service.
        private void Window_Loaded(object sender, RoutedEventArgs e)
            => s_bl.Call.AddObserver(openCallsObserver);

        // Event handler for when the window is closed.
        // Removes the openCallsObserver from the Call service.
        private void Window_Closed(object sender, EventArgs e)
            => s_bl.Call.RemoveObserver(openCallsObserver);

        // Property to store the type of call. It is initialized to 'None' by default.
        public BO.CallType callType { get; set; } = BO.CallType.None;

        // Method to handle the selection of a call type in the ComboBox.
        // This method is triggered when the selection in the ComboBox changes.
        private void CallTyps_CB(object sender, SelectionChangedEventArgs e)
        {
            // Update the OpenCalls based on the selected call type.
            // If the call type is 'None', retrieve all open calls.
            // Otherwise, filter the open calls by the selected call type.
            OpenCalls = (callType == BO.CallType.None) ?
                s_bl?.Call.GetOpenCallsForVolunteer(_volunteerId, null, null)! :
                s_bl?.Call.GetOpenCallsForVolunteer(_volunteerId, callType, null)!;
        }

        public static readonly DependencyProperty SelectedCallProperty =
            DependencyProperty.Register("SelectedCall", typeof(BO.OpenCallInList), typeof(SelectCallWindow), new PropertyMetadata(null, OnSelectedCallChanged));

        public BO.OpenCallInList? SelectedCall
        {
            get { return (BO.OpenCallInList?)GetValue(SelectedCallProperty); }
            set { SetValue(SelectedCallProperty, value); }
        }

        private static void OnSelectedCallChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var window = d as SelectCallWindow;
            window?.UpdateMapImage(window._volunteerId);
        }

        // Event handler for double-clicking on a call in the list.
        private void CallsDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (SelectedCall != null)
            {
                try
                {
                    s_bl.Call.AssignCallToVolunteer(_volunteerId, SelectedCall.Id);
                    queryOpenCalls();
                    MessageBox.Show("Call assigned successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.Close(); // Close the SelectCallWindow
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred while assigning the call: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void BtnUpdateAddress_Click(object sender, RoutedEventArgs e)
        {
            // יצירת חלון חדש לקבלת הכתובת החדשה
            var addressWindow = new UpdateAddressWindow();
            if (addressWindow.ShowDialog() == true)
            {
                // קבלת הכתובת החדשה מהחלון
                string newAddress = addressWindow.NewAddress;

                // קבלת פרטי המתנדב הנוכחיים
                var volunteer = s_bl.Volunteer.GetVolunteerDetails(_volunteerId);

                // עדכון הכתובת של המתנדב
                volunteer.CurrentAddress = newAddress;

                // שמירת העדכונים
                s_bl.Volunteer.UpdateVolunteer(_volunteerId, volunteer);
                UpdateMapImage(_volunteerId);
                // עדכון רשימת הקריאות
                queryOpenCalls();
                MessageBox.Show("Address updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        // DependencyProperty for MapImageSource
        public static readonly DependencyProperty MapImageSourceProperty =
            DependencyProperty.Register("MapImageSource", typeof(ImageSource), typeof(SelectCallWindow), new PropertyMetadata(null));

        public ImageSource MapImageSource
        {
            get { return (ImageSource)GetValue(MapImageSourceProperty); }
            set { SetValue(MapImageSourceProperty, value); }
        }

        // DependencyProperty for IsMapLoading
        public static readonly DependencyProperty IsMapLoadingProperty =
            DependencyProperty.Register("IsMapLoading", typeof(bool), typeof(SelectCallWindow), new PropertyMetadata(false));

        public bool IsMapLoading
        {
            get { return (bool)GetValue(IsMapLoadingProperty); }
            set { SetValue(IsMapLoadingProperty, value); }
        }
    }

}


