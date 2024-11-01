namespace coin_api01.Models
{
    public class CurrentPriceModel
    {
        public string chartName { get; set; }
        public string disclaimer { get; set; }
        public IDictionary<string, CurrentPriceInfoModel> bpi { get; set; }
        public CurrentPriceTime time { get; set; }
    }
}
