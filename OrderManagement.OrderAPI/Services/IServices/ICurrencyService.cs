using OrderManagement.OrderAPI.Models;

namespace OrderManagement.OrderAPI.Services.IServices
{
    public interface ICurrencyService
    {
        Task<decimal?> GetExchangeRate(string currencyCode);
        Task<int> CalculatePriority(Order order);
    }
}
