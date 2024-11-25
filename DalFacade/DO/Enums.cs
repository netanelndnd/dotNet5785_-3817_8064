namespace DO;


public enum CallType
{
    //לשנות אחר כך  
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
}

public enum CompletionType
{
    Treated,            // The call was treated on time, before the maximum completion time
    SelfCancellation,   // The volunteer chose to cancel the treatment before the maximum completion time
    ManagerCancellation, // The manager canceled the assignment for the current volunteer before the maximum completion time
    Expired,             // The call was canceled because it was not treated and reached the maximum completion time
}
public enum Role
{
    Manager,
    Volunteer
}

public enum DistanceType
{
    AirDistance,
    WalkingDistance,
    DrivingDistance
}
