using BlApi;
using Helpers;
using BO;
using System.Runtime.CompilerServices;


namespace BlTest;
internal class Program
{
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
    static void Main(string[] args)
    {
        while (true)
        {
            Console.WriteLine("Main Menu:");
            Console.WriteLine("1. Admin");
            Console.WriteLine("2. Volunteer");
            Console.WriteLine("3. Call");
            Console.WriteLine("0. Exit");
            Console.Write("Select an option: ");
            if (!int.TryParse(Console.ReadLine(), out int mainOption) || mainOption < 0 || mainOption > 3)
            {
                Console.WriteLine("Invalid option. Please try again.");
                continue;
            }

            switch (mainOption)
            {
                case 1:
                    AdminMenu();
                    break;
                case 2:
                    VolunteerMenu();
                    break;
                case 3:
                    CallMenu();
                    break;
                case 0:
                    return;
            }
        }
    }

    static void AdminMenu()
    {
        while (true)
        {
            Console.WriteLine("Admin Menu:");
            Console.WriteLine("1. Reset Database");
            Console.WriteLine("2. Initialize Database");
            Console.WriteLine("3. Forward Clock");
            Console.WriteLine("4. Get Clock");
            Console.WriteLine("0. Back to Main Menu");
            Console.Write("Select an option: ");
            if (!int.TryParse(Console.ReadLine(), out int adminOption) || adminOption < 0 || adminOption > 4)
            {
                Console.WriteLine("Invalid option. Please try again.");
                continue;
            }

            try
            {
                switch (adminOption)
                {
                    case 1:
                        s_bl.Admin.ResetDatabase();
                        Console.WriteLine("Database reset successfully.");
                        break;
                    case 2:
                        s_bl.Admin.InitializeDatabase();
                        Console.WriteLine("Database initialized successfully.");
                        break;
                    case 3:
                        Console.Write("Enter time unit to forward (Minute, Hour, Day, Month, Year): ");
                        if (Enum.TryParse(Console.ReadLine(), out BO.TimeUnit timeUnit))
                        {
                            s_bl.Admin.AdvanceSystemClock(timeUnit);
                            Console.WriteLine("Clock forwarded successfully.");
                        }
                        else
                        {
                            Console.WriteLine("Invalid time unit.");
                        }
                        break;
                    case 4:
                        Console.WriteLine($"Current Clock: {s_bl.Admin.GetSystemClock()}");
                        break;
                    case 0:
                        return;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.GetType().Name}, Message: {ex.Message}, InnerException: {ex.InnerException?.Message}");
            }
        }
    }

