using System.Threading;
using System.Threading.Tasks;
using MiniBank.Core.Transfers;

namespace MiniBank.Core.BankAccounts.Services
{
    public interface IBankAccountService
    {
        Task Create(BankAccount account, CancellationToken token);
        Task CloseById(string id, CancellationToken token);
        Task<double> CalculateTransferCommission(Transfer transferInfo, CancellationToken token);
        Task MakeTransfer(Transfer transferInfo, CancellationToken token);
    }
}