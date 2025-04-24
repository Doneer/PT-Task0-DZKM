using Library.Data.Factories;
using Library.Data.Interfaces;
using Library.Data.Interfaces.Models;
using Library.Data.Models;
using Library.Data.Repositories;
using System;
using System.Collections.Generic;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;

namespace Library.Tests.DataGenerators
{
    public class PredefinedDataGenerator
    {
        public IDataRepository GenerateData()
        {
            var users = GeneratePredefinedUsers();
            var books = GeneratePredefinedBooks();
            var bookCopies = GeneratePredefinedBookCopies();
            var events = GeneratePredefinedEvents();

            return new DataRepository(
                new UserRepository(users),
                new CatalogRepository(books),
                new StateRepository(bookCopies),
                new EventRepository(events)
            );
        }

        private List<IUser> GeneratePredefinedUsers()
        {
            return new List<IUser>
            {
                UserFactory.CreateUser(1, "John Doe", "john@example.com", "555-1234", UserType.Patron, new DateTime(2023, 1, 15)),
                UserFactory.CreateUser(2, "Jane Smith", "jane@example.com", "555-5678", UserType.Patron, new DateTime(2023, 2, 20)),
                UserFactory.CreateUser(3, "Michael Johnson", "michael@example.com", "555-9012", UserType.Librarian, new DateTime(2022, 11, 10)),
                UserFactory.CreateUser(4, "Emily Brown", "emily@example.com", "555-3456", UserType.Administrator, new DateTime(2022, 10, 5))
            };
        }

        private List<IBook> GeneratePredefinedBooks()
        {
            return new List<IBook>
            {
                BookFactory.CreateBook("978-0-061-12241-5", "To Kill a Mockingbird", "Harper Lee", "HarperCollins", 1960, "Fiction", "Classic novel about racial injustice"),
                BookFactory.CreateBook("978-0-743-27325-1", "The Great Gatsby", "F. Scott Fitzgerald", "Scribner", 1925, "Fiction", "Classic novel about the American Dream"),
                BookFactory.CreateBook("978-0-141-03614-4", "1984", "George Orwell", "Penguin", 1949, "Science Fiction", "Dystopian novel about totalitarianism"),
                BookFactory.CreateBook("978-0-316-76948-0", "The Catcher in the Rye", "J.D. Salinger", "Little, Brown", 1951, "Fiction", "Novel about teenage alienation"),
                BookFactory.CreateBook("978-0-060-85040-2", "The Hobbit", "J.R.R. Tolkien", "HarperCollins", 1937, "Fantasy", "Fantasy novel about a hobbit's adventure")
            };
        }

        private List<IBookCopy> GeneratePredefinedBookCopies()
        {
            return new List<IBookCopy>
            {
                BookCopyFactory.CreateBookCopy(1, "978-0-061-12241-5", BookStatus.Available, new DateTime(2022, 5, 12), "Shelf A1"),
                BookCopyFactory.CreateBookCopy(2, "978-0-061-12241-5", BookStatus.CheckedOut, new DateTime(2022, 5, 12), "Shelf A1", 1, DateTime.Now.AddDays(7)),
                BookCopyFactory.CreateBookCopy(3, "978-0-743-27325-1", BookStatus.Available, new DateTime(2022, 6, 15), "Shelf A2"),
                BookCopyFactory.CreateBookCopy(4, "978-0-141-03614-4", BookStatus.CheckedOut, new DateTime(2022, 7, 20), "Shelf B1", 2, DateTime.Now.AddDays(3)),
                BookCopyFactory.CreateBookCopy(5, "978-0-316-76948-0", BookStatus.UnderMaintenance, new DateTime(2022, 8, 5), "Shelf B2"),
                BookCopyFactory.CreateBookCopy(6, "978-0-060-85040-2", BookStatus.Available, new DateTime(2022, 9, 10), "Shelf C1"),
                BookCopyFactory.CreateBookCopy(7, "978-0-060-85040-2", BookStatus.Lost, new DateTime(2022, 9, 10), "Unknown")
            };
        }

        private List<ILibraryEvent> GeneratePredefinedEvents()
        {
            return new List<ILibraryEvent>
            {
                LibraryEventFactory.CreateEvent(1, EventType.BookAdded, new DateTime(2022, 5, 12), "Added new copy of To Kill a Mockingbird", null, "978-0-061-12241-5", 1),
                LibraryEventFactory.CreateEvent(2, EventType.BookAdded, new DateTime(2022, 5, 12), "Added new copy of To Kill a Mockingbird", null, "978-0-061-12241-5", 2),
                LibraryEventFactory.CreateEvent(3, EventType.BookBorrowed, DateTime.Now.AddDays(-7), "John Doe borrowed To Kill a Mockingbird", 1, "978-0-061-12241-5", 2),
                LibraryEventFactory.CreateEvent(4, EventType.BookAdded, new DateTime(2022, 6, 15), "Added new copy of The Great Gatsby", null, "978-0-743-27325-1", 3),
                LibraryEventFactory.CreateEvent(5, EventType.BookAdded, new DateTime(2022, 7, 20), "Added new copy of 1984", null, "978-0-141-03614-4", 4),
                LibraryEventFactory.CreateEvent(6, EventType.BookBorrowed, DateTime.Now.AddDays(-10), "Jane Smith borrowed 1984", 2, "978-0-141-03614-4", 4),
                LibraryEventFactory.CreateEvent(7, EventType.BookAdded, new DateTime(2022, 8, 5), "Added new copy of The Catcher in the Rye", null, "978-0-316-76948-0", 5),
                LibraryEventFactory.CreateEvent(8, EventType.BookAdded, new DateTime(2022, 9, 10), "Added new copy of The Hobbit", null, "978-0-060-85040-2", 6),
                LibraryEventFactory.CreateEvent(9, EventType.BookAdded, new DateTime(2022, 9, 10), "Added new copy of The Hobbit", null, "978-0-060-85040-2", 7),
                LibraryEventFactory.CreateEvent(10, EventType.BookLost, new DateTime(2023, 1, 5), "Copy of The Hobbit marked as lost", null, "978-0-060-85040-2", 7)
            };
        }
    }
}