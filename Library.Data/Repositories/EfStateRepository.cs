using Microsoft.EntityFrameworkCore;
using Library.Data.Database;
using Library.Data.Factories;
using Library.Data.Interfaces;
using Library.Data.Interfaces.Models;

namespace Library.Data.Repositories
{
    public class EfStateRepository : IStateRepository
    {
        private readonly DbContextOptions<LibraryDbContext> _options;

        public EfStateRepository(DbContextOptions<LibraryDbContext> options)
        {
            _options = options;
        }

        public IEnumerable<IBookCopy> GetAllBookCopies()
        {
            using var context = new LibraryDbContext(_options);

            var bookCopies = from bc in context.BookCopies
                             orderby bc.Id
                             select bc;

            return bookCopies.ToList().Select(MapToBookCopy);
        }

        public IBookCopy GetBookCopyById(int id)
        {
            using var context = new LibraryDbContext(_options);

            var bookCopy = context.BookCopies
                .Where(bc => bc.Id == id)
                .FirstOrDefault();

            return bookCopy != null ? MapToBookCopy(bookCopy) : null;
        }

        public IEnumerable<IBookCopy> GetAvailableBooks()
        {
            using var context = new LibraryDbContext(_options);

            var availableBooks = context.BookCopies
                .Where(bc => bc.Status == (int)BookStatus.Available)
                .ToList();

            return availableBooks.Select(MapToBookCopy);
        }

        public IEnumerable<IBookCopy> GetCheckedOutBooks()
        {
            using var context = new LibraryDbContext(_options);

            var checkedOutBooks = context.BookCopies
                .Where(bc => bc.Status == (int)BookStatus.CheckedOut)
                .ToList();

            return checkedOutBooks.Select(MapToBookCopy);
        }

        public void AddBookCopy(IBookCopy bookCopy)
        {
            using var context = new LibraryDbContext(_options);

            if (context.BookCopies.Any(bc => bc.Id == bookCopy.Id))
            {
                throw new ArgumentException($"Book copy with ID {bookCopy.Id} already exists.");
            }

            var bookCopyEntity = new BookCopyEntity
            {
                ISBN = bookCopy.ISBN,
                Status = (int)bookCopy.Status,
                AcquisitionDate = bookCopy.AcquisitionDate,
                Location = bookCopy.Location,
                CurrentBorrowerId = bookCopy.CurrentBorrowerId,
                DueDate = bookCopy.DueDate
            };

            context.BookCopies.Add(bookCopyEntity);
            context.SaveChanges();
        }

        public void UpdateBookCopy(IBookCopy bookCopy)
        {
            using var context = new LibraryDbContext(_options);

            var existingBookCopy = context.BookCopies.Find(bookCopy.Id);
            if (existingBookCopy == null)
            {
                throw new ArgumentException($"Book copy with ID {bookCopy.Id} not found.");
            }

            existingBookCopy.ISBN = bookCopy.ISBN;
            existingBookCopy.Status = (int)bookCopy.Status;
            existingBookCopy.AcquisitionDate = bookCopy.AcquisitionDate;
            existingBookCopy.Location = bookCopy.Location;
            existingBookCopy.CurrentBorrowerId = bookCopy.CurrentBorrowerId;
            existingBookCopy.DueDate = bookCopy.DueDate;

            context.SaveChanges();
        }

        public void DeleteBookCopy(int id)
        {
            using var context = new LibraryDbContext(_options);

            var bookCopy = context.BookCopies.Find(id);
            if (bookCopy != null)
            {
                context.BookCopies.Remove(bookCopy);
                context.SaveChanges();
            }
        }

        private static IBookCopy MapToBookCopy(BookCopyEntity entity)
        {
            return BookCopyFactory.CreateBookCopy(
                entity.Id,
                entity.ISBN,
                (BookStatus)entity.Status,
                entity.AcquisitionDate,
                entity.Location,
                entity.CurrentBorrowerId,
                entity.DueDate
            );
        }
    }
}