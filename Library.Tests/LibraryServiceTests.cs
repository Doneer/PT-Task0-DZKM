using Microsoft.VisualStudio.TestTools.UnitTesting;
using Library.Data.Interfaces;
using Library.Data.Models;
using Library.Data.Repositories;
using Library.Logic.Services;
using System;
using System.Linq;
using Library.Data.DataGenerators;

namespace Library.Tests
{
    [TestClass]
    public class LibraryServiceTests
    {
        [TestMethod]
        public void RandomDataGenerator_GeneratesValidData()
        {
            var dataGenerator = new RandomDataGenerator();

            var dataRepository = dataGenerator.GenerateData();

            Assert.IsNotNull(dataRepository);
            Assert.IsTrue(dataRepository.Users.GetAllUsers().Any());
            Assert.IsTrue(dataRepository.Catalog.GetAllBooks().Any());
            Assert.IsTrue(dataRepository.State.GetAllBookCopies().Any());
            Assert.IsTrue(dataRepository.Events.GetAllEvents().Any());
        }

        [TestMethod]
        public void PredefinedDataGenerator_GeneratesExpectedData()
        {
            var dataGenerator = new PredefinedDataGenerator();

            var dataRepository = dataGenerator.GenerateData();

            Assert.IsNotNull(dataRepository);

            var users = dataRepository.Users.GetAllUsers().ToList();
            Assert.AreEqual(4, users.Count);

            var books = dataRepository.Catalog.GetAllBooks().ToList();
            Assert.AreEqual(5, books.Count);

            var bookCopies = dataRepository.State.GetAllBookCopies().ToList();
            Assert.AreEqual(7, bookCopies.Count);

            var events = dataRepository.Events.GetAllEvents().ToList();
            Assert.AreEqual(10, events.Count);
        }

        [TestMethod]
        public void UserRepository_AddGetUpdateDeleteUser_SuccessfulOperations()
        {
            var userRepository = new UserRepository();
            var user = new User
            {
                Id = 1,
                Name = "Test User",
                Email = "test@example.com",
                PhoneNumber = "555-1234",
                Type = UserType.Patron,
                RegistrationDate = DateTime.Now
            };

            userRepository.AddUser(user);
            var users = userRepository.GetAllUsers();
            Assert.AreEqual(1, users.Count());

            var retrievedUser = userRepository.GetUserById(1);
            Assert.IsNotNull(retrievedUser);
            Assert.AreEqual("Test User", retrievedUser.Name);

            user.Name = "Updated Name";
            userRepository.UpdateUser(user);
            retrievedUser = userRepository.GetUserById(1);
            Assert.AreEqual("Updated Name", retrievedUser.Name);

            userRepository.DeleteUser(1);
            users = userRepository.GetAllUsers();
            Assert.AreEqual(0, users.Count());
        }

        [TestMethod]
        public void CatalogRepository_AddGetUpdateDeleteBook_SuccessfulOperations()
        {
            var catalogRepository = new CatalogRepository();
            var book = new Book
            {
                ISBN = "TEST-ISBN",
                Title = "Test Book",
                Author = "Test Author",
                Publisher = "Test Publisher",
                PublicationYear = 2023,
                Genre = "Test Genre",
                Description = "Test Description"
            };

            catalogRepository.AddBook(book);
            var books = catalogRepository.GetAllBooks();
            Assert.AreEqual(1, books.Count());

            var retrievedBook = catalogRepository.GetBookById("TEST-ISBN");
            Assert.IsNotNull(retrievedBook);
            Assert.AreEqual("Test Book", retrievedBook.Title);

            book.Title = "Updated Title";
            catalogRepository.UpdateBook(book);
            retrievedBook = catalogRepository.GetBookById("TEST-ISBN");
            Assert.AreEqual("Updated Title", retrievedBook.Title);

            catalogRepository.DeleteBook("TEST-ISBN");
            books = catalogRepository.GetAllBooks();
            Assert.AreEqual(0, books.Count());
        }

        [TestMethod]
        public void StateRepository_AddGetUpdateDeleteBookCopy_SuccessfulOperations()
        {
            var stateRepository = new StateRepository();
            var bookCopy = new BookCopy
            {
                Id = 1,
                ISBN = "TEST-ISBN",
                Status = BookStatus.Available,
                AcquisitionDate = DateTime.Now,
                Location = "Test Location"
            };

            stateRepository.AddBookCopy(bookCopy);
            var bookCopies = stateRepository.GetAllBookCopies();
            Assert.AreEqual(1, bookCopies.Count());

            var retrievedBookCopy = stateRepository.GetBookCopyById(1);
            Assert.IsNotNull(retrievedBookCopy);
            Assert.AreEqual("Test Location", retrievedBookCopy.Location);

            bookCopy.Location = "Updated Location";
            stateRepository.UpdateBookCopy(bookCopy);
            retrievedBookCopy = stateRepository.GetBookCopyById(1);
            Assert.AreEqual("Updated Location", retrievedBookCopy.Location);

            stateRepository.DeleteBookCopy(1);
            bookCopies = stateRepository.GetAllBookCopies();
            Assert.AreEqual(0, bookCopies.Count());
        }

