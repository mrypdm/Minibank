using System;
using System.Threading;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using MiniBank.Core.BankAccounts;
using MiniBank.Core.BankAccounts.Repositories;
using MiniBank.Core.BankAccounts.Services;
using MiniBank.Core.Currencies;
using MiniBank.Core.DateTimes;
using MiniBank.Core.Transfers;
using MiniBank.Core.Transfers.Repositories;
using Xunit;
using Moq;

namespace MiniBank.Core.Tests
{
    public class BankAccountTests
    {
        private readonly Mock<IBankAccountRepository> _accountRepositoryMock;
        private readonly Mock<ITransferRepository> _transferRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<ICurrencyConverter> _currencyConverterMock;
        private readonly Mock<IValidator<BankAccount>> _accountValidatorMock;
        private readonly Mock<IValidator<Transfer>> _transferValidatorMock;
        private readonly Mock<IDateTimeProvider> _dateTimeProviderMock;

        private static readonly ILogger<BankAccountService> Logger =
            LoggerFactory.Create(logBuilder => logBuilder.AddConsole()).CreateLogger<BankAccountService>();

        private readonly IBankAccountService _accountService;

        public BankAccountTests()
        {
            _accountRepositoryMock = new Mock<IBankAccountRepository>();
            _transferRepositoryMock = new Mock<ITransferRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _currencyConverterMock = new Mock<ICurrencyConverter>();
            _accountValidatorMock = new Mock<IValidator<BankAccount>>();
            _transferValidatorMock = new Mock<IValidator<Transfer>>();
            _dateTimeProviderMock = new Mock<IDateTimeProvider>();

            _accountService = new BankAccountService(_accountRepositoryMock.Object, _transferRepositoryMock.Object,
                _currencyConverterMock.Object, _unitOfWorkMock.Object, _accountValidatorMock.Object,
                _transferValidatorMock.Object, _dateTimeProviderMock.Object, Logger);
        }

        #region Tests for CreateAcccount

