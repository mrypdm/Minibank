using FluentValidation;
using MiniBank.Core.Users.Repositories;

namespace MiniBank.Core.BankAccounts.Validators
{
    public class BankAccountValidator : AbstractValidator<BankAccount>
    {
        public BankAccountValidator(IUserRepository userRepository)
        {   
            RuleFor(account => account.Amount)
                .GreaterThanOrEqualTo(0);

            RuleFor(account => account.UserId)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .MustAsync(async (userId, token) => await userRepository.ExistsWithId(userId, token))
                .WithMessage(account => $"User with id '{account.UserId}' doesn't exist");
        }
    }
}