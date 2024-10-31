using coin_api01.Data;
using coin_api01.Models;
using coin_api01.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace coin_api01.Controllers
{
    [ApiController]
    [Route("v1/coin")]
    public class ApiCoinController : ControllerBase
    {

        private readonly ILogger<ApiCoinController> _logger;
        private readonly CoinApiOptions _options;
        private ICoinService _coinDesk;
        private AppDbContext _appDbContext;

        public ApiCoinController(ILogger<ApiCoinController> logger, IOptions<CoinApiOptions> options,
            ICoinService coinDeskService, AppDbContext appDbContext)
        {
            _logger = logger;
            _options = options.Value;
            _coinDesk = coinDeskService;
            _appDbContext = appDbContext;
        }

        [HttpGet]
        [Route("price")]
        public IActionResult Get()
        {
            try
            {
                ResponseCoinPriceModel result = new ResponseCoinPriceModel();

                var get_online = _coinDesk.GetCurrentPrice();

                if (get_online == null)
                {
                    return StatusCode(StatusCodes.Status503ServiceUnavailable);
                }



                DateTimeOffset localdateTimeOffset = get_online.time.updatedISO.ToOffset(TimeSpan.FromHours(_options.DefaultTimeZone));
                result.update = localdateTimeOffset;

                var get_sql = _appDbContext.CoinLang.ToList();

                CurrentPriceInfoModel tmp = null;
                foreach (var col in get_sql)
                {
                    if (get_online.bpi.TryGetValue(col.Code, out tmp))
                    {
                        result.data.Add(
                            new CoinPriceModel()
                            {
                                Code = col.Code,
                                Name = col.Name,
                                Rate = tmp.rate_float
                            }
                         );
                    }
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Get price fail, {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}
