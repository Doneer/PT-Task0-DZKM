using Library.Data.Interfaces.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Data.Models
{
    internal class BookCopy : IBookCopy
    {
        public int Id { get; set; }
        public string ISBN { get; set; }
        public BookStatus Status { get; set; }
        public DateTime AcquisitionDate { get; set; }
        public string Location { get; set; }
        public int? CurrentBorrowerId { get; set; }
        public DateTime? DueDate { get; set; }
    }

    public enum BookStatus
    {
        Available,
        CheckedOut,
        UnderMaintenance,
        Lost
    }
}
