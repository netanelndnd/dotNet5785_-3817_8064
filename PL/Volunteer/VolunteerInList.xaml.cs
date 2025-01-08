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
    /// <summary>
    /// Interaction logic for VolunteerInList.xaml
    /// </summary>
    public partial class VolunteerInList : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        public VolunteerInList()
        {
            InitializeComponent();
        }
        public IEnumerable<BO.VolunteerInList> VolunteerList
        {
            get { return (IEnumerable<BO.VolunteerInList>)GetValue(VolunteerListProperty); }
            set { SetValue(VolunteerListProperty, value); }
        }

        public static readonly DependencyProperty VolunteerListProperty =
            DependencyProperty.Register("VolunteerList", typeof(IEnumerable<BO.VolunteerInList>), typeof(VolunteerInList), new PropertyMetadata(null));
        public BO.CallType callType { get; set; } = BO.CallType.None;

        private void CallTyps_CB(object sender, SelectionChangedEventArgs e)
        {
            VolunteerList = (callType == BO.CallType.None) ?
                s_bl?.Volunteer.GetVolunteers(null, null,null)! :
                s_bl?.Volunteer.GetVolunteers(null, null,callType)!;
        }


        private void queryVolunteerList()
             => VolunteerList = (callType == BO.CallType.None) ?
             s_bl?.Volunteer.GetVolunteers(null, null, null)! : s_bl?.Volunteer.GetVolunteers(null, null, callType)!;

        private void volunteerListObserver()
            => queryVolunteerList();

        private void Window_Loaded(object sender, RoutedEventArgs e)
            => s_bl.Volunteer.AddObserver(volunteerListObserver);

        private void Window_Closed(object sender, EventArgs e)
            => s_bl.Volunteer.RemoveObserver(volunteerListObserver);


    }
}
