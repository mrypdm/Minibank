using Microsoft.AspNetCore.Mvc;
using MiniBank.Core;

namespace MiniBank.Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RublesConverterController : ControllerBase
    {
        private readonly IRublesConverter _converter;

        public RublesConverterController(IRublesConverter converter)
        {
            _converter = converter;
        }
        
        [HttpGet]
        public int ConvertRub(int rublesAmount, string currencyCode)
        {
            return _converter.Convert(rublesAmount, currencyCode);
        }
    }
}