namespace DO;

[Serializable]
public class DalDoesNotExistException : Exception // Exception for non-existent file
{
    public DalDoesNotExistException(string? message) : base(message) { }
}

[Serializable]
public class DalAlreadyExistsException : Exception // Exception for already existing file
{
    public DalAlreadyExistsException(string? message) : base(message) { }
}
[Serializable]
public class DalDeletionImpossible : Exception // Exception for impossible deletion
{
    public DalDeletionImpossible(string? message) : base(message) { }
}

[Serializable]
public class DalItIsNullException : Exception // Exception for null value
{
    public DalItIsNullException(string? message) : base(message) { }
}

[Serializable]
public class DalXMLFileLoadCreateException : Exception // Exception for XML file load/create error
{
    public DalXMLFileLoadCreateException(string? message) : base(message) { }
}

[Serializable]
public class DalConnectionFailedException : Exception // Exception for connection failure
{
    public DalConnectionFailedException(string? message) : base(message) { }
}

[Serializable]
public class DalQueryTimeoutException : Exception // Exception for query timeout
{
    public DalQueryTimeoutException(string? message) : base(message) { }
}

[Serializable]
public class DalInvalidQueryException : Exception // Exception for invalid query
{
    public DalInvalidQueryException(string? message) : base(message) { }
}

[Serializable]
public class InvalidCallIdException : Exception // Exception for invalid call ID in CreateCall, ReadCall, DeleteCall
{
    public InvalidCallIdException(string? message) : base(message) { }
}

[Serializable]
public class InvalidVolunteerIdException : Exception // Exception for invalid volunteer ID in CreateVolunteer, ReadVolunteer, DeleteVolunteer
{
    public InvalidVolunteerIdException(string? message) : base(message) { }
}

[Serializable]
public class InvalidAssignmentIdException : Exception // Exception for invalid assignment ID in ReadAssignment, DeleteAssignment
{
    public InvalidAssignmentIdException(string? message) : base(message) { }
}

[Serializable]
public class InvalidFullNameException : Exception // Exception for invalid full name in CreateVolunteer
{
    public InvalidFullNameException(string? message) : base(message) { }
}

[Serializable]
public class InvalidPhoneNumberException : Exception // Exception for invalid phone number in CreateVolunteer
{
    public InvalidPhoneNumberException(string? message) : base(message) { }
}

[Serializable]
public class InvalidEmailException : Exception // Exception for invalid email in CreateVolunteer
{
    public InvalidEmailException(string? message) : base(message) { }
}

[Serializable]
public class InvalidCallTypeException : Exception // Exception for invalid call type in CreateCall
{
    public InvalidCallTypeException(string? message) : base(message) { }
}

[Serializable]
public class InvalidAddressException : Exception // Exception for invalid address in CreateCall
{
    public InvalidAddressException(string? message) : base(message) { }
}
[Serializable]
/// <summary>
/// Represents an exception thrown when a call's risk range is invalid.
/// </summary>
public class InvalidRiskRangeException : Exception // Exception for invalid risk range in SetNewRiskRange
{
    public InvalidRiskRangeException(string? message) : base(message) { }
}
