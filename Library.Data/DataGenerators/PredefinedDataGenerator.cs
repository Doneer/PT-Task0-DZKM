using Library.Data.Interfaces;
using Library.Data.Models;
using Library.Data.Repositories;
using System;
using System.Collections.Generic;

namespace Library.Data.DataGenerators
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

        private List<User> GeneratePredefinedUsers()
        {
            return new List<User>
            {
                new User { Id = 1, Name = "John Doe", Email = "john@example.com", PhoneNumber = "555-1234", Type = UserType.Patron, RegistrationDate = new DateTime(2023, 1, 15) },
                new User { Id = 2, Name = "Jane Smith", Email = "jane@example.com", PhoneNumber = "555-5678", Type = UserType.Patron, RegistrationDate = new DateTime(2023, 2, 20) },
                new User { Id = 3, Name = "Michael Johnson", Email = "michael@example.com", PhoneNumber = "555-9012", Type = UserType.Librarian, RegistrationDate = new DateTime(2022, 11, 10) },
                new User { Id = 4, Name = "Emily Brown", Email = "emily@example.com", PhoneNumber = "555-3456", Type = UserType.Administrator, RegistrationDate = new DateTime(2022, 10, 5) }
            };
        }

        private List<Book> GeneratePredefinedBooks()
        {
            return new List<Book>
            {
                new Book { ISBN = "978-0-061-12241-5", Title = "To Kill a Mockingbird", Author = "Harper Lee", Publisher = "HarperCollins", PublicationYear = 1960, Genre = "Fiction", Description = "Classic novel about racial injustice" },
                new Book { ISBN = "978-0-743-27325-1", Title = "The Great Gatsby", Author = "F. Scott Fitzgerald", Publisher = "Scribner", PublicationYear = 1925, Genre = "Fiction", Description = "Classic novel about the American Dream" },
                new Book { ISBN = "978-0-141-03614-4", Title = "1984", Author = "George Orwell", Publisher = "Penguin", PublicationYear = 1949, Genre = "Science Fiction", Description = "Dystopian novel about totalitarianism" },
                new Book { ISBN = "978-0-316-76948-0", Title = "The Catcher in the Rye", Author = "J.D. Salinger", Publisher = "Little, Brown", PublicationYear = 1951, Genre = "Fiction", Description = "Novel about teenage alienation" },
                new Book { ISBN = "978-0-060-85040-2", Title = "The Hobbit", Author = "J.R.R. Tolkien", Publisher = "HarperCollins", PublicationYear = 1937, Genre = "Fantasy", Description = "Fantasy novel about a hobbit's adventure" }
            };
        }

        private List<BookCopy> GeneratePredefinedBookCopies()
        {
            return new List<BookCopy>
            {
                new BookCopy { Id = 1, ISBN = "978-0-061-12241-5", Status = BookStatus.Available, AcquisitionDate = new DateTime(2022, 5, 12), Location = "Shelf A1" },
                new BookCopy { Id = 2, ISBN = "978-0-061-12241-5", Status = BookStatus.CheckedOut, AcquisitionDate = new DateTime(2022, 5, 12), Location = "Shelf A1", CurrentBorrowerId = 1, DueDate = DateTime.Now.AddDays(7) },
                new BookCopy { Id = 3, ISBN = "978-0-743-27325-1", Status = BookStatus.Available, AcquisitionDate = new DateTime(2022, 6, 15), Location = "Shelf A2" },
                new BookCopy { Id = 4, ISBN = "978-0-141-03614-4", Status = BookStatus.CheckedOut, AcquisitionDate = new DateTime(2022, 7, 20), Location = "Shelf B1", CurrentBorrowerId = 2, DueDate = DateTime.Now.AddDays(3) },
                new BookCopy { Id = 5, ISBN = "978-0-316-76948-0", Status = BookStatus.UnderMaintenance, AcquisitionDate = new DateTime(2022, 8, 5), Location = "Shelf B2" },
                new BookCopy { Id = 6, ISBN = "978-0-060-85040-2", Status = BookStatus.Available, AcquisitionDate = new DateTime(2022, 9, 10), Location = "Shelf C1" },
                new BookCopy { Id = 7, ISBN = "978-0-060-85040-2", Status = BookStatus.Lost, AcquisitionDate = new DateTime(2022, 9, 10), Location = "Unknown" }
            };
        }

        private List<LibraryEvent> GeneratePredefinedEvents()
        {
            return new List<LibraryEvent>
            {
                new LibraryEvent { Id = 1, Type = EventType.BookAdded, ISBN = "978-0-061-12241-5", BookCopyId = 1, Timestamp = new DateTime(2022, 5, 12), Description = "Added new copy of To Kill a Mockingbird" },
                new LibraryEvent { Id = 2, Type = EventType.BookAdded, ISBN = "978-0-061-12241-5", BookCopyId = 2, Timestamp = new DateTime(2022, 5, 12), Description = "Added new copy of To Kill a Mockingbird" },
                new LibraryEvent { Id = 3, Type = EventType.BookBorrowed, UserId = 1, ISBN = "978-0-061-12241-5", BookCopyId = 2, Timestamp = DateTime.Now.AddDays(-7), Description = "John Doe borrowed To Kill a Mockingbird" },
                new LibraryEvent { Id = 4, Type = EventType.BookAdded, ISBN = "978-0-743-27325-1", BookCopyId = 3, Timestamp = new DateTime(2022, 6, 15), Description = "Added new copy of The Great Gatsby" },
                new LibraryEvent { Id = 5, Type = EventType.BookAdded, ISBN = "978-0-141-03614-4", BookCopyId = 4, Timestamp = new DateTime(2022, 7, 20), Description = "Added new copy of 1984" },
                new LibraryEvent { Id = 6, Type = EventType.BookBorrowed, UserId = 2, ISBN = "978-0-141-03614-4", BookCopyId = 4, Timestamp = DateTime.Now.AddDays(-10), Description = "Jane Smith borrowed 1984" },
                new LibraryEvent { Id = 7, Type = EventType.BookAdded, ISBN = "978-0-316-76948-0", BookCopyId = 5, Timestamp = new DateTime(2022, 8, 5), Description = "Added new copy of The Catcher in the Rye" },
                new LibraryEvent { Id = 8, Type = EventType.BookAdded, ISBN = "978-0-060-85040-2", BookCopyId = 6, Timestamp = new DateTime(2022, 9, 10), Description = "Added new copy of The Hobbit" },
                new LibraryEvent { Id = 9, Type = EventType.BookAdded, ISBN = "978-0-060-85040-2", BookCopyId = 7, Timestamp = new DateTime(2022, 9, 10), Description = "Added new copy of The Hobbit" },
                new LibraryEvent { Id = 10, Type = EventType.BookLost, ISBN = "978-0-060-85040-2", BookCopyId = 7, Timestamp = new DateTime(2023, 1, 5), Description = "Copy of The Hobbit marked as lost" }
            };
        }
    }
}