using coin_api01.Models;

namespace coin_api01.Services
{
    public interface ICoinService
    {
        public CurrentPriceModel? GetCurrentPrice();
    }
}
