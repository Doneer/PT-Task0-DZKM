using Library.Data.Interfaces.Models;
using Library.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Data.Factories
{
    public static class BookFactory
    {
        public static IBook CreateBook(string isbn, string title, string author, string publisher, int publicationYear, string genre, string description)
        {
            return new Book
            {
                ISBN = isbn,
                Title = title,
                Author = author,
                Publisher = publisher,
                PublicationYear = publicationYear,
                Genre = genre,
                Description = description
            };
        }
    }
}
