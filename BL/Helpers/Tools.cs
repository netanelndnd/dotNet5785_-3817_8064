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



    //פונרקציה שנועדה להיות בשביל הטסטים
    public static (double Latitude, double Longitude, bool IsInIsrael) GetCoordinate(string address)
    {
        try
        {
            string apiKey = "AIzaSyBnuV561P8tA08Y7DQDH0GAu5AhQ86m5xs";

            if (string.IsNullOrWhiteSpace(address))
            {
                throw new ArgumentException("התרחש ניסיון לקבל את נקודות הציון אבל הכתובת אינה תקינה או ריקה.");
            }

            string encodedAddress = Uri.EscapeDataString(address);
            string url = $"https://maps.googleapis.com/maps/api/geocode/json?address={encodedAddress}&key={apiKey}";

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


    /// <summary>
    /// Gets the coordinates of an address using the Google Maps Geocoding API.
    /// </summary>
    /// <param name="address"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="Exception"></exception>
    public static async Task<(double Latitude, double Longitude, bool IsInIsrael)> GetCoordinatesAsync(string address)
    {
        try
        {
            string apiKey = "AIzaSyBnuV561P8tA08Y7DQDH0GAu5AhQ86m5xs";

            if (string.IsNullOrWhiteSpace(address))
            {
                throw new ArgumentException("התרחש ניסיון לקבל את נקודות הציון אבל הכתובת אינה תקינה או ריקה.");
            }

            string encodedAddress = Uri.EscapeDataString(address);
            string url = $"https://maps.googleapis.com/maps/api/geocode/json?address={encodedAddress}&key={apiKey}";

            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                string jsonResponse = await response.Content.ReadAsStringAsync();
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

    // פונקציה לחישוב המרחק בין שתי נקודות על פי קו רוחב וקו אורך
    public static double CalculateDistance(double? lat1, double? lon1, double? lat2, double? lon2)
    {
        if (lat1 == null || lon1 == null || lat2 == null || lon2 == null)
        {
            throw new ArgumentNullException("Coordinates cannot be null");
        }

        // קבועים עבור חישוב המרחק
        double R = 6371; // רדיוס כדור הארץ בקילומטרים
        double dLat = DegreesToRadians((double)(lat2 - lat1)); // שינוי בקו הרוחב
        double dLon = DegreesToRadians((double)(lon2 - lon1)); // שינוי בקו האורך

        // חישוב המרחק באמצעות נוסחת Haversine
        double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                   Math.Cos(DegreesToRadians((double)lat1)) * Math.Cos(DegreesToRadians((double)lat2)) *
                   Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        double distance = R * c; // המרחק בקילומטרים

        return distance;
    }


    // פונקציה להמיר מעלות לרדיאנים
    private static double DegreesToRadians(double degrees)
    {
        return degrees * (Math.PI / 180);
    }



}
