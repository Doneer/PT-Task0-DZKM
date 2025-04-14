using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Data.Interfaces
{
    public interface IDataRepository
    {
        IUserRepository Users { get; }
        ICatalogRepository Catalog { get; }
        IStateRepository State { get; }
        IEventRepository Events { get; }
    }
}
