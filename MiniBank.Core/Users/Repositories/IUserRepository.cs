using System.Threading;
using System.Threading.Tasks;

namespace MiniBank.Core.Users.Repositories
{
    public interface IUserRepository
    {
        Task<bool> ExistsWithId(string id, CancellationToken token);
        Task<User> GetById(string id, CancellationToken token);
        Task Create(User user, CancellationToken token);
        Task Update(User user, CancellationToken token);
        Task DeleteById(string id, CancellationToken token);
    }
}