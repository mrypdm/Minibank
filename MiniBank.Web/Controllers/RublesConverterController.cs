using Microsoft.AspNetCore.Mvc;
using MiniBank.Core;

namespace MiniBank.Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RublesConverterController : ControllerBase
    {
        private readonly ICurrencyConverter _converter;

        public RublesConverterController(ICurrencyConverter converter)
        {
            _converter = converter;
        }
        
        [HttpGet]
        public int Convert(int rublesAmount, string currencyCode)
        {
            return _converter.Convert(rublesAmount, "RUB", currencyCode);
        }
    }
}