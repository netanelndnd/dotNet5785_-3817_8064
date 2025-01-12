using System.Windows;

namespace PL
{
    public partial class ManagerChoiceWindow : Window
    {
        public ManagerChoiceWindow()
        {
            InitializeComponent();
        }

        private void btnVolunteerScreen_Click(object sender, RoutedEventArgs e)
        {
            //למלא

        }

        private void btnManagementScreen_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }
    }
}
