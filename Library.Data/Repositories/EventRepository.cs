using Library.Data.Interfaces;
using Library.Data.Interfaces.Models;
using Library.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Library.Data.Repositories
{
    public class EventRepository : IEventRepository
    {
        private readonly List<LibraryEvent> _events;

        public EventRepository(IEnumerable<ILibraryEvent> initialEvents = null)
        {
            _events = new List<LibraryEvent>();
            if (initialEvents != null)
            {
                foreach (var evt in initialEvents)
                {
                    _events.Add(new LibraryEvent
                    {
                        Id = evt.Id,
                        Type = evt.Type,
                        UserId = evt.UserId,
                        ISBN = evt.ISBN,
                        BookCopyId = evt.BookCopyId,
                        Timestamp = evt.Timestamp,
                        Description = evt.Description
                    });
                }
            }
        }

        public IEnumerable<ILibraryEvent> GetAllEvents() => _events.Cast<ILibraryEvent>().ToList();

        public ILibraryEvent GetEventById(int id) => _events.FirstOrDefault(e => e.Id == id);

        public IEnumerable<ILibraryEvent> GetEventsByUser(int userId) =>
            _events.Where(e => e.UserId == userId).Cast<ILibraryEvent>().ToList();

        public IEnumerable<ILibraryEvent> GetEventsByBook(string isbn) =>
            _events.Where(e => e.ISBN == isbn).Cast<ILibraryEvent>().ToList();

        public void AddEvent(ILibraryEvent libraryEvent)
        {
            if (_events.Any(e => e.Id == libraryEvent.Id))
            {
                throw new ArgumentException($"Event with ID {libraryEvent.Id} already exists.");
            }

            var internalEvent = new LibraryEvent
            {
                Id = libraryEvent.Id,
                Type = libraryEvent.Type,
                UserId = libraryEvent.UserId,
                ISBN = libraryEvent.ISBN,
                BookCopyId = libraryEvent.BookCopyId,
                Timestamp = libraryEvent.Timestamp,
                Description = libraryEvent.Description
            };

            _events.Add(internalEvent);
        }
    }
}