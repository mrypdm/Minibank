using System;

namespace MiniBank.Core.DateTimes
{
    public interface IDateTimeProvider
    {
        DateTime Now { get; }
    }
}