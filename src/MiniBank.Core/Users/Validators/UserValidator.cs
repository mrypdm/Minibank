using FluentValidation;

namespace MiniBank.Core.Users.Validators
{
    public class UserValidator : AbstractValidator<User>
    {
        public UserValidator()
        {
            RuleSet("create", () =>
            {
                RuleFor(user => user.Login).NotEmpty();
                RuleFor(user => user.Email).NotEmpty();
            });
            
            RuleSet("update", () =>
            {
                RuleFor(user => user.Id).NotEmpty();
                RuleFor(user => user.Login).NotEmpty();
                RuleFor(user => user.Email).NotEmpty();
            });
        }
    }
}