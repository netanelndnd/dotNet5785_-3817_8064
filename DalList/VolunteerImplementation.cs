namespace Dal;
using DalApi;
using DO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Security.Cryptography.X509Certificates;

internal class VolunteerImplementation : IVolunteer
{
    /// <summary>
    /// Create new Volunteer
    /// </summary>
    /// <param name="item"></param>
    /// <exception cref="Exception"></exception>
    public void Create(Volunteer item)
    {
        //for entities with normal id (not auto id)
        if (Read(item.Id) is not null)
            throw new Exception($"Volunteer with ID={item.Id} already exists");
        DataSource.Volunteers.Append(item);
    }

    /// <summary>
    /// Deletes a volunteer by their ID
    /// </summary>
    /// <param name="id">The ID of the volunteer to delete</param>
    /// <exception cref="Exception">Thrown when the volunteer with the specified ID does not exist</exception>
    public void Delete(int id)
    {
        var volunteer = Read(id);
        if (volunteer is null)
            throw new Exception($"Volunteer with ID={id} does not exist");
        else
            DataSource.Volunteers.Remove(volunteer);
    }

    /// <summary>
    /// Deletes all volunteers
    /// </summary>
    public void DeleteAll()
    {
        DataSource.Volunteers.Clear();
    }

    /// <summary>
    /// Reads a volunteer by their ID
    /// </summary>
    /// <param name="id">The ID of the volunteer to read</param>
    /// <returns>The volunteer with the specified ID, or null if not found</returns>
    public Volunteer? Read(int id)
    {
        foreach (var item in DataSource.Volunteers)
        {
            if (item.Id == id)
                return item;
        }
        return null;
    }

    /// <summary>
    /// Reads all volunteers
    /// </summary>
    /// <returns>A list of all volunteers</returns>
    public List<Volunteer> ReadAll()
    {
        return new List<Volunteer>(DataSource.Volunteers);
    }

    /// <summary>
    /// Updates an existing Volunteer
    /// </summary>
    /// <param name="item">The updated Volunteer object</param>
    public void Update(Volunteer item)
    {
        var existingVolunteer = Read(item.Id);
        if (existingVolunteer is null)
            throw new Exception($"Volunteer with ID={item.Id} does not exist");

        DataSource.Volunteers.Remove(existingVolunteer);
        DataSource.Volunteers.Append(item);
    }
}
