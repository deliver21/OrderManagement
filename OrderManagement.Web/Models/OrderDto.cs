namespace OrderManagement.Web.Models
{
    public class OrderDto
    {
        public int Id { get; set; }
        public string CustomerName { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Currency { get; set; }
        public string Status { get; set; } = "Pending";
        public int Priority { get; set; }
        public decimal TotalAmountInBaseCurrency { get; set; }
    }
}