    static void VolunteerMenu()
    {
        while (true)
        {
            Console.WriteLine("Volunteer Menu:");
            Console.WriteLine("1. Add Volunteer - Add a new volunteer to the system");
            Console.WriteLine("2. Delete Volunteer - Remove an existing volunteer from the system");
            Console.WriteLine("3. Get Volunteer Details - View details of a specific volunteer");
            Console.WriteLine("4. Get Volunteers - List all volunteers with optional filters and sorting");
            Console.WriteLine("5. Login - Volunteer login to the system");
            Console.WriteLine("6. Update Volunteer - Update details of an existing volunteer");
            Console.WriteLine("0. Back to Main Menu - Return to the main menu");
            Console.Write("Select an option: ");
            if (!int.TryParse(Console.ReadLine(), out int volunteerOption) || volunteerOption < 0 || volunteerOption > 6)
            {
                Console.WriteLine("Invalid option. Please try again.");
                continue;
            }

            try
            {
                switch (volunteerOption)
                {
                    case 1:
                        AddVolunteer();
                        break;
                    case 2:
                        Console.Write("Enter volunteer ID to delete: ");
                        if (!int.TryParse(Console.ReadLine(), out int deleteId))
                        {
                            Console.WriteLine("Invalid ID. Please enter a numeric value.");
                            return;
                        }
                        s_bl.Volunteer.DeleteVolunteer(deleteId);
                        Console.WriteLine("Volunteer deleted successfully.");
                        break;
                    case 3:
                        Console.Write("Enter volunteer ID to get details: ");
                        if (!int.TryParse(Console.ReadLine(), out int detailsId))
                        {
                            Console.WriteLine("Invalid ID. Please enter a numeric value.");
                            break;
                        }
                        var volunteerDetails = s_bl.Volunteer.GetVolunteerDetails(detailsId);
                        Console.WriteLine($"Volunteer ID: {volunteerDetails.Id}");
                        Console.WriteLine($"Name: {volunteerDetails.FullName}");
                        Console.WriteLine($"Phone: {volunteerDetails.PhoneNumber}");
                        Console.WriteLine($"Email: {volunteerDetails.Email}");
                        Console.WriteLine($"Role: {volunteerDetails.Role}");
                        Console.WriteLine($"Distance Type: {volunteerDetails.DistanceType}");
                        Console.WriteLine($"Password: {volunteerDetails.Password ?? "N/A"}");
                        Console.WriteLine($"Address: {volunteerDetails.CurrentAddress ?? "N/A"}");
                        Console.WriteLine($"Latitude: {volunteerDetails.Latitude?.ToString() ?? "N/A"}");
                        Console.WriteLine($"Longitude: {volunteerDetails.Longitude?.ToString() ?? "N/A"}");
                        Console.WriteLine($"Is the volunteer Active? {(volunteerDetails.IsActive ? "Yes" : "No")}");
                        Console.WriteLine($"Max Distance: {volunteerDetails.MaxDistance?.ToString() ?? "N/A"}");
                        Console.WriteLine($"Sum of Cared Calls: {volunteerDetails.TotalCallsHandled}");
                        Console.WriteLine($"Sum of Cancelled Calls: {volunteerDetails.TotalCallsCancelled}");
                        Console.WriteLine($"Sum of Expired Calls: {volunteerDetails.TotalExpiredCalls}");

                        if (volunteerDetails.CurrentCall != null)
                        {
                            Console.WriteLine("Current Call in Progress:");
                            Console.WriteLine($"  Call ID: {volunteerDetails.CurrentCall.CallId}");
                            Console.WriteLine($"  Call Type: {volunteerDetails.CurrentCall.CallType}");
                            Console.WriteLine($"  Description: {volunteerDetails.CurrentCall.Description ?? "N/A"}");
                            Console.WriteLine($"  Address: {volunteerDetails.CurrentCall.FullAddress}");
                            Console.WriteLine($"  Opened At: {volunteerDetails.CurrentCall.OpenedAt}");
                            Console.WriteLine($"  Max Completion Time: {volunteerDetails.CurrentCall.MaxCompletionTime?.ToString() ?? "N/A"}");
                            Console.WriteLine($"  Started At: {volunteerDetails.CurrentCall.StartedAt}");
                            Console.WriteLine($"  Distance From Volunteer: {volunteerDetails.CurrentCall.DistanceFromVolunteer}");
                            Console.WriteLine($"  Status: {volunteerDetails.CurrentCall.Status}");
                        }
                        else
                        {
                            Console.WriteLine("No current call in progress.");
                        }
                        break;
                    case 4:
                        Console.Write("Enter 1 to filter active volunteers, 2 for inactive, or 0 for all: ");
                        bool? isActive = Console.ReadLine() switch
                        {
                            "1" => true,
                            "2" => false,
                            _ => (bool?)null
                        };
                        Console.WriteLine("Enter the number to sort by:");
                        Console.WriteLine("1. ID");
                        Console.WriteLine("2. Name");
                        Console.WriteLine("3. IsActive");
                        Console.WriteLine("4. TotalCallsHandled");
                        Console.WriteLine("5. TotalCallsCancelled");
                        Console.WriteLine("6. TotalExpiredCalls");
                        Console.WriteLine("7. CurrentCallId");
                        Console.WriteLine("8. CurrentCallType");
                        Console.WriteLine("0. Default");
                        BO.VolunteerInListFields? sortField = Console.ReadLine() switch
                        {
                            "1" => BO.VolunteerInListFields.Id,
                            "2" => BO.VolunteerInListFields.FullName,
                            "3" => BO.VolunteerInListFields.IsActive,
                            "4" => BO.VolunteerInListFields.TotalCallsHandled,
                            "5" => BO.VolunteerInListFields.TotalCallsCancelled,
                            "6" => BO.VolunteerInListFields.TotalExpiredCalls,
                            "7" => BO.VolunteerInListFields.CurrentCallId,
                            "8" => BO.VolunteerInListFields.CurrentCallType,
                            _ => (BO.VolunteerInListFields?)null
                        };
                        var volunteers = s_bl.Volunteer.GetVolunteers(isActive, sortField);
                            foreach (var volunteer in volunteers)
                            {
                            Console.WriteLine(volunteer);

                            var volunteerDetails3 = s_bl.Volunteer.GetVolunteerDetails(volunteer.Id);
                            Console.WriteLine($"Role: {volunteerDetails3.Role}");

                        }
                        break;
                    case 5:
                        Console.Write("Enter username: ");
                        string? username = Console.ReadLine();
                        Console.Write("Enter password: ");
                        string? password = Console.ReadLine();
                        try
                        {
                            var role = s_bl.Volunteer.Login(username, password);
                            Console.WriteLine($"Logged in as {role}");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Invalid username or password. Please try again.", ex);
                        }
                        break;
                    case 6:
                        UpdateVolunteer();
                        break;
                    case 0:
                        return;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.GetType().Name}, Message: {ex.Message}, InnerException: {ex.InnerException?.Message}");
            }
        }
    }

