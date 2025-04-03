using System.Collections.Generic;

namespace LibrarySystem.Data.Interfaces
{
    public interface IDataRepository<T>
    {
        IEnumerable<T> GetAll();
        T GetById(int id);
        void Add(T entity);
        void Update(T entity);
        void Delete(int id);
    }
}