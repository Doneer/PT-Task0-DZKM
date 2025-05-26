using Microsoft.EntityFrameworkCore;
using Library.Data.Database;
using Library.Data.Factories;
using Library.Data.Interfaces;
using Library.Data.Interfaces.Models;

namespace Library.Data.Repositories
{
    public class EfCatalogRepository : ICatalogRepository
    {
        private readonly DbContextOptions<LibraryDbContext> _options;

        public EfCatalogRepository(DbContextOptions<LibraryDbContext> options)
        {
            _options = options;
        }

        public IEnumerable<IBook> GetAllBooks()
        {
            using var context = new LibraryDbContext(_options);

            var books = from b in context.Books
                        orderby b.Title
                        select b;

            return books.ToList().Select(MapToBook);
        }

        public IBook GetBookById(string isbn)
        {
            using var context = new LibraryDbContext(_options);

            var book = context.Books
                .Where(b => b.ISBN == isbn)
                .FirstOrDefault();

            return book != null ? MapToBook(book) : null;
        }

        public void AddBook(IBook book)
        {
            using var context = new LibraryDbContext(_options);

            if (context.Books.Any(b => b.ISBN == book.ISBN))
            {
                throw new ArgumentException($"Book with ISBN {book.ISBN} already exists.");
            }

            var bookEntity = new BookEntity
            {
                ISBN = book.ISBN,
                Title = book.Title,
                Author = book.Author,
                Publisher = book.Publisher,
                PublicationYear = book.PublicationYear,
                Genre = book.Genre,
                Description = book.Description
            };

            context.Books.Add(bookEntity);
            context.SaveChanges();
        }

        public void UpdateBook(IBook book)
        {
            using var context = new LibraryDbContext(_options);

            var existingBook = context.Books.Find(book.ISBN);
            if (existingBook == null)
            {
                throw new ArgumentException($"Book with ISBN {book.ISBN} not found.");
            }

            existingBook.Title = book.Title;
            existingBook.Author = book.Author;
            existingBook.Publisher = book.Publisher;
            existingBook.PublicationYear = book.PublicationYear;
            existingBook.Genre = book.Genre;
            existingBook.Description = book.Description;

            context.SaveChanges();
        }

        public void DeleteBook(string isbn)
        {
            using var context = new LibraryDbContext(_options);

            var book = context.Books.Find(isbn);
            if (book != null)
            {
                context.Books.Remove(book);
                context.SaveChanges();
            }
        }

        private static IBook MapToBook(BookEntity entity)
        {
            return BookFactory.CreateBook(
                entity.ISBN,
                entity.Title,
                entity.Author,
                entity.Publisher,
                entity.PublicationYear,
                entity.Genre,
                entity.Description
            );
        }
    }
}