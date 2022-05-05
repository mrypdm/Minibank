using System;

namespace MiniBank.Core.DateTimes
{
    public class DateTimeProvider : IDateTimeProvider
    {
        public DateTime Now => DateTime.Now;
    }
}