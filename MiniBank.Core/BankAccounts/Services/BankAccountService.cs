using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MiniBank.Core.BankAccounts.Repositories;
using MiniBank.Core.Currencies;
using MiniBank.Core.DateTimes;
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
        private readonly IDateTimeProvider _dateTimeProvider;

        private readonly IValidator<BankAccount> _accountValidator;
        private readonly IValidator<Transfer> _transferValidator;

        public BankAccountService(IBankAccountRepository accountRepository, ITransferRepository transferRepository,
            ICurrencyConverter converter, IUnitOfWork unitOfWork,
            IValidator<BankAccount> accountValidator, IValidator<Transfer> transferValidator, IDateTimeProvider dateTimeProvider)
        {
            _accountRepository = accountRepository;
            _transferRepository = transferRepository;
            _converter = converter;
            _unitOfWork = unitOfWork;
            _accountValidator = accountValidator;
            _transferValidator = transferValidator;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task Create(BankAccount account, CancellationToken token)
        {
            await _accountValidator.ValidateAndThrowAsync(account, token);

            account.Id = Guid.NewGuid().ToString();
            account.OpeningDate = _dateTimeProvider.Now;

            await _accountRepository.Create(account, token);

            await _unitOfWork.SaveChanges(token);
        }

        public async Task CloseById(string id, CancellationToken token)
        {
            var account = await _accountRepository.GetById(id, token);

            if (account.IsClosed)
            {
                throw ValidationException.ClosingAccountAlreadyClosedException;
            }

            if (account.Amount != 0)
            {
                throw ValidationException.ClosingAccountHasMoneyException;
            }

            account.ClosingDate = _dateTimeProvider.Now;
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
                throw ValidationException.SenderAccountIsClosedException;
            }

            if (toAccount.IsClosed)
            {
                throw ValidationException.BeneficiaryAccountIsClosedException;
            }

            if (fromAccount.Amount < transferInfo.Amount)
            {
                throw ValidationException.SenderDontHaveEnoughMoneyException;
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
            transferInfo.TransferDateTime = _dateTimeProvider.Now;

            await _transferRepository.Create(transferInfo, token);

            await _unitOfWork.SaveChanges(token);
        }
    }
}