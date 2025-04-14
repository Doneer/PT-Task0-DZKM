using Library.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Data.Interfaces
{
    internal interface IEventRepository
    {
        IEnumerable<LibraryEvent> GetAllEvents();
        LibraryEvent GetEventById(int id);
        IEnumerable<LibraryEvent> GetEventsByUser(int userId);
        IEnumerable<LibraryEvent> GetEventsByBook(string isbn);
        void AddEvent(LibraryEvent libraryEvent);
    }
}
