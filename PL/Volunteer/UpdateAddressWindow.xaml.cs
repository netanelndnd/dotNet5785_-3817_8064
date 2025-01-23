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
    /// Interaction logic for UpdateAddressWindow.xaml
    /// </summary>
    public partial class UpdateAddressWindow : Window
    {
        public string NewAddress { get; private set; }

        public UpdateAddressWindow()
        {
            InitializeComponent();

        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            // מציאת TextBox בעץ הוויזואליזציה
            var textBox = (TextBox)((Grid)Content).Children[1];
            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                MessageBox.Show("Please enter a valid address.", "Invalid Address", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            NewAddress = textBox.Text;
            DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
