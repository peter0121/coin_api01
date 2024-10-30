namespace coin_api01.Models
{
    public class CurrentPriceInfoModel
    {
        public string code { get; set; }
        public string symbol { get; set; }
        public string rate { get; set; }
        public string description { get; set; }
        public decimal rate_float { get; set; }
    }
}
