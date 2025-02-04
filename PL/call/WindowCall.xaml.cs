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
using DalApi;

namespace PL.call
{
    /// <summary>
    /// Interaction logic for WindowCall.xaml
    /// </summary>
    public partial class WindowCall : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        public WindowCall(int id = 0, int userId = 0)
        {
            // Assign the CurrentCall property based on the id parameter
            CurrentCall = (id != 0) ? s_bl.Call.GetCallDetails(id) :
                new BO.Call { OpenedAt = s_bl.Admin.GetSystemClock() };
            ButtonText = (id != 0) ? "Update" : "Add";
            CurrentManager = userId;

            InitializeComponent();

            // Register event handlers for loading and closing the window
            this.Loaded += Window_Loaded;
            this.Closed += Window_Closed;
        }

        int CurrentManager = 0;

        // CLR wrapper for the Dependency Property
        public BO.Call? CurrentCall
        {
            get { return (BO.Call?)GetValue(CurrentCallProperty); }
            set { SetValue(CurrentCallProperty, value); }
        }

        // Define the Dependency Property
        public static readonly DependencyProperty CurrentCallProperty =
            DependencyProperty.Register("CurrentCall", typeof(BO.Call), typeof(WindowCall), new PropertyMetadata(null));

        public string ButtonText { get; set; }

        private void btnAddUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validate address format
                if (string.IsNullOrWhiteSpace(CurrentCall?.FullAddress))
                {
                    MessageBox.Show("Invalid address format.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (ButtonText == "Add")
                {
                    s_bl.Call.AddCall(CurrentCall!);
                    MessageBox.Show("Call added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Call updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                this.Close();
            }
            catch (BO.BlDoesNotExistException ex)
            {
                MessageBox.Show("Call not found: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to delete this call?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    if (CurrentCall!.Id != 0)
                        s_bl.Call.RemoveObserver(CallObserver);
                    s_bl.Call.DeleteCall(CurrentCall!.Id);
                    MessageBox.Show("Call deleted successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.Close();
                }
                catch (BO.BlDoesNotExistException ex)
                {
                    MessageBox.Show("Call not found: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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

        private void btnCancelAssignment_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentCall?.Status == BO.CallStatus.InProgress || CurrentCall?.Status == BO.CallStatus.InProgressInRisk)
            {
                var result = MessageBox.Show("Are you sure you want to cancel the current assignment?", "Confirm Cancel", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        var assignmentId = Helpers.AssignmentManager.GetAssignmentIdByCallId(CurrentCall.Id);
                        if (assignmentId.HasValue)
                        {
                            s_bl.Call.CancelCallHandling(CurrentManager, assignmentId.Value);
                            MessageBox.Show("Assignment cancelled successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                            CallObserver(); // Refresh the call details
                        }
                        else
                        {
                            MessageBox.Show("No active assignment found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("An error occurred while cancelling the assignment: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Assignment can only be cancelled if it is currently in progress.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private volatile DispatcherOperation? _observerOperation = null; //stage 7

        // Observer method to refresh the call details
        private void CallObserver() //stage 7
        {
            if (_observerOperation is null || _observerOperation.Status == DispatcherOperationStatus.Completed)
                _observerOperation = Dispatcher.BeginInvoke(() =>
                {
                    int id = CurrentCall!.Id;
                    CurrentCall = null;
                    CurrentCall = s_bl.Call.GetCallDetails(id);
                });
        }

        // Event handler for when the window is loaded
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (CurrentCall!.Id != 0)
            {
                s_bl.Call.AddObserver(CallObserver);
            }
        }

        // Event handler for when the window is closed
        private void Window_Closed(object sender, EventArgs e)
        {
            if (CurrentCall != null && CurrentCall.Id != 0)
                s_bl.Call.RemoveObserver(CallObserver);
        }
    }
}
