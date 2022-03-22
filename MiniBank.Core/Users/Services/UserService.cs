using System;
using System.Collections.Generic;
using MiniBank.Core.BankAccounts.Repositories;
using MiniBank.Core.Exceptions;
using MiniBank.Core.Users.Repositories;

namespace MiniBank.Core.Users.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IBankAccountRepository _accountRepository;

        public UserService(IUserRepository userRepository, IBankAccountRepository accountRepository)
        {
            _userRepository = userRepository;
            _accountRepository = accountRepository;
        }

        public User GetById(string id)
        {
            return _userRepository.GetById(id);
        }

        public IEnumerable<User> GetAll()
        {
            return _userRepository.GetAll();
        }

        public void Create(User user)
        {
            user.Id = Guid.NewGuid().ToString();
            
            _userRepository.Create(user);
        }

        public void Update(User user)
        {
            _userRepository.Update(user);
        }

        public void DeleteById(string id)
        {
            if (_accountRepository.ExistsAccountsForUserById(id))
            {
                throw new ValidationException("The user has linked accounts");
            }

            _userRepository.DeleteById(id);
        }
    }
}