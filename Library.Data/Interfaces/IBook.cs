using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Data.Interfaces
{
    internal interface IBook
    {
        string ISBN { get; set; }
        string Title { get; set; }
        string Author { get; set; }
        string Publisher { get; set; }
        int PublicationYear { get; set; }
        string Genre { get; set; }
        string Description { get; set; }
    }
}
