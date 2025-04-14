using Library.Data.Interfaces;

namespace Library.Data.Repositories
{
    public class DataRepository : IDataRepository
    {
        public IUserRepository Users { get; }
        public ICatalogRepository Catalog { get; }
        public IStateRepository State { get; }
        public IEventRepository Events { get; }

        public DataRepository(
            IUserRepository userRepository,
            ICatalogRepository catalogRepository,
            IStateRepository stateRepository,
            IEventRepository eventRepository)
        {
            Users = userRepository;
            Catalog = catalogRepository;
            State = stateRepository;
            Events = eventRepository;
        }
    }
}