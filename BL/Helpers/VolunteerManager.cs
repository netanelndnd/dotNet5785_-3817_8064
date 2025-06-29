﻿using DalApi;
using System.Text.RegularExpressions;
using BlImplementation;
namespace Helpers;

public static class VolunteerManager
{
    private static IDal s_dal = Factory.Get; //stage 4

    internal static ObserverManager Observers = new(); //stage 5

    /// <summary>
    /// Get the number of calls that a volunteer has handled
    /// </summary>
    /// <param name="volunteerId">The ID of the volunteer</param>
    /// <returns>The number of completed assignments</returns>
    public static int GetCompletedAssignmentsCount(int volunteerId)
    {
        IEnumerable<DO.Assignment> assignments;
        lock (AdminManager.BlMutex)
        {
            assignments = s_dal.Assignment.ReadAll().Where(a => a.VolunteerId == volunteerId);
        }
        return assignments.Count(a => a.CompletionStatus.ToString() == "Treated");
    }

    /// <summary>
    /// Get the number of calls that a volunteer has canceled
    /// </summary>
    /// <param name="volunteerId">The ID of the volunteer</param>
    /// <returns>The number of canceled calls</returns>
    public static int GetTotalCallsCancelled(int volunteerId)
    {
        IEnumerable<DO.Assignment> assignments;
        lock (AdminManager.BlMutex)
        {
            assignments = s_dal.Assignment.ReadAll().Where(a => a.VolunteerId == volunteerId);
        }
        return assignments.Count(a => a.CompletionStatus.ToString() == "SelfCancellation");
    }

    /// <summary>
    /// Get the number of assignments that have expired for a volunteer
    /// </summary>
    /// <param name="volunteerId">The ID of the volunteer</param>
    /// <returns>The number of expired assignments</returns>
    public static int GetExpiredAssignmentsCount(int volunteerId)
    {
        IEnumerable<DO.Assignment> assignments;
        lock (AdminManager.BlMutex)
        {
            assignments = s_dal.Assignment.ReadAll().Where(a => a.VolunteerId == volunteerId);
        }
        return assignments.Count(a => a.CompletionStatus.ToString() == "Expired");
    }

    /// <summary>
    /// Check if there is an assignment for the volunteer that has not been completed yet and return the assignmentID
    /// </summary>
    /// <param name="volunteerId">The ID of the volunteer</param>
    /// <returns>The ID of the pending assignment, or null if none exists</returns>
    public static int? GetPendingAssignmentId(int volunteerId)
    {
        DO.Assignment? assignment;
        lock (AdminManager.BlMutex)
        {
            assignment = s_dal.Assignment.ReadAll().FirstOrDefault(a => a.VolunteerId == volunteerId && a.CompletionStatus == null);
        }
        return assignment?.Id;
    }

    /// <summary>
    /// Get the call ID of the pending assignment for a volunteer
    /// </summary>
    /// <param name="volunteerId">The ID of the volunteer</param>
    /// <returns>The call ID of the pending assignment, or null if none exists</returns>
    public static int? GetPendingAssignmentCallId(int volunteerId)
    {
        DO.Assignment? assignment;
        lock (AdminManager.BlMutex)
        {
            assignment = s_dal.Assignment.ReadAll().FirstOrDefault(a => a.VolunteerId == volunteerId && a.CompletionStatus == null);
        }
        return assignment != null ? assignment.CallId : (int?)null;
    }

    /// <summary>
    /// Get the call type of the pending assignment for a volunteer
    /// </summary>
    /// <param name="volunteerId">The ID of the volunteer</param>
    /// <returns>The call type of the pending assignment, or BO.CallType.None if none exists</returns>
    public static BO.CallType? GetPendingAssignmentCallType(int volunteerId)
    {
        int? callId = GetPendingAssignmentCallId(volunteerId);
        if (callId.HasValue)
        {
            int callType = CallManager.GetCallTypeById(callId.Value);
            return (BO.CallType)callType;
        }
        return BO.CallType.None;
    }

    /// <summary>
    /// Sort a list of volunteers by a specific field
    /// </summary>
    /// <param name="volunteerList">The list of volunteers to sort</param>
    /// <param name="sortField">The field to sort by</param>
    /// <returns>A sorted list of volunteers</returns>
    public static IEnumerable<BO.VolunteerInList> SortVolunteers(IEnumerable<BO.VolunteerInList> volunteerList, BO.VolunteerInListFields? sortField)
    {
        if (sortField.HasValue)
        {
            // Get the property name from the field
            var propertyName = sortField.Value.ToString();

            // Get property information using Reflection
            var property = typeof(BO.VolunteerInList).GetProperty(propertyName);

            // Validate the field
            if (property == null)
            {
                throw new ArgumentException($"The field '{propertyName}' does not exist in 'BO.VolunteerInList'.");
            }

            // Sort the list by the value of the property
            return volunteerList.OrderBy(volunteer => property.GetValue(volunteer));
        }

        // Default sorting by ID
        return volunteerList.OrderBy(v => v.Id);
    }

