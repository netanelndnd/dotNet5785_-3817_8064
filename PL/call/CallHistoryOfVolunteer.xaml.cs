using BO;
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
    /// Interaction logic for CallHistoryOfVolunteer.xaml
    /// </summary>
    public partial class CallHistoryOfVolunteer : Window
    {
        private readonly int _volunteerId;
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        public CallHistoryOfVolunteer(int volunterrID)
        {
            InitializeComponent();
            _volunteerId = volunterrID;
        }
        public BO.CallType callType { get; set; } = BO.CallType.None;
        public IEnumerable<BO.ClosedCallInList> callHistory
        {
            get { return (IEnumerable<BO.ClosedCallInList>)GetValue(callHistoryProperty); }

            // Setter method to set the value of the CallListProperty.
            set { SetValue(callHistoryProperty, value); }
        }
        public static readonly DependencyProperty callHistoryProperty =
            DependencyProperty.Register("callHistory", typeof(IEnumerable<BO.ClosedCallInList>), typeof(CallInListWindow), new PropertyMetadata(null));
        private void queryCallHistory() => 
            callHistory = (callType == BO.CallType.None) ?
            s_bl.Call.GetClosedCallsByVolunteer(_volunteerId, null, null)! : s_bl?.Call.GetClosedCallsByVolunteer(_volunteerId, callType, null)!;
        private void callHistoryObserver() => queryCallHistory();
        private void Window_Loaded(object sender, RoutedEventArgs e)
            => s_bl.Call.AddObserver(callHistoryObserver);

        // Event handler for when the window is closed.
        // Removes the callListObserver from the Call service.
        private void Window_Closed(object sender, EventArgs e)
            => s_bl.Call.RemoveObserver(callHistoryObserver);

        
        private void callHistory_CB(object sender, SelectionChangedEventArgs e)
        {
            callHistory = (callType == BO.CallType.None) ?
                s_bl?.Call.GetClosedCallsByVolunteer(_volunteerId, null, null)! :
                s_bl?.Call.GetClosedCallsByVolunteer(_volunteerId, callType, null)!;
        }
    }
}
