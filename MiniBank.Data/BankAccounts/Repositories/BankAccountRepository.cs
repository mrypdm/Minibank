using System.Collections.Generic;
using System.Linq;
using MiniBank.Core.BankAccounts;
using MiniBank.Core.BankAccounts.Repositories;
using MiniBank.Core.Exceptions;
using MiniBank.Data.BankAccounts.Mappers;

namespace MiniBank.Data.BankAccounts.Repositories
{
    public class BankAccountRepository : IBankAccountRepository
    {
        private static readonly List<BankAccountDbModel> BankAccountsStorage = new();

        public BankAccount GetById(string id)
        {
            var dbModel = BankAccountsStorage.Find(ac => ac.Id == id);

            if (dbModel == null)
            {
                throw new ValidationException($"Account with id {id} doesn't exist");
            }
            
            return dbModel.ToModel();
        }

        public IEnumerable<BankAccount> GetAll()
        {
            return BankAccountsStorage.Select(account => account.ToModel());
        }

        public void Create(BankAccount account)
        {
            BankAccountsStorage.Add(account.ToDbModel());
        }

        public void Update(BankAccount account)
        {
            var dbModel = BankAccountsStorage.Find(ac => ac.Id == account.Id);

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

        public bool ExistsAccountsForUserById(string userId)
        {
            return BankAccountsStorage.Any(account => account.UserId == userId);
        }
    }
}