namespace DO;
public enum CallType
{
    FoodPreparation,
    FoodTransport,
    // Add other call types as needed
}
public enum CompletionType
{
    Treated,            // The call was treated on time, before the maximum completion time
    SelfCancellation,   // The volunteer chose to cancel the treatment before the maximum completion time
    ManagerCancellation, // The manager canceled the assignment for the current volunteer before the maximum completion time
    Expired             // The call was canceled because it was not treated and reached the maximum completion time
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
