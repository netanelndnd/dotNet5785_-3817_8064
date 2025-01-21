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

namespace PL.call
{
    /// <summary>
    /// Interaction logic for CallInListWindow.xaml
    /// </summary>
    public partial class CallInListWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        public CallInListWindow()
        {
            InitializeComponent();

            // Register event handlers for loading and closing the window
            this.Loaded += Window_Loaded;
            this.Closed += Window_Closed;
        }

        // Property to store and retrieve the list of calls.
        // This property uses a DependencyProperty to enable data binding in WPF.
        public IEnumerable<BO.CallInList> CallList
        {
            // Getter method to retrieve the value of the CallListProperty.
            get { return (IEnumerable<BO.CallInList>)GetValue(CallListProperty); }

            // Setter method to set the value of the CallListProperty.
            set { SetValue(CallListProperty, value); }
        }

        // DependencyProperty to enable data binding for the CallList property in WPF.
        // This allows the CallList property to be used in XAML for data binding.
        public static readonly DependencyProperty CallListProperty =
            DependencyProperty.Register("CallList", typeof(IEnumerable<BO.CallInList>), typeof(CallInListWindow), new PropertyMetadata(null));

        // Property to store the type of call. It is initialized to 'None' by default.
        public BO.CallType callType { get; set; } = BO.CallType.None;

        // Property to store the selected call in the list.
        public BO.CallInList? SelectedCall { get; set; }

        // Method to handle the selection of a call type in the ComboBox.
        // This method is triggered when the selection in the ComboBox changes.
        private void CallTyps_CB(object sender, SelectionChangedEventArgs e)
        {
            // Update the CallList based on the selected call type.
            // If the call type is 'None', retrieve all calls.
            // Otherwise, filter the calls by the selected call type.
            CallList = (callType == BO.CallType.None) ?
                s_bl?.Call.GetCallList(null, null, null)! :
                s_bl?.Call.GetCallList(BO.CallInListFields.CallType, callType, null)!;
        }

        // Method to query the list of calls based on the current call type.
        // If the call type is 'None', it retrieves all calls.
        // Otherwise, it filters the calls by the selected call type.
        private void queryCallList()
            => CallList = (callType == BO.CallType.None) ?
               s_bl?.Call.GetCallList(null, null, null)! : s_bl?.Call.GetCallList(BO.CallInListFields.CallType, callType, null)!;

        // Observer method to update the call list when changes occur.
        private void callListObserver()
            => queryCallList();

        // Event handler for when the window is loaded.
        // Adds the callListObserver as an observer to the Call service.
        private void Window_Loaded(object sender, RoutedEventArgs e)
            => s_bl.Call.AddObserver(callListObserver);

        // Event handler for when the window is closed.
        // Removes the callListObserver from the Call service.
        private void Window_Closed(object sender, EventArgs e)
            => s_bl.Call.RemoveObserver(callListObserver);

        // Event handler for double-clicking on a call in the list.
        private void lsvCallsList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (SelectedCall != null)
                new WindowCall(SelectedCall.CallId).Show();
        }

        // Event handler for clicking the Add button.
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            new WindowCall().Show();
        }

        
    }
}
