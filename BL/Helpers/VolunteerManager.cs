using DalApi;
using System.Text.RegularExpressions;
using BlImplementation;
namespace Helpers;

internal static class VolunteerManager
{
    private static IDal s_dal = Factory.Get; //stage 4

    /// <summary>
    /// Get the number of calls that a volunteer has handled
    /// </summary>
    /// <param name="volunteerId"></param>
    /// <returns></returns
    public static int GetCompletedAssignmentsCount(int volunteerId)
    {
        var assignments = s_dal.Assignment.ReadAll().Where(a => a.VolunteerId == volunteerId);
        return assignments.Count(a => a.CompletionStatus.ToString() == "Treated");
    }

    /// <summary>
    /// Get the number of calls that a volunteer has canceled
    /// </summary>
    /// <param name="volunteerId"></param>
    /// <returns></returns>
    public static int GetTotalCallsCancelled(int volunteerId)
    {
        var assignments = s_dal.Assignment.ReadAll().Where(a => a.VolunteerId == volunteerId);
        return assignments.Count(a => a.CompletionStatus.ToString() == "SelfCancellation");
    }

    /// <summary>
    /// Get the number of assignments that have expired for a volunteer
    /// </summary>
    /// <param name="volunteerId"></param>
    /// <returns></returns>
    public static int GetExpiredAssignmentsCount(int volunteerId)
    {
        var assignments = s_dal.Assignment.ReadAll().Where(a => a.VolunteerId == volunteerId);
        return assignments.Count(a => a.CompletionStatus.ToString() == "Expired");
    }

    /// <summary>
    /// <summary>
    /// Check if there is an assignment for the volunteer that has not been completed yet and return the assignmentID
    /// </summary>
    /// <param name="volunteerId"></param>
    /// <returns></returns>
    public static int? GetPendingAssignmentId(int volunteerId)
    {
        var assignment = s_dal.Assignment.ReadAll().FirstOrDefault(a => a.VolunteerId == volunteerId && a.CompletionTime == null);
        return assignment?.Id;
    }

    /// <summary>
    /// Get the call ID of the pending assignment for a volunteer
    /// </summary>
    /// <param name="volunteerId"></param>
    /// <returns></returns>
    public static int? GetPendingAssignmentCallId(int volunteerId)
    {
        var assignment = s_dal.Assignment.ReadAll().FirstOrDefault(a => a.VolunteerId == volunteerId && a.CompletionTime == null);
        return assignment != null ? assignment.CallId : (int?)null;
    }

    /// <summary>
    /// Get the call type of the pending assignment for a volunteer
    /// </summary>
    /// <param name="volunteerId"></param>
    /// <returns></returns>
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
    /// <param name="volunteerList"></param>
    /// <param name="sortField"></param>
    /// <returns></returns>
    public static IEnumerable<BO.VolunteerInList> SortVolunteers(IEnumerable<BO.VolunteerInList> volunteerList, BO.VolunteerFields? sortField)
    {
        switch (sortField)
        {
            case BO.VolunteerFields.FullName:
                return volunteerList.OrderBy(v => v.FullName);
            case BO.VolunteerFields.TotalCallsHandled:
                return volunteerList.OrderBy(v => v.TotalCallsHandled);
            case BO.VolunteerFields.TotalCallsCancelled:
                return volunteerList.OrderBy(v => v.TotalCallsCancelled);
            case BO.VolunteerFields.TotalExpiredCalls:
                return volunteerList.OrderBy(v => v.TotalExpiredCalls);
            case BO.VolunteerFields.CurrentCallId:
                return volunteerList.OrderBy(v => v.CurrentCallId);
            case BO.VolunteerFields.CurrentCallType:
                return volunteerList.OrderBy(v => v.CurrentCallType);
            default:
                return volunteerList.OrderBy(v => v.Id);
        }
    }

