using Hangfire.Server;
using OrderManagement.OrderAPI.Services.IServices;
using OrderManagement.OrderAPI.Utilities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrderManagement.OrderAPI.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }
        public string CustomerName { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Currency { get; set; }
        public string Status { get; set; } = SD.StatusPending;
        public int Priority { get; set; } = 0;
        public decimal TotalAmountInBaseCurrency { get; set; }    
    }
}
