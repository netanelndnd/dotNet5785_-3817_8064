    namespace BlTest
{
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
                            // Add Volunteer
                            // Collect data and call s_bl.Volunteer.AddVolunteer
                            break;
                        case 2:
                            // Delete Volunteer
                            // Collect data and call s_bl.Volunteer.DeleteVolunteer
                            break;
                        case 3:
                            // Get Volunteer Details
                            // Collect data and call s_bl.Volunteer.GetVolunteerDetails
                            break;
                        case 4:
                            // Get Volunteers
                            // Collect data and call s_bl.Volunteer.GetVolunteers
                            break;
                        case 5:
                            // Login
                            // Collect data and call s_bl.Volunteer.Login
                            break;
                        case 6:
                            // Update Volunteer
                            // Collect data and call s_bl.Volunteer.UpdateVolunteer
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
                            // Add Call
                            // Collect data and call s_bl.Call.AddCall
                            break;
                        case 2:
                            // Delete Call
                            // Collect data and call s_bl.Call.DeleteCall
                            break;
                        case 3:
                            // Get Call Details
                            // Collect data and call s_bl.Call.GetCallDetails
                            break;
                        case 4:
                            // Get Calls
                            // Collect data and call s_bl.Call.GetCallList
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


