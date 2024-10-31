using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Diagnostics;

namespace coin_api01.Controllers
{
    [ApiController]
    [Route("v1")]
    public class ApiVer1Controller : ControllerBase
    {
        private readonly ILogger<ApiVer1Controller> _logger;
        private readonly CoinApiOptions _options;
        private Stopwatch _sw = new Stopwatch();
        private Dictionary<string, string> _result = new Dictionary<string, string>();

        public ApiVer1Controller(ILogger<ApiVer1Controller> logger, IOptions<CoinApiOptions> options)
        {
            _logger = logger;
            _options = options.Value;
            _sw.Start();
        }

        [HttpGet]
        public IActionResult Get()
        {
            _logger.LogInformation("call Get");
            _result.Add("local", DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(_options.DefaultTimeZone)).ToString("yyyy-MM-ddTHH:mm:ss.fffK"));
            _sw.Stop();
            _result.Add("time", _sw.Elapsed.TotalMilliseconds.ToString("n4") + "ms");
            return new JsonResult(_result);
        }
    }
}
