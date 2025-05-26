using Microsoft.EntityFrameworkCore;
using Library.Data.Database;
using Library.Data.Interfaces;

namespace Library.Data.Repositories
{
    public class EfDataRepository : IDataRepository
    {
        public IUserRepository Users { get; }
        public ICatalogRepository Catalog { get; }
        public IStateRepository State { get; }
        public IEventRepository Events { get; }

        public EfDataRepository(DbContextOptions<LibraryDbContext> options)
        {
            Users = new EfUserRepository(options);
            Catalog = new EfCatalogRepository(options);
            State = new EfStateRepository(options);
            Events = new EfEventRepository(options);
        }
    }
}