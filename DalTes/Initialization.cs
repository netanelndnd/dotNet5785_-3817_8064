namespace DalTest;
using DalApi;
using DO;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;

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

    static int CallOpenDuration = 200; // כמות הזמן שאנחנו נותנים לכל קריאה שתהיה פתוחה

    /// <summary>
    /// Creates a list of volunteers with random data and adds them to the DAL.
    /// </summary>
    private static void CreateVolunteers()
    {
        string[] names = { "Yossi Cohen", "Rivka Levi", "Moshe Mizrahi", "Yael Katz", "David Peretz", "Sara Ben-David", "Avi Shalom", "Miriam Gold", "Daniel Azulay", "Leah Bar", "Yitzhak Shimon", "Esther Malka", "Ronen Alon", "Tamar Shani", "Elior Ben-Ami", "Shira Tal" };
        string[] emails = { "yossi.cohen@gmail.com", "rivka.levi@gmail.com", "moshe.mizrahi@gmail.com", "yael.katz@gmail.com", "david.peretz@gmail.com", "sara.ben-david@gmail.com", "avi.shalom@gmail.com", "miriam.gold@gmail.com", "daniel.azulay@gmail.com", "leah.bar@gmail.com", "yitzhak.shimon@gmail.com", "esther.malka@gmail.com", "ronen.alon@gmail.com", "tamar.shani@gmail.com", "elior.ben-ami@gmail.com", "shira.tal@gmail.com" };
        string[] phoneNumbers = { "0501234567", "0501234568", "0501234569", "0501234570", "0501234571", "0501234572", "0501234573", "0501234574", "0501234575", "0501234576", "0501234577", "0501234578", "0501234579", "0501234580", "0501234581", "0501234582","0542858949" };

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

        // מערך תעודות זהות חוקיות
        int[] validIDs = new int[]
        {
    212041966, 437208333, 110115870, 772602942, 327448890,
    846816759, 502724511, 328030119, 147250120, 988011417,
    478346588, 465589265, 717759393, 803305234, 347882722,
    271468613, 625830781, 817591332, 129652475, 353771595,
        };

        // אינדקס עוקב כדי לבחור מזהים בסדר
        int idIndex = 0;

        // יצירת מתנדבים
        for (int i = 0; i < names.Length; i++)
        {
            // שימוש בתעודת זהות מהמערך
            int id = validIDs[idIndex];
            idIndex = (idIndex + 1) % validIDs.Length; // מעבר לתעודת הזהות הבאה במעגליות

            double? maxDistance = 100 + s_rand.NextDouble() * 100; // מרחק מקסימלי רנדומלי עם מינימום 100 ועד מקסימום 200
            Role role = i == 0 ? Role.Manager : Role.Volunteer; // המתנדב הראשון הוא מנהל
            int addressIndex = 10;

            Volunteer volunteer = new Volunteer(
                id,
                names[i],
                phoneNumbers[i],
                emails[i],
                emails[i],
                addresses[addressIndex],
                latitudes[addressIndex],
                longitudes[addressIndex],
                maxDistance,
                role,
                true,
                DistanceType.AirDistance
            );

            addressIndex++; // מעבר לכתובת הבאה

            s_dal?.Volunteer?.Create(volunteer); // שמירת המתנדב בבסיס הנתונים
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

        for (int i = 0; i < 65; i++) // Changed to 65 to create 15 more calls
        {
            int addressIndex = i < 50 ? i : s_rand.Next(0, 50); // Use existing addresses randomly for the additional 20 calls
            DateTime openTime = s_dal?.Config.Clock.AddMinutes(-s_rand.Next(0, CallOpenDuration)) ?? throw new InvalidOperationException("s_dalConfig is null");
            DateTime? maxCompletionTime = openTime.AddMinutes(CallOpenDuration);

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
        var volunteers = s_dal?.Volunteer.ReadAll() ?? throw new DalItIsNullException("s_dalVolunteer is null");
        var calls = s_dal?.Call.ReadAll() ?? throw new DalItIsNullException("s_dalCall is null");

        // Ensure there are volunteers and calls available
        if (!volunteers.Any() || !calls.Any())
            throw new DalItIsNullException("No volunteers or calls available");

        // Remove 3 volunteers from the list to ensure they do not get assignments
        volunteers = volunteers.OrderBy(x => s_rand.Next()).Skip(3);

        // Define the number of calls for each status
        int openCallsCount = 20;
        int expiredCallsCount = 5;
        int closedCallsCount = 30;
        int inProgressCallsCount = 7;
        int openRiskCallsCount = 3;

        // Assign calls to volunteers
        var assignments = new List<Assignment>();

        // Open calls (not assigned)
        var openCalls = calls.Take(openCallsCount).ToList();
        var assignableCalls = calls.Skip(openCallsCount).ToList();

        // Expired calls
        var expiredCalls = assignableCalls.Take(expiredCallsCount).ToList();
        assignableCalls = assignableCalls.Skip(expiredCallsCount).ToList();
        foreach (var call in expiredCalls)
        {
            var volunteer1 = volunteers.FirstOrDefault(v => GeoUtils.CalculateDistance(v.Latitude, v.Longitude, call.Latitude, call.Longitude) <= v.MaxDistance);
            if (volunteer1 != null)
            {
                assignments.Add(new Assignment(
                    0,
                    call.Id,
                    volunteer1.Id,
                    call.OpenTime,
                    call.MaxCompletionTime?.AddMinutes(s_rand.Next(1, CallOpenDuration + 30)),
                    CompletionType.Expired
                ));
            }
        }

        // Closed calls
        var closedCalls = assignableCalls.Take(closedCallsCount).ToList();
        assignableCalls = assignableCalls.Skip(closedCallsCount).ToList();

        // חלוקת הקריאות הסגורות ל-2 קבוצות
        var closedCallsCancelled = closedCalls.Take(10).ToList(); // 10 קריאות שבוטלו ברבע השעה הראשונה
        var closedCallsWithVolunteers = closedCalls.Skip(10).Take(20).ToList(); // 20 קריאות שמוקצות כרגיל למתנדבים

        // טיפול בקבוצה הראשונה: קריאות שבוטלו ברבע השעה הראשונה
        foreach (var call in closedCallsCancelled)
        {
            var volunteer5 = volunteers.FirstOrDefault(v => GeoUtils.CalculateDistance(v.Latitude, v.Longitude, call.Latitude, call.Longitude) <= v.MaxDistance);
            if (volunteer5 != null)
            {
                DateTime entryTime = call.OpenTime.AddMinutes(s_rand.Next(0, 15));
                CompletionType completionType = (CompletionType)s_rand.Next(1, 2);

                assignments.Add(new Assignment(
                    0,
                    call.Id,
                    volunteer5.Id,
                    entryTime,
                    call.OpenTime.AddMinutes(15),
                    completionType
                ));

                var volunteer6 = volunteers.FirstOrDefault(v => GeoUtils.CalculateDistance(v.Latitude, v.Longitude, call.Latitude, call.Longitude) <= v.MaxDistance);
                if (volunteer6 != null)
                {
                    DateTime completionEntryTime = entryTime.AddMinutes(s_rand.Next(15, 25));
                    DateTime? completionTime = completionEntryTime.AddMinutes(s_rand.Next(1, (int)((call.MaxCompletionTime - completionEntryTime)?.TotalMinutes ?? 0)));

                    assignments.Add(new Assignment(
                        0,
                        call.Id,
                        volunteer6.Id,
                        completionEntryTime,
                        completionTime,
                        CompletionType.Treated
                    ));
                }
            }
        }

        // טיפול בקבוצה השנייה: קריאות שמוקצות כרגיל למתנדבים
        foreach (var call in closedCallsWithVolunteers)
        {
            var volunteer = volunteers.FirstOrDefault(v => GeoUtils.CalculateDistance(v.Latitude, v.Longitude, call.Latitude, call.Longitude) <= v.MaxDistance);
            if (volunteer != null)
            {
                DateTime entryTime = call.OpenTime.AddMinutes(s_rand.Next(0, (int)((call.MaxCompletionTime - call.OpenTime)?.TotalMinutes ?? 0)));
                DateTime? completionTime = entryTime.AddMinutes(s_rand.Next(1, (int)((call.MaxCompletionTime - entryTime)?.TotalMinutes ?? 0)));

                assignments.Add(new Assignment(
                    0,
                    call.Id,
                    volunteer.Id,
                    entryTime,
                    completionTime,
                    CompletionType.Treated
                ));
            }
        }

        // In-progress calls
        var inProgressCalls = assignableCalls.Take(inProgressCallsCount).ToList();
        assignableCalls = assignableCalls.Skip(inProgressCallsCount).ToList();
        var inProgressVolunteers = new List<Volunteer>();

        foreach (var call in inProgressCalls)
        {
            var volunteer3 = volunteers.FirstOrDefault(v => GeoUtils.CalculateDistance(v.Latitude, v.Longitude, call.Latitude, call.Longitude) <= v.MaxDistance);
            if (volunteer3 != null)
            {
                inProgressVolunteers.Add(volunteer3);
                DateTime entryTime = call.OpenTime.AddMinutes(s_rand.Next(0, (int)((call.MaxCompletionTime - call.OpenTime)?.TotalMinutes ?? 0)));

                assignments.Add(new Assignment(
                    0,
                    call.Id,
                    volunteer3.Id,
                    entryTime,
                    null,
                    null
                ));

                volunteers = volunteers.Where(v => v.Id != volunteer3.Id).ToList();
            }
        }

        // Open risk calls
        var openRiskCalls = assignableCalls.Take(openRiskCallsCount).ToList();
        assignableCalls = assignableCalls.Skip(openRiskCallsCount).ToList();
        var remainingVolunteers = volunteers.Except(inProgressVolunteers).ToList();
        var riskRange = s_dal?.Config.RiskRange ?? TimeSpan.FromMinutes(30);

        foreach (var call in openRiskCalls)
        {
            var volunteer4 = remainingVolunteers.FirstOrDefault(v => GeoUtils.CalculateDistance(v.Latitude, v.Longitude, call.Latitude, call.Longitude) <= v.MaxDistance);
            if (volunteer4 != null && call.MaxCompletionTime.HasValue)
            {
                DateTime riskStartTime = call.MaxCompletionTime.Value - riskRange;
                DateTime riskEndTime = call.MaxCompletionTime.Value;
                DateTime entryTime = riskStartTime.AddMinutes(s_rand.Next(0, (int)(riskEndTime - riskStartTime).TotalMinutes));

                assignments.Add(new Assignment(
                    0,
                    call.Id,
                    volunteer4.Id,
                    entryTime,
                    null,
                    null
                ));

                remainingVolunteers = remainingVolunteers.Where(v => v.Id != volunteer4.Id).ToList();
            }
        }

        // Add all assignments to the DAL
        foreach (var assignment in assignments)
        {
            s_dal?.Assignment.Create(assignment);
        }
    }

    public static class GeoUtils
    {
        public static double CalculateDistance(double? lat1, double? lon1, double? lat2, double? lon2)
        {
            if (lat1 == null || lon1 == null || lat2 == null || lon2 == null)
            {
                throw new ArgumentNullException("Latitude and longitude values cannot be null");
            }

            const double R = 6371; // Radius of the Earth in kilometers
            double latDistance = ToRadians(lat2.Value - lat1.Value);
            double lonDistance = ToRadians(lon2.Value - lon1.Value);
            double a = Math.Sin(latDistance / 2) * Math.Sin(latDistance / 2) +
                       Math.Cos(ToRadians(lat1.Value)) * Math.Cos(ToRadians(lat2.Value)) *
                       Math.Sin(lonDistance / 2) * Math.Sin(lonDistance / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c; // Distance in kilometers
        }

        private static double ToRadians(double angle)
        {
            return angle * (Math.PI / 180);
        }
    }


    //public static void Do(IDal dal) //stage 2
    public static void Do() //stage 4
    {
        //Initialization.s_dalAssignment = s_dalAssignment ?? throw new NullReferenceException("DAL object can not be null!"); //stage 1
        //Initialization.s_dalVolunteer = s_dalVolunteer ?? throw new NullReferenceException("DAL object can not be null!"); //stage 1    
        //Initialization.s_dalCall = s_dalCall ?? throw new NullReferenceException("DAL object can not be null!"); //stage 1  
        //Initialization.s_dalConfig = s_dalConfig ?? throw new NullReferenceException("DAL object can not be null!"); //stage 1
        //s_dal = dal ?? throw new NullReferenceException("DAL object can not be null!"); //stage 2
        s_dal = DalApi.Factory.Get; //stage 4
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