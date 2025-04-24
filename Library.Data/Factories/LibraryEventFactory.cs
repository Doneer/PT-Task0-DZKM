using Library.Data.Interfaces.Models;
using Library.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Data.Factories
{
    public static class LibraryEventFactory
    {
        public static ILibraryEvent CreateEvent(int id, EventType type, DateTime timestamp, string description, int? userId = null, string isbn = null, int? bookCopyId = null)
        {
            return new LibraryEvent
            {
                Id = id,
                Type = type,
                UserId = userId,
                ISBN = isbn,
                BookCopyId = bookCopyId,
                Timestamp = timestamp,
                Description = description
            };
        }
    }
}
