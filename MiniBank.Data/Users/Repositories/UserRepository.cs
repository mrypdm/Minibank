using System.Collections.Generic;
using System.Linq;
using MiniBank.Core.Exceptions;
using MiniBank.Core.Users;
using MiniBank.Core.Users.Repositories;
using MiniBank.Data.Users.Mappers;

namespace MiniBank.Data.Users.Repositories
{
    public class UserRepository : IUserRepository
    {
        private static readonly List<UserDbModel> UsersStorage = new();

        public bool ExistsWithId(string id)
        {
            return UsersStorage.Any(user => user.Id == id);
        }

        public User GetById(string id)
        {
            var dbModel = UsersStorage.Find(u => u.Id == id);

            if (dbModel == null)
            {
                throw new ValidationException($"User with id {id} doesn't exist");
            }
            
            return dbModel.ToModel();
        }

        public IEnumerable<User> GetAll()
        {
            return UsersStorage.Select(user => user.ToModel());
        }

        public void Create(User user)
        {
            UsersStorage.Add(user.ToDbModel());
        }

        public void Update(User user)
        {
            var dbModel = UsersStorage.Find(u => u.Id == user.Id);

            if (dbModel == null)
            {
                throw new ValidationException($"User with id {user.Id} doesn't exist");
            }

            dbModel.Login = user.Login;
            dbModel.Email = user.Email;
        }

        public void DeleteById(string id)
        {
            var dbModel = UsersStorage.Find(u => u.Id == id);

            if (dbModel == null)
            {
                throw new ValidationException($"User with id {id} doesn't exist");
            }

            UsersStorage.Remove(dbModel);
        }
    }
}