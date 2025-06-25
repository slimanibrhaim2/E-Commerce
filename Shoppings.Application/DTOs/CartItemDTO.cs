using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shoppings.Application.DTOs
{
    public class CartItemDTO
    {
        public Guid Id { get; set; }

        public Guid CartId { get; set; }

        public Guid BaseItemId { get; set; }

        public Guid ItemId { get; set; }

        public int Quantity { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public DateTime? DeletedAt { get; set; }
    }
}
