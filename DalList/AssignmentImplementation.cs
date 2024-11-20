namespace Dal;
using DalApi;
using DO;
public class AssignmentImplementation : IAssignment
{
    public void Create(Assignment item)
    {
        int id = Config.NextAssigmentId;
        Assignment copy = item with { Id = id };
        DataSource.Assignments.Add(copy);
    }

    public void Delete(int id)
    {
        if (Read(id) is null)
            throw new Exception($"Volunteer with ID={id} not exists");
        else
            DataSource.Assignments.Remove(Read(id));
    }

    public void DeleteAll()
    {
        DataSource.Assignments.Clear();
    }

    public Assignment? Read(int id)
    {
        foreach (var item in DataSource.Assignments)
        {
            if (item.Id == id)
                return item;
        }
        return null;
    }

    public List<Assignment> ReadAll()
    {
       return new List<Assignment>(DataSource.Assignments);

    }

    public void Update(Assignment item)
    {
        Delete(item.Id);
        Create(item);
    }
}
