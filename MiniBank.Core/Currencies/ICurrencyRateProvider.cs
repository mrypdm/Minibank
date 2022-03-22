namespace MiniBank.Core.Currencies
{
    public interface ICurrencyRateProvider
    {
        double GetRate(CurrencyCodes fromCurrency, CurrencyCodes toCurrency);
    }
}