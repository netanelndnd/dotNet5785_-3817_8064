using System.Reflection;
using System.Text;
using System.IO;
using System.Net;
using System.Text.Json; // שימוש ב-System.Text.Json


namespace Helpers;

public static class Tools
{
    /// <summary>
    /// Converts the properties of an object to a string representation.
    /// </summary>
    /// <typeparam name="T">The type of the object.</typeparam>
    /// <param name="t">The object instance.</param>
    /// <returns>A string representation of the object's properties.</returns>
    public static string ToStringProperty<T>(this T t)
    {
        if (t == null)
        {
            return "null";
        }

        // Using StringBuilder for efficient string concatenation
        var sb = new StringBuilder();

        // Get all properties of the type T using Reflection
        var properties = t.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var property in properties)
        {
            var value = property.GetValue(t);

            // Add property name and value to the string
            sb.AppendLine($"{property.Name}: {FormatValue(value)}");
        }

        return sb.ToString();
    }

    // Helper method to format the property value (handle collections and nulls)
    private static string FormatValue(object value)
    {
        if (value == null)
        {
            return "null";
        }

        // If the value is a collection, process each item in the collection
        if (value is System.Collections.IEnumerable collection && !(value is string))
        {
            var collectionItems = string.Join(", ", collection.Cast<object>().Select(item => item?.ToString() ?? "null"));
            return $"[{collectionItems}]";
        }

        // For other types, simply return the value as a string
        return value.ToString();
    }

    /// <summary>
    /// Checks if the given latitude and longitude coordinates are within the geographical boundaries of Israel.
    /// </summary>
    /// <param name="latitude">The latitude coordinate to check.</param>
    /// <param name="longitude">The longitude coordinate to check.</param>
    /// <returns>True if the coordinates are within Israel, otherwise false.</returns>
    //public static bool IsLocationInIsrael(double? latitude, double? longitude)
    //{
    //    // Latitude and longitude range of Israel
    //    const double minLatitude = 29.0;   // South - Eilat
    //    const double maxLatitude = 33.3;   // North - Golan Heights
    //    const double minLongitude = 34.3;  // West - Mediterranean Sea
    //    const double maxLongitude = 35.9;  // East - Dead Sea and Jordan area

    //    // Check if the coordinates are within the range
    //    return latitude >= minLatitude && latitude <= maxLatitude &&
    //           longitude >= minLongitude && longitude <= maxLongitude;
    //}



    public static (double Latitude, double Longitude, bool IsInIsrael) GetCoordinates(string address)
    {
        string apiKey = "AIzaSyBnuV561P8tA08Y7DQDH0GAu5AhQ86m5xs";

        try
        {
            if (string.IsNullOrWhiteSpace(address))
            {
                throw new ArgumentException("התרחש ניסיון לקבל את נקודות הציון אבל הכתובת אינה תקינה או ריקה.");
            }
        }
        catch (ArgumentException ex)
        {
            throw new BO.BlSystemException("Error processing address: " + ex.Message, ex);
        }

        string encodedAddress = Uri.EscapeDataString(address);
        string url = $"https://maps.googleapis.com/maps/api/geocode/json?address={encodedAddress}&key={apiKey}";

        try
        {
            using (var httpClient = new HttpClient())
            {
                var response = httpClient.GetAsync(url).Result;
                response.EnsureSuccessStatusCode();

                string jsonResponse = response.Content.ReadAsStringAsync().Result;
                JsonDocument jsonDocument = JsonDocument.Parse(jsonResponse);

                var status = jsonDocument.RootElement.GetProperty("status").GetString();
                if (status != "OK")
                {
                    throw new Exception($"שגיאה בבקשת Geocoding: {status}");
                }

                var location = jsonDocument.RootElement
                    .GetProperty("results")[0]
                    .GetProperty("geometry")
                    .GetProperty("location");

                double latitude = location.GetProperty("lat").GetDouble();
                double longitude = location.GetProperty("lng").GetDouble();

                // בדיקת אם המיקום נמצא בישראל על פי התשובה מגוגל
                bool isInIsrael = IsLocationInIsrael(jsonDocument);

                return (latitude, longitude, isInIsrael);
            }
        }
        catch (HttpRequestException ex)
        {
            throw new Exception("Error in network request: " + ex.Message, ex);
        }
        catch (Exception ex)
        {
            throw new Exception("Error processing address: " + ex.Message, ex);
        }
    }

    // פונקציה שבודקת אם המיקום בישראל על פי תשובת גוגל
    public static bool IsLocationInIsrael(JsonDocument jsonDocument)
    {
        // חיפוש אם יש רכיב "ישראל" ב-address_components
        foreach (var result in jsonDocument.RootElement.GetProperty("results").EnumerateArray())
        {
            var addressComponents = result.GetProperty("address_components");

            foreach (var component in addressComponents.EnumerateArray())
            {
                // אם חלק מהכתובת הוא מדינה, נבדוק אם היא ישראל
                if (component.GetProperty("types").EnumerateArray().Any(type => type.GetString() == "country"))
                {
                    string country = component.GetProperty("long_name").GetString();
                    if (country == "Israel")
                    {
                        return true; // אם נמצאה ישראל
                    }
                }
            }
        }

        return false; // לא נמצאה ישראל
    }
}
