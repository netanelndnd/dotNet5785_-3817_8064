using DalApi;
using BO;
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

    public static bool IsValidEmail(string email)
    {
        // Validate email format
        return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
    }
    public static bool IsValidID(int id)
    {
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

    public static bool IsValidPhoneNumber(string phoneNumber)
    {
        // Remove spaces and special characters
        phoneNumber = phoneNumber.Replace(" ", "").Replace("-", "");

        // Pattern for Israeli phone number
        string pattern = @"^(\+972|0)([23489]\d{1}|5[02489])\d{7}$";

        // Validate with regex
        return Regex.IsMatch(phoneNumber, pattern);
    }
    public static void Chengeforvolunteer(BO.Volunteer volunteerB)
    {
        DO.Volunteer volunteerD = new()
        {
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
        s_dal.Volunteer.Update(volunteerD);
    }

    public static void Chengeformaneger(BO.Volunteer volunteerB)
    {
        DO.Volunteer volunteerD = new()
        {
            FullName = volunteerB.FullName,
            PhoneNumber = volunteerB.PhoneNumber,
            Email = volunteerB.Email,
            Password = volunteerB.Password,
            CurrentAddress = volunteerB.CurrentAddress,
            Latitude = volunteerB.Latitude,
            Longitude = volunteerB.Longitude,
            MaxDistance = volunteerB.MaxDistance,
            DistanceType = (DO.DistanceType)volunteerB.DistanceType,
            VolunteerRole = (DO.Role)volunteerB.Role,
            Id = volunteerB.Id,
        };
        s_dal.Volunteer.Update(volunteerD);
    }
}















