using System;
using System.Collections.Generic;
using System.Linq;
using Library.Data.Interfaces;
using Library.Data.Models;

namespace Library.Data.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly List<User> _users;
        private readonly IModelFactory _modelFactory;

        public UserRepository(List<User> initialUsers = null)
        {
            _users = initialUsers ?? new List<User>();
            _modelFactory = new Factories.ModelFactory();
        }

        public IEnumerable<IUser> GetAllUsers() => _users.Cast<IUser>().ToList();

        public IUser GetUserById(int id) => _users.FirstOrDefault(u => u.Id == id);

        public void AddUser(IUser user)
        {
            if (_users.Any(u => u.Id == user.Id))
            {
                throw new ArgumentException($"User with ID {user.Id} already exists.");
            }

            if (user is User concreteUser)
            {
                _users.Add(concreteUser);
            }
            else
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

        public void UpdateUser(IUser user)
        {
            var existingUser = _users.FirstOrDefault(u => u.Id == user.Id);
            if (existingUser == null)
            {
                throw new ArgumentException($"User with ID {user.Id} not found.");
            }

            _users.Remove(existingUser);

            if (user is User concreteUser)
            {
                _users.Add(concreteUser);
            }
            else
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