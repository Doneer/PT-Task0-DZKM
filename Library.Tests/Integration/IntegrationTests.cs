using Microsoft.EntityFrameworkCore;
using Library.Data.Database;
using Library.Data.Repositories;
using Library.Logic.Services;
using Library.Data.Factories;
using Library.Data.Models;

namespace Library.Tests.Integration
{
    [TestClass]
    public class IntegrationTests
    {
        private DbContextOptions<LibraryDbContext> _options;
        private EfDataRepository _dataRepository;
        private LibraryService _libraryService;

        [TestInitialize]
        public void Setup()
        {
            _options = new DbContextOptionsBuilder<LibraryDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _dataRepository = new EfDataRepository(_options);
            _libraryService = new LibraryService(_dataRepository);

            using var context = new LibraryDbContext(_options);
            context.Database.EnsureCreated();
        }

        [TestMethod]
        public void FullStack_UserRegistration_WorksEndToEnd()
        {
            var user = UserFactory.CreateUser(
                0,
                "Integration Test User",
                "integration@test.com",
                "555-9999",
                UserType.Patron,
                DateTime.Now
            );

            _libraryService.RegisterUser(user);

            var users = _libraryService.GetAllUsers();
            Assert.AreEqual(1, users.Count());
            Assert.AreEqual("Integration Test User", users.First().Name);

            var events = _libraryService.GetAllEvents();
            Assert.IsTrue(events.Any(e => e.Type == EventType.UserRegistered));
        }

        [TestMethod]
        public void FullStack_BookManagement_WorksEndToEnd()
        {
            var book = BookFactory.CreateBook(
                "INTEGRATION-TEST",
                "Integration Test Book",
                "Test Author",
                "Test Publisher",
                2024,
                "Test",
                "Integration testing"
            );

            _libraryService.AddBook(book);

            var allBooks = _libraryService.GetAllBooks();
            var specificBook = _libraryService.GetBookByIsbn("INTEGRATION-TEST");

            Assert.AreEqual(1, allBooks.Count());
            Assert.IsNotNull(specificBook);
            Assert.AreEqual("Integration Test Book", specificBook.Title);
        }

        [TestMethod]
        public void FullStack_DatabasePersistence_WorksCorrectly()
        {
            var user = UserFactory.CreateUser(0, "Persistence Test", "persist@test.com", "555-0000", UserType.Patron, DateTime.Now);
            var book = BookFactory.CreateBook("PERSIST-001", "Persistence Book", "Author", "Publisher", 2024, "Test", "Description");

            _libraryService.RegisterUser(user);
            _libraryService.AddBook(book);

            var newDataRepository = new EfDataRepository(_options);
            var newLibraryService = new LibraryService(newDataRepository);

            var persistedUsers = newLibraryService.GetAllUsers();
            var persistedBooks = newLibraryService.GetAllBooks();

            Assert.AreEqual(1, persistedUsers.Count());
            Assert.AreEqual(1, persistedBooks.Count());
            Assert.AreEqual("Persistence Test", persistedUsers.First().Name);
            Assert.AreEqual("Persistence Book", persistedBooks.First().Title);
        }

        [TestCleanup]
        public void Cleanup()
        {
            using var context = new LibraryDbContext(_options);
            context.Database.EnsureDeleted();
        }
    }
}