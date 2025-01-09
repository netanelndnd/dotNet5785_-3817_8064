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
    /// Interaction logic for OneVolunteer.xaml
    /// </summary>
    public partial class WindowVolunteer : Window
    {
        public WindowVolunteer(int id = 0)
        {
            InitializeComponent();

            // Assign the CurrentVolunteer property based on the id parameter
            CurrentVolunteer = (id != 0) ? s_bl.Volunteer.GetVolunteerDetails(id) : new BO.Volunteer();
        }

        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        // CLR wrapper for the Dependency Property
        public BO.Volunteer? CurrentVolunteer
        {
            get { return (BO.Volunteer?)GetValue(CurrentVolunteerProperty); }
            set { SetValue(CurrentVolunteerProperty, value); }
        }

        // Define the Dependency Property
        public static readonly DependencyProperty CurrentVolunteerProperty =
            DependencyProperty.Register("CurrentVolunteer", typeof(BO.Volunteer), typeof(WindowVolunteer), new PropertyMetadata(null));


    }
}
