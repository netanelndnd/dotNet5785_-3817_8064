namespace Dal;

using DO;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;


static class XMLTools
{
    /// <summary>
    /// Directory path for storing XML files.
    /// </summary>
    const string s_xmlDir = @"..\xml\";
    /// <summary>
    /// Static constructor to ensure the XML directory exists.
    /// </summary>
    static XMLTools()
    {
        if (!Directory.Exists(s_xmlDir))
            Directory.CreateDirectory(s_xmlDir);
    }
    // This region contains helper methods for working with XML files using XmlSerializer.
    #region SaveLoadWithXMLSerializer
    /// <summary>
    /// Saves a list of objects to an XML file using XmlSerializer.
    /// </summary>
    /// <typeparam name="T">The type of objects in the list.</typeparam>
    /// <param name="list">The list of objects to save.</param>
    /// <param name="xmlFileName">The name of the XML file.</param>
    /// <exception cref="DalXMLFileLoadCreateException">Thrown when the XML file cannot be created.</exception>
    public static void SaveListToXMLSerializer<T>(List<T> list, string xmlFileName) where T : class
    {
        string xmlFilePath = Path.Combine(s_xmlDir, xmlFileName); // שימוש ב-Path.Combine לבנייה בטוחה של נתיבים

        try
        {
            using FileStream file = new(xmlFilePath, FileMode.Create, FileAccess.Write, FileShare.None);
            var serializer = new XmlSerializer(typeof(List<T>));
            serializer.Serialize(file, list);
        }
        catch (Exception ex)
        {
            throw new DalXMLFileLoadCreateException($"fail to create xml file: {s_xmlDir + xmlFilePath}, {ex.Message}");
        }
    }


    /// <summary>
    /// Loads a list of objects from an XML file using XmlSerializer.
    /// </summary>
    /// <typeparam name="T">The type of objects in the list.</typeparam>
    /// <param name="xmlFileName">The name of the XML file.</param>
    /// <returns>The list of objects loaded from the XML file.</returns>
    /// <exception cref="DalXMLFileLoadCreateException">Thrown when the XML file cannot be loaded.</exception>
    public static List<T> LoadListFromXMLSerializer<T>(string xmlFileName) where T : class
    {
        string xmlFilePath = s_xmlDir + xmlFileName;

        try
        {
            if (!File.Exists(xmlFilePath)) return new();
            using FileStream file = new(xmlFilePath, FileMode.Open);
            XmlSerializer x = new(typeof(List<T>));
            return x.Deserialize(file) as List<T> ?? new();
        }
        catch (Exception ex)
        {
            throw new DalXMLFileLoadCreateException($"fail to load xml file: {xmlFilePath}, {ex.Message}");
        }
    }
    #endregion

    // This region contains helper methods for working with XML files using XElement.
    #region SaveLoadWithXElement
    /// <summary>
    /// Saves an XElement to an XML file.
    /// </summary>
    /// <param name="rootElem">The root XElement to save.</param>
    /// <param name="xmlFileName">The name of the XML file.</param>
    /// <exception cref="DalXMLFileLoadCreateException">Thrown when the XML file cannot be created.</exception>
    public static void SaveListToXMLElement(XElement rootElem, string xmlFileName)
    {
        string xmlFilePath = s_xmlDir + xmlFileName;

        try
        {
            rootElem.Save(xmlFilePath);
        }
        catch (Exception ex)
        {
            throw new DalXMLFileLoadCreateException($"fail to create xml file: {s_xmlDir + xmlFilePath}, {ex.Message}");
        }
    }
    /// <summary>
    /// Loads an XElement from an XML file.
    /// </summary>
    /// <param name="xmlFileName">The name of the XML file.</param>
    /// <returns>The loaded XElement.</returns>
    /// <exception cref="DalXMLFileLoadCreateException">Thrown when the XML file cannot be loaded.</exception>
    public static XElement LoadListFromXMLElement(string xmlFileName)
    {
        string xmlFilePath = s_xmlDir + xmlFileName;

        try
        {
            if (File.Exists(xmlFilePath))
                return XElement.Load(xmlFilePath);
            XElement rootElem = new(xmlFileName);
            rootElem.Save(xmlFilePath);
            return rootElem;
        }
        catch (Exception ex)
        {
            throw new DalXMLFileLoadCreateException($"fail to load xml file: {s_xmlDir + xmlFilePath}, {ex.Message}");
        }
    }
    #endregion

