using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MiniBank.Core.Exceptions;
using MiniBank.Core.Transfers;
using MiniBank.Core.Transfers.Repositories;
using MiniBank.Data.Context;
using MiniBank.Data.Transfers.Mappers;

namespace MiniBank.Data.Transfers.Repositories
{
    public class TransferRepository : ITransferRepository
    {
        private readonly MiniBankContext _context;

        public TransferRepository(MiniBankContext context)
        {
            _context = context;
        }

        public async Task<Transfer> GetById(string id, CancellationToken token)
        {
            var dbModel = await _context.Transfers.FirstOrDefaultAsync(transfer => transfer.Id == id, token);

            if (dbModel == null)
            {
                throw new ValidationException($"Transfer with id {id} doesn't exist");
            }

            return dbModel.ToModel();
        }

        public async Task Create(Transfer transfer, CancellationToken token)
        {
            await _context.Transfers.AddAsync(transfer.ToDbModel(), token);
        }
    }
}