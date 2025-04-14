using Library.Data.Interfaces;
using Library.Data.Models;
using System.Collections.Generic;
using System.Linq;

namespace Library.Data.Repositories
{
    public class EventRepository : IEventRepository
    {
        private readonly List<LibraryEvent> _events;

        public EventRepository(List<LibraryEvent> initialEvents = null)
        {
            _events = initialEvents ?? new List<LibraryEvent>();
        }

        public IEnumerable<LibraryEvent> GetAllEvents() => _events.ToList();

        public LibraryEvent GetEventById(int id) => _events.FirstOrDefault(e => e.Id == id);

        public IEnumerable<LibraryEvent> GetEventsByUser(int userId) =>
            _events.Where(e => e.UserId == userId).ToList();

        public IEnumerable<LibraryEvent> GetEventsByBook(string isbn) =>
            _events.Where(e => e.ISBN == isbn).ToList();

        public void AddEvent(LibraryEvent libraryEvent)
        {
            if (_events.Any(e => e.Id == libraryEvent.Id))
            {
                throw new ArgumentException($"Event with ID {libraryEvent.Id} already exists.");
            }
            _events.Add(libraryEvent);
        }
    }
}