    /// <summary>
    /// Converts a volunteer ID to a BO.Volunteer object
    /// </summary>
    /// <param name="volunteerId">The ID of the volunteer</param>
    /// <returns>A BO.Volunteer object</returns>
    public static BO.Volunteer ConvertVolunteerIdToBO(int volunteerId)
    {
        DO.Volunteer volunteer;
        lock (AdminManager.BlMutex)
        {
            volunteer = s_dal.Volunteer.Read(volunteerId);
        }
        return new BO.Volunteer
        {
            Id = volunteerId,
            FullName = volunteer.FullName,
            PhoneNumber = volunteer.PhoneNumber,
            Email = volunteer.Email,
            Password = volunteer.Password,
            CurrentAddress = volunteer.CurrentAddress,
            Latitude = volunteer.Latitude,
            Longitude = volunteer.Longitude,
            Role = (BO.VolunteerRole)volunteer.VolunteerRole,
            IsActive = volunteer.IsActive,
            MaxDistance = volunteer.MaxDistance,
            DistanceType = (BO.DistanceType)volunteer.DistanceType,
            TotalCallsHandled = GetCompletedAssignmentsCount(volunteerId),
            TotalCallsCancelled = GetTotalCallsCancelled(volunteerId),
            TotalExpiredCalls = GetExpiredAssignmentsCount(volunteerId),
            CurrentCall = GetPendingAssignmentCallId(volunteerId) != null ?
                          CallManager.ConvertCallIdToCallInProgress((int)GetPendingAssignmentCallId(volunteerId), volunteerId) :
                          null
        };
    }

    /// <summary>
    /// Determines if a volunteer is a manager or a regular volunteer based on their ID
    /// </summary>
    /// <param name="volunteerId">The ID of the volunteer</param>
    /// <returns>True if the volunteer is a manager, otherwise false</returns>
    public static bool IsManager(int volunteerId)
    {
        DO.Volunteer volunteer;
        lock (AdminManager.BlMutex)
        {
            volunteer = s_dal.Volunteer.Read(volunteerId);
        }
        return volunteer.VolunteerRole == 0;
    }

    /// <summary>
    /// Validates if the given email address is in a proper format
    /// </summary>
    /// <param name="email">The email address to validate</param>
    /// <returns>True if the email address is valid, otherwise false</returns>
    public static bool IsValidEmail(string email)
    {
        // Validate email format
        return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
    }

    /// <summary>
    /// Validates if the given ID is a valid Israeli ID number
    /// </summary>
    /// <param name="id">The ID number to validate</param>
    /// <returns>True if the ID is valid, otherwise false</returns>
    /// <exception cref="KeyNotFoundException">Thrown when the ID does not exist in the data layer</exception>
    public static bool IsValidID(int id)
    {
        //אנחנו לא בודקים האם קיים ת"ז בשכבת הנתונים, כי בעדכון אנחנו נבדוק זאת ותיזרק חריגה מתאימה

        string idString = id.ToString();

        // Check if there are exactly 9 digits
        if (idString.Length != 9 || !int.TryParse(idString, out _))
        {
            return false;
        }

        // Calculate the check digit
        int sum = 0;
        for (int i = 0; i < 8; i++) // First eight digits
        {
            int digit = int.Parse(idString[i].ToString());
            int multiplied = digit * (i % 2 == 0 ? 1 : 2); // Alternating multiplication by 1 and 2
            sum += multiplied > 9 ? multiplied - 9 : multiplied; // If the result is above 9, subtract 9
        }

        int checkDigit = (10 - (sum % 10)) % 10;

        // Compare to the check digit
        return checkDigit == int.Parse(idString[8].ToString());
    }

    /// <summary>
    /// Validates if the given phone number is in a proper format
    /// </summary>
    /// <param name="phoneNumber">The phone number to validate</param>
    /// <returns>True if the phone number is valid, otherwise false</returns>
    public static bool IsValidPhoneNumber(string phoneNumber)
    {
        // Remove spaces and special characters to validate the structure only
        phoneNumber = phoneNumber.Replace(" ", "").Replace("-", "");

        // Pattern for numbers in the format 0501234567, 0521234567, 0541234567, 0551234567, 0571234567, 0581234567
        // ^05[024578] - the number must start with 050, 052, 054, 055, 057, or 058
        // \d{7}$ - exactly 7 additional digits
        string pattern = @"^05[024578]\d{7}$";

        // Check if the number matches the pattern
        return Regex.IsMatch(phoneNumber, pattern);
    }

