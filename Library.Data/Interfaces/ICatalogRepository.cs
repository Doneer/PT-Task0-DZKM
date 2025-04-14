using Library.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Data.Interfaces
{
    public interface ICatalogRepository
    {
        IEnumerable<Book> GetAllBooks();
        Book GetBookById(string isbn);
        void AddBook(Book book);
        void UpdateBook(Book book);
        void DeleteBook(string isbn);
    }
}
