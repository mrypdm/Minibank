using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MiniBank.Core.BankAccounts.Repositories;
using MiniBank.Core.Currencies;
using MiniBank.Core.Transfers;
using MiniBank.Core.Transfers.Repositories;
using ValidationException = MiniBank.Core.Exceptions.ValidationException;

namespace MiniBank.Core.BankAccounts.Services
{
    public class BankAccountService : IBankAccountService
    {
        private readonly IBankAccountRepository _accountRepository;
        private readonly ITransferRepository _transferRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrencyConverter _converter;

        private readonly IValidator<BankAccount> _accountValidator;
        private readonly IValidator<Transfer> _transferValidator;

        public BankAccountService(IBankAccountRepository accountRepository, ITransferRepository transferRepository,
            ICurrencyConverter converter, IUnitOfWork unitOfWork,
            IValidator<BankAccount> accountValidator, IValidator<Transfer> transferValidator)
        {
            _accountRepository = accountRepository;
            _transferRepository = transferRepository;
            _converter = converter;
            _unitOfWork = unitOfWork;
            _accountValidator = accountValidator;
            _transferValidator = transferValidator;
        }

        public async Task Create(BankAccount account, CancellationToken token)
        {
            await _accountValidator.ValidateAndThrowAsync(account, token);

            account.Id = Guid.NewGuid().ToString();
            account.OpeningDate = DateTime.Now;

            await _accountRepository.Create(account, token);

            await _unitOfWork.SaveChanges(token);
        }

        public async Task CloseById(string id, CancellationToken token)
        {
            var account = await _accountRepository.GetById(id, token);

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

            await _accountRepository.Update(account, token);

            await _unitOfWork.SaveChanges(token);
        }

        public async Task<double> CalculateTransferCommission(Transfer transferInfo, CancellationToken token)
        {
            await _transferValidator.ValidateAndThrowAsync(transferInfo, token);

            var fromAccount = await _accountRepository.GetById(transferInfo.FromAccountId, token);
            var toAccount = await _accountRepository.GetById(transferInfo.ToAccountId, token);

            if (fromAccount.UserId == toAccount.UserId)
            {
                return 0;
            }

            return Math.Round(transferInfo.Amount * 0.02, 2);
        }

        public async Task MakeTransfer(Transfer transferInfo, CancellationToken token)
        {
            await _transferValidator.ValidateAndThrowAsync(transferInfo, token);

            var fromAccount = await _accountRepository.GetById(transferInfo.FromAccountId, token);
            var toAccount = await _accountRepository.GetById(transferInfo.ToAccountId, token);

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

            transferInfo.CurrencyCode = fromAccount.CurrencyCode;

            double newAmount =
                fromAccount.CurrencyCode == toAccount.CurrencyCode
                    ? transferInfo.Amount
                    : await _converter.Convert(transferInfo.Amount, fromAccount.CurrencyCode, toAccount.CurrencyCode);

            if (fromAccount.UserId != toAccount.UserId)
            {
                newAmount = Math.Round(0.98 * newAmount, 2);
            }

            fromAccount.Amount -= transferInfo.Amount;
            toAccount.Amount += newAmount;

            await _accountRepository.Update(fromAccount, token);
            await _accountRepository.Update(toAccount, token);

            transferInfo.Id = Guid.NewGuid().ToString();
            transferInfo.TransferDateTime = DateTime.Now;

            await _transferRepository.Create(transferInfo, token);

            await _unitOfWork.SaveChanges(token);
        }
    }
}