    /// <summary>
    /// Updates the details by the volunteer
    /// </summary>
    /// <param name="volunteerB">The volunteer object with updated details</param>
    public static void UpdateVolunteerDetails(BO.Volunteer volunteerB)
    {
        DO.Volunteer volunteerD = new()
        {
            Id = volunteerB.Id,
            FullName = volunteerB.FullName,
            PhoneNumber = volunteerB.PhoneNumber,
            Email = volunteerB.Email,
            Password = volunteerB.Password,
            CurrentAddress = volunteerB.CurrentAddress,
            Latitude = volunteerB.Latitude,
            Longitude = volunteerB.Longitude,
            MaxDistance = volunteerB.MaxDistance,
            DistanceType = (DO.DistanceType)volunteerB.DistanceType,
            IsActive = volunteerB.IsActive,
        };
        lock (AdminManager.BlMutex) //stage 7
        {
            //תיזרק חריגה אם אין כזה ת"ז
            s_dal.Volunteer.Update(volunteerD);
        }
        Observers.NotifyListUpdated();
        Observers.NotifyItemUpdated(volunteerD.Id);
        _ = UpdateCoordinatesForVolunteerAddressAsync(volunteerD);
    }

    /// <summary>
    /// Updates the details by the manager
    /// </summary>
    /// <param name="volunteerB">The volunteer object with updated details</param>
    public static void UpdateManagerDetails(BO.Volunteer volunteerB)
    {
        DO.Volunteer volunteerD = new()
        {
            Id = volunteerB.Id,
            FullName = volunteerB.FullName,
            PhoneNumber = volunteerB.PhoneNumber,
            Email = volunteerB.Email,
            Password = volunteerB.Password,
            CurrentAddress = volunteerB.CurrentAddress,
            Latitude = volunteerB.Latitude,
            Longitude = volunteerB.Longitude,
            MaxDistance = volunteerB.MaxDistance,
            DistanceType = (DO.DistanceType)volunteerB.DistanceType,
            //פה רק מנהל יכול לשנות
            VolunteerRole = (DO.Role)volunteerB.Role,
            IsActive = volunteerB.IsActive,
        };
        lock (AdminManager.BlMutex) //stage 7
        {
            //תיזרק חיריגה אם אין כזה ת"ז
            s_dal.Volunteer.Update(volunteerD);
        }
        Observers.NotifyListUpdated();
        Observers.NotifyItemUpdated(volunteerD.Id);
        _ = UpdateCoordinatesForVolunteerAddressAsync(volunteerD);
    }

    /// <summary>
    /// Converts a volunteer ID to a BO.VolunteerInList object
    /// </summary>
    /// <param name="volunteerId">The ID of the volunteer</param>
    /// <returns>A BO.VolunteerInList object</returns>
    public static BO.VolunteerInList ConvertVolunteerIdToVolunteerInList(int volunteerId)
    {
        DO.Volunteer volunteer;
        lock (AdminManager.BlMutex)
        {
            volunteer = s_dal.Volunteer.Read(volunteerId);
        }
        return new BO.VolunteerInList
        {
            Id = volunteerId,
            FullName = volunteer.FullName,
            IsActive = volunteer.IsActive,
            TotalCallsHandled = GetCompletedAssignmentsCount(volunteerId),
            TotalCallsCancelled = GetTotalCallsCancelled(volunteerId),
            TotalExpiredCalls = GetExpiredAssignmentsCount(volunteerId),
            CurrentCallId = GetPendingAssignmentCallId(volunteerId),
            CurrentCallType = GetPendingAssignmentCallType(volunteerId) ?? BO.CallType.None
        };
    }

    /// <summary>
    /// Get all volunteers as a list of VolunteerInList
    /// </summary>
    /// <returns>A list of VolunteerInList</returns>
    public static IEnumerable<BO.VolunteerInList> GetAllVolunteers()
    {
        IEnumerable<DO.Volunteer> volunteers;
        lock (AdminManager.BlMutex)
        {
            volunteers = s_dal.Volunteer.ReadAll();
        }
        return volunteers.Select(v => new BO.VolunteerInList
        {
            Id = v.Id,
            FullName = v.FullName,
            IsActive = v.IsActive,
            TotalCallsHandled = GetCompletedAssignmentsCount(v.Id),
            TotalCallsCancelled = GetTotalCallsCancelled(v.Id),
            TotalExpiredCalls = GetExpiredAssignmentsCount(v.Id),
            CurrentCallId = GetPendingAssignmentCallId(v.Id),
            CurrentCallType = GetPendingAssignmentCallType(v.Id) ?? BO.CallType.None
        });
    }




    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

