using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniBank.Core.Currencies;

namespace MiniBank.Web.Controllers.Currencies
{
    [ApiController]
    [Authorize]
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
        public Task<double> Convert(double amount, CurrencyCodes fromCurrencyCode, CurrencyCodes toCurrencyCode)
        {
            return _converter.Convert(amount, fromCurrencyCode, toCurrencyCode);
        }
    }
}