using System;
using System.Collections.Generic;
using Library.Data.Interfaces;
using Library.Data.Models;

namespace Library.Logic.Interfaces
{
    public interface ILibraryService
    {
        IEnumerable<IUser> GetAllUsers();
        IUser GetUserById(int id);
        void RegisterUser(IUser user);
        void UpdateUserInformation(IUser user);
        void RemoveUser(int id);

        IEnumerable<IBook> GetAllBooks();
        IBook GetBookByIsbn(string isbn);
        void AddBook(IBook book);
        void UpdateBookInformation(IBook book);
        void RemoveBook(string isbn);

        IEnumerable<IBookCopy> GetAllBookCopies();
        IBookCopy GetBookCopyById(int id);
        IEnumerable<IBookCopy> GetAvailableBooks();
        IEnumerable<IBookCopy> GetCheckedOutBooks();
        void AddBookCopy(IBookCopy bookCopy);

        bool BorrowBook(int userId, int bookCopyId, DateTime dueDate);
        bool ReturnBook(int bookCopyId);
        IEnumerable<IBookCopy> GetBorrowedBooksByUser(int userId);
        IEnumerable<IUser> GetUsersWithOverdueBooks();

        IEnumerable<ILibraryEvent> GetAllEvents();
        IEnumerable<ILibraryEvent> GetEventsByUser(int userId);
        IEnumerable<ILibraryEvent> GetEventsByBook(string isbn);
    }
}