    private static int s_simulatorCounter = 0;

    /// <summary>
    /// Simulates volunteer activity over the system's lifetime.
    /// </summary>
    internal static void VolunteerActivitySimulation()
    {

        Thread.CurrentThread.Name = $"Simulator{++s_simulatorCounter}";

        List<DO.Volunteer> activeVolunteers;

        lock (AdminManager.BlMutex) //stage 7
        {
            activeVolunteers = s_dal.Volunteer.ReadAll().Where(v => v.IsActive).ToList();
        }

        foreach (var volunteer in activeVolunteers)
        {
            lock (AdminManager.BlMutex) //stage 7
            {

                int? pendingAssignmentId = GetPendingAssignmentId(volunteer.Id);
                if (pendingAssignmentId == null)
                {
                    // No current assignment, randomly select a new call to handle
                    if (new Random().NextDouble() < 0.5) // 20% chance
                    {
                        var openCalls = CallManager.GetOpenCallsForVolunteer(volunteer.Id)?.ToList();
                        if (openCalls != null && openCalls.Count != 0)
                        {
                            var selectedCall = openCalls[new Random().Next(openCalls.Count)];
                            if (AdminManager.Now < selectedCall.MaxCompletionTime)
                                s_bl.Call.AssignCallToVolunteer(volunteer.Id, selectedCall.Id);
                        }
                    }
                }
                else
                {
                    // There is a current assignment
                    var assignment = s_dal.Assignment.Read(pendingAssignmentId.Value);
                    var call = s_bl.Call.GetCallDetails(assignment.CallId);
                    var timeElapsed = (AdminManager.Now - assignment.EntryTime);
                    var requiredTime = CalculateRequiredTime(volunteer, call);

                    if (timeElapsed >= requiredTime)
                    {
                        s_bl.Call.CompleteCallHandling(volunteer.Id, assignment.Id);
                    }
                    else if (new Random().NextDouble() < 0.1) // 10% chance
                    {
                        s_bl.Call.CancelCallHandling(volunteer.Id, assignment.Id);
                    }
                }

            }//lock
        }
        Observers.NotifyListUpdated();
    }

    /// <summary>
    /// Calculates the required time for a volunteer to handle a call.
    /// </summary>
    /// <param name="volunteer">The volunteer handling the call</param>
    /// <param name="call">The call being handled</param>
    /// <returns>The required time to handle the call</returns>
    private static TimeSpan CalculateRequiredTime(DO.Volunteer volunteer, BO.Call call)
    {
        // Implement your logic to calculate the required time based on distance and other factors
        // For example, you can use the distance between the volunteer and the call location
        // and add a random time factor to simulate real-world scenarios
        double distance = Tools.CalculateDistance(volunteer.Latitude, volunteer.Longitude, call.Latitude, call.Longitude);
        return TimeSpan.FromMinutes(distance / 10 + new Random().Next(5, 15)); // Example calculation
    }

    public static async Task UpdateCoordinatesForVolunteerAddressAsync(DO.Volunteer volunteerD)
    {
        if (volunteerD.CurrentAddress is not null)
        {
            var coordinates = await Tools.GetCoordinatesAsync(volunteerD.CurrentAddress);
            var updatedVolunteer = volunteerD with
            {
                Latitude = coordinates.Latitude,
                Longitude = coordinates.Longitude
            };
            lock (AdminManager.BlMutex)
                s_dal.Volunteer.Update(updatedVolunteer);
            VolunteerManager.Observers.NotifyListUpdated();
            VolunteerManager.Observers.NotifyItemUpdated(updatedVolunteer.Id);
        }
    }


    /// <summary>
    /// Calculates the distance between two geographical points.
    /// </summary>
    /// <param name="lat1">Latitude of the first point</param>
    /// <param name="lon1">Longitude of the first point</param>
    /// <param name="lat2">Latitude of the second point</param>
    /// <param name="lon2">Longitude of the second point</param>
    /// <returns>The distance between the two points</returns>
    private static double CalculateDistance(double? lat1, double? lon1, double? lat2, double? lon2)
    {
        // Implement your logic to calculate the distance between two geographical points
        // For example, you can use the Haversine formula or any other distance calculation method
        // This is a placeholder implementation
        return Math.Sqrt(Math.Pow(lat2.Value - lat1.Value, 2) + Math.Pow(lon2.Value - lon1.Value, 2));
    }
}

















