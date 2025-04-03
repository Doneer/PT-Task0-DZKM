using LibrarySystem.Data.Models;
using System.Collections.Generic;

namespace LibrarySystem.Data.Interfaces
{
    public interface IEventRepository : IDataRepository<LibraryEvent>
    {
        IEnumerable<LibraryEvent> GetByUserId(int userId);
        IEnumerable<LibraryEvent> GetByBookId(int bookId);
    }
}