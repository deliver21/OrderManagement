namespace OrderManagement.OrderAPI.Services.IServices
{
    public interface IOrderProcessingService
    {
        Task ProcessPendingOrdersAsync();
        void ProcessCompletedOrdersAsync();
    }
}
