using System;
using MiniBank.Core.Exceptions;

namespace MiniBank.Core
{
    public class RublesConverter : IRublesConverter
    {
        private readonly IDatabase _database;

        public RublesConverter(IDatabase database)
        {
            _database = database;
        }

        public int Convert(int amount, string currencyCode)
        {
            if (amount < 0)
                throw new UserFriendlyException($"Amount must be positive");

            return amount / _database.Get(currencyCode);
        }
    }
}