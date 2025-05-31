using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shoppings.Application.DTOs
{
    public class CreateOrderItemDTO
    {
        public Guid OrderId { get; set; }

        public Guid BaseItemId { get; set; }

        public double Quantity { get; set; }

        public double Price { get; set; }

        public Guid CouponId { get; set; }
    }
}
