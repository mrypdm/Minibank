using System.Collections.Generic;
using MiniBank.Core.Transfers;

namespace MiniBank.Core.BankAccounts.Services
{
    public interface IBankAccountService
    {
        BankAccount GetById(string id);
        IEnumerable<BankAccount> GetAll();
        void Create(BankAccount account);
        void CloseById(string id);
        double CalculateTransferCommission(Transfer transferInfo);
        void MakeTransfer(Transfer transferInfo);
    }
}