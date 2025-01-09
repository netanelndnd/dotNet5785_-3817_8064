namespace Dal;
using DalApi;
using DO;
using System;
using System.Collections.Generic;

/// <summary>
/// Implementation of the IAssignment interface for managing assignments using XML storage.
/// </summary>
public class AssignmentImplementation : IAssignment
{
    /// <summary>
    /// Creates a new assignment and saves it to the XML file.
    /// </summary>
    /// <param name="item">The assignment to create.</param>
    public void Create(Assignment item)
    {
        var Assignments = XMLTools.LoadListFromXMLSerializer<Assignment>(Config.s_assignments_xml);
        if (Assignments.Any(it => it.Id == item.Id))
            throw new DalAlreadyExistsException($"Assignment with ID={item.Id} already exists");
        item = item with { Id = Config.NextAssignmentId };
        Assignments.Add(item);
        XMLTools.SaveListToXMLSerializer(Assignments, Config.s_assignments_xml);
    }

    /// <summary>
    /// Deletes an assignment by its ID.
    /// </summary>
    /// <param name="id">The ID of the assignment to delete.</param>
    /// <exception cref="DalDeletionImpossible">Thrown when the assignment with the specified ID does not exist.</exception>
    public void Delete(int id)
    {
        var Assignments = XMLTools.LoadListFromXMLSerializer<Assignment>(Config.s_assignments_xml);
        if (Assignments.RemoveAll(it => it.Id == id) == 0)
            throw new DalDeletionImpossible($"Assignment with ID={id} does not exist");
        XMLTools.SaveListToXMLSerializer(Assignments, Config.s_assignments_xml);
    }

    /// <summary>
    /// Deletes all assignments.
    /// </summary>
    public void DeleteAll()
    {
        XMLTools.SaveListToXMLSerializer(new List<Assignment>(), Config.s_assignments_xml);
    }

    /// <summary>
    /// Reads an assignment by its ID.
    /// </summary>
    /// <param name="id">The ID of the assignment to read.</param>
    /// <returns>The assignment with the specified ID, or null if not found.</returns>
    public Assignment? Read(int id)
    {
        var Assignments = XMLTools.LoadListFromXMLSerializer<Assignment>(Config.s_assignments_xml);
        return Assignments.FirstOrDefault(it => it.Id == id);
    }

    /// <summary>
    /// Reads an assignment that matches the specified filter.
    /// </summary>
    /// <param name="filter">The filter function to apply.</param>
    /// <returns>The first assignment that matches the filter, or null if not found.</returns>
    public Assignment? Read(Func<Assignment, bool> filter)
    {
        var Assignments = XMLTools.LoadListFromXMLSerializer<Assignment>(Config.s_assignments_xml);
        return Assignments.FirstOrDefault(filter);
    }

    /// <summary>
    /// Reads all assignments that match the specified filter.
    /// </summary>
    /// <param name="filter">The filter function to apply, or null to read all assignments.</param>
    /// <returns>An enumerable of assignments that match the filter.</returns>
    public IEnumerable<Assignment> ReadAll(Func<Assignment, bool>? filter = null)
    {
        var Assignments = XMLTools.LoadListFromXMLSerializer<Assignment>(Config.s_assignments_xml);
        return filter == null ? Assignments : Assignments.Where(filter);
    }

    /// <summary>
    /// Updates an existing assignment.
    /// </summary>
    /// <param name="item">The assignment to update.</param>
    /// <exception cref="DalDoesNotExistException">Thrown when the assignment with the specified ID does not exist.</exception>
    public void Update(Assignment item)
    {
        var Assignments = XMLTools.LoadListFromXMLSerializer<Assignment>(Config.s_assignments_xml);
        if (Assignments.RemoveAll(it => it.Id == item.Id) == 0)
            throw new DalDoesNotExistException($"Assignment with ID={item.Id} does not exist");
        Assignments.Add(item);
        XMLTools.SaveListToXMLSerializer(Assignments, Config.s_assignments_xml);
    }
}