    #region XmlConfig
    /// <summary>
    /// Retrieves the current integer value from the specified XML element, increments it by one, 
    /// and saves the updated value back to the XML file.
    /// </summary>
    /// <param name="xmlFileName">The name of the XML file.</param>
    /// <param name="elemName">The name of the XML element.</param>
    /// <returns>The current integer value before incrementing.</returns>
    /// <exception cref="FormatException">Thrown when the element value cannot be converted to an integer.</exception>
    public static int GetAndIncreaseConfigIntVal(string xmlFileName, string elemName)
    {
        XElement root = XMLTools.LoadListFromXMLElement(xmlFileName); // Load the XML file into an XElement object
        int nextId = root.ToIntNullable(elemName) ?? throw new FormatException($"can't convert:  {xmlFileName}, {elemName}"); // Retrieve the current integer value from the specified element
        root.Element(elemName)?.SetValue((nextId + 1).ToString()); // Increment the value by one and update the element
        XMLTools.SaveListToXMLElement(root, xmlFileName); // Save the updated XML back to the file
        return nextId; // Return the original value before incrementing
    }

    /// <summary>
    /// Loads a configuration XML file, retrieves the value of the specified element,
    /// and converts it to an integer.
    /// </summary>
    /// <param name="xmlFileName">The name of the configuration XML file.</param>
    /// <param name="elemName">The name of the XML element to retrieve the value from.</param>
    /// <returns>The integer value of the specified element.</returns>
    /// <exception cref="FormatException">
    /// Thrown when the specified element cannot be found or its value is not a valid integer.
    /// </exception>
    public static int GetConfigIntVal(string xmlFileName, string elemName)
    {
        XElement root = XMLTools.LoadListFromXMLElement(xmlFileName);
        int num = root.ToIntNullable(elemName) ?? throw new FormatException($"can't convert:  {xmlFileName}, {elemName}");
        return num;
    }
    /// <summary>
    /// Loads a configuration XML file, retrieves the value of the specified element,
    /// and converts it to a DateTime.
    /// </summary>
    /// <param name="xmlFileName">The name of the configuration XML file.</param>
    /// <param name="elemName">The name of the XML element to retrieve the value from.</param>
    /// <returns>The DateTime value of the specified element.</returns>
    /// <exception cref="FormatException">
    /// Thrown when the specified element cannot be found or its value is not a valid DateTime.
    /// </exception>
    public static DateTime GetConfigDateVal(string xmlFileName, string elemName)
    {
        XElement root = XMLTools.LoadListFromXMLElement(xmlFileName);
        DateTime dt = root.ToDateTimeNullable(elemName) ?? throw new FormatException($"can't convert:  {xmlFileName}, {elemName}");
        return dt;
    }
    /// <summary>
    /// Loads a configuration XML file, retrieves the value of the specified element,
    /// and converts it to a TimeSpan.
    /// </summary>
    /// <param name="xmlFileName">The name of the configuration XML file.</param>
    /// <param name="elemName">The name of the XML element to retrieve the value from.</param>
    /// <returns>The TimeSpan value of the specified element.</returns>
    /// <exception cref="FormatException">Thrown when the specified element cannot be found or its value is not a valid TimeSpan.</exception>
    public static TimeSpan GetConfigTimeSpanVal(string xmlFileName, string elemName)
    {
        XElement root = XMLTools.LoadListFromXMLElement(xmlFileName);
        TimeSpan dt = root.ToTimeSpanNullable(elemName) ?? throw new FormatException($"can't convert:  {xmlFileName}, {elemName}");
        return dt;
    }

    /// <summary>
    /// Sets the value of the specified XML element to the given integer value and saves the updated XML file.
    /// </summary>
    /// <param name="xmlFileName">The name of the XML file.</param>
    /// <param name="elemName">The name of the XML element.</param>
    /// <param name="elemVal">The integer value to set.</param>
    public static void SetConfigIntVal(string xmlFileName, string elemName, int elemVal)
    {
        XElement root = XMLTools.LoadListFromXMLElement(xmlFileName);
        root.Element(elemName)?.SetValue((elemVal).ToString());
        XMLTools.SaveListToXMLElement(root, xmlFileName);
    }

