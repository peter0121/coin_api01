using coin_api01.Data;
using coin_api01.Models;
using coin_api01.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.Collections.Immutable;

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
                    return StatusCode(StatusCodes.Status503ServiceUnavailable, "");
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

                //排序
                result.data = result.data.OrderBy(x => x.Code).ToList();

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Get price fail, {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "");
            }

            return StatusCode(StatusCodes.Status500InternalServerError, "");
        }

        [HttpGet]
        [Route("support")]
        public IActionResult GetSupport()
        {
            CoinSupportModel result = new CoinSupportModel();
            result.Codes = GetSupportCode();
            return Ok(result);
        }

        //Add CoinLang
        [HttpPost]
        public IActionResult AddLang([FromBody] CoinLangModel coin)
        {
            if (String.IsNullOrEmpty(coin.Code) || String.IsNullOrEmpty(coin.Name))
            {
                return StatusCode(StatusCodes.Status400BadRequest, "");
            }

            var get_support_list = GetSupportCode();
            if (!get_support_list.Contains(coin.Code))
            {
                //Code不再支援內
                return StatusCode(StatusCodes.Status400BadRequest, "");
            }

            try
            {
                var get_lang = _appDbContext.CoinLang.Where(x => x.Code == coin.Code).FirstOrDefault();
                if (get_lang != null)
                {
                    //資料重複
                    return StatusCode(StatusCodes.Status400BadRequest, "");
                }

                _appDbContext.CoinLang.Add(coin);
                _appDbContext.SaveChanges();
                _logger.LogInformation($"AddLang success,{coin.Code},{coin.Name}");
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError($"AddLang fail,{coin.Code},{coin.Name},{ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "");
            }
        }

        [HttpPut("{code}")]
        public IActionResult UpdateLang(string code, [FromBody] CoinLangModel input)
        {
            if (String.IsNullOrEmpty(code) || input == null)
            {
                return StatusCode(StatusCodes.Status400BadRequest, "");
            }

            try
            {
                var get_lang = _appDbContext.CoinLang.FirstOrDefault(x => x.Code == code);
                if (get_lang != null)
                {
                    string old_name = get_lang.Name;
                    get_lang.Name = input.Name;
                    _appDbContext.SaveChanges();
                    _logger.LogInformation($"UpdateLang success, {code},{old_name},{input.Name}");
                    return Ok();
                }

                return StatusCode(StatusCodes.Status400BadRequest, "");
            }
            catch (Exception ex)
            {
                _logger.LogError($"UpdateLang fail,{code},{ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "");
            }
        }

        [HttpDelete("{code}")]
        public IActionResult DeleteLang(string code)
        {
            if (String.IsNullOrEmpty(code))
            {
                return StatusCode(StatusCodes.Status400BadRequest, "");
            }
            try
            {
                var get_lang = _appDbContext.CoinLang.FirstOrDefault(x => x.Code == code);
                if (get_lang != null)
                {
                    _appDbContext.CoinLang.Remove(get_lang);
                    _appDbContext.SaveChanges();
                    return Ok();
                }
                return StatusCode(StatusCodes.Status400BadRequest, "");
            }
            catch (Exception ex)
            {
                _logger.LogError($"DeleteLang fail,{code},{ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "");
            }
        }

        private List<string> GetSupportCode()
        {
            var list = new List<string>()
            {
                "USD",
                "GBP",
                "EUR"
            };

            return list;
        }
    }
}
