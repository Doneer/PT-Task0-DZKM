using Library.Data.Interfaces;
using Library.Data.Models;
using Library.Logic.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Library.Logic.Services
{
    public class LibraryService : ILibraryService
    {
        private readonly IDataRepository _dataRepository;

        public LibraryService(IDataRepository dataRepository)
        {
            _dataRepository = dataRepository;
        }

        public IEnumerable<User> GetAllUsers()
        {
            return _dataRepository.Users.GetAllUsers();
        }

        public User GetUserById(int id)
        {
            return _dataRepository.Users.GetUserById(id);
        }

        public void RegisterUser(User user)
        {
            user.RegistrationDate = DateTime.Now;
            _dataRepository.Users.AddUser(user);

            var newEvent = new LibraryEvent
            {
                Id = GetNextEventId(),
                Type = EventType.UserRegistered,
                UserId = user.Id,
                Timestamp = DateTime.Now,
                Description = $"User {user.Name} registered"
            };

            _dataRepository.Events.AddEvent(newEvent);
        }

        public void UpdateUserInformation(User user)
        {
            _dataRepository.Users.UpdateUser(user);
        }

        public void RemoveUser(int id)
        {
            var user = _dataRepository.Users.GetUserById(id);
            if (user == null)
                throw new ArgumentException($"User with ID {id} not found");

            var borrowedBooks = GetBorrowedBooksByUser(id);
            if (borrowedBooks.Any())
                throw new InvalidOperationException($"Cannot remove user with ID {id} because they have borrowed books");

            _dataRepository.Users.DeleteUser(id);

            var newEvent = new LibraryEvent
            {
                Id = GetNextEventId(),
                Type = EventType.UserRemoved,
                UserId = id,
                Timestamp = DateTime.Now,
                Description = $"User {user.Name} removed"
            };

            _dataRepository.Events.AddEvent(newEvent);
        }

        public IEnumerable<Book> GetAllBooks()
        {
            return _dataRepository.Catalog.GetAllBooks();
        }

        public Book GetBookByIsbn(string isbn)
        {
            return _dataRepository.Catalog.GetBookById(isbn);
        }

        public void AddBook(Book book)
        {
            _dataRepository.Catalog.AddBook(book);
        }

        public void UpdateBookInformation(Book book)
        {
            _dataRepository.Catalog.UpdateBook(book);
        }

        public void RemoveBook(string isbn)
        {
            var book = _dataRepository.Catalog.GetBookById(isbn);
            if (book == null)
                throw new ArgumentException($"Book with ISBN {isbn} not found");

            var bookCopies = _dataRepository.State.GetAllBookCopies()
                .Where(bc => bc.ISBN == isbn)
                .ToList();

            if (bookCopies.Any())
                throw new InvalidOperationException($"Cannot remove book with ISBN {isbn} because there are copies in the library");

            _dataRepository.Catalog.DeleteBook(isbn);
        }

        public IEnumerable<BookCopy> GetAllBookCopies()
        {
            return _dataRepository.State.GetAllBookCopies();
        }

        public BookCopy GetBookCopyById(int id)
        {
            return _dataRepository.State.GetBookCopyById(id);
        }

        public IEnumerable<BookCopy> GetAvailableBooks()
        {
            return _dataRepository.State.GetAvailableBooks();
        }

        public IEnumerable<BookCopy> GetCheckedOutBooks()
        {
            return _dataRepository.State.GetCheckedOutBooks();
        }

        public void AddBookCopy(BookCopy bookCopy)
        {
            var book = _dataRepository.Catalog.GetBookById(bookCopy.ISBN);
            if (book == null)
                throw new ArgumentException($"Book with ISBN {bookCopy.ISBN} not found in catalog");

            bookCopy.AcquisitionDate = DateTime.Now;
            bookCopy.Status = BookStatus.Available;
            _dataRepository.State.AddBookCopy(bookCopy);

            var newEvent = new LibraryEvent
            {
                Id = GetNextEventId(),
                Type = EventType.BookAdded,
                ISBN = bookCopy.ISBN,
                BookCopyId = bookCopy.Id,
                Timestamp = DateTime.Now,
                Description = $"Added new copy of {book.Title}"
            };

            _dataRepository.Events.AddEvent(newEvent);
        }

        public bool BorrowBook(int userId, int bookCopyId, DateTime dueDate)
        {
            var user = _dataRepository.Users.GetUserById(userId);
            if (user == null)
                throw new ArgumentException($"User with ID {userId} not found");

            var bookCopy = _dataRepository.State.GetBookCopyById(bookCopyId);
            if (bookCopy == null)
                throw new ArgumentException($"Book copy with ID {bookCopyId} not found");

            if (bookCopy.Status != BookStatus.Available)
                return false;

            var book = _dataRepository.Catalog.GetBookById(bookCopy.ISBN);
            if (book == null)
                throw new InvalidOperationException($"Book with ISBN {bookCopy.ISBN} not found in catalog");

            bookCopy.Status = BookStatus.CheckedOut;
            bookCopy.CurrentBorrowerId = userId;
            bookCopy.DueDate = dueDate;
            _dataRepository.State.UpdateBookCopy(bookCopy);

            var newEvent = new LibraryEvent
            {
                Id = GetNextEventId(),
                Type = EventType.BookBorrowed,
                UserId = userId,
                ISBN = bookCopy.ISBN,
                BookCopyId = bookCopyId,
                Timestamp = DateTime.Now,
                Description = $"{user.Name} borrowed {book.Title}"
            };

            _dataRepository.Events.AddEvent(newEvent);

            return true;
        }

        public bool ReturnBook(int bookCopyId)
        {
            var bookCopy = _dataRepository.State.GetBookCopyById(bookCopyId);
            if (bookCopy == null)
                throw new ArgumentException($"Book copy with ID {bookCopyId} not found");

            if (bookCopy.Status != BookStatus.CheckedOut)
                return false;

            var book = _dataRepository.Catalog.GetBookById(bookCopy.ISBN);
            var userId = bookCopy.CurrentBorrowerId;
            var user = _dataRepository.Users.GetUserById(userId.Value);

            bookCopy.Status = BookStatus.Available;
            bookCopy.CurrentBorrowerId = null;
            bookCopy.DueDate = null;
            _dataRepository.State.UpdateBookCopy(bookCopy);

            var newEvent = new LibraryEvent
            {
                Id = GetNextEventId(),
                Type = EventType.BookReturned,
                UserId = userId,
                ISBN = bookCopy.ISBN,
                BookCopyId = bookCopyId,
                Timestamp = DateTime.Now,
                Description = $"{user.Name} returned {book.Title}"
            };

            _dataRepository.Events.AddEvent(newEvent);

            if (bookCopy.DueDate.HasValue && bookCopy.DueDate.Value < DateTime.Now)
            {
                var overdueEvent = new LibraryEvent
                {
                    Id = GetNextEventId(),
                    Type = EventType.FineAssessed,
                    UserId = userId,
                    ISBN = bookCopy.ISBN,
                    BookCopyId = bookCopyId,
                    Timestamp = DateTime.Now,
                    Description = $"Fine assessed for overdue book {book.Title}"
                };

                _dataRepository.Events.AddEvent(overdueEvent);
            }

            return true;
        }

        public IEnumerable<BookCopy> GetBorrowedBooksByUser(int userId)
        {
            return _dataRepository.State.GetCheckedOutBooks()
                .Where(bc => bc.CurrentBorrowerId == userId)
                .ToList();
        }

        public IEnumerable<User> GetUsersWithOverdueBooks()
        {
            var today = DateTime.Now;
            var overdueBookCopies = _dataRepository.State.GetCheckedOutBooks()
                .Where(bc => bc.DueDate.HasValue && bc.DueDate.Value < today)
                .ToList();

            var userIds = overdueBookCopies.Select(bc => bc.CurrentBorrowerId.Value).Distinct();

            return userIds.Select(id => _dataRepository.Users.GetUserById(id)).ToList();
        }

        public IEnumerable<LibraryEvent> GetAllEvents()
        {
            return _dataRepository.Events.GetAllEvents();
        }

        public IEnumerable<LibraryEvent> GetEventsByUser(int userId)
        {
            return _dataRepository.Events.GetEventsByUser(userId);
        }

        public IEnumerable<LibraryEvent> GetEventsByBook(string isbn)
        {
            return _dataRepository.Events.GetEventsByBook(isbn);
        }

        private int GetNextEventId()
        {
            var events = _dataRepository.Events.GetAllEvents();
            return events.Any() ? events.Max(e => e.Id) + 1 : 1;
        }
    }
}