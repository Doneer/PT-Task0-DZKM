using Library.Data.Interfaces;
using Library.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Data.Factories
{
    public class ModelFactory : IModelFactory
    {
        public IUser CreateUser(int id, string name, string email, string phoneNumber, UserType type, DateTime registrationDate)
        {
            return new User
            {
                Id = id,
                Name = name,
                Email = email,
                PhoneNumber = phoneNumber,
                Type = type,
                RegistrationDate = registrationDate
            };
        }

        public IBook CreateBook(string isbn, string title, string author, string publisher, int publicationYear, string genre, string description)
        {
            return new Book
            {
                ISBN = isbn,
                Title = title,
                Author = author,
                Publisher = publisher,
                PublicationYear = publicationYear,
                Genre = genre,
                Description = description
            };
        }

        public IBookCopy CreateBookCopy(int id, string isbn, BookStatus status, DateTime acquisitionDate, string location, int? currentBorrowerId = null, DateTime? dueDate = null)
        {
            return new BookCopy
            {
                Id = id,
                ISBN = isbn,
                Status = status,
                AcquisitionDate = acquisitionDate,
                Location = location,
                CurrentBorrowerId = currentBorrowerId,
                DueDate = dueDate
            };
        }

        public ILibraryEvent CreateLibraryEvent(int id, EventType type, int? userId, string isbn, int? bookCopyId, DateTime timestamp, string description)
        {
            return new LibraryEvent
            {
                Id = id,
                Type = type,
                UserId = userId,
                ISBN = isbn,
                BookCopyId = bookCopyId,
                Timestamp = timestamp,
                Description = description
            };
        }
    }
}
