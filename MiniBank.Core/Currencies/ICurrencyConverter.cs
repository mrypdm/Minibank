using System.Threading.Tasks;

namespace MiniBank.Core.Currencies
{
    public interface ICurrencyConverter
    {
        Task<double> Convert(double amount, CurrencyCodes fromCurrency, CurrencyCodes toCurrency);
    }
}