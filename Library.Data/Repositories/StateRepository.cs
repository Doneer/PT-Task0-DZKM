using Library.Data.Interfaces;
using Library.Data.Models;
using System.Collections.Generic;
using System.Linq;

namespace Library.Data.Repositories
{
    public class StateRepository : IStateRepository
    {
        private readonly List<BookCopy> _bookCopies;

        public StateRepository(List<BookCopy> initialBookCopies = null)
        {
            _bookCopies = initialBookCopies ?? new List<BookCopy>();
        }

        public IEnumerable<BookCopy> GetAllBookCopies() => _bookCopies.ToList();

        public BookCopy GetBookCopyById(int id) => _bookCopies.FirstOrDefault(bc => bc.Id == id);

        public IEnumerable<BookCopy> GetAvailableBooks() =>
            _bookCopies.Where(bc => bc.Status == BookStatus.Available).ToList();

        public IEnumerable<BookCopy> GetCheckedOutBooks() =>
            _bookCopies.Where(bc => bc.Status == BookStatus.CheckedOut).ToList();

        public void AddBookCopy(BookCopy bookCopy)
        {
            if (_bookCopies.Any(bc => bc.Id == bookCopy.Id))
            {
                throw new ArgumentException($"Book copy with ID {bookCopy.Id} already exists.");
            }
            _bookCopies.Add(bookCopy);
        }

        public void UpdateBookCopy(BookCopy bookCopy)
        {
            var existingBookCopy = _bookCopies.FirstOrDefault(bc => bc.Id == bookCopy.Id);
            if (existingBookCopy == null)
            {
                throw new ArgumentException($"Book copy with ID {bookCopy.Id} not found.");
            }

            _bookCopies.Remove(existingBookCopy);
            _bookCopies.Add(bookCopy);
        }

        public void DeleteBookCopy(int id)
        {
            var bookCopy = _bookCopies.FirstOrDefault(bc => bc.Id == id);
            if (bookCopy != null)
            {
                _bookCopies.Remove(bookCopy);
            }
        }
    }
}