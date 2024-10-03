using FluentValidation;
using OrderManagement.OrderAPI.Models.DTO;

namespace OrderManagement.OrderAPI.Utilities.FluentValidator
{
    public class OrderValidator : AbstractValidator<OrderDto>
    {
        public OrderValidator()
        {
            // Ensure that CustomerName is not empty
            RuleFor(order => order.CustomerName)
                .NotEmpty().WithMessage("Customer name is required.");
            RuleFor(order => order.CustomerName).MinimumLength(3).WithMessage("Customer name is too short");
            // Ensure that TotalAmount is greater than zero
            RuleFor(order => order.TotalAmount)
                .GreaterThan(0).WithMessage("Total amount must be greater than zero.");

            // Ensure that OrderDate is not in the future
            RuleFor(order => order.OrderDate)
                .LessThanOrEqualTo(DateTime.Now).WithMessage("Order date cannot be in the future.");

            // Ensure that Currency is supported (EUR)
            RuleFor(order => order.Currency)
                .Must(IsSupportedCurrency).WithMessage("Currency is not supported. Supported currencies: USD, EUR.");
        }

        // Helper function to check if the currency is supported
        private bool IsSupportedCurrency(string currency)
        {
            var supportedCurrencies = new[] {"EUR" ,"USD"};
            return Array.Exists(supportedCurrencies, curr => curr == currency);
        }
    }
}
