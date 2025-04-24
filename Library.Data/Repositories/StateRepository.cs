using System;
using System.Collections.Generic;
using System.Linq;
using Library.Data.Interfaces;
using Library.Data.Models;

namespace Library.Data.Repositories
{
    public class StateRepository : IStateRepository
    {
        private readonly List<BookCopy> _bookCopies;
        private readonly IModelFactory _modelFactory;

        public StateRepository(List<BookCopy> initialBookCopies = null)
        {
            _bookCopies = initialBookCopies ?? new List<BookCopy>();
            _modelFactory = new Factories.ModelFactory();
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

            if (bookCopy is BookCopy concreteBookCopy)
            {
                _bookCopies.Add(concreteBookCopy);
            }
            else
            {
                _bookCopies.Add(new BookCopy
                {
                    Id = bookCopy.Id,
                    ISBN = bookCopy.ISBN,
                    Status = bookCopy.Status,
                    AcquisitionDate = bookCopy.AcquisitionDate,
                    Location = bookCopy.Location,
                    CurrentBorrowerId = bookCopy.CurrentBorrowerId,
                    DueDate = bookCopy.DueDate
                });
            }
        }

        public void UpdateBookCopy(IBookCopy bookCopy)
        {
            var existingBookCopy = _bookCopies.FirstOrDefault(bc => bc.Id == bookCopy.Id);
            if (existingBookCopy == null)
            {
                throw new ArgumentException($"Book copy with ID {bookCopy.Id} not found.");
            }

            _bookCopies.Remove(existingBookCopy);

            if (bookCopy is BookCopy concreteBookCopy)
            {
                _bookCopies.Add(concreteBookCopy);
            }
            else
            {
                _bookCopies.Add(new BookCopy
                {
                    Id = bookCopy.Id,
                    ISBN = bookCopy.ISBN,
                    Status = bookCopy.Status,
                    AcquisitionDate = bookCopy.AcquisitionDate,
                    Location = bookCopy.Location,
                    CurrentBorrowerId = bookCopy.CurrentBorrowerId,
                    DueDate = bookCopy.DueDate
                });
            }
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