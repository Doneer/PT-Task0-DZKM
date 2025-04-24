using Microsoft.VisualStudio.TestTools.UnitTesting;
using Library.Data.Interfaces;
using Library.Data.Interfaces.Models;
using Library.Data.Models;
using Library.Data.Repositories;
using Library.Data.Factories;
using Library.Logic.Services;
using System;
using System.Linq;
using Library.Tests.DataGenerators;

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
            var user = UserFactory.CreateUser(
                1,
                "Test User",
                "test@example.com",
                "555-1234",
                UserType.Patron,
                DateTime.Now
            );

            userRepository.AddUser(user);
            var users = userRepository.GetAllUsers();
            Assert.AreEqual(1, users.Count());

            var retrievedUser = userRepository.GetUserById(1);
            Assert.IsNotNull(retrievedUser);
            Assert.AreEqual("Test User", retrievedUser.Name);

            var updatedUser = UserFactory.CreateUser(
                1,
                "Updated Name",
                "test@example.com",
                "555-1234",
                UserType.Patron,
                DateTime.Now
            );
            userRepository.UpdateUser(updatedUser);
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
            var book = BookFactory.CreateBook(
                "TEST-ISBN",
                "Test Book",
                "Test Author",
                "Test Publisher",
                2023,
                "Test Genre",
                "Test Description"
            );

            catalogRepository.AddBook(book);
            var books = catalogRepository.GetAllBooks();
            Assert.AreEqual(1, books.Count());

            var retrievedBook = catalogRepository.GetBookById("TEST-ISBN");
            Assert.IsNotNull(retrievedBook);
            Assert.AreEqual("Test Book", retrievedBook.Title);

            var updatedBook = BookFactory.CreateBook(
                "TEST-ISBN",
                "Updated Title",
                "Test Author",
                "Test Publisher",
                2023,
                "Test Genre",
                "Test Description"
            );
            catalogRepository.UpdateBook(updatedBook);
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
            var bookCopy = BookCopyFactory.CreateBookCopy(
                1,
                "TEST-ISBN",
                BookStatus.Available,
                DateTime.Now,
                "Test Location"
            );

            stateRepository.AddBookCopy(bookCopy);
            var bookCopies = stateRepository.GetAllBookCopies();
            Assert.AreEqual(1, bookCopies.Count());

            var retrievedBookCopy = stateRepository.GetBookCopyById(1);
            Assert.IsNotNull(retrievedBookCopy);
            Assert.AreEqual("Test Location", retrievedBookCopy.Location);

            var updatedBookCopy = BookCopyFactory.CreateBookCopy(
                1,
                "TEST-ISBN",
                BookStatus.Available,
                DateTime.Now,
                "Updated Location"
            );
            stateRepository.UpdateBookCopy(updatedBookCopy);
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
            var libraryEvent = LibraryEventFactory.CreateEvent(
                1,
                EventType.BookAdded,
                DateTime.Now,
                "Test Event",
                null,
                "TEST-ISBN",
                1
            );

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

            var user = UserFactory.CreateUser(
                1,
                "Test User",
                "test@example.com",
                "555-1234",
                UserType.Patron,
                DateTime.Now
            );
            userRepository.AddUser(user);

            var book = BookFactory.CreateBook(
                "TEST-ISBN",
                "Test Book",
                "Test Author",
                "Test Publisher",
                2023,
                "Test Genre",
                "Test Description"
            );
            catalogRepository.AddBook(book);

            var bookCopy = BookCopyFactory.CreateBookCopy(
                1,
                "TEST-ISBN",
                BookStatus.Available,
                DateTime.Now,
                "Test Location"
            );
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

            var user1 = UserFactory.CreateUser(1, "User 1", "user1@example.com", "555-1111", UserType.Patron, DateTime.Now);
            var user2 = UserFactory.CreateUser(2, "User 2", "user2@example.com", "555-2222", UserType.Patron, DateTime.Now);
            userRepository.AddUser(user1);
            userRepository.AddUser(user2);

            var book = BookFactory.CreateBook("TEST-ISBN", "Test Book", "Test Author", "Test Publisher", 2023, "Test Genre", "Test Description");
            catalogRepository.AddBook(book);

            var bookCopy1 = BookCopyFactory.CreateBookCopy(
                1,
                "TEST-ISBN",
                BookStatus.CheckedOut,
                DateTime.Now.AddDays(-10),
                "Shelf A",
                1,
                DateTime.Now.AddDays(-1)  // Overdue
            );

            var bookCopy2 = BookCopyFactory.CreateBookCopy(
                2,
                "TEST-ISBN",
                BookStatus.CheckedOut,
                DateTime.Now.AddDays(-10),
                "Shelf B",
                2,
                DateTime.Now.AddDays(5)   // Not overdue
            );

            stateRepository.AddBookCopy(bookCopy1);
            stateRepository.AddBookCopy(bookCopy2);

            var usersWithOverdueBooks = libraryService.GetUsersWithOverdueBooks().ToList();

            Assert.AreEqual(1, usersWithOverdueBooks.Count);
            Assert.AreEqual(1, usersWithOverdueBooks[0].Id);
        }
    }
}