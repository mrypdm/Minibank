using System.Collections.Generic;
using System.Linq;
using MiniBank.Core.Exceptions;
using MiniBank.Core.Transfers;
using MiniBank.Core.Transfers.Repositories;
using MiniBank.Data.Transfers.Mappers;

namespace MiniBank.Data.Transfers.Repositories
{
    public class TransferRepository : ITransferRepository
    {
        private static readonly List<TransferDbModel> TransfersStorage = new();

        public Transfer GetById(string id)
        {
            var dbModel = TransfersStorage.Find(t => t.Id == id);

            if (dbModel == null)
            {
                throw new ValidationException($"Transfer with id {id} doesn't exist");
            }
            
            return dbModel.ToModel();
        }

        public IEnumerable<Transfer> GetAll()
        {
            return TransfersStorage.Select(transfer => transfer.ToModel());
        }

        public void Create(Transfer transfer)
        {
            TransfersStorage.Add(transfer.ToDbModel());
        }
    }
}