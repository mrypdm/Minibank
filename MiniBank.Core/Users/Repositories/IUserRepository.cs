using System.Collections.Generic;

namespace MiniBank.Core.Users.Repositories
{
    public interface IUserRepository
    {
        bool ExistsWithId(string id);
        User GetById(string id);
        IEnumerable<User> GetAll();
        void Create(User user);
        void Update(User user);
        void DeleteById(string id);
    }
}