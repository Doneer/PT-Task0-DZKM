using Library.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Data.Interfaces
{
    public interface IEventRepository
    {
        IEnumerable<ILibraryEvent> GetAllEvents();
        ILibraryEvent GetEventById(int id);
        IEnumerable<ILibraryEvent> GetEventsByUser(int userId);
        IEnumerable<ILibraryEvent> GetEventsByBook(string isbn);
        void AddEvent(ILibraryEvent libraryEvent);
    }
}