    static void CallMenu()
    {
        while (true)
        {
            Console.WriteLine("Call Menu:");
            Console.WriteLine("1. Add a new call");
            Console.WriteLine("2. Delete an existing call");
            Console.WriteLine("3. Get details of a specific call");
            Console.WriteLine("4. Get a list of calls");
            Console.WriteLine("5. Get quantities of calls by status");
            Console.WriteLine("6. Update details of a call");
            Console.WriteLine("7. Get closed calls handled by a volunteer");
            Console.WriteLine("8. Get open calls available for a volunteer");
            Console.WriteLine("9. Mark a call as completed by a volunteer");
            Console.WriteLine("10. Cancel handling of a call by a volunteer");
            Console.WriteLine("11. Assign a call to a volunteer");
            Console.WriteLine("0. Back to Main Menu");
            Console.Write("Select an option: ");
            if (!int.TryParse(Console.ReadLine(), out int callOption) || callOption < 0 || callOption > 11)
            {
                Console.WriteLine("Invalid option. Please try again.");
                continue;
            }

            try
            {
                switch (callOption)
                {
                    case 1:
                        AddCall();
                        break;
                    case 2:
                        Console.Write("Enter call ID to delete: ");
                        if (!int.TryParse(Console.ReadLine(), out int callId))
                        {
                            Console.WriteLine("Invalid ID. Please enter a numeric value.");
                            return;
                        }
                        s_bl.Call.DeleteCall(callId);
                        Console.WriteLine("Call deleted successfully.");
                        break;
                    case 3:
                        Console.Write("Enter call ID to get details: ");
                        if (!int.TryParse(Console.ReadLine(), out int callDetailsId))
                        {
                            Console.WriteLine("Invalid ID. Please enter a numeric value.");
                            break;
                        }
                        var callDetails = s_bl.Call.GetCallDetails(callDetailsId);
                        Console.WriteLine(callDetails);
                        break;
                    case 4:
                        Console.WriteLine("The method `GetCallList` retrieves a list of calls based on the following parameters:");
                        Console.WriteLine("1. Field to filter by (nullable).");
                        Console.WriteLine("2. Value to filter by (nullable).");
                        Console.WriteLine("3. Field to sort by (nullable).");
                        Console.WriteLine("If no filter field is provided, all calls are returned.");
                        Console.WriteLine("If no sort field is provided, calls are sorted by call ID.");
                        Console.WriteLine("Each call appears only once with its latest assignment (if any).");

                        Console.WriteLine("Enter the number of the element that you want to filter by:");
                        Console.WriteLine("1. AssignmentId");
                        Console.WriteLine("2. CallId");
                        Console.WriteLine("3. CallType");
                        Console.WriteLine("4. OpeningTime");
                        Console.WriteLine("5. RemainingTime");
                        Console.WriteLine("6. LastVolunteerName");
                        Console.WriteLine("7. CompletionDuration");
                        Console.WriteLine("8. Status");
                        Console.WriteLine("9. TotalAssignments");
                        Console.WriteLine("Leave empty for no filter.");
                        string? filterInput = Console.ReadLine();
                        BO.CallInListFields? filterField = string.IsNullOrWhiteSpace(filterInput) ? null : (BO.CallInListFields?)int.Parse(filterInput);

                        Console.WriteLine("Enter the number of the element that you want to sort by:");
                        Console.WriteLine("1. AssignmentId");
                        Console.WriteLine("2. CallId");
                        Console.WriteLine("3. CallType");
                        Console.WriteLine("4. OpeningTime");
                        Console.WriteLine("5. RemainingTime");
                        Console.WriteLine("6. LastVolunteerName");
                        Console.WriteLine("7. CompletionDuration");
                        Console.WriteLine("8. Status");
                        Console.WriteLine("9. TotalAssignments");
                        Console.WriteLine("Leave empty for no sorting.");
                        string? sortInput = Console.ReadLine();
                        BO.CallInListFields? sortField = string.IsNullOrWhiteSpace(sortInput) ? null : (BO.CallInListFields?)int.Parse(sortInput);

                        Console.WriteLine("Enter any value that will filter the object (or leave empty for no value):");
                        string? objInput = Console.ReadLine();
                        object? filterValue = string.IsNullOrWhiteSpace(objInput) ? null : objInput;

                        var calls = s_bl.Call.GetCallList(filterField, filterValue, sortField);
                        if (calls == null || !calls.Any())
                        {
                            Console.WriteLine("No calls available.");
                            return;
                        }
                        foreach (var call in calls)
                        {
                            Console.WriteLine(call);
                        }
                        break;
                    case 5:
                        var quantities = s_bl.Call.GetCallQuantitiesByStatus();
                        Console.WriteLine("Call Quantities By Status:");
                        for (int i = 0; i < quantities.Length; i++)
                        {
                            Console.WriteLine($"{(BO.CallStatus)i}: {quantities[i]}");
                        }
                        break;
                    case 6:
                        UpdateCall();
                        break;
                    case 7:
                        Console.WriteLine("Enter volunteer ID to get a list of closed calls handled by the volunteer:");
                        Console.Write("Enter volunteer ID: ");
                        if (!int.TryParse(Console.ReadLine(), out int volunteerId))
                        {
                            Console.WriteLine("Invalid ID. Please enter a numeric value.");
                            return;
                        }

                        // הסבר על הכנסת סוג הקריאה לסינון
                        Console.WriteLine("Enter the number of the call type to filter by (or leave empty for no filter):");
                        Console.WriteLine("1. FoodPreparation");
                        Console.WriteLine("2. FoodTransport");
                        Console.WriteLine("3. CarTrouble");
                        Console.WriteLine("4. FlatTire");
                        Console.WriteLine("5. BatteryJumpStart");
                        Console.WriteLine("6. FuelDelivery");
                        Console.WriteLine("7. ChildLockedInCar");
                        Console.WriteLine("8. RoadsideAssistance");
                        Console.WriteLine("9. MedicalEmergency");
                        Console.WriteLine("10. LostPerson");
                        string? callTypeInput = Console.ReadLine();
                        BO.CallType? callType = string.IsNullOrWhiteSpace(callTypeInput) ? null : (BO.CallType?)int.Parse(callTypeInput);

                        // הסבר על הכנסת שדה המיון
                        Console.WriteLine("Enter the number of the field to sort by (or leave empty for default sorting by call ID):");
                        Console.WriteLine("1. Id");
                        Console.WriteLine("2. CallType");
                        Console.WriteLine("3. FullAddress");
                        Console.WriteLine("4. OpenedAt");
                        Console.WriteLine("5. StartedAt");
                        Console.WriteLine("6. CompletedAt");
                        Console.WriteLine("7. CompletionStatus");
                        string? sortFieldInput = Console.ReadLine();
                        BO.ClosedCallInListFields? sortField1 = string.IsNullOrWhiteSpace(sortFieldInput) ? null : (BO.ClosedCallInListFields?)int.Parse(sortFieldInput);

                        // קריאה לפונקציה והצגת התוצאות
                        Console.WriteLine("Retrieving closed calls handled by the volunteer...");
                        var closedCalls = s_bl.Call.GetClosedCallsByVolunteer(volunteerId, callType, sortField1);
                        if (closedCalls == null || !closedCalls.Any())
                        {
                            Console.WriteLine("No closed calls found for the specified volunteer.");
                            return;
                        }

                        Console.WriteLine("Closed Calls:");
                        foreach (var closedCall in closedCalls)
                        {
                            Console.WriteLine(closedCall);
                        }
                        break;
                    case 8:
                        Console.Write("Enter volunteer ID: ");
                        if (!int.TryParse(Console.ReadLine(), out int openVolunteerId))
                        {
                            Console.WriteLine("Invalid ID. Please enter a numeric value.");
                            return;
                        }

                        // Explanation for entering the call type to filter
                        Console.WriteLine("Enter the number of the call type to filter by (or leave empty for no filter):");
                        Console.WriteLine("1. FoodPreparation");
                        Console.WriteLine("2. FoodTransport");
                        Console.WriteLine("3. CarTrouble");
                        Console.WriteLine("4. FlatTire");
                        Console.WriteLine("5. BatteryJumpStart");
                        Console.WriteLine("6. FuelDelivery");
                        Console.WriteLine("7. ChildLockedInCar");
                        Console.WriteLine("8. RoadsideAssistance");
                        Console.WriteLine("9. MedicalEmergency");
                        Console.WriteLine("10. LostPerson");
                        string? callTypeInput2 = Console.ReadLine();
                        BO.CallType? callType2 = string.IsNullOrWhiteSpace(callTypeInput2) ? null : (BO.CallType?)int.Parse(callTypeInput2);

                        // Explanation for entering the sorting field
                        Console.WriteLine("Enter the number of the field to sort by (or leave empty for default sorting by call ID):");
                        Console.WriteLine("1. Id");
                        Console.WriteLine("2. CallType");
                        Console.WriteLine("3. Description");
                        Console.WriteLine("4. FullAddress");
                        Console.WriteLine("5. OpenedAt");
                        Console.WriteLine("6. MaxCompletionTime");
                        Console.WriteLine("7. DistanceFromVolunteer");
                        string? sortFieldInput2 = Console.ReadLine();
                        BO.OpenCallInListFields? sortField2 = string.IsNullOrWhiteSpace(sortFieldInput2) ? null : (BO.OpenCallInListFields?)int.Parse(sortFieldInput2);

                        // Call the function and display the results
                        Console.WriteLine("Retrieving open calls available for the volunteer...");
                        var openCalls = s_bl.Call.GetOpenCallsForVolunteer(openVolunteerId, callType2, sortField2);
                        if (openCalls == null || !openCalls.Any())
                        {
                            Console.WriteLine("No open calls available for the specified volunteer.");
                            return;
                        }
                            Console.WriteLine("Open Calls:");
                            foreach (var openCall in openCalls)
                            {
                                Console.WriteLine(openCall);
                            }
                            break;
                    case 9:
                        Console.Write("Enter volunteer ID: ");
                        if (!int.TryParse(Console.ReadLine(), out int completeVolunteerId))
                        {
                            Console.WriteLine("Invalid ID. Please enter a numeric value.");
                            return;
                        }
                        Console.Write("Enter assignment ID: ");
                        if (!int.TryParse(Console.ReadLine(), out int assignmentId))
                        {
                            Console.WriteLine("Invalid ID. Please enter a numeric value.");
                            return;
                        }
                        s_bl.Call.CompleteCallHandling(completeVolunteerId, assignmentId);
                        Console.WriteLine("Call handling completed successfully.");
                        break;
                    case 10:
                        Console.Write("Enter requester ID: ");
                        if (!int.TryParse(Console.ReadLine(), out int requesterId))
                        {
                            Console.WriteLine("Invalid ID. Please enter a numeric value.");
                            return;
                        }
                        Console.Write("Enter assignment ID: ");
                        if (!int.TryParse(Console.ReadLine(), out int cancelAssignmentId))
                        {
                            Console.WriteLine("Invalid ID. Please enter a numeric value.");
                            return;
                        }
                        s_bl.Call.CancelCallHandling(requesterId, cancelAssignmentId);
                        Console.WriteLine("Call handling cancelled successfully.");
                        break;
                    case 11:
                        Console.Write("Enter volunteer ID: ");
                        if (!int.TryParse(Console.ReadLine(), out int assignVolunteerId))
                        {
                            Console.WriteLine("Invalid ID. Please enter a numeric value.");
                            return;
                        }
                        Console.Write("Enter call ID: ");
                        if (!int.TryParse(Console.ReadLine(), out int assignCallId))
                        {
                            Console.WriteLine("Invalid ID. Please enter a numeric value.");
                            return;
                        }
                        s_bl.Call.AssignCallToVolunteer(assignVolunteerId, assignCallId);
                        Console.WriteLine("Call assigned to volunteer successfully.");
                        break;
                    case 0:
                        return;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.GetType().Name}, Message: {ex.Message}, InnerException: {ex.InnerException?.Message}");
            }
        }
    }


