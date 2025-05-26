using Microsoft.EntityFrameworkCore;
using Library.Data.Database;
using Library.Data.Factories;
using Library.Data.Interfaces;
using Library.Data.Interfaces.Models;

namespace Library.Data.Repositories
{
    public class EfUserRepository : IUserRepository
    {
        private readonly DbContextOptions<LibraryDbContext> _options;

        public EfUserRepository(DbContextOptions<LibraryDbContext> options)
        {
            _options = options;
        }

        public IEnumerable<IUser> GetAllUsers()
        {
            using var context = new LibraryDbContext(_options);

            var users = from u in context.Users
                        select u;

            return users.ToList().Select(MapToUser);
        }

        public IUser GetUserById(int id)
        {
            using var context = new LibraryDbContext(_options);

            var user = context.Users
                .Where(u => u.Id == id)
                .FirstOrDefault();

            return user != null ? MapToUser(user) : null;
        }

        public void AddUser(IUser user)
        {
            using var context = new LibraryDbContext(_options);

            if (context.Users.Any(u => u.Id == user.Id))
            {
                throw new ArgumentException($"User with ID {user.Id} already exists.");
            }

            var userEntity = new UserEntity
            {
                Name = user.Name,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Type = (int)user.Type,
                RegistrationDate = user.RegistrationDate
            };

            context.Users.Add(userEntity);
            context.SaveChanges();
        }

        public void UpdateUser(IUser user)
        {
            using var context = new LibraryDbContext(_options);

            var existingUser = context.Users.Find(user.Id);
            if (existingUser == null)
            {
                throw new ArgumentException($"User with ID {user.Id} not found.");
            }

            existingUser.Name = user.Name;
            existingUser.Email = user.Email;
            existingUser.PhoneNumber = user.PhoneNumber;
            existingUser.Type = (int)user.Type;
            existingUser.RegistrationDate = user.RegistrationDate;

            context.SaveChanges();
        }

        public void DeleteUser(int id)
        {
            using var context = new LibraryDbContext(_options);

            var user = context.Users.Find(id);
            if (user != null)
            {
                context.Users.Remove(user);
                context.SaveChanges();
            }
        }

        private static IUser MapToUser(UserEntity entity)
        {
            return UserFactory.CreateUser(
                entity.Id,
                entity.Name,
                entity.Email,
                entity.PhoneNumber,
                (UserType)entity.Type,
                entity.RegistrationDate
            );
        }
    }
}