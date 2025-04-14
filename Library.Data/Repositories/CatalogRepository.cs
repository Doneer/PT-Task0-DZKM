using Library.Data.Interfaces;
using Library.Data.Models;
using System.Collections.Generic;
using System.Linq;

namespace Library.Data.Repositories
{
    public class CatalogRepository : ICatalogRepository
    {
        private readonly List<Book> _books;

        public CatalogRepository(List<Book> initialBooks = null)
        {
            _books = initialBooks ?? new List<Book>();
        }

        public IEnumerable<Book> GetAllBooks() => _books.ToList();

        public Book GetBookById(string isbn) => _books.FirstOrDefault(b => b.ISBN == isbn);

        public void AddBook(Book book)
        {
            if (_books.Any(b => b.ISBN == book.ISBN))
            {
                throw new ArgumentException($"Book with ISBN {book.ISBN} already exists.");
            }
            _books.Add(book);
        }

        public void UpdateBook(Book book)
        {
            var existingBook = _books.FirstOrDefault(b => b.ISBN == book.ISBN);
            if (existingBook == null)
            {
                throw new ArgumentException($"Book with ISBN {book.ISBN} not found.");
            }

            _books.Remove(existingBook);
            _books.Add(book);
        }

        public void DeleteBook(string isbn)
        {
            var book = _books.FirstOrDefault(b => b.ISBN == isbn);
            if (book != null)
            {
                _books.Remove(book);
            }
        }
    }
}