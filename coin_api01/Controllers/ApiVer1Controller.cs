using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace coin_api01.Controllers
{
    [ApiController]
    [Route("v1")]
    public class ApiVer1Controller : ControllerBase
    {
        private readonly ILogger<ApiVer1Controller> _logger;
        private Stopwatch _sw = new Stopwatch();
        private Dictionary<string, string> _result = new Dictionary<string, string>();

        public ApiVer1Controller(ILogger<ApiVer1Controller> logger)
        {
            _logger = logger;
            _sw.Start();
        }

        [HttpGet]
        public IActionResult Get()
        {
            _logger.LogInformation("call Get");
            _result.Add("local", DateTimeOffset.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffK"));
            _sw.Stop();
            _result.Add("time", _sw.Elapsed.TotalMilliseconds.ToString("n4") + "ms");
            return new JsonResult(_result);
        }
    }
}
