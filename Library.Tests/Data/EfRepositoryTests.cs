using Microsoft.EntityFrameworkCore;
using Library.Data.Database;
using Library.Data.Repositories;
using Library.Data.Factories;
using Library.Data.Models;

namespace Library.Tests.Data
{
    [TestClass]
    public class EfRepositoryTests
    {
        private DbContextOptions<LibraryDbContext> _options;
        private EfUserRepository _userRepository;
        private EfCatalogRepository _catalogRepository;

        [TestInitialize]
        public void Setup()
        {
            _options = new DbContextOptionsBuilder<LibraryDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _userRepository = new EfUserRepository(_options);
            _catalogRepository = new EfCatalogRepository(_options);

            using var context = new LibraryDbContext(_options);
            context.Database.EnsureCreated();
        }

        [TestMethod]
        public void EfUserRepository_AddUser_InsertsIntoDatabase()
        {
            var user = UserFactory.CreateUser(
                0, 
                "Test User",
                "test@example.com",
                "555-1234",
                UserType.Patron,
                DateTime.Now
            );

            _userRepository.AddUser(user);

            var users = _userRepository.GetAllUsers();
            Assert.AreEqual(1, users.Count());
            Assert.AreEqual("Test User", users.First().Name);
        }

        [TestMethod]
        public void EfUserRepository_GetUserById_ReturnsCorrectUser()
        {
            var user = UserFactory.CreateUser(
                0,
                "Test User",
                "test@example.com",
                "555-1234",
                UserType.Patron,
                DateTime.Now
            );

            _userRepository.AddUser(user);
            var addedUser = _userRepository.GetAllUsers().First();

            var retrievedUser = _userRepository.GetUserById(addedUser.Id);

            Assert.IsNotNull(retrievedUser);
            Assert.AreEqual("Test User", retrievedUser.Name);
            Assert.AreEqual("test@example.com", retrievedUser.Email);
        }

        [TestMethod]
        public void EfUserRepository_UpdateUser_ModifiesExistingUser()
        {
            var user = UserFactory.CreateUser(
                0,
                "Original Name",
                "test@example.com",
                "555-1234",
                UserType.Patron,
                DateTime.Now
            );

            _userRepository.AddUser(user);
            var addedUser = _userRepository.GetAllUsers().First();

            var updatedUser = UserFactory.CreateUser(
                addedUser.Id,
                "Updated Name",
                "test@example.com",
                "555-1234",
                UserType.Librarian,
                addedUser.RegistrationDate
            );

            _userRepository.UpdateUser(updatedUser);

            var retrievedUser = _userRepository.GetUserById(addedUser.Id);
            Assert.AreEqual("Updated Name", retrievedUser.Name);
            Assert.AreEqual(UserType.Librarian, retrievedUser.Type);
        }

        [TestMethod]
        public void EfUserRepository_DeleteUser_RemovesFromDatabase()
        {
            var user = UserFactory.CreateUser(
                0,
                "Test User",
                "test@example.com",
                "555-1234",
                UserType.Patron,
                DateTime.Now
            );

            _userRepository.AddUser(user);
            var addedUser = _userRepository.GetAllUsers().First();

            _userRepository.DeleteUser(addedUser.Id);

            var users = _userRepository.GetAllUsers();
            Assert.AreEqual(0, users.Count());
        }

        [TestMethod]
        public void EfCatalogRepository_LinqQuerySyntax_Works()
        {
            var book1 = BookFactory.CreateBook("ISBN-001", "Book A", "Author A", "Publisher", 2020, "Fiction", "Description");
            var book2 = BookFactory.CreateBook("ISBN-002", "Book B", "Author B", "Publisher", 2021, "Fiction", "Description");

            _catalogRepository.AddBook(book1);
            _catalogRepository.AddBook(book2);

            using var context = new LibraryDbContext(_options);

            var books = from b in context.Books
                        where b.PublicationYear > 2020
                        orderby b.Title
                        select b;

            var result = books.ToList();
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Book B", result.First().Title);
        }

        [TestMethod]
        public void EfCatalogRepository_LinqMethodSyntax_Works()
        {
            var book1 = BookFactory.CreateBook("ISBN-001", "Fiction Book", "Author A", "Publisher", 2020, "Fiction", "Description");
            var book2 = BookFactory.CreateBook("ISBN-002", "Science Book", "Author B", "Publisher", 2021, "Science", "Description");

            _catalogRepository.AddBook(book1);
            _catalogRepository.AddBook(book2);

            using var context = new LibraryDbContext(_options);

            var fictionBooks = context.Books
                .Where(b => b.Genre == "Fiction")
                .OrderBy(b => b.Title)
                .ToList();

            Assert.AreEqual(1, fictionBooks.Count);
            Assert.AreEqual("Fiction Book", fictionBooks.First().Title);
        }

        [TestCleanup]
        public void Cleanup()
        {
            using var context = new LibraryDbContext(_options);
            context.Database.EnsureDeleted();
        }
    }
}