    /// <summary>
    /// Sets the value of the specified XML element to the given DateTime value and saves the updated XML file.
    /// </summary>
    /// <param name="xmlFileName">The name of the XML file.</param>
    /// <param name="elemName">The name of the XML element.</param>
    /// <param name="elemVal">The DateTime value to set.</param>
    public static void SetConfigDateVal(string xmlFileName, string elemName, DateTime elemVal)
    {
        XElement root = XMLTools.LoadListFromXMLElement(xmlFileName);
        root.Element(elemName)?.SetValue((elemVal).ToString());
        XMLTools.SaveListToXMLElement(root, xmlFileName);
    }

    /// <summary>
    /// Sets the value of the specified XML element to the given TimeSpan value and saves the updated XML file.
    /// </summary>
    /// <param name="xmlFileName">The name of the XML file.</param>
    /// <param name="elemName">The name of the XML element.</param>
    /// <param name="elemVal">The TimeSpan value to set.</param>
    public static void SetConfigTimeSpanVal(string xmlFileName, string elemName, TimeSpan elemVal)
    {
        XElement root = XMLTools.LoadListFromXMLElement(xmlFileName);
        root.Element(elemName)?.SetValue((elemVal).ToString());
        XMLTools.SaveListToXMLElement(root, xmlFileName);
    }
    #endregion

    // This region contains extension methods for the XElement class to convert text values from an XML file.
    #region ExtensionFuctions
    /// <summary>
    /// Converts the value of the specified element to an enumeration of the specified type.
    /// </summary>
    /// <typeparam name="T">The enumeration type to convert to.</typeparam>
    /// <param name="element">The XML element containing the value to convert.</param>
    /// <param name="name">The name of the XML element to retrieve the value from.</param>
    /// <returns>The enumeration value of the specified type, or null if the conversion fails.</returns>
    public static T? ToEnumNullable<T>(this XElement element, string name) where T : struct, Enum =>
        Enum.TryParse<T>((string?)element.Element(name), out var result) ? (T?)result : null;

    /// <summary>
    /// Converts the value of the specified element to a nullable DateTime.
    /// </summary>
    /// <param name="element">The XML element containing the value to convert.</param>
    /// <param name="name">The name of the XML element to retrieve the value from.</param>
    /// <returns>The DateTime value of the specified element, or null if the conversion fails.</returns>
    public static DateTime? ToDateTimeNullable(this XElement element, string name) =>
        DateTime.TryParse((string?)element.Element(name), out var result) ? (DateTime?)result : null;

    /// <summary>
    /// Converts the value of the specified element to a nullable TimeSpan.
    /// </summary>
    /// <param name="element">The XML element containing the value to convert.</param>
    /// <param name="name">The name of the XML element to retrieve the value from.</param>
    /// <returns>The TimeSpan value of the specified element, or null if the conversion fails.</returns>
    public static TimeSpan? ToTimeSpanNullable(this XElement element, string name) =>
        TimeSpan.TryParse((string?)element.Element(name), out var result) ? (TimeSpan?)result : null;

    /// <summary>
    /// Converts the value of the specified element to a nullable double.
    /// </summary>
    /// <param name="element">The XML element containing the value to convert.</param>
    /// <param name="name">The name of the XML element to retrieve the value from.</param>
    /// <returns>The double value of the specified element, or null if the conversion fails.</returns>
    public static double? ToDoubleNullable(this XElement element, string name) =>
        double.TryParse((string?)element.Element(name), out var result) ? (double?)result : null;

    /// <summary>
    /// Converts the value of the specified element to a nullable integer.
    /// </summary>
    /// <param name="element">The XML element containing the value to convert.</param>
    /// <param name="name">The name of the XML element to retrieve the value from.</param>
    /// <returns>The integer value of the specified element, or null if the conversion fails.</returns>
    public static int? ToIntNullable(this XElement element, string name) =>
        int.TryParse((string?)element.Element(name), out var result) ? (int?)result : null;
    #endregion
}
