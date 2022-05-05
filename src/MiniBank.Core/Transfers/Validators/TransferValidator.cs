using FluentValidation;

namespace MiniBank.Core.Transfers.Validators
{
    public class TransferValidator : AbstractValidator<Transfer>
    {
        public TransferValidator()
        {
            RuleFor(transfer => transfer.Amount)
                .GreaterThanOrEqualTo(0);

            RuleFor(transfer => transfer.FromAccountId)
                .Cascade(CascadeMode.Stop)
                .NotEmpty();

            RuleFor(transfer => transfer.ToAccountId)
                .Cascade(CascadeMode.Stop)
                .NotEmpty();
        }
    }
}