namespace Dal;
using DalApi;
using DO;
using System;
using System.Collections.Generic;

/// <summary>
/// Implementation of ICall interface for managing call records using XML serialization.
/// </summary>
public class CallImplementation : ICall
{
    /// <summary>
    /// Creates a new call record and saves it to the XML file.
    /// </summary>
    /// <param name="item">The call record to create.</param>
    public void Create(Call item)
    {
        List<Call> Calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_calls_xml);
        item = item with { Id = Config.NextCallId };
        Calls.Add(item);
        XMLTools.SaveListToXMLSerializer(Calls, Config.s_calls_xml);
    }

    /// <summary>
    /// Deletes a call record by its ID.
    /// </summary>
    /// <param name="id">The ID of the call record to delete.</param>
    /// <exception cref="DalDoesNotExistException">Thrown when the call record with the specified ID does not exist.</exception>
    public void Delete(int id)
    {
        List<Call> Calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_calls_xml);
        if (Calls.RemoveAll(it => it.Id == id) == 0)
            throw new DalDoesNotExistException($"Call with ID={id} does not exist");
        XMLTools.SaveListToXMLSerializer(Calls, Config.s_calls_xml);
    }

    /// <summary>
    /// Deletes all call records from the XML file.
    /// </summary>
    public void DeleteAll()
    {
        XMLTools.SaveListToXMLSerializer(new List<Call>(), Config.s_calls_xml);
    }

    /// <summary>
    /// Reads a call record by its ID.
    /// </summary>
    /// <param name="id">The ID of the call record to read.</param>
    /// <returns>The call record with the specified ID, or null if not found.</returns>
    public Call? Read(int id)
    {
        List<Call> Calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_calls_xml);
        return Calls.FirstOrDefault(it => it.Id == id);
    }

    /// <summary>
    /// Reads a call record by a specified filter.
    /// </summary>
    /// <param name="filter">The filter function to apply to the call records.</param>
    /// <returns>The call record that matches the filter, or null if not found.</returns>
    public Call? Read(Func<Call, bool> filter)
    {
        List<Call> Calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_calls_xml);
        return Calls.FirstOrDefault(filter);
    }

    /// <summary>
    /// Reads all call records, optionally filtered by a specified condition.
    /// </summary>
    /// <param name="filter">The filter function to apply to the call records, or null to return all records.</param>
    /// <returns>An enumerable collection of call records that match the filter, or all records if no filter is specified.</returns>
    public IEnumerable<Call> ReadAll(Func<Call, bool>? filter = null)
    {
        List<Call> Calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_calls_xml);
        return filter == null ? Calls : Calls.Where(filter);
    }

    /// <summary>
    /// Updates an existing call record.
    /// </summary>
    /// <param name="item">The call record to update.</param>
    /// <exception cref="DalDoesNotExistException">Thrown when the call record with the specified ID does not exist.</exception>
    public void Update(Call item)
    {
        List<Call> Calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_calls_xml);
        if (Calls.RemoveAll(it => it.Id == item.Id) == 0)
            throw new DalDoesNotExistException($"Call with ID={item.Id} does not exist");
        Calls.Add(item);
        XMLTools.SaveListToXMLSerializer(Calls, Config.s_calls_xml);
    }
}
