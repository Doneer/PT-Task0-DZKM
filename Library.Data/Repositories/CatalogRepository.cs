using Library.Data.Interfaces;
using Library.Data.Interfaces.Models;
using Library.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Library.Data.Repositories
{
    public class CatalogRepository : ICatalogRepository
    {
        private readonly List<Book> _books;

        public CatalogRepository(IEnumerable<IBook> initialBooks = null)
        {
            _books = new List<Book>();
            if (initialBooks != null)
            {
                foreach (var book in initialBooks)
                {
                    _books.Add(new Book
                    {
                        ISBN = book.ISBN,
                        Title = book.Title,
                        Author = book.Author,
                        Publisher = book.Publisher,
                        PublicationYear = book.PublicationYear,
                        Genre = book.Genre,
                        Description = book.Description
                    });
                }
            }
        }

        public IEnumerable<IBook> GetAllBooks() => _books.Cast<IBook>().ToList();

        public IBook GetBookById(string isbn) => _books.FirstOrDefault(b => b.ISBN == isbn);

        public void AddBook(IBook book)
        {
            if (_books.Any(b => b.ISBN == book.ISBN))
            {
                throw new ArgumentException($"Book with ISBN {book.ISBN} already exists.");
            }

            var internalBook = new Book
            {
                ISBN = book.ISBN,
                Title = book.Title,
                Author = book.Author,
                Publisher = book.Publisher,
                PublicationYear = book.PublicationYear,
                Genre = book.Genre,
                Description = book.Description
            };

            _books.Add(internalBook);
        }

        public void UpdateBook(IBook book)
        {
            var existingBook = _books.FirstOrDefault(b => b.ISBN == book.ISBN);
            if (existingBook == null)
            {
                throw new ArgumentException($"Book with ISBN {book.ISBN} not found.");
            }

            _books.Remove(existingBook);

            var updatedBook = new Book
            {
                ISBN = book.ISBN,
                Title = book.Title,
                Author = book.Author,
                Publisher = book.Publisher,
                PublicationYear = book.PublicationYear,
                Genre = book.Genre,
                Description = book.Description
            };

            _books.Add(updatedBook);
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