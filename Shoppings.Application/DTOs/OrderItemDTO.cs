using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shoppings.Application.DTOs
{
    public class OrderItemDTO
    {
        public Guid ItemId { get; set; }
        public string? ImageUrl { get; set; }
        public string Name { get; set; } = null!;
        public double Price { get; set; }
        public double TotalPrice { get; set; }
        public double Quantity { get; set; }
    }
}
