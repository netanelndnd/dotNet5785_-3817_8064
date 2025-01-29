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
using System.Windows.Threading;

namespace PL.Volunteer
{
    /// <summary>
    /// Interaction logic for OneVolunteer.xaml
    /// </summary>
    public partial class WindowVolunteer : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        public WindowVolunteer(int id = 0)
        {
            // Assign the CurrentVolunteer property based on the id parameter
            CurrentVolunteer = (id != 0) ? s_bl.Volunteer.GetVolunteerDetails(id) : new BO.Volunteer();
            ButtonText = (id != 0) ? "Update" : "Add";

            InitializeComponent();


            // Register event handlers for loading and closing the window
            this.Loaded += Window_Loaded;
            this.Closed += Window_Closed;
        }
       

        // CLR wrapper for the Dependency Property
        public BO.Volunteer? CurrentVolunteer
        {
            get { return (BO.Volunteer?)GetValue(CurrentVolunteerProperty); }
            set { SetValue(CurrentVolunteerProperty, value); }
        }

        // Define the Dependency Property
        public static readonly DependencyProperty CurrentVolunteerProperty =
            DependencyProperty.Register("CurrentVolunteer", typeof(BO.Volunteer), typeof(WindowVolunteer), new PropertyMetadata(null));

        public string ButtonText { get; set; }

        private void btnAddUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validate email format
                if (string.IsNullOrWhiteSpace(CurrentVolunteer?.Email) || !Helpers.VolunteerManager.IsValidEmail(CurrentVolunteer.Email))
                {
                    MessageBox.Show("Invalid email format.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Validate phone number format
                if (string.IsNullOrWhiteSpace(CurrentVolunteer?.PhoneNumber) || !Helpers.VolunteerManager.IsValidPhoneNumber(CurrentVolunteer.PhoneNumber))
                {
                    MessageBox.Show("Invalid phone number format.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (ButtonText == "Add")
                {
                    s_bl.Volunteer.AddVolunteer(CurrentVolunteer!);
                    MessageBox.Show("Volunteer added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.Close();
                }
                else
                {
                    s_bl.Volunteer.UpdateVolunteer(CurrentVolunteer!.Id, CurrentVolunteer);
                    MessageBox.Show("Volunteer updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                
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
                });
        }

        // Event handler for when the window is loaded
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (CurrentVolunteer!.Id != 0)
                s_bl.Volunteer.AddObserver(CurrentVolunteer!.Id, VolunteerObserver);
        }

        // Event handler for when the window is closed
        private void Window_Closed(object sender, EventArgs e)
        {
            if (CurrentVolunteer!.Id != 0)
                s_bl.Volunteer.RemoveObserver(CurrentVolunteer!.Id, VolunteerObserver);
        }

        // Event handler for clicking the Delete button.
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button != null)
            {
                int volunteerId = CurrentVolunteer!.Id;
                var result = MessageBox.Show("Are you sure you want to delete this volunteer?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        s_bl.Volunteer.DeleteVolunteer(volunteerId);
                        MessageBox.Show("Volunteer deleted successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        this.Close();
                    }
                    catch (BO.BlDoesNotExistException ex)
                    {
                        MessageBox.Show("Volunteer not found: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    catch (BO.BlSystemException ex)
                    {
                        MessageBox.Show("An error occurred while deleting the volunteer: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("An unexpected error occurred: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

    }
}
