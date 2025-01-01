namespace DO;
/// <summary>
/// Exception for non-existent file
/// </summary>
[Serializable]
public class DalDoesNotExistException : Exception
{
    public DalDoesNotExistException(string? message) : base(message) { }
}
/// <summary>
/// Exception for already existing file
/// </summary>
[Serializable]
public class DalAlreadyExistsException : Exception
{
    public DalAlreadyExistsException(string? message) : base(message) { }
}
/// <summary>
/// Exception for impossible deletion
/// </summary>
[Serializable]
public class DalDeletionImpossible : Exception
{
    public DalDeletionImpossible(string? message) : base(message) { }
}

/// <summary>
/// Exception for null value
/// </summary>
[Serializable]
public class DalItIsNullException : Exception
{
    public DalItIsNullException(string? message) : base(message) { }
}

/// <summary>
/// Exception for XML file load/create error
/// </summary>
[Serializable]
public class DalXMLFileLoadCreateException : Exception
{
    public DalXMLFileLoadCreateException(string? message) : base(message) { }
}


/// <summary>
/// Exception for invalid call ID in CreateCall, ReadCall, DeleteCall
/// </summary>
[Serializable]
public class InvalidCallIdException : Exception
{
    public InvalidCallIdException(string? message) : base(message) { }
}

/// <summary>
/// Exception for invalid volunteer ID in CreateVolunteer, ReadVolunteer, DeleteVolunteer
/// </summary>
[Serializable]
public class InvalidVolunteerIdException : Exception
{
    public InvalidVolunteerIdException(string? message) : base(message) { }
}

/// <summary>
/// Exception for invalid assignment ID in ReadAssignment, DeleteAssignment
/// </summary>
[Serializable]
public class InvalidAssignmentIdException : Exception
{
    public InvalidAssignmentIdException(string? message) : base(message) { }
}

/// <summary>
/// Exception for invalid full name in CreateVolunteer
/// </summary>
[Serializable]
public class InvalidFullNameException : Exception
{
    public InvalidFullNameException(string? message) : base(message) { }
}

/// <summary>
/// Exception for invalid phone number in CreateVolunteer
/// </summary>
[Serializable]
public class InvalidPhoneNumberException : Exception
{
    public InvalidPhoneNumberException(string? message) : base(message) { }
}

/// <summary>
/// Exception for invalid email in CreateVolunteer
/// </summary>
[Serializable]
public class InvalidEmailException : Exception
{
    public InvalidEmailException(string? message) : base(message) { }
}

/// <summary>
/// Exception for invalid call type in CreateCall
/// </summary>
[Serializable]
public class InvalidCallTypeException : Exception
{
    public InvalidCallTypeException(string? message) : base(message) { }
}

/// <summary>
/// Exception for invalid address in CreateCall
/// </summary>
[Serializable]
public class InvalidAddressException : Exception
{
    public InvalidAddressException(string? message) : base(message) { }
}
/// <summary>
/// Exception for invalid risk range in SetNewRiskRange
/// </summary>
[Serializable]
public class InvalidRiskRangeException : Exception
{
    public InvalidRiskRangeException(string? message) : base(message) { }
}
