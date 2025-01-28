namespace Dal;
using DalApi;
using DO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;

internal class VolunteerImplementation : IVolunteer
{
    /// <summary>
    /// Creates a new Volunteer
    /// </summary>
    /// <param name="item">The Volunteer object to create</param>
    /// <exception cref="DalAlreadyExistsException">Thrown when a volunteer with the same ID already exists</exception>
    [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
    public void Create(Volunteer item)
    {
        // For entities with normal id (not auto id)
        if (Read(item.Id) is not null)
            throw new DalAlreadyExistsException($"Volunteer with ID={item.Id} already exists");
        DataSource.Volunteers.Add(item);
    }

    /// <summary>
    /// Deletes a volunteer by their ID
    /// </summary>
    /// <param name="id">The ID of the volunteer to delete</param>
    /// <exception cref="DalDeletionImpossible">Thrown when the volunteer with the specified ID does not exist</exception>
    [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
    public void Delete(int id)
    {
        var volunteer = Read(id);
        if (volunteer is null)
            throw new DalDeletionImpossible($"Volunteer with ID={id} does not exist");
        else
            DataSource.Volunteers.Remove(volunteer);
    }

    /// <summary>
    /// Deletes all volunteers
    /// </summary>
    [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
    public void DeleteAll()
    {
        DataSource.Volunteers.Clear();
    }

    /// <summary>
    /// Reads a volunteer by their ID
    /// </summary>
    /// <param name="id">The ID of the volunteer to read</param>
    /// <returns>The volunteer with the specified ID, or null if not found</returns>
    [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
    public Volunteer? Read(int id)
    {
        return DataSource.Volunteers.FirstOrDefault(item => item.Id == id);
    }

    /// <summary>
    /// Reads a volunteer based on a filter condition
    /// </summary>
    /// <param name="filter">The filter condition to apply</param>
    /// <returns>The volunteer that matches the filter condition, or null if not found</returns>
    [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
    public Volunteer? Read(Func<Volunteer, bool> filter)
    {
        return DataSource.Volunteers.FirstOrDefault(filter);
    }

    /// <summary>
    /// Reads all volunteers
    /// </summary>
    /// <param name="filter">Optional filter condition to apply</param>
    /// <returns>A list of all volunteers, optionally filtered</returns>
    [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
    public IEnumerable<Volunteer> ReadAll(Func<Volunteer, bool>? filter = null)
    {
        return filter != null
               ? DataSource.Volunteers.Where(filter)
               : DataSource.Volunteers;
    }

    /// <summary>
    /// Updates an existing Volunteer
    /// </summary>
    /// <param name="item">The updated Volunteer object</param>
    /// <exception cref="DalDoesNotExistException">Thrown when the volunteer with the specified ID does not exist</exception>
    [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
    public void Update(Volunteer item)
    {
        var existingVolunteer = Read(item.Id);
        if (existingVolunteer is null)
            throw new DalDoesNotExistException($"Volunteer with ID={item.Id} does not exist");

        Delete(existingVolunteer.Id);
        DataSource.Volunteers.Add(item);
    }
}
