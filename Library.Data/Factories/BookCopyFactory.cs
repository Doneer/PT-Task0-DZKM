using Library.Data.Interfaces.Models;
using Library.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Data.Factories
{
    public static class BookCopyFactory
    {
        public static IBookCopy CreateBookCopy(int id, string isbn, BookStatus status, DateTime acquisitionDate, string location, int? currentBorrowerId = null, DateTime? dueDate = null)
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
    }
}