using Library.Data.Interfaces.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Data.Interfaces
{
    public interface IUserRepository
    {
        IEnumerable<IUser> GetAllUsers();
        IUser GetUserById(int id);
        void AddUser(IUser user);
        void UpdateUser(IUser user);
        void DeleteUser(int id);
    }
}
