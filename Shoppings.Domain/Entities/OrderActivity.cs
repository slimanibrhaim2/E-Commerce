namespace Shoppings.Domain.Entities
{
    public class OrderActivity
    {
        public Guid Id { get; set; }

        public Guid Status { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public DateTime? DeletedAt { get; set; }

        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}