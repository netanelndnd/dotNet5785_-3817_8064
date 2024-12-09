namespace DalTest;
using DalApi;
using DO;

/// <summary>
/// A class that initializes the DAL with random data.
/// </summary>
public static class Initialization
{
    //private static IAssignment? s_dalAssignment;//stage 1
    //private static IVolunteer? s_dalVolunteer;//stage 1
    //private static ICall? s_dalCall;//stage 1
    //private static IConfig? s_dalConfig;//stage 1

    private static IDal? s_dal; //stage 2

    private static readonly Random s_rand = new();

    /// <summary>
    /// Creates a list of volunteers with random data and adds them to the DAL.
    /// </summary>
    private static void CreateVolunteers()
    {
        string[] names = {
            "Yossi Cohen", "Rivka Levi", "Moshe Mizrahi", "Yael Katz", "David Peretz", "Sara Ben-David", "Avi Shalom", "Miriam Gold", "Daniel Azulay", "Leah Bar",
            "Yitzhak Shimon", "Esther Malka", "Ronen Alon", "Tamar Shani", "Elior Ben-Ami", "Shira Tal"
        };
        string[] emails = {
            "yossi.cohen@gmail.com", "rivka.levi@gmail.com", "moshe.mizrahi@gmail.com", "yael.katz@gmail.com", "david.peretz@gmail.com", "sara.ben-david@gmail.com", "avi.shalom@gmail.com", "miriam.gold@gmail.com", "daniel.azulay@gmail.com", "leah.bar@gmail.com",
            "yitzhak.shimon@gmail.com", "esther.malka@gmail.com", "ronen.alon@gmail.com", "tamar.shani@gmail.com", "elior.ben-ami@gmail.com", "shira.tal@gmail.com" };
        string[] phoneNumbers = {
            "0501234567", "0501234568", "0501234569", "0501234570", "0501234571", "0501234572", "0501234573", "0501234574", "0501234575", "0501234576",
            "0501234577", "0501234578", "0501234579", "0501234580", "0501234581", "0501234582"
           
        };

        for (int i = 0; i < names.Length; i++)
        {
            int id = s_rand.Next(200000000, 400000000);
            double? maxDistance = s_rand.NextDouble() * 100; // Random max distance up to 100 km
            Role role = i == 0 ? Role.Manager : Role.Volunteer; // First volunteer is a manager

            Volunteer volunteer = new Volunteer(
                id,
                names[i],
                phoneNumbers[i],
                emails[i],
                null,
                null,
                null,
                null,
                maxDistance,
                role,
                true,
                DistanceType.AirDistance
            );

            s_dal?.Volunteer?.Create(volunteer);
        }
    }
    /// <summary>
    /// Creates a list of calls with random data and adds them to the DAL.
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    private static void CreateCalls()
    {
        string[] addresses = {
            "רחוב ההדר 12, קרית אונו",
            "שדרות הרצל 45, ירושלים",
            "החשמל 3, בת ים",
            "הבנים 5, בת ים",
            "בר אילן 42, בת ים",
            "ברנר 3, בת ים",
            "בלפור 80, בת ים",
            "פולה בן גוריון 23, באר שבע",
            "אבא אחימאיר 43, באר שבע",
            "דרך מצדה 81, באר שבע",
            "מבצע חירם 1, באר שבע",
            "יעקב דורי 27, באר שבע",
            "מרדכי מקלף 24, באר שבע",
            "ח\"ן 2, באר שבע",
            "חרוב 8, חיפה",
            "יפה נוף 61, חיפה",
            "שדרות בן גוריון 50, חיפה",
            "שמריהו לוין 25, חיפה",
            "שדרות אליהו גולומב 47, חיפה",
            "שלום עליכם 4, חיפה",
            "זלמן שניאור 1, חיפה",
            "הפלמ\"ח 11, קרית אתא",
            "וינגייט 17, קרית אתא",
            "נורדאו 72, קרית אתא",
            "חנקין 20, קרית אתא",
            "חנקין 1, קרית אתא",
            "שדה בוקר 33, קרית אתא",
            "העצמאות 66, קרית אתא",
            "זבולון 14, קרית אתא",
            "העצמאות 37, קרית אתא",
            "שדרות כצנלסון 32, קרית אתא",
            "דרך רופין 11, ירושלים",
            "שמואל סטפן וויז 21, ירושלים",
            "שדרות המוזיאונים 3, ירושלים",
            "חיים נחמן ביאליק 20, ירושלים",
            "שדרות הרצל 137, ירושלים",
            "שדרות הרצל 102, ירושלים",
            "מעגל בית המדרש 7, ירושלים",
            "הפסגה 8, ירושלים",
            "תורה ועבודה 6, ירושלים",
            "דרך יצחק רבין 13, בית שמש",
            "האומן 2, בית שמש",
            "שדרות יגאל אלון 1, בית שמש",
            "רחוב האצ״ל 1, בית שמש",
            "יהודה המכבי 3, בית שמש",
            "דרך יצחק רבין 19, בית שמש",
            "בן אליעזר 3, בית שמש",
            "שדרות רבי יהודה הנשיא 15, בית שמש",
            "שפת אמת 25, בית שמש",
            "נחל קטלב 2, בית שמש"
        };

        double[] latitudes = {
            32.053749046013294, 31.783873413383848, 32.0071058665407, 32.00701116245335, 32.01172928404402, 32.0172133953351, 32.02013905850679, 31.269544643819525, 31.26026280850482, 31.258875412278247, 31.261355773950353, 31.26599817239094, 31.26810025342388, 31.270759076690847, 32.8145186081906, 32.8116315773075, 32.81798598644384, 32.81117266997046, 32.80613804310656, 32.78278227095474, 32.77623531406369, 32.81065288451351, 32.815870631371325, 32.812499553433696, 32.81180658026046, 32.81275577804192, 32.81615758691202, 32.807085520556775, 32.80528851344975, 32.803728386688256, 32.801608920874536, 31.772244046048172, 31.775074755486816, 31.77869899329079, 31.778192331660847, 31.77690503728285, 31.7800584327404, 31.782083272129658, 31.770631783960987, 31.767111237602762, 31.74747891568372, 31.754609993381465, 31.756461050918553, 31.7513775722062, 31.749055738229274, 31.74555793800893, 31.74094091979134, 31.729964852998858, 31.73938560185697, 31.716310931732224
        };

        double[] longitudes = {
            34.8618141278376, 35.19590856003538, 34.743271985523506, 34.752186728407715, 34.74362385383784, 34.747229546496264, 34.745333810322535, 34.748627700497515, 34.77308048033317, 34.784702171018964, 34.78533897059906, 34.77990632146705, 34.78262253224672, 34.80778224155118, 34.96942136688308, 34.98523717981069, 34.98876182539728, 34.99657405678095, 34.99240903307264, 35.01498376162042, 35.008282393139794, 35.12234042641816, 35.1139504148565, 35.11011456487845, 35.10917157396484, 35.10525108926754, 35.112780076486196, 35.10462231648692, 35.101859943618564, 35.10342631423813, 35.10634890958312, 35.204097845199364, 35.20240377805922, 35.20038113098719, 35.18947099850603, 35.1864233661719, 35.187628477045756, 35.19075423337481, 35.18204009422545, 35.18112523364712, 34.99378563017565, 34.99482705774016, 34.98983391964748, 34.982859631374865, 34.9808196442101, 34.99371105686167, 34.98401323833724, 34.991448806063076, 34.99613311692805, 34.998070548696916
        };

        for (int i = 0; i < 50; i++)
        {
            int addressIndex = i;
            DateTime openTime = s_dal?.Config.Clock.AddMinutes(-s_rand.Next(1, 10000)) ?? throw new InvalidOperationException("s_dalConfig is null");
            DateTime? maxCompletionTime = openTime + s_dal?.Config.RiskRange;

            Call call = new Call(
                0, // ID will be auto-generated
                (CallType)s_rand.Next(Enum.GetValues(typeof(CallType)).Length),
                addresses[addressIndex],
                latitudes[addressIndex],
                longitudes[addressIndex],
                openTime,
                "Description " + i,
                maxCompletionTime
            );

            s_dal?.Call.Create(call);
        }
    }
    /// <summary>
    /// Creates a list of assignments with random data and adds them to the DAL.
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    private static void CreateAssignments()
    {
        // Retrieve all volunteers and calls from the DAL
        IEnumerable<Volunteer> volunteers = s_dal?.Volunteer.ReadAll() ?? throw new DalItIsNullException("s_dalVolunteer is null");
        IEnumerable<Call> calls = s_dal?.Call.ReadAll() ?? throw new DalItIsNullException("s_dalCall is null");

        // Ensure there are volunteers and calls available
        if (!volunteers.Any() || !calls.Any())
            throw new DalItIsNullException("No volunteers or calls available");

        // Remove 3 volunteers from the list to ensure they do not get assignments
        volunteers = volunteers.OrderBy(x => s_rand.Next()).Skip(3);

        // Ensure at least 15 calls are not assigned
        var unassignedCalls = calls.OrderBy(x => s_rand.Next()).Take(15);
        var assignableCalls = calls.Except(unassignedCalls).ToList();

        // Assign 5 calls to 5 volunteers who never handled calls
        var assignments = volunteers.Take(5).Select(volunteer =>
        {
            var call = assignableCalls.ElementAt(s_rand.Next(assignableCalls.Count));
            assignableCalls.Remove(call);
            DateTime entryTime = call.OpenTime.AddMinutes(s_rand.Next(0, (int)((call.MaxCompletionTime - call.OpenTime)?.TotalMinutes ?? 0)));
            DateTime? completionTime = entryTime.AddMinutes(s_rand.Next(1, (int)((call.MaxCompletionTime - entryTime)?.TotalMinutes ?? 0)));
            CompletionType? completionType = (CompletionType?)s_rand.Next(Enum.GetValues(typeof(CompletionType)).Length);

            return new Assignment(
                0, // ID will be auto-generated
                call.Id,
                volunteer.Id,
                entryTime,
                completionTime,
                completionType
            );
        }).ToList();

        // Assign 10 calls to 1 volunteers with random CompletionType
        assignments.AddRange(Enumerable.Range(0, 10).Select(_ =>
        {
            var volunteer = volunteers.ElementAt(10);
            var call = assignableCalls.ElementAt(s_rand.Next(assignableCalls.Count));
            assignableCalls.Remove(call);
            DateTime entryTime = call.OpenTime.AddMinutes(s_rand.Next(0, (int)((call.MaxCompletionTime - call.OpenTime)?.TotalMinutes ?? 0)));
            DateTime? completionTime = entryTime.AddMinutes(s_rand.Next(1, (int)((call.MaxCompletionTime - entryTime)?.TotalMinutes ?? 0)));
            CompletionType? completionType = (CompletionType?)s_rand.Next(Enum.GetValues(typeof(CompletionType)).Length);

            return new Assignment(
                0, // ID will be auto-generated
                call.Id,
                volunteer.Id,
                entryTime,
                completionTime,
                completionType
            );
        }));

        // Assign 10 calls that are still open (CompletionType is null)
        assignments.AddRange(Enumerable.Range(0, 10).Select(_ =>
        {
            // רשימה זמנית של מתנדבים פנויים (כאלה שלא שובצו למשימות עד כה)
            var availableVolunteers = volunteers
                .Where(v => !assignments.Any(a => a.VolunteerId == v.Id)) // סינון מתנדבים שכבר יש להם משימות
                .ToList();

            // בחירת מתנדב אקראי מרשימת המתנדבים 
            var volunteer = volunteers.ElementAt(s_rand.Next(volunteers.Count()));

            // בחירת קריאה (Call) אקראית מתוך הרשימה הזמינה
            if (!assignableCalls.Any())
                throw new InvalidOperationException("No assignable calls available.");

            var call = assignableCalls[s_rand.Next(assignableCalls.Count)];
            assignableCalls.Remove(call); // הסרה של הקריאה מרשימת הקריאות הזמינות

            // חישוב זמן כניסה וזמן סיום למשימה
            DateTime entryTime = call.OpenTime.AddMinutes(
                s_rand.Next(0, (int)((call.MaxCompletionTime - call.OpenTime)?.TotalMinutes ?? 0))
            );

            DateTime? completionTime = entryTime.AddMinutes(
                s_rand.Next(1, (int)((call.MaxCompletionTime - entryTime)?.TotalMinutes ?? 0))
            );

            // יצירת משימה חדשה עם סטטוס פתוח (CompletionType = null)
            return new Assignment(
                0, // המזהה יוגדר אוטומטית
                call.Id, // מזהה השיחה
                volunteer.Id, // מזהה המתנדב
                entryTime, // זמן התחלה
                completionTime, // זמן סיום (יכול להיות null אם לא הוגדר נכון)
                null // סטטוס המשימה - פתוחה
            );
        }));

        // Assign 5 calls that were closed after the maximum completion time
        assignments.AddRange(Enumerable.Range(0, 5).Select(_ =>
        {
            // רשימה זמנית של מתנדבים פנויים
            var availableVolunteers = volunteers
                .Where(v => !assignments.Any(a => a.VolunteerId == v.Id)) // סינון מתנדבים שכבר שובצו
                .ToList();

            // בחירת מתנדב אקראי מרשימת המתנדבים 
            var volunteer = volunteers.ElementAt(s_rand.Next(volunteers.Count()));

            // בדיקה אם יש מספיק קריאות זמינות
            if (assignableCalls.Count < 5)
                throw new InvalidOperationException("Not enough assignable calls available.");

            // בחירת קריאה אקראית
            var call = assignableCalls[s_rand.Next(assignableCalls.Count)];
            assignableCalls.Remove(call); // הסרה של הקריאה מהרשימה

            // חישוב זמן התחלה
            DateTime entryTime = call.OpenTime.AddMinutes(
                s_rand.Next(0, (int)((call.MaxCompletionTime - call.OpenTime)?.TotalMinutes ?? 0))
            );

            // בדיקה ויצירת completionTime עם ערך סביר
            if (call.MaxCompletionTime == null)
                throw new ArgumentException("MaxCompletionTime cannot be null for this operation.");

            DateTime? completionTime = call.MaxCompletionTime.Value.AddMinutes(s_rand.Next(1, 120)); // עד שעתיים אחרי הזמן המקסימלי

            // יצירת משימה חדשה
            return new Assignment(
                0, // ID יוגדר אוטומטית
                call.Id, // מזהה השיחה
                volunteer.Id, // מזהה המתנדב
                entryTime, // זמן התחלה
                completionTime, // זמן סיום
                CompletionType.Expired // סטטוס - פגת תוקף
            );
        }));

        // Add 20 more random assignments
        assignments.AddRange(Enumerable.Range(0, 20).Select(_ =>
        {
            var volunteer = volunteers.ElementAt(s_rand.Next(volunteers.Count()));
            var call = assignableCalls.ElementAt(s_rand.Next(assignableCalls.Count));
            assignableCalls.Remove(call);
            DateTime entryTime = call.OpenTime.AddMinutes(s_rand.Next(0, (int)((call.MaxCompletionTime - call.OpenTime)?.TotalMinutes ?? 0)));
            DateTime? completionTime = entryTime.AddMinutes(s_rand.Next(1, (int)((call.MaxCompletionTime - entryTime)?.TotalMinutes ?? 0)));
            CompletionType? completionType = (CompletionType?)s_rand.Next(Enum.GetValues(typeof(CompletionType)).Length);

            return new Assignment(
                0, // ID will be auto-generated
                call.Id,
                volunteer.Id,
                entryTime,
                completionTime,
                completionType
            );
        }));

        // Add all assignments to the DAL
        foreach (var assignment in assignments)
        {
            s_dal?.Assignment.Create(assignment);
        }
    }

