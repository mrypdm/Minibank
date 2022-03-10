namespace MiniBank.Core
{
    public interface ICurrencyConverter
    {
        int Convert(int amount, string fromCurrency, string toCurrency);
    }
}