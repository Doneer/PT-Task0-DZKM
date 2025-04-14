using Library.Data.Interfaces;
using Library.Data.Models;
using System.Collections.Generic;
using System.Linq;

namespace Library.Data.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly List<User> _users;

        public UserRepository(List<User> initialUsers = null)
        {
            _users = initialUsers ?? new List<User>();
        }

        public IEnumerable<User> GetAllUsers() => _users.ToList();

        public User GetUserById(int id) => _users.FirstOrDefault(u => u.Id == id);

        public void AddUser(User user)
        {
            if (_users.Any(u => u.Id == user.Id))
            {
                throw new ArgumentException($"User with ID {user.Id} already exists.");
            }
            _users.Add(user);
        }

        public void UpdateUser(User user)
        {
            var existingUser = _users.FirstOrDefault(u => u.Id == user.Id);
            if (existingUser == null)
            {
                throw new ArgumentException($"User with ID {user.Id} not found.");
            }

            _users.Remove(existingUser);
            _users.Add(user);
        }

        public void DeleteUser(int id)
        {
            var user = _users.FirstOrDefault(u => u.Id == id);
            if (user != null)
            {
                _users.Remove(user);
            }
        }
    }
}
