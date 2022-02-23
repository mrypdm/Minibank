using System;

namespace MiniBank.Core.Exceptions
{
    public class UserFriendlyException : Exception
    {
        public UserFriendlyException()
        {
        }

        public UserFriendlyException(string message) : base(message)
        {
        }

        public UserFriendlyException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}