        [Fact]
        public async void CreateAccount_SuccessPath()
        {
            // ARRANGE

            var account = new BankAccount
            {
                UserId = "some id",
                CurrencyCode = CurrencyCodes.RUB,
                Amount = 0
            };

            var expectedDateTime = DateTime.Now;
            _dateTimeProviderMock.Setup(provider => provider.Now).Returns(expectedDateTime);

            // ACT

            await _accountService.Create(account, CancellationToken.None);

            // ASSERT

            Assert.Equal(expectedDateTime, account.OpeningDate);

            _accountRepositoryMock.Verify(repo => repo.Create(account, It.IsAny<CancellationToken>()), Times.Once);
            _accountValidatorMock.Verify(
                validator =>
                    validator.ValidateAsync(
                        It.Is<ValidationContext<BankAccount>>(context => context.InstanceToValidate == account),
                        It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkMock.Verify(unit => unit.SaveChanges(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async void CreateAccount_NegativeAmount_Throw()
        {
            // ARRANGE

            var account = new BankAccount
            {
                UserId = "some id",
                CurrencyCode = CurrencyCodes.RUB,
                Amount = -1
            };

            var expectedException = new ValidationException(new[]
            {
                new ValidationFailure(nameof(BankAccount.Amount), "")
            });

            _accountValidatorMock
                .Setup(validator =>
                    validator.ValidateAsync(It.IsAny<ValidationContext<BankAccount>>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(expectedException);

            // ACT

            var exception =
                await Assert.ThrowsAsync<ValidationException>(() =>
                    _accountService.Create(account, CancellationToken.None));

            // ASSERT

            Assert.Equal(expectedException, exception);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async void CreateAccount_InvalidUserId_Throw(string userId)
        {
            // ARRANGE

            var account = new BankAccount
            {
                UserId = userId,
                CurrencyCode = CurrencyCodes.RUB,
                Amount = 0
            };

            var expectedException = new ValidationException(new[]
            {
                new ValidationFailure(nameof(BankAccount.UserId), "")
            });

            _accountValidatorMock
                .Setup(validator =>
                    validator.ValidateAsync(It.IsAny<ValidationContext<BankAccount>>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(expectedException);

            // ACT

            var exception =
                await Assert.ThrowsAsync<ValidationException>(() =>
                    _accountService.Create(account, CancellationToken.None));

            // ASSERT

            Assert.Equal(expectedException, exception);
        }

        [Fact]
        public async void CreateAccount_UserNotExists_Throw()
        {
            // ARRANGE

            var account = new BankAccount
            {
                UserId = "not exist",
                CurrencyCode = CurrencyCodes.RUB,
                Amount = 0
            };

            var expectedException = new ValidationException(new[]
            {
                new ValidationFailure(nameof(BankAccount.UserId), "")
            });

            _accountValidatorMock
                .Setup(validator =>
                    validator.ValidateAsync(It.IsAny<ValidationContext<BankAccount>>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(expectedException);

            // ACT

            var exception =
                await Assert.ThrowsAsync<ValidationException>(() =>
                    _accountService.Create(account, CancellationToken.None));

            // ASSERT

            Assert.Equal(expectedException, exception);
        }

        #endregion

        #region Tests for CloseById

        [Fact]
        public async void CloseById_SuccessPath()
        {
            // ARRANGE

            var account = new BankAccount
            {
                Amount = 0,
                IsClosed = false,
                ClosingDate = null
            };

            _accountRepositoryMock
                .Setup(repo => repo.GetById(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(account);

            var expectedDateTime = DateTime.Now;
            _dateTimeProviderMock.Setup(provider => provider.Now).Returns(expectedDateTime);

            // ACT

            await _accountService.CloseById("some id", CancellationToken.None);

            // ASSERT

            Assert.True(account.IsClosed);
            Assert.Equal(expectedDateTime, account.ClosingDate);

            _accountRepositoryMock.Verify(repo => repo.Update(account, It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkMock.Verify(unit => unit.SaveChanges(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async void CloseById_InvalidId_Throw(string id)
        {
            // ARRANGE

            var expectedException = new Exceptions.ValidationException("");

            _accountRepositoryMock.Setup(repo => repo.GetById(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(expectedException);

            // ACT

            var exception =
                await Assert.ThrowsAsync<Exceptions.ValidationException>(() =>
                    _accountService.CloseById(id, CancellationToken.None));

            // ASSERT

            Assert.Equal(expectedException, exception);
        }

        [Fact]
        public async void CloseById_AccountHasMoney_Throw()
        {
            // ARRANGE

            var account = new BankAccount
            {
                Amount = 10,
                IsClosed = false,
                ClosingDate = null
            };

            _accountRepositoryMock
                .Setup(repo => repo.GetById(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(account);

            // ACT

            var exception =
                await Assert.ThrowsAsync<Exceptions.ValidationException>(() =>
                    _accountService.CloseById("some id", CancellationToken.None));

            // ASSERT

            Assert.Equal(Exceptions.ValidationException.ClosingAccountHasMoneyException.Message, exception.Message);
        }

        [Fact]
        public async void CloseById_AccountAlreadyClosed_Throw()
        {
            // ARRANGE

            var account = new BankAccount
            {
                Amount = 0,
                IsClosed = true,
                ClosingDate = DateTime.Now
            };

            _accountRepositoryMock
                .Setup(repo => repo.GetById(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(account);

            // ACT

            var exception =
                await Assert.ThrowsAsync<Exceptions.ValidationException>(() =>
                    _accountService.CloseById("some id", CancellationToken.None));

            // ASSERT

            Assert.Equal(Exceptions.ValidationException.ClosingAccountAlreadyClosedException.Message,
                exception.Message);
        }

        #endregion

        #region Tests for CalculateCommission

        [Fact]
        public async void CalculateCommission_DiffUsers_Return2Percents()
        {
            // ARRANGE

            var transfer = new Transfer
            {
                Amount = 17.77,
                FromAccountId = "id1",
                ToAccountId = "id2"
            };

            var fromAccount = new BankAccount
            {
                UserId = "userid1"
            };

            var toAccount = new BankAccount
            {
                UserId = "userid2"
            };

            double expectedCommission = Math.Round(transfer.Amount * 0.02, 2);

            _accountRepositoryMock
                .Setup(repo => repo.GetById(transfer.FromAccountId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(fromAccount);
            _accountRepositoryMock
                .Setup(repo => repo.GetById(transfer.ToAccountId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(toAccount);

            // ACT

            var result = await _accountService.CalculateTransferCommission(transfer, CancellationToken.None);

            // ASSERT

            Assert.Equal(expectedCommission, result);

            _transferValidatorMock.Verify(
                validator =>
                    validator.ValidateAsync(
                        It.Is<ValidationContext<Transfer>>(context => context.InstanceToValidate == transfer),
                        It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async void CalculateCommission_SameUser_Return0()
        {
            // ARRANGE

            var transfer = new Transfer
            {
                Amount = 100,
                FromAccountId = "id1",
                ToAccountId = "id2"
            };

            var account = new BankAccount
            {
                UserId = "userid1"
            };

            _accountRepositoryMock.Setup(repo => repo.GetById(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(account);

            // ACT

            var result = await _accountService.CalculateTransferCommission(transfer, CancellationToken.None);

            // ASSERT

            Assert.Equal(0, result);

            _transferValidatorMock.Verify(
                validator =>
                    validator.ValidateAsync(
                        It.Is<ValidationContext<Transfer>>(context => context.InstanceToValidate == transfer),
                        It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async void CalculateCommission_NegativeAmount_Throw()
        {
            // ARRANGE

            var transfer = new Transfer
            {
                Amount = -100
            };

            var expectedException = new ValidationException(new[]
            {
                new ValidationFailure(nameof(transfer.Amount), "")
            });

            _transferValidatorMock
                .Setup(validator =>
                    validator.ValidateAsync(It.IsAny<ValidationContext<Transfer>>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(expectedException);

            // ACT

            var exception =
                await Assert.ThrowsAsync<ValidationException>(() =>
                    _accountService.MakeTransfer(transfer, CancellationToken.None));

            // ASSERT

            Assert.Equal(expectedException, exception);
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData("", "")]
        [InlineData(" ", " ")]
        public async void CalculateCommission_InvalidAccountsIds_Throw(string fromAccountId, string toAccountId)
        {
            // ARRANGE

            var transfer = new Transfer
            {
                Amount = 0,
                FromAccountId = fromAccountId,
                ToAccountId = toAccountId
            };

            var expectedException = new ValidationException(new[]
            {
                new ValidationFailure(nameof(transfer.FromAccountId), ""),
                new ValidationFailure(nameof(transfer.ToAccountId), "")
            });

            _transferValidatorMock
                .Setup(validator =>
                    validator.ValidateAsync(It.IsAny<ValidationContext<Transfer>>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(expectedException);

            // ACT

            var exception =
                await Assert.ThrowsAsync<ValidationException>(() =>
                    _accountService.MakeTransfer(transfer, CancellationToken.None));

            // ASSERT

            Assert.Equal(expectedException, exception);
        }

        [Fact]
        public async void CalculateCommission_AccountsNotExist_Throw()
        {
            // ARRANGE

            var transfer = new Transfer
            {
                Amount = 0,
                FromAccountId = "not exist",
                ToAccountId = "not exits"
            };

            var expectedException = new Exceptions.ValidationException("");

            _accountRepositoryMock
                .Setup(repo => repo.GetById(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(expectedException);

            // ACT

            var exception =
                await Assert.ThrowsAsync<Exceptions.ValidationException>(() =>
                    _accountService.MakeTransfer(transfer, CancellationToken.None));

            // ASSERT

            Assert.Equal(expectedException, exception);

            _transferValidatorMock.Verify(
                validator =>
                    validator.ValidateAsync(
                        It.Is<ValidationContext<Transfer>>(context => context.InstanceToValidate == transfer),
                        It.IsAny<CancellationToken>()), Times.Once);
        }

        #endregion

        #region Tests for MakeTransfer

        [Fact]
        public async void MakeTransfer_SameCurrenciesDiffUsers_Success()
        {
            // ARRANGE

            var transfer = new Transfer
            {
                Amount = 100,
                FromAccountId = "id1",
                ToAccountId = "id2",
                Id = null,
                TransferDateTime = default
            };

            double fromAccountAmount = 1000;

            var fromAccount = new BankAccount
            {
                UserId = "userid1",
                CurrencyCode = CurrencyCodes.USD,
                Amount = fromAccountAmount,
                IsClosed = false
            };

            var toAccount = new BankAccount
            {
                UserId = "userid2",
                CurrencyCode = CurrencyCodes.USD,
                Amount = 0,
                IsClosed = false
            };

            _accountRepositoryMock
                .Setup(repo => repo.GetById(transfer.FromAccountId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(fromAccount);
            _accountRepositoryMock
                .Setup(repo => repo.GetById(transfer.ToAccountId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(toAccount);

            var expectedDateTime = DateTime.Now;
            _dateTimeProviderMock.Setup(provider => provider.Now).Returns(expectedDateTime);

            double expectedNewAmount = Math.Round(transfer.Amount * 0.98, 2);

            // ACT

            await _accountService.MakeTransfer(transfer, CancellationToken.None);

            // ASSERT

            Assert.Equal(fromAccountAmount - transfer.Amount, fromAccount.Amount);
            Assert.Equal(expectedNewAmount, toAccount.Amount);
            Assert.NotNull(transfer.Id);
            Assert.Equal(expectedDateTime, transfer.TransferDateTime);

            _accountRepositoryMock.Verify(repo => repo.Update(fromAccount, It.IsAny<CancellationToken>()), Times.Once);
            _accountRepositoryMock.Verify(repo => repo.Update(toAccount, It.IsAny<CancellationToken>()), Times.Once);
            _transferRepositoryMock.Verify(repo => repo.Create(transfer, It.IsAny<CancellationToken>()), Times.Once);
            _transferValidatorMock.Verify(
                validator =>
                    validator.ValidateAsync(
                        It.Is<ValidationContext<Transfer>>(context => context.InstanceToValidate == transfer),
                        It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkMock.Verify(unit => unit.SaveChanges(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async void MakeTransfer_DiffCurrenciesDiffUsers_Success()
        {
            // ARRANGE

            var transfer = new Transfer
            {
                Amount = 17.7,
                FromAccountId = "id1",
                ToAccountId = "id2",
                CurrencyCode = default
            };

            double fromAccountAmount = 1000;

            var fromAccount = new BankAccount
            {
                UserId = "userid1",
                CurrencyCode = CurrencyCodes.USD,
                Amount = fromAccountAmount,
                IsClosed = false
            };

            var toAccount = new BankAccount
            {
                UserId = "userid2",
                CurrencyCode = CurrencyCodes.RUB,
                Amount = 0,
                IsClosed = false
            };

            double convertedAmount = 10.17;

            _currencyConverterMock
                .Setup(converter =>
                    converter.Convert(It.IsAny<double>(), It.IsAny<CurrencyCodes>(), It.IsAny<CurrencyCodes>()))
                .ReturnsAsync(convertedAmount);

            _accountRepositoryMock
                .Setup(repo => repo.GetById(transfer.FromAccountId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(fromAccount);
            _accountRepositoryMock
                .Setup(repo => repo.GetById(transfer.ToAccountId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(toAccount);

            var expectedDateTime = DateTime.Now;
            _dateTimeProviderMock.Setup(provider => provider.Now).Returns(expectedDateTime);

            var expectedNewAmount = Math.Round(0.98 * convertedAmount, 2);

            // ACT

            await _accountService.MakeTransfer(transfer, CancellationToken.None);

            // ASSERT

            Assert.Equal(fromAccountAmount - transfer.Amount, fromAccount.Amount);
            Assert.Equal(expectedNewAmount, toAccount.Amount);
            Assert.Equal(fromAccount.CurrencyCode, transfer.CurrencyCode);
            Assert.NotNull(transfer.Id);
            Assert.Equal(expectedDateTime, transfer.TransferDateTime);

            _accountRepositoryMock.Verify(repo => repo.Update(fromAccount, It.IsAny<CancellationToken>()), Times.Once);
            _accountRepositoryMock.Verify(repo => repo.Update(toAccount, It.IsAny<CancellationToken>()), Times.Once);
            _transferRepositoryMock.Verify(repo => repo.Create(transfer, It.IsAny<CancellationToken>()), Times.Once);
            _transferValidatorMock.Verify(
                validator =>
                    validator.ValidateAsync(
                        It.Is<ValidationContext<Transfer>>(context => context.InstanceToValidate == transfer),
                        It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkMock.Verify(unit => unit.SaveChanges(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async void MakeTransfer_SameCurrenciesSameUser_Success()
        {
            // ARRANGE

            var transfer = new Transfer
            {
                Amount = 100,
                FromAccountId = "id1",
                ToAccountId = "id2",
                Id = null,
                TransferDateTime = default
            };

            double fromAccountAmount = 1000;

            var fromAccount = new BankAccount
            {
                UserId = "userid1",
                CurrencyCode = CurrencyCodes.USD,
                Amount = fromAccountAmount,
                IsClosed = false
            };

            var toAccount = new BankAccount
            {
                UserId = "userid1",
                CurrencyCode = CurrencyCodes.USD,
                Amount = 0,
                IsClosed = false
            };

            _accountRepositoryMock
                .Setup(repo => repo.GetById(transfer.FromAccountId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(fromAccount);
            _accountRepositoryMock
                .Setup(repo => repo.GetById(transfer.ToAccountId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(toAccount);

            var expectedDateTime = DateTime.Now;
            _dateTimeProviderMock.Setup(provider => provider.Now).Returns(expectedDateTime);

            double expectedNewAmount = transfer.Amount;

            // ACT

            await _accountService.MakeTransfer(transfer, CancellationToken.None);

            // ASSERT

            Assert.Equal(fromAccountAmount - transfer.Amount, fromAccount.Amount);
            Assert.Equal(expectedNewAmount, toAccount.Amount);
            Assert.NotNull(transfer.Id);
            Assert.Equal(expectedDateTime, transfer.TransferDateTime);

            _accountRepositoryMock.Verify(repo => repo.Update(fromAccount, It.IsAny<CancellationToken>()), Times.Once);
            _accountRepositoryMock.Verify(repo => repo.Update(toAccount, It.IsAny<CancellationToken>()), Times.Once);
            _transferRepositoryMock.Verify(repo => repo.Create(transfer, It.IsAny<CancellationToken>()), Times.Once);
            _transferValidatorMock.Verify(
                validator =>
                    validator.ValidateAsync(
                        It.Is<ValidationContext<Transfer>>(context => context.InstanceToValidate == transfer),
                        It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkMock.Verify(unit => unit.SaveChanges(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async void MakeTransfer_DiffCurrenciesSameUser_Success()
        {
            // ARRANGE

            var transfer = new Transfer
            {
                Amount = 17.7,
                FromAccountId = "id1",
                ToAccountId = "id2",
                CurrencyCode = default
            };

            double fromAccountAmount = 1000;

            var fromAccount = new BankAccount
            {
                UserId = "userid1",
                CurrencyCode = CurrencyCodes.USD,
                Amount = fromAccountAmount,
                IsClosed = false
            };

            var toAccount = new BankAccount
            {
                UserId = "userid1",
                CurrencyCode = CurrencyCodes.RUB,
                Amount = 0,
                IsClosed = false
            };

            double convertedAmount = 10.17;

            _currencyConverterMock
                .Setup(converter =>
                    converter.Convert(It.IsAny<double>(), It.IsAny<CurrencyCodes>(), It.IsAny<CurrencyCodes>()))
                .ReturnsAsync(convertedAmount);

            _accountRepositoryMock
                .Setup(repo => repo.GetById(transfer.FromAccountId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(fromAccount);
            _accountRepositoryMock
                .Setup(repo => repo.GetById(transfer.ToAccountId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(toAccount);

            var expectedDateTime = DateTime.Now;
            _dateTimeProviderMock.Setup(provider => provider.Now).Returns(expectedDateTime);

            var expectedNewAmount = convertedAmount;

            // ACT

            await _accountService.MakeTransfer(transfer, CancellationToken.None);

            // ASSERT

            Assert.Equal(fromAccountAmount - transfer.Amount, fromAccount.Amount);
            Assert.Equal(expectedNewAmount, toAccount.Amount);
            Assert.Equal(fromAccount.CurrencyCode, transfer.CurrencyCode);
            Assert.NotNull(transfer.Id);
            Assert.Equal(expectedDateTime, transfer.TransferDateTime);

            _accountRepositoryMock.Verify(repo => repo.Update(fromAccount, It.IsAny<CancellationToken>()), Times.Once);
            _accountRepositoryMock.Verify(repo => repo.Update(toAccount, It.IsAny<CancellationToken>()), Times.Once);
            _transferRepositoryMock.Verify(repo => repo.Create(transfer, It.IsAny<CancellationToken>()), Times.Once);
            _transferValidatorMock.Verify(
                validator =>
                    validator.ValidateAsync(
                        It.Is<ValidationContext<Transfer>>(context => context.InstanceToValidate == transfer),
                        It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkMock.Verify(unit => unit.SaveChanges(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async void MakeTransfer_NegativeAmount_Throw()
        {
            // ARRANGE

            var transfer = new Transfer
            {
                Amount = -100,
                FromAccountId = "id1",
                ToAccountId = "id2"
            };

            var expectedException = new ValidationException(new[]
            {
                new ValidationFailure(nameof(transfer.Amount), "")
            });

            _transferValidatorMock
                .Setup(validator =>
                    validator.ValidateAsync(It.IsAny<ValidationContext<Transfer>>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(expectedException);

            // ACT

            var exception =
                await Assert.ThrowsAsync<ValidationException>(() =>
                    _accountService.MakeTransfer(transfer, CancellationToken.None));

            // ASSERT

            Assert.Equal(expectedException, exception);
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData("", "")]
        [InlineData(" ", " ")]
        public async void MakeTransfer_InvalidAccountIds_Throw(string fromAccountId, string toAccountId)
        {
            // ARRANGE

            var transfer = new Transfer
            {
                Amount = 100,
                FromAccountId = fromAccountId,
                ToAccountId = toAccountId
            };

            var expectedException = new ValidationException(new[]
            {
                new ValidationFailure(nameof(transfer.FromAccountId), ""),
                new ValidationFailure(nameof(transfer.ToAccountId), "")
            });

            _transferValidatorMock
                .Setup(validator =>
                    validator.ValidateAsync(It.IsAny<ValidationContext<Transfer>>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(expectedException);

            // ACT

            var exception =
                await Assert.ThrowsAsync<ValidationException>(() =>
                    _accountService.MakeTransfer(transfer, CancellationToken.None));

            // ASSERT

            Assert.Equal(expectedException, exception);
        }

        [Fact]
        public async void MakeTransfer_AccountsNotExist_Throw()
        {
            // ARRANGE

            var transfer = new Transfer
            {
                Amount = 0,
                FromAccountId = "not exist",
                ToAccountId = "not exits"
            };

            var expectedException = new Exceptions.ValidationException("");

            _accountRepositoryMock
                .Setup(repo => repo.GetById(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(expectedException);

            // ACT

            var exception =
                await Assert.ThrowsAsync<Exceptions.ValidationException>(() =>
                    _accountService.MakeTransfer(transfer, CancellationToken.None));

            // ASSERT

            Assert.Equal(expectedException, exception);

            _transferValidatorMock.Verify(
                validator =>
                    validator.ValidateAsync(
                        It.Is<ValidationContext<Transfer>>(context => context.InstanceToValidate == transfer),
                        It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async void MakeTransfer_SenderAccountClosed_Throw()
        {
            // ARRANGE

            var transfer = new Transfer
            {
                Amount = 100,
                FromAccountId = "id1",
                ToAccountId = "id2",
            };

            var fromAccount = new BankAccount
            {
                UserId = "userid1",
                CurrencyCode = CurrencyCodes.USD,
                Amount = 0,
                IsClosed = true
            };

            var toAccount = new BankAccount
            {
                UserId = "userid2",
                CurrencyCode = CurrencyCodes.USD,
                Amount = 0,
                IsClosed = false
            };

            _accountRepositoryMock
                .Setup(repo => repo.GetById(transfer.FromAccountId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(fromAccount);
            _accountRepositoryMock
                .Setup(repo => repo.GetById(transfer.ToAccountId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(toAccount);

            // ACT

            var exception =
                await Assert.ThrowsAsync<Exceptions.ValidationException>(() =>
                    _accountService.MakeTransfer(transfer, CancellationToken.None));
            
            // ASSERT

            Assert.Equal(Exceptions.ValidationException.SenderAccountIsClosedException.Message, exception.Message);

            _transferValidatorMock.Verify(
                validator =>
                    validator.ValidateAsync(
                        It.Is<ValidationContext<Transfer>>(context => context.InstanceToValidate == transfer),
                        It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async void MakeTransfer_BeneficiaryAccountClosed_Throw()
        {
            // ARRANGE

            var transfer = new Transfer
            {
                Amount = 100,
                FromAccountId = "id1",
                ToAccountId = "id2",
            };

            var fromAccount = new BankAccount
            {
                UserId = "userid1",
                CurrencyCode = CurrencyCodes.USD,
                Amount = 0,
                IsClosed = false
            };

            var toAccount = new BankAccount
            {
                UserId = "userid2",
                CurrencyCode = CurrencyCodes.USD,
                Amount = 0,
                IsClosed = true
            };

            _accountRepositoryMock
                .Setup(repo => repo.GetById(transfer.FromAccountId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(fromAccount);
            _accountRepositoryMock
                .Setup(repo => repo.GetById(transfer.ToAccountId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(toAccount);

            // ACT

            var exception =
                await Assert.ThrowsAsync<Exceptions.ValidationException>(() =>
                    _accountService.MakeTransfer(transfer, CancellationToken.None));
            
            // ASSERT

            Assert.Equal(Exceptions.ValidationException.BeneficiaryAccountIsClosedException.Message, exception.Message);

            _transferValidatorMock.Verify(
                validator =>
                    validator.ValidateAsync(
                        It.Is<ValidationContext<Transfer>>(context => context.InstanceToValidate == transfer),
                        It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async void MakeTransfer_SenderDontHaveEnoughMoney_Throw()
        {
            // ARRANGE

            var transfer = new Transfer
            {
                Amount = 100,
                FromAccountId = "id1",
                ToAccountId = "id2",
            };

            var fromAccount = new BankAccount
            {
                UserId = "userid1",
                CurrencyCode = CurrencyCodes.USD,
                Amount = 0,
                IsClosed = false
            };

            var toAccount = new BankAccount
            {
                UserId = "userid2",
                CurrencyCode = CurrencyCodes.USD,
                Amount = 0,
                IsClosed = false
            };

            _accountRepositoryMock
                .Setup(repo => repo.GetById(transfer.FromAccountId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(fromAccount);
            _accountRepositoryMock
                .Setup(repo => repo.GetById(transfer.ToAccountId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(toAccount);

            // ACT

            var exception =
                await Assert.ThrowsAsync<Exceptions.ValidationException>(() =>
                    _accountService.MakeTransfer(transfer, CancellationToken.None));
            
            // ASSERT

            Assert.Equal(Exceptions.ValidationException.SenderDontHaveEnoughMoneyException.Message, exception.Message);

            _transferValidatorMock.Verify(
                validator =>
                    validator.ValidateAsync(
                        It.Is<ValidationContext<Transfer>>(context => context.InstanceToValidate == transfer),
                        It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async void MakeTransfer_BadAmount_ShouldRound()
        {
            // ARRANGE

            var transfer = new Transfer
            {
                Amount = 10.123,
                FromAccountId = "id1",
                ToAccountId = "id2",
                Id = null,
                TransferDateTime = default
            };

            double fromAccountAmount = 1000;

            var fromAccount = new BankAccount
            {
                UserId = "userid1",
                CurrencyCode = CurrencyCodes.USD,
                Amount = fromAccountAmount,
                IsClosed = false
            };

            var toAccount = new BankAccount
            {
                UserId = "userid1",
                CurrencyCode = CurrencyCodes.USD,
                Amount = 0,
                IsClosed = false
            };

            _accountRepositoryMock
                .Setup(repo => repo.GetById(transfer.FromAccountId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(fromAccount);
            _accountRepositoryMock
                .Setup(repo => repo.GetById(transfer.ToAccountId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(toAccount);

            var expectedDateTime = DateTime.Now;
            _dateTimeProviderMock.Setup(provider => provider.Now).Returns(expectedDateTime);

            // ACT

            await _accountService.MakeTransfer(transfer, CancellationToken.None);

            // ASSERT

            Assert.Equal(Math.Round(transfer.Amount, 2), transfer.Amount);

            Assert.Equal(transfer.Amount, toAccount.Amount);
            Assert.Equal(fromAccountAmount - transfer.Amount, fromAccount.Amount);
            Assert.NotNull(transfer.Id);
            Assert.Equal(expectedDateTime, transfer.TransferDateTime);

            _accountRepositoryMock.Verify(repo => repo.Update(fromAccount, It.IsAny<CancellationToken>()), Times.Once);
            _accountRepositoryMock.Verify(repo => repo.Update(toAccount, It.IsAny<CancellationToken>()), Times.Once);
            _transferRepositoryMock.Verify(repo => repo.Create(transfer, It.IsAny<CancellationToken>()), Times.Once);
            _transferValidatorMock.Verify(
                validator =>
                    validator.ValidateAsync(
                        It.Is<ValidationContext<Transfer>>(context => context.InstanceToValidate == transfer),
                        It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkMock.Verify(unit => unit.SaveChanges(It.IsAny<CancellationToken>()), Times.Once);
        }

        #endregion
    }
}