    public static void Do(IDal dal) //stage 2
    {
        //Initialization.s_dalAssignment = s_dalAssignment ?? throw new NullReferenceException("DAL object can not be null!"); //stage 1
        //Initialization.s_dalVolunteer = s_dalVolunteer ?? throw new NullReferenceException("DAL object can not be null!"); //stage 1    
        //Initialization.s_dalCall = s_dalCall ?? throw new NullReferenceException("DAL object can not be null!"); //stage 1  
        //Initialization.s_dalConfig = s_dalConfig ?? throw new NullReferenceException("DAL object can not be null!"); //stage 1
        s_dal = dal ?? throw new DalItIsNullException("DAL object can not be null!"); // stage 2
        Console.WriteLine("Reset Configuration values and List values...");

        //s_dalConfig.Reset(); //stage 1
        //Console.WriteLine("Deleting all assignments..."); //stage 1
        //s_dalAssignment.DeleteAll(); //stage 1
        //Console.WriteLine("Deleting all volunteers..."); //stage 1
        //s_dalVolunteer.DeleteAll(); //stage 1
        //Console.WriteLine("Deleting all calls..."); //stage 1
        //s_dalCall.DeleteAll(); //stage 1

        s_dal.ResetDB();//stage 2

        Console.WriteLine("Creating volunteers..."); //stage 1
        CreateVolunteers();//stage 1  
        Console.WriteLine("Creating calls..."); //stage 1
        CreateCalls();//stage 1  
        Console.WriteLine("Creating assignments..."); //stage 1
        CreateAssignments();//stage 1  
        Console.WriteLine("Initialization completed successfully.");
    }
}