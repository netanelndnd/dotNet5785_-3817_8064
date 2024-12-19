namespace BO;
// ENUM for volunteer role
public enum VolunteerRole
{
    Manager,
    Volunteer
}


// ENUM for the type of the call
public enum CallType
{
    FoodPreparation = 0, // Preparation of food for those in need  
    FoodTransport, // Transporting food to those in need  
    CarTrouble, // General car trouble assistance  
    FlatTire, // Assistance with a flat tire  
    BatteryJumpStart, // Jump-starting a car battery  
    FuelDelivery, // Delivering fuel to a stranded vehicle  
    ChildLockedInCar, // Assisting with a child locked in a car  
    RoadsideAssistance, // General roadside assistance  
    MedicalEmergency, // Assisting in a medical emergency  
    LostPerson, // Helping locate a lost person 
    None, // No call is currently in progress
}

// ENUM for distance type
public enum DistanceType
{
    AirDistance,
    WalkingDistance,
    DrivingDistance
}

// ENUM for the status of the call
public enum CallStatus
{
    Open, //Not currently assigned to any volunteer
    InProgress, // In progress
    OpenInRisk, //Open and approaching MaxCompletionTime
    InProgressInRisk, // In progress at risk
    Treated,          // The call was treated on time, before the maximum completion time
    Expired,             // The call was canceled because it was not treated and reached the maximum completion time
}

// ENUM for the completion status of the assignment
public enum CompletionType
{
    Treated = 0,            // The call was treated on time, before the maximum completion time
    SelfCancellation = 1,   // The volunteer chose to cancel the treatment before the maximum completion time
    ManagerCancellation = 2, // The manager canceled the assignment for the current volunteer before the maximum completion time
    Expired = 3,             // The call was canceled because it was not treated and reached the maximum completion time
}

/// <summary>
/// Enum representing the fields of a volunteer.
/// </summary>
public enum VolunteerInListFields
{
    Id,
    FullName,
    IsActive,
    TotalCallsHandled,
    TotalCallsCancelled,
    TotalExpiredCalls,
    CurrentCallId,
    CurrentCallType
}

/// <summary>
/// Enum representing the fields of a call in the list.
/// </summary>
public enum CallInListFields
{
    AssignmentId,
    CallId,
    CallType,
    OpeningTime,
    RemainingTime,
    LastVolunteerName,
    CompletionDuration,
    Status,
    TotalAssignments
}

public enum ClosedCallInListFields
{
    Id,
    CallType,
    FullAddress,
    OpenedAt,
    StartedAt,
    CompletedAt,
    CompletionStatus
}

public enum OpenCallInListFields
{
    Id,
    CallType,
    Description,
    FullAddress,
    OpenedAt,
    MaxCompletionTime,
    DistanceFromVolunteer
}

public enum TimeUnit
{
    Minute,
    Hour,
    Day,
    Month,
    Year
}


