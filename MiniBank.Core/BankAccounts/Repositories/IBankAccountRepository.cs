using System.Collections.Generic;

namespace MiniBank.Core.BankAccounts.Repositories
{
    public interface IBankAccountRepository
    {
        BankAccount GetById(string id);
        IEnumerable<BankAccount> GetAll();
        void Create(BankAccount account);
        void Update(BankAccount account);
        bool ExistsAccountsForUserById(string userId);
    }
}