    /// <summary>
    /// Converts a volunteer ID to a BO.Volunteer object
    /// </summary>
    /// <param name="volunteerId">The ID of the volunteer</param>
    /// <returns>A BO.Volunteer object</returns>
    public static BO.Volunteer ConvertVolunteerIdToBO(int volunteerId)
    {
        var volunteer = s_dal.Volunteer.Read(volunteerId);
        return new BO.Volunteer
        {
            Id = volunteerId,
            FullName = volunteer.FullName,
            Email = volunteer.Email,
            PhoneNumber = volunteer.PhoneNumber,
            IsActive = volunteer.IsActive,
            MaxDistance = volunteer.MaxDistance,
            DistanceType = (BO.DistanceType)volunteer.DistanceType,
            Latitude = volunteer.Latitude,
            Longitude = volunteer.Longitude,
            Role = (BO.VolunteerRole)volunteer.VolunteerRole,
            TotalCallsHandled = GetCompletedAssignmentsCount(volunteerId),
            TotalCallsCancelled = GetTotalCallsCancelled(volunteerId),
            TotalExpiredCalls = GetExpiredAssignmentsCount(volunteerId),
            CurrentCall = GetPendingAssignmentCallId(volunteerId) != null ?
                          CallManager.ConvertCallIdToCallInProgress((int)GetPendingAssignmentCallId(volunteerId), volunteerId) :
                          null
        };
    }

    /// <summary>
    /// Determines if a volunteer is a manager or a regular volunteer based on their ID.
    /// </summary>
    /// <param name="volunteerId">The ID of the volunteer.</param>
    /// <returns>True if the volunteer is a manager, otherwise false.</returns>
    public static bool IsManager(int volunteerId)
    {
        var volunteer = s_dal.Volunteer.Read(volunteerId);
        return volunteer.VolunteerRole == 0;
    }

    /// <summary>
    /// Validates if the given email address is in a proper format.
    /// </summary>
    /// <param name="email">The email address to validate.</param>
    /// <returns>True if the email address is valid, otherwise false.</returns>
    public static bool IsValidEmail(string email)
    {
        // Validate email format
        return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
    }

    /// <summary>
    /// Validates if the given ID is a valid Israeli ID number.
    /// </summary>
    /// <param name="id">The ID number to validate.</param>
    /// <returns>True if the ID is valid, otherwise false.</returns>
    /// <exception cref="KeyNotFoundException">Thrown when the ID does not exist in the data layer.</exception>
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
    /// Checks if the given latitude and longitude coordinates are within the geographical boundaries of Israel.
    /// </summary>
    /// <param name="latitude">The latitude coordinate to check.</param>
    /// <param name="longitude">The longitude coordinate to check.</param>
    /// <returns>True if the coordinates are within Israel, otherwise false.</returns>
    public static bool IsLocationInIsrael(double? latitude, double? longitude)
    {
        // Latitude and longitude range of Israel
        const double minLatitude = 29.0;   // South - Eilat
        const double maxLatitude = 33.3;   // North - Golan Heights
        const double minLongitude = 34.3;  // West - Mediterranean Sea
        const double maxLongitude = 35.9;  // East - Dead Sea and Jordan area

        // Check if the coordinates are within the range
        return latitude >= minLatitude && latitude <= maxLatitude &&
               longitude >= minLongitude && longitude <= maxLongitude;
    }

    /// <summary>
    /// Validates if the given phone number is in a proper format.
    /// </summary>
    /// <param name="phoneNumber">The phone number to validate.</param>
    /// <returns>True if the phone number is valid, otherwise false.</returns>
    public static bool IsValidPhoneNumber(string phoneNumber)
    {
        // Remove spaces and special characters to validate the structure only
        phoneNumber = phoneNumber.Replace(" ", "").Replace("-", "");

        // Pattern for numbers in the format 0501234567
        // ^050 - the number must start with 050
        // \d{7}$ - exactly 7 additional digits
        string pattern = @"^050\d{7}$";

        // Check if the number matches the pattern
        return Regex.IsMatch(phoneNumber, pattern);
    }

    /// <summary>
    /// Updates the details by the volunteer.
    /// </summary>
    /// <param name="volunteerB">The volunteer object with updated details.</param>
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
        };
        //תיזרק חריגה אם אין כזה ת"ז
        s_dal.Volunteer.Update(volunteerD);
    }

    /// <summary>
    /// Updates the details by the manager.
    /// </summary>
    /// <param name="volunteerB"></param>
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
        };
        //תיזרק חיריגה אם אין כזה ת"ז
        s_dal.Volunteer.Update(volunteerD);
    }

    /// <summary>
    /// Converts a volunteer ID to a BO.VolunteerInList object
    /// </summary>
    /// <param name="volunteerId">The ID of the volunteer</param>
    /// <returns>A BO.VolunteerInList object</returns>
    public static BO.VolunteerInList ConvertVolunteerIdToVolunteerInList(int volunteerId)
    {
        var volunteer = s_dal.Volunteer.Read(volunteerId);
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
        var volunteers = s_dal.Volunteer.ReadAll();
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
        }).ToList();
    }
}

















