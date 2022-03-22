using Microsoft.AspNetCore.Mvc;
using MiniBank.Core.Currencies;

namespace MiniBank.Web.Controllers.Currencies
{
    [ApiController]
    [Route("[controller]")]
    public class CurrencyConverterController : ControllerBase
    {
        private readonly ICurrencyConverter _converter;

        public CurrencyConverterController(ICurrencyConverter converter)
        {
            _converter = converter;
        }
        
        /// <summary>
        /// Make currency conversion
        /// </summary>
        [HttpGet]
        public double Convert(double amount, CurrencyCodes fromCurrencyCode, CurrencyCodes toCurrencyCode)
        {
            return _converter.Convert(amount, fromCurrencyCode, toCurrencyCode);
        }
    }
}