using System.ComponentModel.DataAnnotations;

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
        public string Status { get; set; } 
        public int Priority { get; set; }
        public decimal TotalAmountInBaseCurrency { get; set; }

        public void CalculatePriority()
        {
            // Priority = TotalAmount (points for currency) + Hours since OrderDate
            var hoursSinceOrder = (DateTime.Now - OrderDate).TotalHours;
            Priority = (int)TotalAmount + (int)hoursSinceOrder;
        }
    }
}
