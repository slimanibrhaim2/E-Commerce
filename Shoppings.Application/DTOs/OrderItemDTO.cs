using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shoppings.Application.DTOs
{
    public class OrderItemDTO
    {
        public Guid Id { get; set; }

        public Guid OrderId { get; set; }

        public Guid BaseItemId { get; set; }

        public double Quantity { get; set; }

        public double Price { get; set; }

        public Guid CouponId { get; set; }
        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public DateTime? DeletedAt { get; set; }

    }
}
