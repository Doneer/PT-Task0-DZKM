using LibrarySystem.Data.Models;

namespace LibrarySystem.Data.Interfaces
{
    public interface IBookRepository : IDataRepository<Book>
    {
        IEnumerable<Book> GetByAuthor(string author);
        IEnumerable<Book> GetByCategory(string category);
    }
}