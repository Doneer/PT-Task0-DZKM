using LibrarySystem.Data.Models;

namespace LibrarySystem.Data.Interfaces
{
    public interface IUserRepository : IDataRepository<User>
    {
        User GetByUsername(string username);
    }
}