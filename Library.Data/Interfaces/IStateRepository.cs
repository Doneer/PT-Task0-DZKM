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
        IEnumerable<IBookCopy> GetAllBookCopies();
        IBookCopy GetBookCopyById(int id);
        IEnumerable<IBookCopy> GetAvailableBooks();
        IEnumerable<IBookCopy> GetCheckedOutBooks();
        void AddBookCopy(IBookCopy bookCopy);
        void UpdateBookCopy(IBookCopy bookCopy);
        void DeleteBookCopy(int id);
    }
}
