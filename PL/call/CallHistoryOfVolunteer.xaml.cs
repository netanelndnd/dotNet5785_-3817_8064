﻿using System;
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

namespace PL.call
{
    /// <summary>
    /// Interaction logic for CallHistoryOfVolunteer.xaml
    /// </summary>
    public partial class CallHistoryOfVolunteer : Window
    {
        private readonly int _volunteerId;
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        private IEnumerable<BO.ClosedCallInList> _callsHistory;
        public CallHistoryOfVolunteer(int volunterrID)
        {
            _volunteerId = volunterrID;
            InitializeComponent();

            // Register event handlers for loading and closing the window
            this.Loaded += Window_Loaded;
            this.Closed += Window_Closed;
        }
        
        public IEnumerable<BO.ClosedCallInList> CallHistory
        {
            get { return (IEnumerable<BO.ClosedCallInList>)GetValue(CallHistoryProperty); }

            // Setter method to set the value of the CallListProperty.
            set { SetValue(CallHistoryProperty, value); }
        }

        public static readonly DependencyProperty CallHistoryProperty =
            DependencyProperty.Register("CallHistory", typeof(IEnumerable<BO.ClosedCallInList>), typeof(CallHistoryOfVolunteer), new PropertyMetadata(null));
        private void queryCallHistory() =>
            CallHistory = (callType == BO.CallType.None) ?
            s_bl.Call.GetClosedCallsByVolunteer(_volunteerId, null, null)! : s_bl?.Call.GetClosedCallsByVolunteer(_volunteerId, callType, null)!;

        private volatile DispatcherOperation? _observerOperation = null; //stage 7

        private void callsHistoryObserver()
        {
            if (_observerOperation is null || _observerOperation.Status == DispatcherOperationStatus.Completed)
                _observerOperation = Dispatcher.BeginInvoke(() =>
                {
                    queryCallHistory();
                });
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
            => s_bl.Call.AddObserver(callsHistoryObserver);

        // Event handler for when the window is closed.
        // Removes the callListObserver from the Call service.
        private void Window_Closed(object sender, EventArgs e)
            => s_bl.Call.RemoveObserver(callsHistoryObserver);

        public BO.CallType callType { get; set; } = BO.CallType.None;

        public BO.ClosedCallInListFields callstatus = BO.ClosedCallInListFields.CompletionStatus;
        private void callsHistory_CB(object sender, SelectionChangedEventArgs e)
        {
            CallHistory = (callType == BO.CallType.None) ?
                s_bl?.Call.GetClosedCallsByVolunteer(_volunteerId, null, callstatus)! :
                s_bl?.Call.GetClosedCallsByVolunteer(_volunteerId, callType, callstatus)!;
        }
    }
}
