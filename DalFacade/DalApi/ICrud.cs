
using DO;

namespace DalApi
{

    public interface ICrud<T> where T : class
    {
        /// <summary>
        /// Creates a new entity object in the DAL.
        /// </summary>
        /// <param name="item">The entity object to create.</param>
        void Create(T item); //Creates new entity object in DAL

        /// <summary>
        /// Reads an entity object by its ID.
        /// </summary>
        /// <param name="id">The ID of the entity object to read.</param>
        /// <returns>The entity object with the specified ID, or null if not found.</returns>
        T? Read(int id); //Reads entity object by its ID 

        /// <summary>
        /// Reads an entity object by a filter.
        /// </summary>
        /// <param name="filter">A function to filter the entity objects.</param>
        /// <returns>The first entity object that matches the filter, or null if not found.</returns>
        T? Read(Func<T, bool> filter); //Reads entity object by a filter

        /// <summary>
        /// Reads all entity objects that match a filter.
        /// </summary>
        /// <param name="filter">A function to filter the entity objects. If null, returns all entity objects.</param>
        /// <returns>An enumerable of all entity objects that match the filter.</returns>
        IEnumerable<T> ReadAll(Func<T, bool>? filter = null); //Reads all entity objects that match a filter

        /// <summary>
        /// Updates an existing entity object in the DAL.
        /// </summary>
        /// <param name="item">The entity object to update.</param>
        void Update(T item); //Updates entity object

        /// <summary>
        /// Deletes an entity object by its ID.
        /// </summary>
        /// <param name="id">The ID of the entity object to delete.</param>
        void Delete(int id); //Deletes an object by its Id

        /// <summary>
        /// Deletes all entity objects in the DAL.
        /// </summary>
        void DeleteAll(); //Delete all entity objects
    }
}