    /// <summary>
    /// Updates the details of a call.
    /// </summary>
    static void UpdateCall()
    {
        Console.Write("Enter call ID to update: ");
        if (!int.TryParse(Console.ReadLine(), out int callId))
        {
            Console.WriteLine("Invalid ID. Please enter a numeric value.");
            return;
        }

        var existingCall = s_bl.Call.GetCallDetails(callId);
        if (existingCall == null)
        {
            Console.WriteLine("Call not found.");
            return;
        }

        Console.WriteLine($"Current type: {existingCall.CallType}. Enter new type (or press Enter to keep current):");
        string newType = Console.ReadLine();
        BO.CallType callType = existingCall.CallType;
        if (!string.IsNullOrWhiteSpace(newType) && Enum.TryParse(newType, out BO.CallType parsedCallType))
        {
            callType = parsedCallType;
        }

        Console.WriteLine($"Current address: {existingCall.FullAddress}. Enter new address (or press Enter to keep current):");
        string newAddress = Console.ReadLine();
        string fullAddress = string.IsNullOrWhiteSpace(newAddress) ? existingCall.FullAddress : newAddress;

        Console.WriteLine($"Current details: {existingCall.Description}. Enter new details (or press Enter to keep current):");
        string newDetails = Console.ReadLine();
        string description = string.IsNullOrWhiteSpace(newDetails) ? existingCall.Description : newDetails;

        Console.WriteLine($"Current max completion time: {existingCall.MaxCompletionTime}. Enter new max completion time (yyyy-MM-dd HH:mm:ss) (or press Enter to keep current):");
        string newMaxCompletionTime = Console.ReadLine();
        DateTime maxCompletionTime = string.IsNullOrWhiteSpace(newMaxCompletionTime) ? existingCall.MaxCompletionTime ?? default(DateTime) : DateTime.Parse(newMaxCompletionTime);

        // Create a new Call object with updated details
        BO.Call updatedCall = new BO.Call
        {
            Id = existingCall.Id,
            CallType = callType,
            FullAddress = fullAddress,
            Description = description,
            Latitude = existingCall.Latitude,
            Longitude = existingCall.Longitude,
            OpenedAt = existingCall.OpenedAt,
            MaxCompletionTime = maxCompletionTime,
            Status = existingCall.Status,
            Assignments = existingCall.Assignments
        };

        s_bl.Call.UpdateCall(updatedCall);
        Console.WriteLine("Call details updated.");
    }

