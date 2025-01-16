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
    public partial class SelectCallWindow : Window
    {
        private readonly int _volunteerId;
        private readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        private IEnumerable<BO.OpenCallInList> _openCalls;

        public SelectCallWindow(int volunteerId)
        {
            _volunteerId = volunteerId;
            InitializeComponent();
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


        // Observer method to update the open calls list when changes occur.
        private void openCallsObserver()
            => queryOpenCalls();

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


        // Event handler for double-clicking on a call in the list.
        private void CallsDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (CallsDataGrid.SelectedItem is BO.OpenCallInList selectedCall)
            {
                try
                {
                    s_bl.Call.AssignCallToVolunteer(_volunteerId, selectedCall.Id);
                    MessageBox.Show("Call assigned successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    queryOpenCalls();
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

                // עדכון הכתובת והקואורדינטות של המתנדב
                var coordinates = Helpers.Tools.GetCoordinates(newAddress);
                if (coordinates.IsInIsrael)
                {
                    volunteer.CurrentAddress = newAddress;
                    volunteer.Latitude = coordinates.Latitude;
                    volunteer.Longitude = coordinates.Longitude;

                    // שמירת העדכונים
                    s_bl.Volunteer.UpdateVolunteer(_volunteerId, volunteer);
                    MessageBox.Show("Address updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                    // עדכון רשימת הקריאות
                    queryOpenCalls();
                }
                else
                {
                    MessageBox.Show("Invalid address. Please enter a valid address in Israel.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

    }
}
