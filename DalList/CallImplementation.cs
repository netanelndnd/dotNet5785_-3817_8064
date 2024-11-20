namespace Dal;
using DalApi;
using DO;
using System.Collections.Generic;

public class CallImplementation : ICall
{
    public void Create(Call item)
    {

        //for entities with auto id
        int id = Config.NextCallId;
        Call copy = item with { Id = id };
        DataSource.Calls.Add(copy);

    }

    public void Delete(int id)
    {
        if (Read(id) is null)
            throw new Exception($"Volunteer with ID={id} not exists");
        else
            DataSource.Calls.Remove(Read(id));
    }

    public void DeleteAll()
    {
        DataSource.Calls.Clear();
    }

    public Call? Read(int id)
    {
        foreach (var item in DataSource.Calls)
        {
            if (item.Id == id)
                return item;
        }
        return null;
    }

    public List<Call> ReadAll()
    {
       return new List<Call>(DataSource.Calls);
    }

    public void Update(Call item)
    {
        Delete(item.Id);
        Create(item);
    }
}
