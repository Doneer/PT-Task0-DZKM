using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Data.Interfaces.Models
{
    public interface ILibraryEvent
    {
        int Id { get; }
        EventType Type { get; }
        int? UserId { get; }
        string ISBN { get; }
        int? BookCopyId { get; }
        DateTime Timestamp { get; }
        string Description { get; }
    }
}
