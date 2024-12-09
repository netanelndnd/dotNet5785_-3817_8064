using DalApi;
using DO;

namespace DalTest
{
    public class Program
    {
        //private static IAssignment? s_dalAssignment = new AssignmentImplementation();//stage 1
        //private static IVolunteer? s_dal.Volunteer = new VolunteerImplementation();//stage 1
        //private static ICall? s_dal.Call = new CallImplementation();//stage 1
        //private static IConfig? s_dal.Config = new ConfigImplementation();//stage 1

        //static readonly IDal s_dal = new Dal.DalList(); //stage 2
        static readonly IDal s_dal = new Dal.DalXml(); //stage 3
        static void Main(string[] args)
        {

            try
            {
                MainMenu();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        private static void MainMenu()
        {
            /// Main menu
            bool exit = false;
            while (!exit)
            {
                Console.WriteLine("Main Menu:");
                Console.WriteLine("1. Exit");
                Console.WriteLine("2. Assignment Menu");
                Console.WriteLine("3. Volunteer Menu");
                Console.WriteLine("4. Call Menu");
                Console.WriteLine("5. Config Menu");
                Console.WriteLine("6. Initialize Data");
                Console.WriteLine("7. Display All Data");
                Console.WriteLine("8. Reset Database and Config");

                // Read user input and navigate to the corresponding menu or action
                switch (Console.ReadLine())
                {
                    case "1":
                        exit = true;
                        break;
                    case "2":
                        AssignmentMenu();
                        break;
                    case "3":
                        VolunteerMenu();
                        break;
                    case "4":
                        CallMenu();
                        break;
                    case "5":
                        ConfigMenu();
                        break;
                    case "6":
                        //Initialization.Do(s_dalAssignment, s_dal.Volunteer, s_dal.Call, s_dal.Config);// stage 1
                        Initialization.Do(s_dal); //stage 2
                        break;
                    case "7":
                        DisplayAllData();
                        break;
                    case "8":
                        ResetDatabaseAndConfig();
                        break;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }
        }

        /// <summary>
        /// Assignment menu
        /// </summary>
        private static void AssignmentMenu()
        {
            bool exit = false;
            while (!exit)
            {
                Console.WriteLine("Assignment Menu:");
                Console.WriteLine("1. Exit");
                Console.WriteLine("2. Create Assignment");
                Console.WriteLine("3. Read Assignment");
                Console.WriteLine("4. Read All Assignments");
                Console.WriteLine("5. Update Assignment");
                Console.WriteLine("6. Delete Assignment");
                Console.WriteLine("7. Delete All Assignments");

                switch (Console.ReadLine())
                {
                    case "1":
                        exit = true;
                        break;
                    case "2":
                        CreateAssignment();
                        break;
                    case "3":
                        ReadAssignment();
                        break;
                    case "4":
                        ReadAllAssignments();
                        break;
                    case "5":
                        UpdateAssignment();
                        break;
                    case "6":
                        DeleteAssignment();
                        break;
                    case "7":
                        DeleteAllAssignments();
                        break;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }
        }
        /// <summary>
        /// Volunteer menu
        /// </summary>
        private static void VolunteerMenu()
        {
            bool exit = false;
            while (!exit)
            {
                Console.WriteLine("Volunteer Menu:");
                Console.WriteLine("1. Exit");
                Console.WriteLine("2. Create Volunteer");
                Console.WriteLine("3. Read Volunteer");
                Console.WriteLine("4. Read All Volunteers");
                Console.WriteLine("5. Update Volunteer");
                Console.WriteLine("6. Delete Volunteer");
                Console.WriteLine("7. Delete All Volunteers");

                switch (Console.ReadLine())
                {
                    case "1":
                        exit = true;
                        break;
                    case "2":
                        CreateVolunteer();
                        break;
                    case "3":
                        ReadVolunteer();
                        break;
                    case "4":
                        ReadAllVolunteers();
                        break;
                    case "5":
                        UpdateVolunteer();
                        break;
                    case "6":
                        DeleteVolunteer();
                        break;
                    case "7":
                        DeleteAllVolunteers();
                        break;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }
        }
        /// <summary>
        /// Call menu
        /// </summary>
        private static void CallMenu()
        {
            bool exit = false;
            while (!exit)
            {
                Console.WriteLine("Call Menu:");
                Console.WriteLine("1. Exit");
                Console.WriteLine("2. Create Call");
                Console.WriteLine("3. Read Call");
                Console.WriteLine("4. Read All Calls");
                Console.WriteLine("5. Update Call");
                Console.WriteLine("6. Delete Call");
                Console.WriteLine("7. Delete All Calls");

                switch (Console.ReadLine())
                {
                    case "1":
                        exit = true;
                        break;
                    case "2":
                        CreateCall();
                        break;
                    case "3":
                        ReadCall();
                        break;
                    case "4":
                        ReadAllCalls();
                        break;
                    case "5":
                        UpdateCall();
                        break;
                    case "6":
                        DeleteCall();
                        break;
                    case "7":
                        DeleteAllCalls();
                        break;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }
        }
        /// <summary>
        /// Config menu
        /// </summary>
        private static void ConfigMenu()
        {
            bool exit = false;
            while (!exit)
            {
                Console.WriteLine("Config Menu:");
                Console.WriteLine("1. Exit");
                Console.WriteLine("2. Advance Clock by Minute");
                Console.WriteLine("3. Advance Clock by Hour");
                Console.WriteLine("4. Display Current Clock Value");
                Console.WriteLine("5. Set New Risk Range");
                Console.WriteLine("6. Display Current Risk Range");
                Console.WriteLine("7. Reset All Config Values");

                /// Read user input and navigate to the corresponding menu or action    
                switch (Console.ReadLine())
                {
                    case "1":
                        exit = true;
                        break;
                    case "2":
                        AdvanceClockByMinute();
                        break;
                    case "3":
                        AdvanceClockByHour();
                        break;
                    case "4":
                        DisplayCurrentClockValue();
                        break;
                    case "5":
                        SetNewRiskRange();
                        break;
                    case "6":
                        DisplayCurrentRiskRange();
                        break;
                    case "7":
                        ResetAllConfigValues();
                        break;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }
        }
        /// <summary>
        /// Create an assignment
        /// </summary>
        /// <exception cref="FormatException"></exception>
        private static void CreateAssignment()
        {
            try
            {
                Console.WriteLine("Enter Call ID:");
                if (!int.TryParse(Console.ReadLine(), out int callId)) throw new InvalidCallIdException("Call ID is invalid!");

                Console.WriteLine("Enter Volunteer ID:");
                if (!int.TryParse(Console.ReadLine(), out int volunteerId)) throw new InvalidVolunteerIdException("Volunteer ID is invalid!");

                Assignment newAssignment = new Assignment
                {
                    CallId = callId,
                    VolunteerId = volunteerId,
                    EntryTime = s_dal.Config.Clock
                };


                s_dal.Assignment.Create(newAssignment);
                Console.WriteLine("Assignment created successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }
        /// <summary>
        /// Read an assignment
        /// </summary>
        /// <exception cref="FormatException"></exception>
        private static void ReadAssignment()
        {
            try
            {
                Console.WriteLine("Enter Assignment ID:");
                if (!int.TryParse(Console.ReadLine(), out int assignmentId)) throw new InvalidAssignmentIdException("Assignment ID is invalid!");

                Assignment? assignment = s_dal.Assignment.Read(assignmentId);
                if (assignment == null)
                {
                    Console.WriteLine("Assignment not found.");
                }
                else
                {
                    Console.WriteLine(assignment);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }
        /// <summary>
        /// Read all assignments
        /// </summary>
        private static void ReadAllAssignments()
        {
            try
            {
                var assignments = s_dal.Assignment.ReadAll();
                if (!assignments.Any())
                {
                    Console.WriteLine("No assignments found.");
                }
                else
                {
                    foreach (var assignment in assignments)
                    {
                        Console.WriteLine(assignment);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }
        /// <summary>
        /// Update an assignment
        /// </summary>
        /// <exception cref="FormatException"></exception>
        private static void UpdateAssignment()
        {
            try
            {
                Console.WriteLine("Enter Assignment ID:");
                if (!int.TryParse(Console.ReadLine(), out int assignmentId)) throw new InvalidAssignmentIdException("Assignment ID is invalid!");

                Assignment? assignment = s_dal.Assignment.Read(assignmentId);
                if (assignment == null)
                {
                    Console.WriteLine("Assignment not found.");
                    return;
                }

                Console.WriteLine($"Current Assignment: {assignment}");

                Console.WriteLine("Enter new Call ID (leave empty to keep current):");
                string? callIdInput = Console.ReadLine();
                if (int.TryParse(callIdInput, out int callId))
                {
                    assignment = assignment with { CallId = callId };
                }

                Console.WriteLine("Enter new Volunteer ID (leave empty to keep current):");
                string? volunteerIdInput = Console.ReadLine();
                if (int.TryParse(volunteerIdInput, out int volunteerId))
                {
                    assignment = assignment with { VolunteerId = volunteerId };
                }

                Console.WriteLine("Enter new Entry Time (leave empty to keep current):");
                string? entryTimeInput = Console.ReadLine();
                if (DateTime.TryParse(entryTimeInput, out DateTime entryTime))
                {
                    assignment = assignment with { EntryTime = entryTime };
                }

                Console.WriteLine("Enter new Completion Time (leave empty to keep current):");
                string? completionTimeInput = Console.ReadLine();
                if (DateTime.TryParse(completionTimeInput, out DateTime completionTime))
                {
                    assignment = assignment with { CompletionTime = completionTime };
                }

                Console.WriteLine("Enter new Completion Status (leave empty to keep current):");
                string? completionStatusInput = Console.ReadLine();
                if (Enum.TryParse(completionStatusInput, out CompletionType completionStatus))
                {
                    assignment = assignment with { CompletionStatus = completionStatus };
                }

                s_dal.Assignment.Update(assignment);
                Console.WriteLine("Assignment updated successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }
        /// <summary>
        /// Delete an assignment
        /// </summary>
        /// <exception cref="FormatException"></exception>
        private static void DeleteAssignment()
        {
            try
            {
                Console.WriteLine("Enter Assignment ID:");
                if (!int.TryParse(Console.ReadLine(), out int assignmentId)) throw new FormatException("Assignment ID is invalid!");

                s_dal.Assignment.Delete(assignmentId);
                Console.WriteLine("Assignment deleted successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }
        /// <summary>
        /// Delete all assignments
        /// </summary>
        private static void DeleteAllAssignments()
        {
            try
            {
                s_dal.Assignment.DeleteAll();
                Console.WriteLine("All assignments deleted successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }
        /// <summary>
        /// Create a new volunteer.
        /// </summary>
        /// <exception cref="FormatException"></exception>
        private static void CreateVolunteer()
        {
            try
            {
                Console.WriteLine("Enter Full Name:");
                string fullName = Console.ReadLine() ?? throw new InvalidFullNameException("Full Name is invalid!");

                Console.WriteLine("Enter Phone Number:");
                string phoneNumber = Console.ReadLine() ?? throw new InvalidPhoneNumberException("Phone Number is invalid!");

                Console.WriteLine("Enter Email:");
                string email = Console.ReadLine() ?? throw new InvalidEmailException("Email is invalid!");

                Volunteer newVolunteer = new Volunteer
                {
                    FullName = fullName,
                    PhoneNumber = phoneNumber,
                    Email = email
                };

                s_dal.Volunteer.Create(newVolunteer);
                Console.WriteLine("Volunteer created successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        /// <summary>
        /// Read a volunteer by ID.
        /// </summary>
        /// <exception cref="FormatException"></exception>
        private static void ReadVolunteer()
        {
            try
            {
                Console.WriteLine("Enter Volunteer ID:");
                if (!int.TryParse(Console.ReadLine(), out int volunteerId)) throw new InvalidVolunteerIdException("Volunteer ID is invalid!");

                Volunteer? volunteer = s_dal.Volunteer.Read(volunteerId);
                if (volunteer == null)
                {
                    Console.WriteLine("Volunteer not found.");
                }
                else
                {
                    Console.WriteLine(volunteer);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        /// <summary>
        /// Read all volunteers.
        /// </summary>
        private static void ReadAllVolunteers()
        {
            try
            {
                var volunteers = s_dal.Volunteer.ReadAll();
                if (!volunteers.Any())
                {
                    Console.WriteLine("No volunteers found.");
                }
                else
                {
                    foreach (var volunteer in volunteers)
                    {
                        Console.WriteLine(volunteer);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        /// <summary>
        /// Update a volunteer by ID.
        /// </summary>
        /// <exception cref="FormatException"></exception>
        private static void UpdateVolunteer()
        {
            try
            {
                Console.WriteLine("Enter Volunteer ID:");
                if (!int.TryParse(Console.ReadLine(), out int volunteerId)) throw new InvalidVolunteerIdException("Volunteer ID is invalid!");

                Volunteer? volunteer = s_dal.Volunteer.Read(volunteerId);
                if (volunteer == null)
                {
                    Console.WriteLine("Volunteer not found.");
                    return;
                }

                Console.WriteLine($"Current Volunteer: {volunteer}");

                Console.WriteLine("Enter new Full Name (leave empty to keep current):");
                string? fullNameInput = Console.ReadLine();
                if (!string.IsNullOrEmpty(fullNameInput))
                {
                    volunteer = volunteer with { FullName = fullNameInput };
                }

                Console.WriteLine("Enter new Phone Number (leave empty to keep current):");
                string? phoneNumberInput = Console.ReadLine();
                if (!string.IsNullOrEmpty(phoneNumberInput))
                {
                    volunteer = volunteer with { PhoneNumber = phoneNumberInput };
                }

                Console.WriteLine("Enter new Email (leave empty to keep current):");
                string? emailInput = Console.ReadLine();
                if (!string.IsNullOrEmpty(emailInput))
                {
                    volunteer = volunteer with { Email = emailInput };
                }

                s_dal.Volunteer.Update(volunteer);
                Console.WriteLine("Volunteer updated successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        /// <summary>
        /// Delete a volunteer by ID.
        /// </summary>
        /// <exception cref="FormatException"></exception>
        private static void DeleteVolunteer()
        {
            try
            {
                Console.WriteLine("Enter Volunteer ID:");
                if (!int.TryParse(Console.ReadLine(), out int volunteerId)) throw new InvalidVolunteerIdException("Volunteer ID is invalid!");

                s_dal.Volunteer.Delete(volunteerId);
                Console.WriteLine("Volunteer deleted successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        /// <summary>
        /// Delete all volunteers.
        /// </summary>
        private static void DeleteAllVolunteers()
        {
            try
            {
                s_dal.Volunteer.DeleteAll();
                Console.WriteLine("All volunteers deleted successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        /// <summary>
        /// Create a new call.
        /// </summary>
        /// <exception cref="FormatException"></exception>
        private static void CreateCall()
        {
            try
            {
                Console.WriteLine("Enter Call Type:");
                if (!Enum.TryParse(Console.ReadLine(), out CallType callType)) throw new InvalidCallTypeException("Call Type is invalid!");

                Console.WriteLine("Enter Address:");
                string address = Console.ReadLine() ?? throw new InvalidAddressException("Address is invalid!");

                Call newCall = new Call
                {
                    CallType = callType,
                    Address = address,
                    OpenTime = s_dal.Config.Clock
                };

                s_dal.Call.Create(newCall);
                Console.WriteLine("Call created successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        /// <summary>
        /// Read a call by ID.
        /// </summary>
        /// <exception cref="FormatException"></exception>
        private static void ReadCall()
        {
            try
            {
                Console.WriteLine("Enter Call ID:");
                if (!int.TryParse(Console.ReadLine(), out int callId)) throw new InvalidCallIdException("Call ID is invalid!");

                Call? call = s_dal.Call.Read(callId);
                if (call == null)
                {
                    Console.WriteLine("Call not found.");
                }
                else
                {
                    Console.WriteLine(call);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        /// <summary>
        /// Read all calls.
        /// </summary>
        private static void ReadAllCalls()
        {
            try
            {
                var calls = s_dal.Call.ReadAll();
                if (!calls.Any())
                {
                    Console.WriteLine("No calls found.");
                }
                else
                {
                    foreach (var call in calls)
                    {
                        Console.WriteLine(call);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        /// <summary>
        /// Update a call by ID.
        /// </summary>
        /// <exception cref="FormatException"></exception>
        private static void UpdateCall()
        {
            try
            {
                Console.WriteLine("Enter Call ID:");
                if (!int.TryParse(Console.ReadLine(), out int callId)) throw new InvalidCallIdException("Call ID is invalid!");

                Call? call = s_dal.Call.Read(callId);
                if (call == null)
                {
                    Console.WriteLine("Call not found.");
                    return;
                }

                Console.WriteLine($"Current Call: {call}");

                Console.WriteLine("Enter new Call Type (leave empty to keep current):");
                string? callTypeInput = Console.ReadLine();
                if (Enum.TryParse(callTypeInput, out CallType callType))
                {
                    call = call with { CallType = callType };
                }

                Console.WriteLine("Enter new Address (leave empty to keep current):");
                string? addressInput = Console.ReadLine();
                if (!string.IsNullOrEmpty(addressInput))
                {
                    call = call with { Address = addressInput };
                }

                s_dal.Call.Update(call);
                Console.WriteLine("Call updated successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        /// <summary>
        /// Delete a call by ID.
        /// </summary>
        /// <exception cref="FormatException"></exception>
        private static void DeleteCall()
        {
            try
            {
                Console.WriteLine("Enter Call ID:");
                if (!int.TryParse(Console.ReadLine(), out int callId)) throw new InvalidCallIdException("Call ID is invalid!");

                s_dal.Call.Delete(callId);
                Console.WriteLine("Call deleted successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        /// <summary>
        /// Delete all calls.
        /// </summary>
        private static void DeleteAllCalls()
        {
            try
            {
                s_dal.Call.DeleteAll();
                Console.WriteLine("All calls deleted successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        /// <summary>
        /// Advance the clock by one minute.
        /// </summary>
        private static void AdvanceClockByMinute()
        {
            try
            {
                s_dal.Config.Clock = s_dal.Config.Clock.AddMinutes(1);
                Console.WriteLine("Clock advanced by one minute.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        /// <summary>
        /// Advance the clock by one hour.
        /// </summary>
        private static void AdvanceClockByHour()
        {
            try
            {
                s_dal.Config.Clock = s_dal.Config.Clock.AddHours(1);
                Console.WriteLine("Clock advanced by one hour.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        /// <summary>
        /// Display the current clock value.
        /// </summary>
        private static void DisplayCurrentClockValue()
        {
            Console.WriteLine($"The current time is: {s_dal.Config.Clock}");
        }

        /// <summary>
        /// Set a new risk range in minutes.
        /// </summary>
        /// <exception cref="FormatException"></exception>
        private static void SetNewRiskRange()
        {
            try
            {
                Console.WriteLine("Enter new Risk Range (in minutes):");
                if (!int.TryParse(Console.ReadLine(), out int riskRangeMinutes)) throw new InvalidRiskRangeException("Risk Range is invalid!");

                s_dal.Config.RiskRange = TimeSpan.FromMinutes(riskRangeMinutes);
                Console.WriteLine("Config value updated successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        /// <summary>
        /// Display the current risk range.
        /// </summary>
        private static void DisplayCurrentRiskRange()
        {
            try
            {
                Console.WriteLine($"Current Risk Range: {s_dal.Config.RiskRange.TotalMinutes} minutes");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        /// Reset all configuration values to their defaults.
        /// </summary>
        private static void ResetAllConfigValues()
        {
            s_dal.Config.Reset();
        }

        /// <summary>
        /// Display all data from assignments, volunteers, calls, and config.
        /// </summary>
        private static void DisplayAllData()
        {
            try
            {
                Console.WriteLine("Assignments:");
                ReadAllAssignments();

                Console.WriteLine("Volunteers:");
                ReadAllVolunteers();

                Console.WriteLine("Calls:");
                ReadAllCalls();

                Console.WriteLine("Config:");
                DisplayCurrentClockValue();
                DisplayCurrentRiskRange();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        /// <summary>
        /// Reset the database and configuration to their initial states.
        /// </summary>
        private static void ResetDatabaseAndConfig()
        {
            s_dal.Assignment.DeleteAll();
            s_dal.Volunteer.DeleteAll();
            s_dal.Call.DeleteAll();
            s_dal.Config.Reset();
        }

    }
}

