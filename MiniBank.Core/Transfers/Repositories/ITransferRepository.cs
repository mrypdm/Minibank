using System.Threading;
using System.Threading.Tasks;

namespace MiniBank.Core.Transfers.Repositories
{
    public interface ITransferRepository
    {
        Task<Transfer> GetById(string id, CancellationToken token);
        Task Create(Transfer transfer, CancellationToken token);
    }
}