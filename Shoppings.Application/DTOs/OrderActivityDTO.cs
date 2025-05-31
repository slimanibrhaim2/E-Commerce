namespace Shoppings.Application.DTOs
{
    public class OrderActivityDTO
    {
        public Guid Id { get; set; }

        public Guid Status { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public DateTime? DeletedAt { get; set; }

    }
}