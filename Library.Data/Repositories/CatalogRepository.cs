using System;
using System.Collections.Generic;
using System.Linq;
using Library.Data.Interfaces;
using Library.Data.Models;

namespace Library.Data.Repositories
{
    public class CatalogRepository : ICatalogRepository
    {
        private readonly List<Book> _books;
        private readonly IModelFactory _modelFactory;

        public CatalogRepository(List<Book> initialBooks = null)
        {
            _books = initialBooks ?? new List<Book>();
            _modelFactory = new Factories.ModelFactory();
        }

        public IEnumerable<IBook> GetAllBooks() => _books.Cast<IBook>().ToList();

        public IBook GetBookById(string isbn) => _books.FirstOrDefault(b => b.ISBN == isbn);

        public void AddBook(IBook book)
        {
            if (_books.Any(b => b.ISBN == book.ISBN))
            {
                throw new ArgumentException($"Book with ISBN {book.ISBN} already exists.");
            }

            if (book is Book concreteBook)
            {
                _books.Add(concreteBook);
            }
            else
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

        public void UpdateBook(IBook book)
        {
            var existingBook = _books.FirstOrDefault(b => b.ISBN == book.ISBN);
            if (existingBook == null)
            {
                throw new ArgumentException($"Book with ISBN {book.ISBN} not found.");
            }

            _books.Remove(existingBook);

            if (book is Book concreteBook)
            {
                _books.Add(concreteBook);
            }
            else
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