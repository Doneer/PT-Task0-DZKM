using System;
using System.Collections.Generic;
using System.Linq;
using Library.Data.Factories;
using Library.Data.Interfaces;
using Library.Data.Models;
using Library.Logic.Interfaces;

namespace Library.Logic.Services
{
    public class LibraryService : ILibraryService
    {
        private readonly IDataRepository _dataRepository;
        private readonly IModelFactory _modelFactory;

        public LibraryService(IDataRepository dataRepository)
        {
            _dataRepository = dataRepository;
            _modelFactory = new Data.Factories.ModelFactory();
        }

        public IEnumerable<IUser> GetAllUsers()
        {
            return _dataRepository.Users.GetAllUsers();
        }

        public IUser GetUserById(int id)
        {
            return _dataRepository.Users.GetUserById(id);
        }

        public void RegisterUser(IUser user)
        {
            user.RegistrationDate = DateTime.Now;
            _dataRepository.Users.AddUser(user);

            var newEvent = _modelFactory.CreateLibraryEvent(
                GetNextEventId(),
                EventType.UserRegistered,
                user.Id,
                null,
                null,
                DateTime.Now,
                $"User {user.Name} registered"
            );

            _dataRepository.Events.AddEvent(newEvent);
        }

        public void UpdateUserInformation(IUser user)
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

            var newEvent = _modelFactory.CreateLibraryEvent(
                GetNextEventId(),
                EventType.UserRemoved,
                id,
                null,
                null,
                DateTime.Now,
                $"User {user.Name} removed"
            );

            _dataRepository.Events.AddEvent(newEvent);
        }

        public IEnumerable<IBook> GetAllBooks()
        {
            return _dataRepository.Catalog.GetAllBooks();
        }

        public IBook GetBookByIsbn(string isbn)
        {
            return _dataRepository.Catalog.GetBookById(isbn);
        }

        public void AddBook(IBook book)
        {
            _dataRepository.Catalog.AddBook(book);
        }

        public void UpdateBookInformation(IBook book)
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

        public IEnumerable<IBookCopy> GetAllBookCopies()
        {
            return _dataRepository.State.GetAllBookCopies();
        }

        public IBookCopy GetBookCopyById(int id)
        {
            return _dataRepository.State.GetBookCopyById(id);
        }

        public IEnumerable<IBookCopy> GetAvailableBooks()
        {
            return _dataRepository.State.GetAvailableBooks();
        }

        public IEnumerable<IBookCopy> GetCheckedOutBooks()
        {
            return _dataRepository.State.GetCheckedOutBooks();
        }

        public void AddBookCopy(IBookCopy bookCopy)
        {
            var book = _dataRepository.Catalog.GetBookById(bookCopy.ISBN);
            if (book == null)
                throw new ArgumentException($"Book with ISBN {bookCopy.ISBN} not found in catalog");

            bookCopy.AcquisitionDate = DateTime.Now;
            bookCopy.Status = BookStatus.Available;
            _dataRepository.State.AddBookCopy(bookCopy);

            var newEvent = _modelFactory.CreateLibraryEvent(
                GetNextEventId(),
                EventType.BookAdded,
                null,
                bookCopy.ISBN,
                bookCopy.Id,
                DateTime.Now,
                $"Added new copy of {book.Title}"
            );

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

            // Create a new BookCopy with updated properties
            var updatedBookCopy = _modelFactory.CreateBookCopy(
                bookCopy.Id,
                bookCopy.ISBN,
                BookStatus.CheckedOut,
                bookCopy.AcquisitionDate,
                bookCopy.Location,
                userId,
                dueDate
            );

            _dataRepository.State.UpdateBookCopy(updatedBookCopy);

            var newEvent = _modelFactory.CreateLibraryEvent(
                GetNextEventId(),
                EventType.BookBorrowed,
                userId,
                bookCopy.ISBN,
                bookCopyId,
                DateTime.Now,
                $"{user.Name} borrowed {book.Title}"
            );

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

            // Create a new BookCopy with updated properties
            var updatedBookCopy = _modelFactory.CreateBookCopy(
                bookCopy.Id,
                bookCopy.ISBN,
                BookStatus.Available,
                bookCopy.AcquisitionDate,
                bookCopy.Location
            );

            _dataRepository.State.UpdateBookCopy(updatedBookCopy);

            var newEvent = _modelFactory.CreateLibraryEvent(
                GetNextEventId(),
                EventType.BookReturned,
                userId,
                bookCopy.ISBN,
                bookCopyId,
                DateTime.Now,
                $"{user.Name} returned {book.Title}"
            );

            _dataRepository.Events.AddEvent(newEvent);

            if (bookCopy.DueDate.HasValue && bookCopy.DueDate.Value < DateTime.Now)
            {
                var overdueEvent = _modelFactory.CreateLibraryEvent(
                    GetNextEventId(),
                    EventType.FineAssessed,
                    userId,
                    bookCopy.ISBN,
                    bookCopyId,
                    DateTime.Now,
                    $"Fine assessed for overdue book {book.Title}"
                );

                _dataRepository.Events.AddEvent(overdueEvent);
            }

            return true;
        }

        public IEnumerable<IBookCopy> GetBorrowedBooksByUser(int userId)
        {
            return _dataRepository.State.GetCheckedOutBooks()
                .Where(bc => bc.CurrentBorrowerId == userId)
                .ToList();
        }

        public IEnumerable<IUser> GetUsersWithOverdueBooks()
        {
            var today = DateTime.Now;
            var overdueBookCopies = _dataRepository.State.GetCheckedOutBooks()
                .Where(bc => bc.DueDate.HasValue && bc.DueDate.Value < today)
                .ToList();

            var userIds = overdueBookCopies.Select(bc => bc.CurrentBorrowerId.Value).Distinct();

            return userIds.Select(id => _dataRepository.Users.GetUserById(id)).ToList();
        }

        public IEnumerable<ILibraryEvent> GetAllEvents()
        {
            return _dataRepository.Events.GetAllEvents();
        }

        public IEnumerable<ILibraryEvent> GetEventsByUser(int userId)
        {
            return _dataRepository.Events.GetEventsByUser(userId);
        }

        public IEnumerable<ILibraryEvent> GetEventsByBook(string isbn)
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