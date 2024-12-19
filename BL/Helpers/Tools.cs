using System.Reflection;
using System.Text;

namespace Helpers;

internal static class Tools
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
    public static bool IsLocationInIsrael(double? latitude, double? longitude)
    {
        // Latitude and longitude range of Israel
        const double minLatitude = 29.0;   // South - Eilat
        const double maxLatitude = 33.3;   // North - Golan Heights
        const double minLongitude = 34.3;  // West - Mediterranean Sea
        const double maxLongitude = 35.9;  // East - Dead Sea and Jordan area

        // Check if the coordinates are within the range
        return latitude >= minLatitude && latitude <= maxLatitude &&
               longitude >= minLongitude && longitude <= maxLongitude;
    }
}
    

