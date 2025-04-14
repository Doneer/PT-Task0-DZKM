using Library.Data.Models;
using System;
using System.Collections.Generic;

namespace Library.Logic.Interfaces
{
    public interface ILibraryService
    {
        IEnumerable<User> GetAllUsers();
        User GetUserById(int id);
        void RegisterUser(User user);
        void UpdateUserInformation(User user);
        void RemoveUser(int id);

        IEnumerable<Book> GetAllBooks();
        Book GetBookByIsbn(string isbn);
        void AddBook(Book book);
        void UpdateBookInformation(Book book);
        void RemoveBook(string isbn);

        IEnumerable<BookCopy> GetAllBookCopies();
        BookCopy GetBookCopyById(int id);
        IEnumerable<BookCopy> GetAvailableBooks();
        IEnumerable<BookCopy> GetCheckedOutBooks();
        void AddBookCopy(BookCopy bookCopy);

        bool BorrowBook(int userId, int bookCopyId, DateTime dueDate);
        bool ReturnBook(int bookCopyId);
        IEnumerable<BookCopy> GetBorrowedBooksByUser(int userId);
        IEnumerable<User> GetUsersWithOverdueBooks();

        IEnumerable<LibraryEvent> GetAllEvents();
        IEnumerable<LibraryEvent> GetEventsByUser(int userId);
        IEnumerable<LibraryEvent> GetEventsByBook(string isbn);
    }
}
