using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using BO;
using Helpers;

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


    public class CurrentCallAndActiveEnabledConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var volunteer = value as BO.Volunteer;
            if (volunteer == null)
                return false;

            return volunteer.IsActive && volunteer.CurrentCall == null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class DeleteButtonEnabledConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var call = value as Call; // Assuming Call is your model class
            if (call == null||call.Id == 0 )
                return false;

            // Check if the call is in an open status and has never been assigned
            if (AssignmentManager.GetAssignmentIdByCallId(call.Id) != null || (CallManager.GetCallStatus(call.Id) != CallStatus.OpenInRisk && CallManager.GetCallStatus(call.Id) != CallStatus.Open))
                return false;
            else
            {
                return true;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }
        public class StatusToEditableConverter : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                if (value is CallStatus status)
                {
                    switch (status)
                    {
                        case CallStatus.Open:
                        case CallStatus.OpenInRisk:
                            return true;
                        case CallStatus.InProgress:
                        case CallStatus.InProgressInRisk:
                            return parameter?.ToString() == "MaxCompletionTime";
                        case CallStatus.Treated:
                        case CallStatus.Expired:
                            return false;
                        default:
                            return false;
                    }
                }
                return false;
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        }


    public class ActiveAssignmentExistsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var call = value as BO.Call;
            if (call == null || call.Assignments == null)
                return false;

            // Check if there is any assignment without an end time
            return call.Assignments.Any(a => a.EndTime == null);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