    /// <summary>
        /// Function to add a new volunteer:
        /// 1. Ask for volunteer details
        /// 2. Calculates latitude and longitude based on the received address
        /// 3. Creates , saves and confirm the new volunteer object
        /// </summary>
    private static void AddVolunteer()
    {
        Console.Write("Enter volunteer id: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("Invalid ID. Please enter a numeric value.");
            return;
        }

        Console.Write("Enter volunteer name: ");
        string name = Console.ReadLine()!;

        Console.Write("Enter volunteer phone: ");
        string phone = Console.ReadLine()!;

        Console.Write("Enter volunteer email: ");
        string email = Console.ReadLine()!;

        Console.Write("Enter 0 for manager and 1 for volunteer: ");
        if (!Enum.TryParse(Console.ReadLine(), out VolunteerRole role))
        {
            Console.WriteLine("Invalid role. Please enter 0 for manager or 1 for volunteer.");
            return;
        }

        Console.Write("Enter 0 for AirDistance, 1 for WalkingDistance, and 2 for DrivingDistance: ");
        if (!Enum.TryParse(Console.ReadLine(), out DistanceType distance))
        {
            Console.WriteLine("Invalid distance type. Please enter 0, 1, or 2.");
            return;
        }

        string passwords;
        Console.Write("Enter 1 if you want to get a password, or 2 to create a password: ");
        if (!int.TryParse(Console.ReadLine(), out int pas) || (pas != 1 && pas != 2))
        {
            Console.WriteLine("Invalid option. Please enter 1 or 2.");
            return;
        }

        if (pas == 1)
        {
            passwords = NewPassword();
            Console.WriteLine($"The password is: {passwords}");
        }
        else
        {
            Console.Write("Enter volunteer password: ");
            passwords = Console.ReadLine();
        }

        string address;
        double latitude, longitude;
        while (true)
        {

            Console.Write("Enter volunteer address: ");
            address = Console.ReadLine()!;
            var coordinates = Helpers.Tools.GetCoordinates(address);
            if (coordinates.IsInIsrael == true)
            {
                latitude = coordinates.Latitude;
                longitude = coordinates.Longitude;
                break;
            }
            else
            {
                Console.WriteLine("Invalid address. Please enter a valid address.");
            }
        }

        Console.Write("Enter maximum distance: ");
        if (!double.TryParse(Console.ReadLine(), out double dis))
        {
            Console.WriteLine("Invalid distance. Please enter a numeric value.");
            return;
        }

        BO.Volunteer v = new BO.Volunteer
        {
            Id = id,
            FullName = name,
            PhoneNumber = phone,
            Email = email,
            Password = passwords,
            CurrentAddress = address,
            Latitude = latitude,
            Longitude = longitude,
            MaxDistance = dis,
            Role = role,
            IsActive = true,
            DistanceType = distance
        };

        s_bl.Volunteer?.AddVolunteer(v);
        Console.WriteLine("Volunteer added.");
    }

