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

        if (DataSource.Volunteers.Contains(item))
            throw new Exception("This volunteer exists in the database");
        else
            DataSource.Volunteers.Add(item);
    }

    public void Delete(int id)
    {
       
    }

    public void DeleteAll()
    {
        throw new NotImplementedException();
    }

    public Volunteer? Read(int id)
    {
        throw new NotImplementedException();
    }

    public List<Volunteer> ReadAll()
    {
        throw new NotImplementedException();
    }

    public void Update(Volunteer item)
    {
        throw new NotImplementedException();
    }
}
