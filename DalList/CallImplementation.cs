namespace Dal;
using DalApi;
using DO;
using System.Collections.Generic;

internal class CallImplementation : ICall
{
    /// <summary>
    /// Create new Call
    /// </summary>
    /// <param name="item"></param>
    public void Create(Call item)
    {
        //for entities with auto id
        int id = Config.NextCallId;
        Call copy = item with { Id = id };
        DataSource.Calls.Append(copy);
    }

    /// <summary>
    /// Delete a call by id
    /// </summary>
    /// <param name="id">The ID of the call to delete</param>
    /// <exception cref="Exception">Thrown when the call with the specified ID does not exist</exception>
    public void Delete(int id)
    {
        var call = DataSource.Calls.FirstOrDefault(c => c.Id == id);
        if (call is null)
            throw new Exception($"Call with ID={id} does not exist");
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
    /// Read all calls
    /// </summary>
    /// <returns>List of all calls</returns>
    public List<Call> ReadAll()
    {
        return new List<Call>(DataSource.Calls);
    }

    /// <summary>
    /// Update an existing call
    /// </summary>
    /// <param name="item">The call item to update</param>
    public void Update(Call item)
    { 
        var existingCall = DataSource.Calls.FirstOrDefault(c => c.Id == item.Id);
        if (existingCall is null)
        {
            throw new Exception($"Call with ID={item.Id} does not exist");
        }
        DataSource.Calls.Remove(existingCall);
        DataSource.Calls.Select(c => c.Id == item.Id ? item : c);
    }
}
