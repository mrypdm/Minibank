using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.Extensions.Logging;
using MiniBank.Core.BankAccounts.Repositories;
using MiniBank.Core.Users.Repositories;
using ValidationException = MiniBank.Core.Exceptions.ValidationException;

namespace MiniBank.Core.Users.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBankAccountRepository _accountRepository;
        private readonly IValidator<User> _userValidator;
        private readonly ILogger<UserService> _logger;

        public UserService(IUserRepository userRepository, IBankAccountRepository accountRepository,
            IUnitOfWork unitOfWork, IValidator<User> userValidator, ILogger<UserService> logger)
        {
            _userRepository = userRepository;
            _accountRepository = accountRepository;
            _unitOfWork = unitOfWork;
            _userValidator = userValidator;
            _logger = logger;
        }

        public Task<User> GetById(string id, CancellationToken token)
        {
            return _userRepository.GetById(id, token);
        }

        public async Task Create(User user, CancellationToken token)
        {
            await _userValidator.ValidateAsync(user, op =>
            {
                op.ThrowOnFailures();
                op.IncludeRuleSets("create");
            }, token);

            user.Id = Guid.NewGuid().ToString();

            await _userRepository.Create(user, token);

            await _unitOfWork.SaveChanges(token);
            
            _logger.LogInformation("Created user with id='{UserId}'", user.Id);
        }

        public async Task Update(User user, CancellationToken token)
        {
            await _userValidator.ValidateAsync(user, op =>
            {
                op.ThrowOnFailures();
                op.IncludeRuleSets("update");
            }, token);

            await _userRepository.Update(user, token);

            await _unitOfWork.SaveChanges(token);
            
            _logger.LogInformation("Updated user with id='{UserId}'", user.Id);
        }

        public async Task DeleteById(string id, CancellationToken token)
        {
            if (await _accountRepository.ExistsAccountsForUserById(id, token))
            {
                throw ValidationException.DeletingUserHasAccountsException;
            }

            await _userRepository.DeleteById(id, token);

            await _unitOfWork.SaveChanges(token);
            
            _logger.LogInformation("Deleted user with id='{UserId}'", id);
        }
    }
}