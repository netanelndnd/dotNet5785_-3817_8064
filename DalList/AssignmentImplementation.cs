namespace Dal;

using System.Runtime.CompilerServices;
using DalApi;
using DO;


/// <summary>
/// Implementation of the IAssignment interface for managing assignments in the DAL.
/// </summary>
internal class AssignmentImplementation : IAssignment
{
    /// <summary>
    /// Create a new assignment.
    /// </summary>
    /// <param name="item">The assignment item to create.</param>
    /// <exception cref="DalAlreadyExistsException">Thrown when an assignment with the same ID already exists.</exception>
    [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
    public void Create(Assignment item)
    {
        if (DataSource.Assignments.Any(a => a.Id == item.Id))
        {
            throw new DalAlreadyExistsException($"Assignment with ID={item.Id} already exists");
        }
        int id = Config.NextAssignmentId;
        Assignment copy = item with { Id = id };
        DataSource.Assignments.Add(copy);
    }

    /// <summary>
    /// Delete an assignment by ID.
    /// </summary>
    /// <param name="id">The ID of the assignment to delete.</param>
    /// <exception cref="DalDeletionImpossible">Thrown when the assignment with the specified ID does not exist.</exception>
    [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
    public void Delete(int id)
    {
        var assignment = DataSource.Assignments.FirstOrDefault(item => item.Id == id);
        if (assignment is null)
            throw new DalDeletionImpossible($"Assignment with ID={id} does not exist");
        else
            DataSource.Assignments.Remove(assignment);
    }

    /// <summary>
    /// Delete all assignments.
    /// </summary>
    [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
    public void DeleteAll()
    {
        DataSource.Assignments.Clear();
    }

    /// <summary>
    /// Read an assignment by ID.
    /// </summary>
    /// <param name="id">The ID of the assignment to read.</param>
    /// <returns>The assignment with the specified ID, or null if not found.</returns>
    [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
    public Assignment? Read(int id)
    {
        return DataSource.Assignments.FirstOrDefault(item => item.Id == id);
    }

    /// <summary>
    /// Read an assignment based on a filter condition.
    /// </summary>
    /// <param name="filter">A function to filter the assignments.</param>
    /// <returns>The first assignment that matches the filter, or null if not found.</returns>
    [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
    public Assignment? Read(Func<Assignment, bool> filter)
    {
        return DataSource.Assignments.FirstOrDefault(filter);
    }

    /// <summary>
    /// Read all assignments.
    /// </summary>
    /// <param name="filter">A function to filter the assignments. If null, returns all assignments.</param>
    /// <returns>An enumerable of all assignments that match the filter.</returns>
    [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
    public IEnumerable<Assignment> ReadAll(Func<Assignment, bool>? filter = null)
    {
        return filter != null
            ? DataSource.Assignments.Where(filter)
            : DataSource.Assignments;
    }

    /// <summary>
    /// Update an existing assignment.
    /// </summary>
    /// <param name="item">The assignment item to update.</param>
    /// <exception cref="DalDoesNotExistException">Thrown when the assignment with the specified ID does not exist.</exception>
    [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
    public void Update(Assignment item)
    {
        var existingAssignment = Read(item.Id);
        if (existingAssignment is null)
            throw new DalDoesNotExistException($"Assignment with ID={item.Id} does not exist");
        Delete(existingAssignment.Id);
        DataSource.Assignments.Add(item);
    }
}


