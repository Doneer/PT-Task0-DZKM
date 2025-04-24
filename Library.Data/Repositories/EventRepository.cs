using System;
using System.Collections.Generic;
using System.Linq;
using Library.Data.Interfaces;
using Library.Data.Models;

namespace Library.Data.Repositories
{
    public class EventRepository : IEventRepository
    {
        private readonly List<LibraryEvent> _events;
        private readonly IModelFactory _modelFactory;

        public EventRepository(List<LibraryEvent> initialEvents = null)
        {
            _events = initialEvents ?? new List<LibraryEvent>();
            _modelFactory = new Factories.ModelFactory();
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

            if (libraryEvent is LibraryEvent concreteEvent)
            {
                _events.Add(concreteEvent);
            }
            else
            {
                _events.Add(new LibraryEvent
                {
                    Id = libraryEvent.Id,
                    Type = libraryEvent.Type,
                    UserId = libraryEvent.UserId,
                    ISBN = libraryEvent.ISBN,
                    BookCopyId = libraryEvent.BookCopyId,
                    Timestamp = libraryEvent.Timestamp,
                    Description = libraryEvent.Description
                });
            }
        }
    }
}