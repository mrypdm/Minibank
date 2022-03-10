using System.Diagnostics.Tracing;
using System.Linq;

namespace MiniBank.Core
{
    public interface ICurrencyRateProvider
    {
        int GetRate(string fromCurrency, string toCurrency);
    }
}