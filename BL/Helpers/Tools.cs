using System.Reflection;
using System.Text;

namespace Helpers;

internal static class Tools
{
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
}
    

