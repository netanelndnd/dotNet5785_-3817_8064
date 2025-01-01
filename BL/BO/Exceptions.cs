namespace BO;

/// <summary>
/// Exception for when a file does not exist.
/// נזרקת כאשר מנסים לגשת לאובייקט או משאב שלא קיים במערכת.
/// דוגמה: ניסיון לקרוא נתונים על מתנדב שלא קיים ב-DAL.
/// </summary>
[Serializable]
public class BlDoesNotExistException : Exception
{
    public BlDoesNotExistException(string? message) : base(message) { }
    public BlDoesNotExistException(string message, Exception innerException) : base(message, innerException) { }
}

/// <summary>
/// Exception for when a file already exists.
/// נזרקת כאשר מנסים להוסיף אובייקט או משאב שכבר קיים במערכת.
/// דוגמה: ניסיון להוסיף מתנדב עם אותו מזהה שכבר קיים.
/// </summary>
[Serializable]
public class BlAlreadyExistsException : Exception
{
    public BlAlreadyExistsException(string? message) : base(message) { }
    public BlAlreadyExistsException(string message, Exception innerException) : base(message, innerException) { }
}

/// <summary>
/// Exception for when a deletion is impossible.
/// נזרקת כאשר פעולה של מחיקה אינה אפשרית, לדוגמה מחיקת אובייקט פעיל.
/// דוגמה: ניסיון למחוק מתנדב שעדיין מסומן כפעיל.
/// </summary>
[Serializable]
public class BlDeletionImpossible : Exception
{
    public BlDeletionImpossible(string? message) : base(message) { }
}

/// <summary>
/// Exception for when a property value is null.
/// נזרקת כאשר שדה קריטי באובייקט או פרמטר חסר או ריק.
/// דוגמה: מתנדב עם שדה שם ריק או מזהה חסר.
/// </summary>
[Serializable]
public class BlNullPropertyException : Exception
{
    public BlNullPropertyException(string? message) : base(message) { }
}

/// <summary>
/// Exception for validation failure.
/// נזרקת כאשר נתונים מסוימים אינם עומדים בדרישות האימות.
/// דוגמה: אימייל לא תקין, מספר טלפון שאינו בפורמט הנכון, או כתובת מחוץ לישראל.
/// </summary>
[Serializable]
public class BlValidationException : Exception
{
    public BlValidationException(string? message) : base(message) { }
    public BlValidationException(string message, Exception innerException) : base(message, innerException) { }
}

/// <summary>
/// Exception for login failure.
/// נזרקת כאשר משתמש מנסה להתחבר עם שם משתמש או סיסמה שגויים.
/// דוגמה: שם משתמש שאינו קיים או סיסמה שאינה תואמת.
/// </summary>
[Serializable]
public class BlLoginException : Exception
{
    public BlLoginException(string? message) : base(message) { }
}

/// <summary>
/// Exception for unauthorized access.
/// נזרקת כאשר משתמש מנסה לבצע פעולה ללא הרשאות מתאימות.
/// דוגמה: מתנדב מנסה לעדכן נתונים שזמינים רק למנהלים.
/// </summary>
[Serializable]
public class BlUnauthorizedAccessException : Exception
{
    public BlUnauthorizedAccessException(string? message) : base(message) { }
}

/// <summary>
/// General exception for BL system errors.
/// נזרקת כאשר מתרחשת תקלה כללית ב-BL, למשל בעיות תקשורת או שגיאות שאינן צפויות.
/// דוגמה: כשל בגישה לדאטה, שגיאות מערכת או באגים פנימיים.
/// </summary>
[Serializable]
public class BlSystemException : Exception
{
    public BlSystemException(string? message) : base(message) { }
    public BlSystemException(string message, Exception innerException) : base(message, innerException) { }
}
/// <summary>
/// Exception for invalid date order.
/// נזרקת כאשר סדר התאריכים אינו תקין (למשל, תאריך סיום לפני תאריך התחלה).
/// </summary>
[Serializable]
public class BlInvalidDateOrderException : Exception
{
    public BlInvalidDateOrderException(string? message) : base(message) { }
    public BlInvalidDateOrderException(string message, Exception innerException) : base(message, innerException) { }
}

/// <summary>
/// Exception for invalid address.
/// נזרקת כאשר הכתובת שנמסרה אינה תקינה (למשל, כתובת מחוץ לישראל או כתובת שאינה קיימת).
/// </summary>
[Serializable]
public class BlInvalidAddressException : Exception
{
    public BlInvalidAddressException(string? message) : base(message) { }
    public BlInvalidAddressException(string message, Exception innerException) : base(message, innerException) { }
}

/// <summary>
/// Exception for invalid arguments.
/// נזרקת כאשר פרמטרים שנמסרו אינם תקינים.
/// </summary>
[Serializable]
public class BlArgumentException : Exception
{
    public BlArgumentException(string? message) : base(message) { }
    public BlArgumentException(string message, Exception innerException) : base(message, innerException) { }
}

/// <summary>
/// Exception for invalid operations.
/// נזרקת כאשר פעולה אינה אפשרית עקב מצב מערכת פנימי.
/// </summary>
[Serializable]
public class BlOperationException : Exception
{
    public BlOperationException(string? message) : base(message) { }
    public BlOperationException(string message, Exception innerException) : base(message, innerException) { }
}