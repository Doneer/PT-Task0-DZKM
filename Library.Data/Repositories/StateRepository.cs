using Library.Data.Interfaces;
using Library.Data.Interfaces.Models;
using Library.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Library.Data.Repositories
{
    public class StateRepository : IStateRepository
    {
        private readonly List<BookCopy> _bookCopies;

        public StateRepository(IEnumerable<IBookCopy> initialBookCopies = null)
        {
            _bookCopies = new List<BookCopy>();
            if (initialBookCopies != null)
            {
                foreach (var copy in initialBookCopies)
                {
                    _bookCopies.Add(new BookCopy
                    {
                        Id = copy.Id,
                        ISBN = copy.ISBN,
                        Status = copy.Status,
                        AcquisitionDate = copy.AcquisitionDate,
                        Location = copy.Location,
                        CurrentBorrowerId = copy.CurrentBorrowerId,
                        DueDate = copy.DueDate
                    });
                }
            }
        }

        public IEnumerable<IBookCopy> GetAllBookCopies() => _bookCopies.Cast<IBookCopy>().ToList();

        public IBookCopy GetBookCopyById(int id) => _bookCopies.FirstOrDefault(bc => bc.Id == id);

        public IEnumerable<IBookCopy> GetAvailableBooks() =>
            _bookCopies.Where(bc => bc.Status == BookStatus.Available).Cast<IBookCopy>().ToList();

        public IEnumerable<IBookCopy> GetCheckedOutBooks() =>
            _bookCopies.Where(bc => bc.Status == BookStatus.CheckedOut).Cast<IBookCopy>().ToList();

        public void AddBookCopy(IBookCopy bookCopy)
        {
            if (_bookCopies.Any(bc => bc.Id == bookCopy.Id))
            {
                throw new ArgumentException($"Book copy with ID {bookCopy.Id} already exists.");
            }

            var internalBookCopy = new BookCopy
            {
                Id = bookCopy.Id,
                ISBN = bookCopy.ISBN,
                Status = bookCopy.Status,
                AcquisitionDate = bookCopy.AcquisitionDate,
                Location = bookCopy.Location,
                CurrentBorrowerId = bookCopy.CurrentBorrowerId,
                DueDate = bookCopy.DueDate
            };

            _bookCopies.Add(internalBookCopy);
        }

        public void UpdateBookCopy(IBookCopy bookCopy)
        {
            var existingBookCopy = _bookCopies.FirstOrDefault(bc => bc.Id == bookCopy.Id);
            if (existingBookCopy == null)
            {
                throw new ArgumentException($"Book copy with ID {bookCopy.Id} not found.");
            }

            _bookCopies.Remove(existingBookCopy);

            var updatedBookCopy = new BookCopy
            {
                Id = bookCopy.Id,
                ISBN = bookCopy.ISBN,
                Status = bookCopy.Status,
                AcquisitionDate = bookCopy.AcquisitionDate,
                Location = bookCopy.Location,
                CurrentBorrowerId = bookCopy.CurrentBorrowerId,
                DueDate = bookCopy.DueDate
            };

            _bookCopies.Add(updatedBookCopy);
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