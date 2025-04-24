using Library.Data.Interfaces;
using Library.Data.Interfaces.Models;
using Library.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Library.Data.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly List<User> _users;

        public UserRepository(IEnumerable<IUser> initialUsers = null)
        {
            _users = new List<User>();
            if (initialUsers != null)
            {
                foreach (var user in initialUsers)
                {
                    _users.Add(new User
                    {
                        Id = user.Id,
                        Name = user.Name,
                        Email = user.Email,
                        PhoneNumber = user.PhoneNumber,
                        Type = user.Type,
                        RegistrationDate = user.RegistrationDate
                    });
                }
            }
        }

        public IEnumerable<IUser> GetAllUsers() => _users.Cast<IUser>().ToList();

        public IUser GetUserById(int id) => _users.FirstOrDefault(u => u.Id == id);

        public void AddUser(IUser user)
        {
            if (_users.Any(u => u.Id == user.Id))
            {
                throw new ArgumentException($"User with ID {user.Id} already exists.");
            }

            var internalUser = new User
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Type = user.Type,
                RegistrationDate = user.RegistrationDate
            };

            _users.Add(internalUser);
        }

        public void UpdateUser(IUser user)
        {
            var existingUser = _users.FirstOrDefault(u => u.Id == user.Id);
            if (existingUser == null)
            {
                throw new ArgumentException($"User with ID {user.Id} not found.");
            }

            _users.Remove(existingUser);

            var updatedUser = new User
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Type = user.Type,
                RegistrationDate = user.RegistrationDate
            };

            _users.Add(updatedUser);
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