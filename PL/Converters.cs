using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using BO;
using Helpers;
using FontAwesome.WPF;


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



    public class InverseBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is bool)
            {
                return !(bool)value;
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is bool)
            {
                return !(bool)value;
            }
            return value;
        }
    }

    public class SimulatorButtonTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is bool && (bool)value)
            {
                return "Stop Simulator";
            }
            return "Start Simulator";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    // המרת סטטוס לצבע
    public class StatusToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is BO.CallStatus callStatus)
            {
                value = callStatus.ToString(); // Assuming CallStatus has a meaningful ToString() implementation
            }

            return value switch
            {
                "OpenInRisk" => "#FFA500",
                "Expired" => "#FF4444",
                "Open" => "#2E8B57",
                _ => "#E0E0E0"
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    // המרת סוג שיחה לאייקון
    public class CallTypeToIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is CallType callType)
            {
                return callType switch
                {
                    CallType.FoodPreparation => "\uf0f5", // Cutlery (f0f5)
                    CallType.FoodTransport => "\uf0d1",    // Truck (f0d1)
                    CallType.CarTrouble => "\uf0ad",       // Wrench (f0ad)
                    CallType.FlatTire => "\uf071",         // Warning (f071)
                    CallType.BatteryJumpStart => "\uf242", // Battery 3/4 (f242)
                    CallType.FuelDelivery => "\uf043",     // Tint (f043)
                    CallType.ChildLockedInCar => "\uf1ae", // Child (f1ae)
                    CallType.RoadsideAssistance => "\uf018",// Road (f018)
                    CallType.MedicalEmergency => "\uf0f9", // Ambulance (f0f9)
                    CallType.LostPerson => "\uf002",       // Search (f002)
                    CallType.None => "",
                    _ => "\uf059"                          // Question Circle (f059)
                };
            }
            return "\uf059";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }


    public class RemainingProgressConverter : IValueConverter
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        private DateTime now;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            now = s_bl.Admin.GetSystemClock();
            if (value is BO.CallInList call)
            {
                if (call.RemainingTime.HasValue)
                {
                    var totalDuration = (now - call.OpeningTime) + call.RemainingTime.Value;
                    var elapsedTime = now - call.OpeningTime;

                    // מניעת חלוקה ב-0
                    if (totalDuration.TotalSeconds > 0)
                    {
                        return (elapsedTime.TotalSeconds / totalDuration.TotalSeconds) * 100;
                    }
                    return 100;
                }
                else if (call.CompletionDuration.HasValue)
                {
                    return 100;
                }
            }
            return 0.0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }



}
