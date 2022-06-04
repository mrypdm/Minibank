using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.Extensions.Logging;
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
        private const double CommissionPercent = 0.02;

        private readonly IBankAccountRepository _accountRepository;
        private readonly ITransferRepository _transferRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrencyConverter _converter;
        private readonly IDateTimeProvider _dateTimeProvider;

        private readonly IValidator<BankAccount> _accountValidator;
        private readonly IValidator<Transfer> _transferValidator;

        private readonly ILogger<BankAccountService> _logger;

        public BankAccountService(IBankAccountRepository accountRepository, ITransferRepository transferRepository,
            ICurrencyConverter converter, IUnitOfWork unitOfWork,
            IValidator<BankAccount> accountValidator, IValidator<Transfer> transferValidator,
            IDateTimeProvider dateTimeProvider, ILogger<BankAccountService> logger)
        {
            _accountRepository = accountRepository;
            _transferRepository = transferRepository;
            _converter = converter;
            _unitOfWork = unitOfWork;
            _accountValidator = accountValidator;
            _transferValidator = transferValidator;
            _dateTimeProvider = dateTimeProvider;
            _logger = logger;
        }

        private double CalculateCommissionBetweenAccounts(double amount, BankAccount fromAccount,
            BankAccount toAccount)
        {
            var commission = fromAccount.UserId == toAccount.UserId ? 0.0 : Math.Round(amount * CommissionPercent, 2);

            _logger.LogInformation(
                "Calculated commission for amount='{Amount}' from account='{FromAccountId}' to account='{ToAccountId}' with result='{Commission}'",
                amount, fromAccount.Id, toAccount.Id, commission);

            return commission;
        }

        public async Task Create(BankAccount account, CancellationToken token)
        {
            await _accountValidator.ValidateAndThrowAsync(account, token);

            account.Id = Guid.NewGuid().ToString();
            account.OpeningDate = _dateTimeProvider.Now;

            await _accountRepository.Create(account, token);

            await _unitOfWork.SaveChanges(token);

            _logger.LogInformation("Created account with id='{AccountId}'", account.Id);
        }

        public async Task CloseById(string id, CancellationToken token)
        {
            var account = await _accountRepository.GetById(id, token);
            _logger.LogInformation("Get account with id='{AccountId}'", account.Id);

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

            _logger.LogInformation("Closed account with id='{AccountId}' at date='{ClosingDate}'", account.Id,
                account.ClosingDate);
        }

        public async Task<double> CalculateTransferCommission(Transfer transferInfo, CancellationToken token)
        {
            await _transferValidator.ValidateAndThrowAsync(transferInfo, token);

            var fromAccount = await _accountRepository.GetById(transferInfo.FromAccountId, token);
            var toAccount = await _accountRepository.GetById(transferInfo.ToAccountId, token);

            return CalculateCommissionBetweenAccounts(transferInfo.Amount, fromAccount, toAccount);
        }

        public async Task MakeTransfer(Transfer transferInfo, CancellationToken token)
        {
            await _transferValidator.ValidateAndThrowAsync(transferInfo, token);

            var fromAccount = await _accountRepository.GetById(transferInfo.FromAccountId, token);
            _logger.LogInformation("Get account with id='{AccountId}'", fromAccount.Id);

            var toAccount = await _accountRepository.GetById(transferInfo.ToAccountId, token);
            _logger.LogInformation("Get account with id='{AccountId}'", toAccount.Id);

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
            transferInfo.Amount = Math.Round(transferInfo.Amount, 2);

            var newAmount =
                fromAccount.CurrencyCode == toAccount.CurrencyCode
                    ? transferInfo.Amount
                    : await _converter.Convert(transferInfo.Amount, fromAccount.CurrencyCode, toAccount.CurrencyCode);

            if (fromAccount.UserId != toAccount.UserId)
            {
                var commission = CalculateCommissionBetweenAccounts(newAmount, fromAccount, toAccount);
                newAmount -= commission;
                _logger.LogInformation("Deducted commission='{Commission}'", commission);
            }

            fromAccount.Amount -= transferInfo.Amount;
            toAccount.Amount += newAmount;

            await _accountRepository.Update(fromAccount, token);
            await _accountRepository.Update(toAccount, token);

            transferInfo.Id = Guid.NewGuid().ToString();
            transferInfo.TransferDateTime = _dateTimeProvider.Now;

            await _transferRepository.Create(transferInfo, token);

            await _unitOfWork.SaveChanges(token);

            _logger.LogInformation("Updated account='{AccountId}'", fromAccount.Id);
            _logger.LogInformation("Updated account='{AccountId}'", toAccount.Id);
            _logger.LogInformation(
                "Created transfer with id='{TransferId}' at date='{TransferCreationDate}' from account='{FromAccountId}' to account='{ToAccountId}'",
                transferInfo.Id, transferInfo.TransferDateTime, fromAccount.Id, toAccount.Id);
        }
    }
}