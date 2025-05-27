using Library.Data.Factories;
using Library.Data.Interfaces.Models;
using Library.Data.Models;
using Library.Logic.Interfaces;
using Library.WPF.Models;
using Library.WPF.Services;
using Library.WPF.ViewModels;

namespace Library.Tests.WPF
{
    [TestClass]
    public class ViewModelTests
    {
        private MockLibraryService _mockLibraryService;
        private MockDialogService _mockDialogService;
        private MainViewModel _mainViewModel;

        [TestInitialize]
        public void Setup()
        {
            _mockLibraryService = new MockLibraryService();
            _mockDialogService = new MockDialogService();
            _mainViewModel = new MainViewModel(_mockLibraryService, _mockDialogService);
        }

        [TestMethod]
        public void UserViewModel_PropertyChanges_RaiseNotification()
        {
            var user = UserFactory.CreateUser(1, "Test User", "test@example.com", "555-1234", UserType.Patron, DateTime.Now);
            var userViewModel = new UserViewModel(user);

            var propertyChangedRaised = false;
            userViewModel.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(UserViewModel.Name))
                    propertyChangedRaised = true;
            };

            userViewModel.Name = "Updated Name";

            Assert.IsTrue(propertyChangedRaised);
            Assert.AreEqual("Updated Name", userViewModel.Name);
        }

        [TestMethod]
        public void BookViewModel_PropertyChanges_RaiseNotification()
        {
            var book = BookFactory.CreateBook("TEST-ISBN", "Test Book", "Test Author", "Test Publisher", 2023, "Fiction", "Test Description");
            var bookViewModel = new BookViewModel(book);

            var propertyChangedRaised = false;
            bookViewModel.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(BookViewModel.Title))
                    propertyChangedRaised = true;
            };

            bookViewModel.Title = "Updated Title";

            Assert.IsTrue(propertyChangedRaised);
            Assert.AreEqual("Updated Title", bookViewModel.Title);
        }

        [TestMethod]
        public void MainViewModel_LoadDataCommand_PopulatesCollections()
        {
            _mockLibraryService.AddTestData();

            var users = _mockLibraryService.GetAllUsers();
            var books = _mockLibraryService.GetAllBooks();

            _mainViewModel.Users.Clear();
            _mainViewModel.Books.Clear();

            foreach (var user in users)
            {
                _mainViewModel.Users.Add(new UserViewModel(user));
            }

            foreach (var book in books)
            {
                _mainViewModel.Books.Add(new BookViewModel(book));
            }

            Assert.IsTrue(_mainViewModel.Users.Count > 0);
            Assert.IsTrue(_mainViewModel.Books.Count > 0);
            Assert.AreEqual(1, _mainViewModel.Users.Count);
            Assert.AreEqual(1, _mainViewModel.Books.Count);
        }

        [TestMethod]
        public void MainViewModel_SelectedUser_UpdatesCorrectly()
        {
            var user = new UserViewModel(UserFactory.CreateUser(1, "Test User", "test@example.com", "555-1234", UserType.Patron, DateTime.Now));
            _mainViewModel.Users.Add(user);

            _mainViewModel.SelectedUser = user;

            Assert.AreEqual(user, _mainViewModel.SelectedUser);
        }

        [TestMethod]
        public void MainViewModel_AddUserCommand_CanExecute()
        {
            Assert.IsTrue(_mainViewModel.AddUserCommand.CanExecute(null));
        }

        [TestMethod]
        public void MainViewModel_EditUserCommand_RequiresSelectedUser()
        {
            _mainViewModel.SelectedUser = null;
            Assert.IsFalse(_mainViewModel.EditUserCommand.CanExecute(null));

            var user = new UserViewModel(UserFactory.CreateUser(1, "Test", "test@example.com", "555-1234", UserType.Patron, DateTime.Now));
            _mainViewModel.SelectedUser = user;
            Assert.IsTrue(_mainViewModel.EditUserCommand.CanExecute(null));
        }
    }

    public class MockLibraryService : ILibraryService
    {
        private List<IUser> _users = new List<IUser>();
        private List<IBook> _books = new List<IBook>();

        public void AddTestData()
        {
            _users.Add(UserFactory.CreateUser(1, "Test User", "test@example.com", "555-1234", UserType.Patron, DateTime.Now));
            _books.Add(BookFactory.CreateBook("TEST-ISBN", "Test Book", "Test Author", "Test Publisher", 2023, "Fiction", "Test Description"));
        }

        public IEnumerable<IUser> GetAllUsers() => _users;
        public IEnumerable<IBook> GetAllBooks() => _books;
        public IUser GetUserById(int id) => _users.FirstOrDefault(u => u.Id == id);
        public IBook GetBookByIsbn(string isbn) => _books.FirstOrDefault(b => b.ISBN == isbn);

        public void RegisterUser(IUser user) => _users.Add(user);
        public void UpdateUserInformation(IUser user) {/*Mock implementation */}
        public void RemoveUser(int id) => _users.RemoveAll(u => u.Id == id);
        public void AddBook(IBook book) => _books.Add(book);
        public void UpdateBookInformation(IBook book) {/*Mock implementation*/}
        public void RemoveBook(string isbn) => _books.RemoveAll(b => b.ISBN == isbn);

        public IEnumerable<IBookCopy> GetAllBookCopies() => new List<IBookCopy>();
        public IBookCopy GetBookCopyById(int id) => null;
        public IEnumerable<IBookCopy> GetAvailableBooks() => new List<IBookCopy>();
        public IEnumerable<IBookCopy> GetCheckedOutBooks() => new List<IBookCopy>();
        public void AddBookCopy(IBookCopy bookCopy) { }
        public bool BorrowBook(int userId, int bookCopyId, DateTime dueDate) => true;
        public bool ReturnBook(int bookCopyId) => true;
        public IEnumerable<IBookCopy> GetBorrowedBooksByUser(int userId) => new List<IBookCopy>();
        public IEnumerable<IUser> GetUsersWithOverdueBooks() => new List<IUser>();
        public IEnumerable<ILibraryEvent> GetAllEvents() => new List<ILibraryEvent>();
        public IEnumerable<ILibraryEvent> GetEventsByUser(int userId) => new List<ILibraryEvent>();
        public IEnumerable<ILibraryEvent> GetEventsByBook(string isbn) => new List<ILibraryEvent>();
    }

    public class MockDialogService : IDialogService
    {
        public bool ShowUserDialogResult { get; set; } = true;
        public bool ShowBookDialogResult { get; set; } = true;

        public void ShowError(string title, string message) { }
        public void ShowInformation(string title, string message) { }
        public bool ShowConfirmation(string title, string message) => true;
        public bool ShowUserDialog(UserViewModel user, string title) => ShowUserDialogResult;
        public bool ShowBookDialog(BookViewModel book, string title) => ShowBookDialogResult;
    }
}