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

namespace PL
{
    /// <summary>
    /// Interaction logic for loginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            string userId = txtUserId.Text;
            string password = txtPassword.Password;

            // בדיקת תעודת זהות וסיסמא
            if (IsValidUser(userId, password, out bool isManager))
            {
                if (isManager)
                {
                    // פתיחת חלון לבחירת סוג המסך למנהל
                    ManagerChoiceWindow managerChoiceWindow = new ManagerChoiceWindow();
                    managerChoiceWindow.Show();
                }
                else
                {
                    // פתיחת מסך מתנדב
                }
                this.Close();
            }
            else
            {
                MessageBox.Show("Invalid User ID or Password", "Login Failed", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool IsValidUser(string userId, string password, out bool isManager)
        {
            // לוגיקה לבדיקה אם המשתמש תקין ואם הוא מנהל
            isManager = false;
            // כאן יש להוסיף את הלוגיקה המתאימה לבדיקה
            return true;
        }
    }
}
