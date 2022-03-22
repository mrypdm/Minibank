using System.Collections.Generic;

namespace MiniBank.Core.Transfers.Repositories
{
    public interface ITransferRepository
    {
        Transfer GetById(string id);
        IEnumerable<Transfer> GetAll();
        void Create(Transfer transfer);
    }
}