using Library.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Data.Interfaces
{
    public interface IStateRepository
    {
        IEnumerable<BookCopy> GetAllBookCopies();
        BookCopy GetBookCopyById(int id);
        IEnumerable<BookCopy> GetAvailableBooks();
        IEnumerable<BookCopy> GetCheckedOutBooks();
        void AddBookCopy(BookCopy bookCopy);
        void UpdateBookCopy(BookCopy bookCopy);
        void DeleteBookCopy(int id);
    }
}
