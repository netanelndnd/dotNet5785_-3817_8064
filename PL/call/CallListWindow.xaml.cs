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
    public partial class CallListWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        public CallListWindow(string status)
        {
            InitializeComponent();
            LoadCalls(status);
        }

        public IEnumerable<BO.CallInList> CallList
        {
            get { return (IEnumerable<BO.CallInList>)GetValue(CallListProperty); }
            set { SetValue(CallListProperty, value); }
        }

        public static readonly DependencyProperty CallListProperty =
            DependencyProperty.Register("CallList", typeof(IEnumerable<BO.CallInList>), typeof(CallListWindow), new PropertyMetadata(null));

        private void LoadCalls(string status)
        {
            BO.CallStatus callStatus = (BO.CallStatus)Enum.Parse(typeof(BO.CallStatus), status.Replace(" ", ""));
            CallList = s_bl.Call.GetCallList(BO.CallInListFields.Status, callStatus, null);
        }
    }
}

