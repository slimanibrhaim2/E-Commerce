namespace Payments.Application.DTOs
{
    public class PaymentWithMethodDTO
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public double Amount { get; set; }
        public Guid PaymentMethodId { get; set; }
        public string PaymentMethodName { get; set; } = string.Empty;
        public Guid StatusId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
} 