using Library.Data.Interfaces;
using Library.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Data.Interfaces
{
    internal interface IModelFactory
    {
        IUser CreateUser(int id, string name, string email, string phoneNumber, UserType type, DateTime registrationDate);
        IBook CreateBook(string isbn, string title, string author, string publisher, int publicationYear, string genre, string description);
        IBookCopy CreateBookCopy(int id, string isbn, BookStatus status, DateTime acquisitionDate, string location, int? currentBorrowerId = null, DateTime? dueDate = null);
        ILibraryEvent CreateLibraryEvent(int id, EventType type, int? userId, string isbn, int? bookCopyId, DateTime timestamp, string description);
    }
}
