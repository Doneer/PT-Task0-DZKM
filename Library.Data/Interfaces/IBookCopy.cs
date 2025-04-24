using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Data.Interfaces
{
    internal interface IBookCopy
    {
        int Id { get; set; }
        string ISBN { get; set; }
        BookStatus Status { get; set; }
        DateTime AcquisitionDate { get; set; }
        string Location { get; set; }
        int? CurrentBorrowerId { get; set; }
        DateTime? DueDate { get; set; }
    }
}
