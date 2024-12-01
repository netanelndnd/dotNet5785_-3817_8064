namespace Dal;
using DalApi;
using DO;


internal class AssignmentImplementation : IAssignment
{
    /// <summary>
    /// Create a new assignment
    /// </summary>
    /// <param name="item">The assignment item to create</param>
    public void Create(Assignment item)
    {
        int id = Config.NextAssignmentId;
        Assignment copy = item with { Id = id };
        DataSource.Assignments.Append(copy);
    }
    /// <summary>
    /// Delete an assignment by id
    /// </summary>
    /// <param name="id">The ID of the assignment to delete</param>
    /// <exception cref="Exception">Thrown when the assignment with the specified ID does not exist</exception>
    public void Delete(int id)
    {
        var assignment = DataSource.Assignments.FirstOrDefault(item => item.Id == id);
        if (assignment is null)
            throw new Exception($"Assignment with ID={id} does not exist");
        else
            DataSource.Assignments.Remove(assignment);
    }

    /// <summary>
    /// Delete all assignments
    /// </summary>
    public void DeleteAll()
    {
        DataSource.Assignments.Clear();
    }

    /// <summary>
    /// Read assignment by id
    /// </summary>
    /// <param name="id">The ID of the assignment to read</param>
    /// <returns>The assignment with the specified ID, or null if not found</returns>
    public Assignment? Read(int id)
    {
        return DataSource.Assignments.FirstOrDefault(item => item.Id == id);
    }

    /// <summary>
    /// Read all assignments
    /// </summary>
    /// <returns>A list of all assignments</returns>
    public List<Assignment> ReadAll()
    {
        return new List<Assignment>(DataSource.Assignments);
    }

    /// <summary>
    /// Update an assignment
    /// </summary>
    /// <param name="item">The assignment item to update</param>
    public void Update(Assignment item)
    {
        var existingAssignment = Read(item.Id);
        if (existingAssignment is null)
            throw new Exception($"Assignment with ID={item.Id} does not exist");
        DataSource.Assignments.Remove(existingAssignment);
        DataSource.Assignments.Add(item);
    }
}
