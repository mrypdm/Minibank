using System;

namespace MiniBank.Core.Exceptions
{
    public class ValidationException : Exception
    {
        public static ValidationException SenderAccountIsClosedException => new("Sender account is closed");

        public static ValidationException BeneficiaryAccountIsClosedException => new("Beneficiary's account is closed");

        public static ValidationException SenderDontHaveEnoughMoneyException =>
            new("Insufficient funds on the sender's account");

        public static ValidationException ClosingAccountHasMoneyException =>
            new("Can't close an account that has money in it");

        public static ValidationException ClosingAccountAlreadyClosedException => new("Account already closed");

        public static ValidationException DeletingUserHasAccountsException => new("The user has linked accounts");

        public ValidationException()
        {
        }

        public ValidationException(string message) : base(message)
        {
        }

        public ValidationException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}