using Microsoft.EntityFrameworkCore;
using Library.Data.Database;
using Library.Data.Factories;
using Library.Data.Interfaces;
using Library.Data.Interfaces.Models;

namespace Library.Data.Repositories
{
    public class EfEventRepository : IEventRepository
    {
        private readonly DbContextOptions<LibraryDbContext> _options;

        public EfEventRepository(DbContextOptions<LibraryDbContext> options)
        {
            _options = options;
        }

        public IEnumerable<ILibraryEvent> GetAllEvents()
        {
            using var context = new LibraryDbContext(_options);

            var events = from e in context.LibraryEvents
                         orderby e.Timestamp descending
                         select e;

            return events.ToList().Select(MapToLibraryEvent);
        }

        public ILibraryEvent GetEventById(int id)
        {
            using var context = new LibraryDbContext(_options);

            var libraryEvent = context.LibraryEvents
                .Where(e => e.Id == id)
                .FirstOrDefault();

            return libraryEvent != null ? MapToLibraryEvent(libraryEvent) : null;
        }

        public IEnumerable<ILibraryEvent> GetEventsByUser(int userId)
        {
            using var context = new LibraryDbContext(_options);

            var userEvents = context.LibraryEvents
                .Where(e => e.UserId == userId)
                .OrderByDescending(e => e.Timestamp)
                .ToList();

            return userEvents.Select(MapToLibraryEvent);
        }

        public IEnumerable<ILibraryEvent> GetEventsByBook(string isbn)
        {
            using var context = new LibraryDbContext(_options);

            var bookEvents = context.LibraryEvents
                .Where(e => e.ISBN == isbn)
                .OrderByDescending(e => e.Timestamp)
                .ToList();

            return bookEvents.Select(MapToLibraryEvent);
        }

        public void AddEvent(ILibraryEvent libraryEvent)
        {
            using var context = new LibraryDbContext(_options);

            if (context.LibraryEvents.Any(e => e.Id == libraryEvent.Id))
            {
                throw new ArgumentException($"Event with ID {libraryEvent.Id} already exists.");
            }

            var eventEntity = new LibraryEventEntity
            {
                Type = (int)libraryEvent.Type,
                UserId = libraryEvent.UserId,
                ISBN = libraryEvent.ISBN,
                BookCopyId = libraryEvent.BookCopyId,
                Timestamp = libraryEvent.Timestamp,
                Description = libraryEvent.Description
            };

            context.LibraryEvents.Add(eventEntity);
            context.SaveChanges();
        }

        private static ILibraryEvent MapToLibraryEvent(LibraryEventEntity entity)
        {
            return LibraryEventFactory.CreateEvent(
                entity.Id,
                (EventType)entity.Type,
                entity.Timestamp,
                entity.Description,
                entity.UserId,
                entity.ISBN,
                entity.BookCopyId
            );
        }
    }
}