using Hangfire;
using Microsoft.AspNetCore.Connections;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using OrderManagement.OrderAPI.Data;
using OrderManagement.OrderAPI.Models;
using OrderManagement.OrderAPI.Services.IServices;
using OrderManagement.OrderAPI.Utilities;
using RabbitMQ.Client;
using System.Text;

namespace OrderManagement.OrderAPI.Services
{
    public class OrderProcessingService:IOrderProcessingService
    {
        private readonly ApplicationDbContext _db;
        private readonly ICurrencyService _currencyService;
        private readonly ILogger<OrderProcessingService> _logger;

        public OrderProcessingService(ApplicationDbContext db, ICurrencyService currencyService, ILogger<OrderProcessingService> logger)
        {
            _db = db;
            _currencyService = currencyService;
            _logger = logger;
        }

        //public void StartBackgroundJob()
        //{
        //    RecurringJob.AddOrUpdate("process-pending-orders", () => ProcessPendingOrdersAsync(), Cron.MinuteInterval(5));

        //    RecurringJob.AddOrUpdate("process-completed-orders", () => ProcessCompletedOrdersAsync(), Cron.MinuteInterval(5));
        //}

        public async Task ProcessPendingOrdersAsync()
        {
            var pendingOrders = await _db.Orders.Where(o => o.Status == SD.StatusPending).ToListAsync();

            foreach (var order in pendingOrders)
            {
                // Recalculate the priority based on the latest data
                order.CalculatePriority();
            }

            // Save the priority
            await _db.SaveChangesAsync();
            foreach (var order in pendingOrders.OrderByDescending(o => o.Priority))
            {
                try
                {
                    // Get the current exchange rate from the Currency Service
                    var exchangeRate = await _currencyService.GetExchangeRate(order.Currency);

                    if (exchangeRate != null)
                    {
                        // Convert the total amount to the base currency
                        order.TotalAmountInBaseCurrency = order.TotalAmount * exchangeRate.Value;

                        // Change the status to 'Processing'
                        order.Status = "Processing";
                        await _db.SaveChangesAsync();
                    }
                    else
                    {
                        // Leave the order in Pending status for retry in the next 5-minute job
                        _logger.LogError($"Failed to fetch exchange rate for order {order.Id}");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error processing order {order.Id}");
                }
            }
        }
        public async Task ProcessCompletedOrdersAsync()
        {
            var processingOrders = await _db.Orders.Where(o => o.Status == "Processing").ToListAsync();

            foreach (var order in processingOrders)
            {
                try
                {
                    // Mark order as Completed
                    order.Status = "Completed";
                    await _db.SaveChangesAsync();

                    // Publish the completed order message to RabbitMQ
                    PublishOrderToRabbitMQ(order);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error completing order {order.Id}");
                }
            }
        }

        private void PublishOrderToRabbitMQ(Order order)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();
            channel.QueueDeclare(queue: "order_queue", durable: false, exclusive: false, autoDelete: false, arguments: null);

            var message = new
            {
                Id = order.Id,
                CustomerName = order.CustomerName,
                TotalAmount = order.TotalAmount,
                TotalAmountInBaseCurrency = order.TotalAmountInBaseCurrency
            };

            var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));

            channel.BasicPublish(exchange: "", routingKey: "order_queue", basicProperties: null, body: body);
            // Log the information that the message has been sent
            _logger.LogInformation($"Order {order.Id} has been published to RabbitMQ. Message: {JsonConvert.SerializeObject(message)}");

        }

    }

}
