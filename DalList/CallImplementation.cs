namespace Dal;
using DalApi;
using DO;
using System.Collections.Generic;
using System.Drawing;

internal class CallImplementation : ICall
{
    /// <summary>
    /// Create new Call
    /// </summary>
    /// <param name="item">The call item to create</param>
    /// <exception cref="DalAlreadyExistsException">Thrown when a call with the same ID already exists</exception>
    public void Create(Call item)
    {
        // Check if a call with the same ID already exists
        if (DataSource.Calls.Any(c => c.Id == item.Id))
        {
            throw new DalAlreadyExistsException($"Call with ID={item.Id} already exists");
        }

        // For entities with auto id
        int id = Config.NextCallId;
        Call copy = item with { Id = id };
        DataSource.Calls.Append(copy);
    }

    /// <summary>
    /// Delete a call by id
    /// </summary>
    /// <param name="id">The ID of the call to delete</param>
    /// <exception cref="DalDeletionImpossible">Thrown when the call with the specified ID does not exist</exception>
    public void Delete(int id)
    {
        var call = DataSource.Calls.FirstOrDefault(c => c.Id == id);
        if (call is null)
            throw new DalDeletionImpossible($"Call with ID={id} does not exist");
        else
            DataSource.Calls.Remove(call);
    }

    /// <summary>
    /// Delete all calls
    /// </summary>
    public void DeleteAll()
    {
        DataSource.Calls.Clear();
    }

    /// <summary>
    /// Read call by id
    /// </summary>
    /// <param name="id">The ID of the call to read</param>
    /// <returns>The call with the specified ID, or null if not found</returns>
    public Call? Read(int id)
    {
        return DataSource.Calls.FirstOrDefault(item => item.Id == id);
    }

    /// <summary>
    /// Read a call based on a filter condition
    /// </summary>
    /// <param name="filter">The filter condition to apply</param>
    /// <returns>The call that matches the filter condition, or null if not found</returns>
    public Call? Read(Func<Call, bool> filter)
    {
        return DataSource.Calls.FirstOrDefault(filter);
    }

    /// <summary>
    /// Read all calls
    /// </summary>
    /// <param name="filter">Optional filter condition to apply</param>
    /// <returns>List of all calls, filtered if a filter condition is provided</returns>
    public IEnumerable<Call> ReadAll(Func<Call, bool>? filter = null)
    {
        return filter != null
                ? DataSource.Calls.Where(filter)
                : DataSource.Calls;
    }

    /// <summary>
    /// Update an existing call
    /// </summary>
    /// <param name="item">The call item to update</param>
    /// <exception cref="DalDoesNotExistException">Thrown when the call with the specified ID does not exist</exception>
    public void Update(Call item)
    {
        var existingCall = DataSource.Calls.FirstOrDefault(c => c.Id == item.Id);
        if (existingCall is null)
        {
            throw new DalDoesNotExistException($"Call with ID={item.Id} does not exist");
        }
        DataSource.Calls.Remove(existingCall);
        DataSource.Calls.Append(item);
    }
}