    /// <summary>
        /// function to generate a new Password
        /// </summary>
        /// <returns></returns>
    private static string NewPassword()
    {
        // יצירת אובייקט של מחלקת Random ליצירת ערכים רנדומליים
        Random random = new Random();

        // הגדרת מערך של תווים שיאחסן את הסיסמה החדשה
        char[] password = new char[9];

        // לולאה ליצירת 4 האותיות הראשונות של הסיסמה, בטווח בין 'a' ל-'z'
        for (int i = 0; i < 4; i++)
            password[i] = (char)random.Next('a', 'z' + 1);

        // הוספת התו '@' במיקום החמישי (אינדקס 4)
        password[4] = '@';

        // לולאה ליצירת 4 האותיות האחרונות של הסיסמה, בטווח בין 'a' ל-'z'
        for (int i = 5; i < 9; i++)
            password[i] = (char)random.Next('a', 'z' + 1);

        // המרת מערך התווים למחרוזת והחזרתו כערך מהפונקציה
        return new string(password);
    }

    /// <summary>
    /// Updates the details of a volunteer.
    /// </summary>
    private static void UpdateVolunteer()
    {
        Console.Write("Enter volunteer ID to update: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("Invalid ID. Please enter a numeric value.");
            return;
        }

        var volunteer = s_bl.Volunteer.GetVolunteerDetails(id);
        if (volunteer == null)
        {
            Console.WriteLine("Volunteer not found.");
            return;
        }

        Console.WriteLine($"Current name: {volunteer.FullName}. Enter new name (or press Enter to keep current):");
        string newName = Console.ReadLine();
        if (!string.IsNullOrEmpty(newName))
            volunteer.FullName = newName;

        Console.WriteLine($"Current phone: {volunteer.PhoneNumber}. Enter new phone (or press Enter to keep current):");
        string newPhone = Console.ReadLine();
        if (!string.IsNullOrEmpty(newPhone))
            volunteer.PhoneNumber = newPhone;

        Console.WriteLine($"Current email: {volunteer.Email}. Enter new email (or press Enter to keep current):");
        string newEmail = Console.ReadLine();
        if (!string.IsNullOrEmpty(newEmail))
            volunteer.Email = newEmail;

        Console.WriteLine($"Current password: {volunteer.Password}. Enter new password (or press Enter to keep current):");
        string newPassword = Console.ReadLine();
        if (!string.IsNullOrEmpty(newPassword))
            volunteer.Password = newPassword;

        Console.WriteLine($"Current address: {volunteer.CurrentAddress}. Enter new address (or press Enter to keep current):");
        string newAddress = Console.ReadLine();
        if (!string.IsNullOrEmpty(newAddress))
        {
            var coordinates = Helpers.Tools.GetCoordinates(newAddress);
            if (coordinates.IsInIsrael)
            {
                volunteer.CurrentAddress = newAddress;
                volunteer.Latitude = coordinates.Latitude;
                volunteer.Longitude = coordinates.Longitude;
            }
            else
            {
                Console.WriteLine("Invalid address. Keeping current address.");
            }
        }

        Console.WriteLine($"Current distance: {volunteer.MaxDistance?.ToString() ?? "N/A"}. Enter new distance (or press Enter to keep current):");
        string newDistanceInput = Console.ReadLine();
        if (double.TryParse(newDistanceInput, out double newDistance))
            volunteer.MaxDistance = newDistance;

        s_bl.Volunteer.UpdateVolunteer(id, volunteer);
        Console.WriteLine("Volunteer details updated.");
    }


