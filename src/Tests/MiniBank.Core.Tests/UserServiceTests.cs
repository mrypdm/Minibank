using System.Threading;
using FluentValidation;
using FluentValidation.Results;
using MiniBank.Core.BankAccounts.Repositories;
using MiniBank.Core.Users;
using MiniBank.Core.Users.Repositories;
using MiniBank.Core.Users.Services;
using Moq;
using Xunit;

namespace MiniBank.Core.Tests
{
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IBankAccountRepository> _accountRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IValidator<User>> _userValidatorMock;

        private readonly IUserService _userService;

        public UserServiceTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _accountRepositoryMock = new Mock<IBankAccountRepository>();
            _userValidatorMock = new Mock<IValidator<User>>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();

            _userService = new UserService(_userRepositoryMock.Object, _accountRepositoryMock.Object,
                _unitOfWorkMock.Object, _userValidatorMock.Object);
        }

        #region Tests for GetUserById

        [Fact]
        public async void GetUserById_SuccessPath_ReturnUserModel()
        {
            // ARRANGE

            var expectedUser = new User();

            _userRepositoryMock.Setup(repo => repo.GetById(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(expectedUser);

            // ACT

            var user = await _userService.GetById("id", CancellationToken.None);
            
            // ASSERT

            Assert.Equal(expectedUser, user);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async void GetUserById_InvalidId_Throw(string id)
        {
            // ARRANGE

            var expectedException = new Exceptions.ValidationException("");

            _userRepositoryMock
                .Setup(repo => repo.GetById(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(expectedException);
            
            // ACT

            var exception = await Assert.ThrowsAsync<Exceptions.ValidationException>(() => _userService.GetById(id, CancellationToken.None));
            
            // ASSERT

            Assert.Equal(expectedException, exception);
        }

        #endregion

        #region Tests for CreateUser

        [Fact]
        public async void CreateUser_SuccessPath()
        {
            // ARRANGE

            var user = new User
            {
                Email = "some email",
                Login = "some login"
            };

            // ACT
            
            await _userService.Create(user, CancellationToken.None);
            
            // ASSERT

            Assert.NotEmpty(user.Id);

            _userRepositoryMock.Verify(repo => repo.Create(user, It.IsAny<CancellationToken>()), Times.Once);
            _userValidatorMock.Verify(validator =>
                    validator.ValidateAsync(
                        It.Is<ValidationContext<User>>(context => context.InstanceToValidate == user),
                        It.IsAny<CancellationToken>()),
                Times.Once);
            _unitOfWorkMock.Verify(unit => unit.SaveChanges(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData("", "")]
        [InlineData(" ", " ")]
        public async void CreateUser_InvalidUser_Throw(string email, string login)
        {
            // ARRANGE

            var expectedException = new ValidationException(new[]
            {
                new ValidationFailure(nameof(User.Email), ""),
                new ValidationFailure(nameof(User.Login), "")
            });

            _userValidatorMock
                .Setup(validator =>
                    validator.ValidateAsync(It.IsAny<ValidationContext<User>>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(expectedException);

            var user = new User
            {
                Email = email,
                Login = login
            };

            // ACT

            var exception = await Assert.ThrowsAsync<ValidationException>(() => _userService.Create(user, CancellationToken.None));
            
            // ASSERT

            Assert.Equal(expectedException, exception);
        }

        #endregion

        #region Tests for UpdateUser

        [Fact]
        public async void UpdateUser_SuccessPath()
        {
            // ARRANGE

            var user = new User
            {
                Id = "some id",
                Email = "some email",
                Login = "some login"
            };

            // ACT

            await _userService.Update(user, CancellationToken.None);
            
            // ASSERT

            _userValidatorMock.Verify(validator =>
                    validator.ValidateAsync(
                        It.Is<ValidationContext<User>>(context => context.InstanceToValidate == user),
                        It.IsAny<CancellationToken>()),
                Times.Once);
            _userRepositoryMock.Verify(repo => repo.Update(user, It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkMock.Verify(unit => unit.SaveChanges(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Theory]
        [InlineData(null, null, null)]
        [InlineData("", "", "")]
        [InlineData(" ", " ", " ")]
        public async void UpdateUser_InvalidUser_Throw(string id, string email, string login)
        {
            // ARRANGE

            var expectedException = new ValidationException(new[]
            {
                new ValidationFailure(nameof(User.Id), ""),
                new ValidationFailure(nameof(User.Email), ""),
                new ValidationFailure(nameof(User.Login), "")
            });

            _userValidatorMock
                .Setup(validator =>
                    validator.ValidateAsync(It.IsAny<ValidationContext<User>>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(expectedException);

            var user = new User
            {
                Id = id,
                Email = email,
                Login = login
            };

            // ACT

            var exception = await Assert.ThrowsAsync<ValidationException>(() => _userService.Create(user, CancellationToken.None));
            
            // ASSERT

            Assert.Equal(expectedException, exception);
        }

        #endregion

        #region Tests for DeleteUser

        [Fact]
        public async void DeleteUser_SuccessPath()
        {
            // ARRANGE

            _accountRepositoryMock
                .Setup(repo => repo.ExistsAccountsForUserById(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // ACT

            await _userService.DeleteById("some id", CancellationToken.None);
            
            // ASSERT

            _userRepositoryMock.Verify(repo => repo.DeleteById("some id", It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkMock.Verify(unit => unit.SaveChanges(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async void DeleteUser_UserHasAccounts_Throw()
        {
            // ARRANGE

            _accountRepositoryMock
                .Setup(repo => repo.ExistsAccountsForUserById(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // ACT

            var exception =
                await Assert.ThrowsAsync<Exceptions.ValidationException>(() => _userService.DeleteById("some id", CancellationToken.None));
            
            // ASSERT

            Assert.Equal(Exceptions.ValidationException.DeletingUserHasAccountsException.Message, exception.Message);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async void DeleteUser_InvalidId_Throw(string id)
        {
            // ARRANGE

            var expectedException = new Exceptions.ValidationException("");

            _userRepositoryMock
                .Setup(repo => repo.DeleteById(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(expectedException);

            // ACT

            var exception = await Assert.ThrowsAsync<Exceptions.ValidationException>(() => _userService.DeleteById(id, CancellationToken.None));
            
            // ASSERT

            Assert.Equal(expectedException, exception);
        }

        #endregion
    }
}