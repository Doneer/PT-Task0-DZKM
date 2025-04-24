using Library.Data.Interfaces.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Data.Interfaces
{
    public interface ICatalogRepository
    {
        IEnumerable<IBook> GetAllBooks();
        IBook GetBookById(string isbn);
        void AddBook(IBook book);
        void UpdateBook(IBook book);
        void DeleteBook(string isbn);
    }
}
