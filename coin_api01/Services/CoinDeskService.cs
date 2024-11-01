using Azure;
using coin_api01.Models;
using System;
using System.Text;
using System.Text.Json;
using static System.Net.WebRequestMethods;

namespace coin_api01.Services
{
    public class CoinDeskService : ICoinService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<CoinDeskService> _logger;
        private string _coinDesk_url_currentprice = "https://api.coindesk.com/v1/bpi/currentprice.json";

        public CoinDeskService(ILogger<CoinDeskService> logger,
            IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }

        public CurrentPriceModel? GetCurrentPrice()
        {
            CurrentPriceModel? result = null;
            var get = CallGetApi(_coinDesk_url_currentprice);
            if (!String.IsNullOrEmpty(get))
            {
                result = JsonSerializer.Deserialize<CurrentPriceModel>(get);
            }

            return result;
        }

        private string? CallGetApi(string url)
        {
            var client = _httpClientFactory.CreateClient();
            //HttpClient client = new HttpClient();

            HttpRequestMessage msg = new HttpRequestMessage(HttpMethod.Get, url);

            var result = client.SendAsync(msg).Result;
            string? responseStr = null;

            if (result.IsSuccessStatusCode)
            {
                responseStr = result.Content.ReadAsStringAsync().Result;
            }

            return responseStr;
        }
    }
}
