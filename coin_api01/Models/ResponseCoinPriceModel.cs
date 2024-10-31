namespace coin_api01.Models
{
    public class ResponseCoinPriceModel
    {
        public IList<CoinPriceModel> data = new List<CoinPriceModel>();
        public DateTimeOffset update { get; set; }
    }
}
