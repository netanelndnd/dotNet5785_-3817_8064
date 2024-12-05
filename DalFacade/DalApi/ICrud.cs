
using DO;

namespace DalApi
{

        public interface ICrud<T> where T : class
    {
        void Create(T item); //Creates new entity object in DAL
        T? Read(int id); //Reads entity object by its ID 
        // המתודה תקבל מצביע לפונקציה בוליאנית, delegate מסוג Func, שתפעל על אחד מאיברי הרשימה מסוג T 
        // ותחזיר את רשימת כל האובייקטים ברשימה שהפונקציה מחזירה עליהם True. במידה ולא ישלח מצביע, תוחזר כל הרשימה כמו בשלב הקודם (שלב 1).
        T? Read(Func<T, bool> filter); // stage 2
        IEnumerable<T> ReadAll(Func<T, bool>? filter = null); // stage 2
        void Update(T item); //Updates entity object
        void Delete(int id); //Deletes an object by its Id
        void DeleteAll(); //Delete all entity objects
    }
}

