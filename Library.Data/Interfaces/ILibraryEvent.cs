using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Data.Interfaces
{
    internal interface ILibraryEvent
    {
        int Id { get; set; }
        EventType Type { get; set; }
        int? UserId { get; set; }
        string ISBN { get; set; }
        int? BookCopyId { get; set; }
        DateTime Timestamp { get; set; }
        string Description { get; set; }
    }
}
