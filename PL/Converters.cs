using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace PL
{
    // Converter that converts the value of ButtonText to the IsReadOnly property
    public class ConvertUpdateToTrueConverter : IValueConverter
    {
        // Function that performs the conversion
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Checks if the value is "Update" and returns true if it is, otherwise false
            return value?.ToString() == "Update";
        }

        // Function that performs the reverse conversion (not implemented in this case)
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Throws an exception because the function is not implemented
            throw new NotImplementedException();
        }
    }

    // Converter that converts the value of ButtonText to the Visibility property
    public class ConvertUpdateToVisibleConverter : IValueConverter
    {
        // Function that performs the conversion
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Checks if the value is "Update" and returns Visibility.Visible if it is, otherwise Visibility.Collapsed
            return value?.ToString() == "Update" ? Visibility.Visible : Visibility.Collapsed;
        }

        // Function that performs the reverse conversion (not implemented in this case)
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Throws an exception because the function is not implemented
            throw new NotImplementedException();
        }
    }

    // Converter that converts the value of CurrentCall to the Visibility property
    public class CurrentCallVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    // Converter that converts the value of CurrentCall to the IsEnabled property
    public class CurrentCallEnabledConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Improved readability by using a ternary operator
            return value == null ? true : false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class CurrentCallbackEnabledConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Improved readability by using a ternary operator
            return value != null ? true : false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
