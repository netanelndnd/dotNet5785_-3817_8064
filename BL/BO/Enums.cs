namespace BO;
// ENUM for volunteer role
public enum VolunteerRole
{
    Manager,
    Volunteer
}

// ENUM for distance type
public enum DistanceType
{
    Aerial, // Aerial distance
    Walking, // Walking distance
    Driving // Driving distance
}
// ENUM for the type of the call
public enum CallType
{
    FoodPreparation, // Preparation of food for those in need  
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

// ENUM for the status of the call
public enum CallStatus
{
    InProgress, // In progress
    AtRisk // In progress at risk
}

// ENUM for the completion status of the call
public enum CompletionType
{
    Treated,            // The call was treated on time, before the maximum completion time
    SelfCancellation,   // The volunteer chose to cancel the treatment before the maximum completion time
    ManagerCancellation, // The manager canceled the assignment for the current volunteer before the maximum completion time
    Expired,             // The call was canceled because it was not treated and reached the maximum completion time
}
