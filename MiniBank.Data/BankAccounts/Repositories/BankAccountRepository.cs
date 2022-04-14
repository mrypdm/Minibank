using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MiniBank.Core.BankAccounts;
using MiniBank.Core.BankAccounts.Repositories;
using MiniBank.Core.Exceptions;
using MiniBank.Data.BankAccounts.Mappers;
using MiniBank.Data.Context;

namespace MiniBank.Data.BankAccounts.Repositories
{
    public class BankAccountRepository : IBankAccountRepository
    {
        private readonly MiniBankContext _context;

        public BankAccountRepository(MiniBankContext context)
        {
            _context = context;
        }

        public Task<bool> ExistsWithId(string id, CancellationToken token)
        {
            return _context.BankAccounts.AnyAsync(ac => ac.Id == id, token);
        }

        public async Task<BankAccount> GetById(string id, CancellationToken token)
        {
            var dbModel = await _context.BankAccounts.FirstOrDefaultAsync(ac => ac.Id == id, token);

            if (dbModel == null)
            {
                throw new ValidationException($"Account with id {id} doesn't exist");
            }

            return dbModel.ToModel();
        }

        public async Task Create(BankAccount account, CancellationToken token)
        {
            await _context.BankAccounts.AddAsync(account.ToDbModel(), token);
        }

        public async Task Update(BankAccount account, CancellationToken token)
        {
            var dbModel = await _context.BankAccounts.FirstOrDefaultAsync(ac => ac.Id == account.Id, token);

            if (dbModel == null)
            {
                throw new ValidationException($"Account with id {account.Id} doesn't exist");
            }

            dbModel.UserId = account.UserId;
            dbModel.Amount = account.Amount;
            dbModel.CurrencyCode = account.CurrencyCode;
            dbModel.OpeningDate = account.OpeningDate;
            dbModel.ClosingDate = account.ClosingDate;
            dbModel.IsClosed = account.IsClosed;
        }

        public Task<bool> ExistsAccountsForUserById(string userId, CancellationToken token)
        {
            return _context.BankAccounts.AnyAsync(ac => ac.UserId == userId, token);
        }
    }
}