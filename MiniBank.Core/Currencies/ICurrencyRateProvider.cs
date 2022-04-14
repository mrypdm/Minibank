using System.Threading.Tasks;

namespace MiniBank.Core.Currencies
{
    public interface ICurrencyRateProvider
    {
        Task<double> GetRate(CurrencyCodes fromCurrency, CurrencyCodes toCurrency);
    }
}