    /// <summary>
    /// Function to add a new call. The function prompts the user to enter the call details and then adds the call to the system.
    /// </summary>
    static void AddCall()
    {
        // Display call types
        Console.WriteLine("Enter call type:");
        Console.WriteLine("0. FoodPreparation");
        Console.WriteLine("1. FoodTransport");
        Console.WriteLine("2. CarTrouble");
        Console.WriteLine("3. FlatTire");
        Console.WriteLine("4. BatteryJumpStart");
        Console.WriteLine("5. FuelDelivery");
        Console.WriteLine("6. ChildLockedInCar");
        Console.WriteLine("7. RoadsideAssistance");
        Console.WriteLine("8. MedicalEmergency");
        Console.WriteLine("9. LostPerson");
        Console.Write("Your choice: ");
        if (!Enum.TryParse(Console.ReadLine(), out BO.CallType callType))
        {
            Console.WriteLine("Invalid call type.");
            return;
        }
        Console.Write("Enter address: ");
        string address = Console.ReadLine();

        Console.Write("Enter details: ");
        string details = Console.ReadLine();

        Console.Write("Enter max completion time (yyyy-MM-dd HH:mm:ss): ");
        if (!DateTime.TryParse(Console.ReadLine(), out DateTime maxCompletionTime))
        {
            Console.WriteLine("Invalid date format.");
            return;
        }

        BO.Call call = new BO.Call
        {
            CallType = callType,
            FullAddress = address,
            OpenedAt = s_bl.Admin.GetSystemClock(),
            MaxCompletionTime = maxCompletionTime,
            Description = details
        };

        s_bl.Call.AddCall(call);
        Console.WriteLine("Call added successfully.");
    }



}