        [TestMethod]
        public void EventRepository_AddGetEvents_SuccessfulOperations()
        {
            var eventRepository = new EventRepository();
            var libraryEvent = new LibraryEvent
            {
                Id = 1,
                Type = EventType.BookAdded,
                ISBN = "TEST-ISBN",
                BookCopyId = 1,
                Timestamp = DateTime.Now,
                Description = "Test Event"
            };

            eventRepository.AddEvent(libraryEvent);
            var events = eventRepository.GetAllEvents();
            Assert.AreEqual(1, events.Count());

            var retrievedEvent = eventRepository.GetEventById(1);
            Assert.IsNotNull(retrievedEvent);
            Assert.AreEqual("Test Event", retrievedEvent.Description);

            var eventsByBook = eventRepository.GetEventsByBook("TEST-ISBN");
            Assert.AreEqual(1, eventsByBook.Count());
        }

        [TestMethod]
        public void LibraryService_BorrowAndReturnBook_SuccessfulOperations()
        {
            var userRepository = new UserRepository();
            var catalogRepository = new CatalogRepository();
            var stateRepository = new StateRepository();
            var eventRepository = new EventRepository();
            var dataRepository = new DataRepository(userRepository, catalogRepository, stateRepository, eventRepository);
            var libraryService = new LibraryService(dataRepository);

            var user = new User
            {
                Id = 1,
                Name = "Test User",
                Email = "test@example.com",
                PhoneNumber = "555-1234",
                Type = UserType.Patron,
                RegistrationDate = DateTime.Now
            };
            userRepository.AddUser(user);

            var book = new Book
            {
                ISBN = "TEST-ISBN",
                Title = "Test Book",
                Author = "Test Author",
                Publisher = "Test Publisher",
                PublicationYear = 2023,
                Genre = "Test Genre",
                Description = "Test Description"
            };
            catalogRepository.AddBook(book);

            var bookCopy = new BookCopy
            {
                Id = 1,
                ISBN = "TEST-ISBN",
                Status = BookStatus.Available,
                AcquisitionDate = DateTime.Now,
                Location = "Test Location"
            };
            stateRepository.AddBookCopy(bookCopy);

            var dueDate = DateTime.Now.AddDays(14);
            var borrowResult = libraryService.BorrowBook(1, 1, dueDate);
            Assert.IsTrue(borrowResult);

            var updatedBookCopy = stateRepository.GetBookCopyById(1);
            Assert.AreEqual(BookStatus.CheckedOut, updatedBookCopy.Status);
            Assert.AreEqual(1, updatedBookCopy.CurrentBorrowerId);

            var events = eventRepository.GetAllEvents();
            Assert.AreEqual(1, events.Count());
            Assert.AreEqual(EventType.BookBorrowed, events.First().Type);

            var returnResult = libraryService.ReturnBook(1);
            Assert.IsTrue(returnResult);

            updatedBookCopy = stateRepository.GetBookCopyById(1);
            Assert.AreEqual(BookStatus.Available, updatedBookCopy.Status);
            Assert.IsNull(updatedBookCopy.CurrentBorrowerId);

            events = eventRepository.GetAllEvents();
            Assert.AreEqual(2, events.Count());
            Assert.AreEqual(EventType.BookReturned, events.ElementAt(1).Type);
        }

        [TestMethod]
        public void LibraryService_GetBorrowedBooksByUser_ReturnsCorrectBooks()
        {
            var dataRepository = new PredefinedDataGenerator().GenerateData();
            var libraryService = new LibraryService(dataRepository);

            var borrowedBooks = libraryService.GetBorrowedBooksByUser(1).ToList();

            Assert.AreEqual(1, borrowedBooks.Count);
            Assert.AreEqual(2, borrowedBooks[0].Id);
            Assert.AreEqual("978-0-061-12241-5", borrowedBooks[0].ISBN);
        }

        [TestMethod]
        public void LibraryService_GetUsersWithOverdueBooks_ReturnsCorrectUsers()
        {
            var userRepository = new UserRepository();
            var catalogRepository = new CatalogRepository();
            var stateRepository = new StateRepository();
            var eventRepository = new EventRepository();
            var dataRepository = new DataRepository(userRepository, catalogRepository, stateRepository, eventRepository);
            var libraryService = new LibraryService(dataRepository);

            userRepository.AddUser(new User { Id = 1, Name = "User 1" });
            userRepository.AddUser(new User { Id = 2, Name = "User 2" });

            catalogRepository.AddBook(new Book { ISBN = "TEST-ISBN", Title = "Test Book" });

            stateRepository.AddBookCopy(new BookCopy
            {
                Id = 1,
                ISBN = "TEST-ISBN",
                Status = BookStatus.CheckedOut,
                CurrentBorrowerId = 1,
                DueDate = DateTime.Now.AddDays(-1)
            });

            stateRepository.AddBookCopy(new BookCopy
            {
                Id = 2,
                ISBN = "TEST-ISBN",
                Status = BookStatus.CheckedOut,
                CurrentBorrowerId = 2,
                DueDate = DateTime.Now.AddDays(5) 
            });

            var usersWithOverdueBooks = libraryService.GetUsersWithOverdueBooks().ToList();

            Assert.AreEqual(1, usersWithOverdueBooks.Count);
            Assert.AreEqual(1, usersWithOverdueBooks[0].Id);
        }
    }
}
