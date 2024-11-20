namespace Dal;
using DalApi;
using DO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Security.Cryptography.X509Certificates;

public class VolunteerImplementation : IVolunteer
{
    public void Create(Volunteer item)
    {

        //for entities with normal id (not auto id)
        if (Read(item.Id) is not null)
            throw new Exception($"Volunteer with ID={item.Id} already exists");
        DataSource.Volunteers.Add(item);

    }

    public void Delete(int id)
    {
        if (Read(id) is null)
            throw new Exception($"Volunteer with ID={id} not exists");
        else
            DataSource.Volunteers.Remove(Read(id));
    }

    public void DeleteAll()
    {
        DataSource.Volunteers.Clear();
    }

    public Volunteer? Read(int id)
    {
        foreach (var item in DataSource.Volunteers)
        {
            if (item.Id == id)
                return item;
        }
        return null;
    }

    public List<Volunteer> ReadAll()
    {
        return new List<Volunteer>(DataSource.Volunteers);
    }

    public void Update(Volunteer item)
    {
        Delete(item.Id);
        Create(item);
    }
}
