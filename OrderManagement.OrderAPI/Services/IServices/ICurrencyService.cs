namespace OrderManagement.OrderAPI.Services.IServices
{
    public interface ICurrencyService
    {
        Task<decimal?> GetExchangeRate(string currencyCode);
    }
}
