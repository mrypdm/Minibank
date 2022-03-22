using System.Collections.Generic;

namespace MiniBank.Core.Users.Services
{
    public interface IUserService
    {
        User GetById(string id);
        IEnumerable<User> GetAll();
        void Create(User user);
        void Update(User user);
        void DeleteById(string id);
    }
}