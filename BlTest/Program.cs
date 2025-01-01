using DO;

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
                Console.WriteLine("1. Add Volunteer");
                Console.WriteLine("2. Delete Volunteer");
                Console.WriteLine("3. Get Volunteer Details");
                Console.WriteLine("4. Get Volunteers");
                Console.WriteLine("5. Login");
                Console.WriteLine("6. Update Volunteer");
                Console.WriteLine("0. Back to Main Menu");
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
                            Console.Write("Enter volunteer Full name: ");
                            string name = Console.ReadLine();
                            Console.Write("Enter volunteer Id: ");
                            int id = int.Parse(Console.ReadLine());
                            Console.Write("Enter volunteer email: ");
                            string email = Console.ReadLine();
                            Console.Write("Enter volunteer Phone Number: ");
                            string phonenumber = Console.ReadLine();
                            Console.Write("Enter volunteer Password: ");
                            string password = Console.ReadLine();
                            Console.Write("Enter volunteer Current Address: ");
                            string currentaddress = Console.ReadLine();
                            Console.Write("Enter volunteer role (Volunteer/Manager): ");
                            string roleInput = Console.ReadLine();
                            BO.VolunteerRole roleinput = (BO.VolunteerRole)Enum.Parse(typeof(BO.VolunteerRole), roleInput, true);
                            BO.Volunteer newVolunteer = new BO.Volunteer
                            {
                                FullName = name,
                                Id = id,
                                Email = email,
                                PhoneNumber = phonenumber,
                                Password = password,
                                CurrentAddress = currentaddress,
                                Latitude = null,
                                Longitude = null,
                                IsActive = true,
                                MaxDistance = null,
                                Role = roleinput,
                                DistanceType = BO.DistanceType.AirDistance
                            };

                            s_bl.Volunteer.AddVolunteer(newVolunteer);
                            Console.WriteLine("Volunteer added successfully.");
                            break;
                        case 2:
                            Console.Write("Enter volunteer ID to delete: ");
                            int deleteId = int.Parse(Console.ReadLine());
                            s_bl.Volunteer.DeleteVolunteer(deleteId);
                            Console.WriteLine("Volunteer deleted successfully.");
                            break;
                        case 3:
                            Console.Write("Enter volunteer ID to get details: ");
                            int detailsId = int.Parse(Console.ReadLine());
                            var volunteerDetails = s_bl.Volunteer.GetVolunteerDetails(detailsId);
                            Console.WriteLine($"Name: {volunteerDetails.FullName}, Phone Number: {volunteerDetails.PhoneNumber} , Email: {volunteerDetails.Email}");
                            break;
                        case 4:
                            var volunteers = s_bl.Volunteer.GetVolunteers(null, null);
                            foreach (var volunteer in volunteers)
                            {
                                Console.WriteLine(volunteer);
                            }
                            break;
                        case 5:
                            Console.Write("Enter username: ");
                            string username = Console.ReadLine();
                            Console.Write("Enter password: ");
                            string Password = Console.ReadLine();
                            var role = s_bl.Volunteer.Login(username, Password);
                            Console.WriteLine($"Logged in as {role}");
                            break;
                        case 6:
                            Console.Write("Enter volunteer ID to update: ");
                            int updateId = int.Parse(Console.ReadLine());
                            Console.Write("Enter new volunteer name: ");
                            string newName = Console.ReadLine();
                            Console.Write("Enter new volunteer Phone Number: ");
                            string newPhonenumber = (Console.ReadLine());
                            Console.Write("Enter new volunteer email: ");
                            string newEmail = Console.ReadLine();
                            Console.Write("Enter new volunteer Current Address: ");
                            string newCurrentaddress = Console.ReadLine();
                            BO.Volunteer updatedVolunteer = new BO.Volunteer
                            {
                                FullName = newName,
                                Email = newEmail,
                                PhoneNumber = newPhonenumber,
                                CurrentAddress = newCurrentaddress,
                                Latitude = null,
                                Longitude = null,
                                Role = BO.VolunteerRole.Volunteer,
                                IsActive = true,
                                MaxDistance = null,
                                DistanceType = BO.DistanceType.AirDistance,
                            };
                            s_bl.Volunteer.UpdateVolunteer(updateId, updatedVolunteer);
                            Console.WriteLine("Volunteer updated successfully.");
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
                Console.WriteLine("1. Add Call");
                Console.WriteLine("2. Delete Call");
                Console.WriteLine("3. Get Call Details");
                Console.WriteLine("4. Get Calls");
                Console.WriteLine("0. Back to Main Menu");
                Console.Write("Select an option: ");
                if (!int.TryParse(Console.ReadLine(), out int callOption) || callOption < 0 || callOption > 4)
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
    }
}
