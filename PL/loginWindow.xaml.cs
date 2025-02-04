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
using PL.Volunteer;

namespace PL
{
    /// <summary>
    /// Interaction logic for loginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        //access to the BL
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        public LoginWindow()
        {
            InitializeComponent();
            s_bl.Admin.InitializeDatabase(); // כאשר עובדים עם רשימה אז חייבים לאתחל את זה כי אם לא זה לא יעבוד
        }

     

        public string UserID
        {
            get { return (string)GetValue(UserIDValueProperty); }
            set { SetValue(UserIDValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for UserID
        public static readonly DependencyProperty UserIDValueProperty =
            DependencyProperty.Register("UserID", typeof(string), typeof(LoginWindow));

        public string PasswordValue
        {
            get { return (string)GetValue(PasswordValueProperty); }
            set { SetValue(PasswordValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PasswordValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PasswordValueProperty =
            DependencyProperty.Register("PasswordValue", typeof(string), typeof(LoginWindow));

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (sender is PasswordBox passwordBox)
            {
                PasswordValue = passwordBox.Password;
            }
        }

        /// <summary>
        /// login function : checks the role and open the right window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(UserID) || string.IsNullOrWhiteSpace(PasswordValue))
            {
                MessageBox.Show("Please fill all the required fields.", "Missing Values", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                if (!int.TryParse(UserID, out int userId))
                {
                    MessageBox.Show("Invalid ID format. Please enter numeric ID.", "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var role = s_bl.Volunteer.Login(UserID, PasswordValue);

                switch (role)
                {
                    case "Volunteer":
                        var volunteerWindow = new WindowMyVolunteer(userId);
                        volunteerWindow.Show();
                        break;

                    case "Manager":
                        var result = MessageBox.Show("Which screen do you want to open ?\n" +
                                 " 'Yes' for Manager or 'No' for Volunteer.",
                                 "Choice of  screen",
                                 MessageBoxButton.YesNo,
                                 MessageBoxImage.Question);

                        if (result == MessageBoxResult.Yes)
                        {
                            var existingMainWindow = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
                            if (existingMainWindow != null)
                            {
                                MessageBox.Show("A main window is already open.", "Window Already Open", MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                            else
                            {
                                var mainWindow = new MainWindow(userId);
                                mainWindow.Show();
                            }
                        }
                        else if (result == MessageBoxResult.No)
                        {
                            var volunteer2Window = new WindowMyVolunteer(userId);
                            volunteer2Window.Show();
                        }
                        break;

                    default:
                        MessageBox.Show("Unknown role. Please contact support.", "Unknown Role", MessageBoxButton.OK, MessageBoxImage.Warning);
                        break;
                }
            }
            catch (BO.BlLoginException ex)
            {
                MessageBox.Show(ex.Message, "Login Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (BO.BlSystemException ex)
            {
                MessageBox.Show(ex.Message, "Login Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("An unexpected error occurred: " + ex.Message + " " + ex.InnerException, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}


