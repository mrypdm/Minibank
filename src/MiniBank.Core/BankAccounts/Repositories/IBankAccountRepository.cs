using System.Threading;
using System.Threading.Tasks;

namespace MiniBank.Core.BankAccounts.Repositories
{
    public interface IBankAccountRepository
    {
        Task<bool> ExistsWithId(string id, CancellationToken token);
        Task<BankAccount> GetById(string id, CancellationToken token);
        Task Create(BankAccount account, CancellationToken token);
        Task Update(BankAccount account, CancellationToken token);
        Task<bool> ExistsAccountsForUserById(string userId, CancellationToken token);
    }
}