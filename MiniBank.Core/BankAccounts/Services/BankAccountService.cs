using System;
using System.Collections.Generic;
using MiniBank.Core.BankAccounts.Repositories;
using MiniBank.Core.Currencies;
using MiniBank.Core.Exceptions;
using MiniBank.Core.Transfers;
using MiniBank.Core.Transfers.Repositories;
using MiniBank.Core.Users.Repositories;

namespace MiniBank.Core.BankAccounts.Services
{
    public class BankAccountService : IBankAccountService
    {
        private readonly IBankAccountRepository _accountRepository;
        private readonly IUserRepository _userRepository;
        private readonly ITransferRepository _transferRepository;
        private readonly ICurrencyConverter _converter;

        public BankAccountService(IBankAccountRepository accountRepository, IUserRepository userRepository,
            ITransferRepository transferRepository, ICurrencyConverter converter)
        {
            _accountRepository = accountRepository;
            _userRepository = userRepository;
            _transferRepository = transferRepository;
            _converter = converter;
        }

        public BankAccount GetById(string id)
        {
            return _accountRepository.GetById(id);
        }

        public IEnumerable<BankAccount> GetAll()
        {
            return _accountRepository.GetAll();
        }

        public void Create(BankAccount account)
        {
            if (!_userRepository.ExistsWithId(account.UserId))
            {
                throw new ValidationException($"Can't create account because user with id {account.UserId} doesn't exist");
            }

            account.Id = Guid.NewGuid().ToString();
            account.OpeningDate = DateTime.Now;

            _accountRepository.Create(account);
        }

        public void CloseById(string id)
        {
            var account = _accountRepository.GetById(id);

            if (account.IsClosed)
            {
                throw new ValidationException("Account already closed");
            }

            if (account.Amount != 0)
            {
                throw new ValidationException("Can't close an account that has money in it");
            }

            account.ClosingDate = DateTime.Now;
            account.IsClosed = true;

            _accountRepository.Update(account);
        }

        public double CalculateTransferCommission(Transfer transferInfo)
        {
            var fromAccount = _accountRepository.GetById(transferInfo.FromAccountId);
            var toAccount = _accountRepository.GetById(transferInfo.ToAccountId);

            if (fromAccount.UserId == toAccount.UserId)
            {
                return 0;
            }

            return Math.Round(transferInfo.Amount * 0.02, 2);
        }

        public void MakeTransfer(Transfer transferInfo)
        {
            var fromAccount = _accountRepository.GetById(transferInfo.FromAccountId);
            var toAccount = _accountRepository.GetById(transferInfo.ToAccountId);

            if (fromAccount.IsClosed)
            {
                throw new ValidationException("Sender account is closed");
            }

            if (toAccount.IsClosed)
            {
                throw new ValidationException("Beneficiary's account is closed");
            }

            if (fromAccount.Amount < transferInfo.Amount)
            {
                throw new ValidationException("Insufficient funds on the sender's account");
            }

            double newAmount = 
                fromAccount.CurrencyCode == toAccount.CurrencyCode
                ? transferInfo.Amount
                : _converter.Convert(transferInfo.Amount, fromAccount.CurrencyCode, toAccount.CurrencyCode);

            if (fromAccount.UserId != toAccount.UserId)
            {
                newAmount = Math.Round(0.98 * newAmount, 2);
            }

            fromAccount.Amount -= transferInfo.Amount;
            toAccount.Amount += newAmount;

            _accountRepository.Update(fromAccount);
            _accountRepository.Update(toAccount);

            transferInfo.Id = Guid.NewGuid().ToString();
            transferInfo.TransferDateTime = DateTime.Now;

            _transferRepository.Create(transferInfo);
        }
    }
}