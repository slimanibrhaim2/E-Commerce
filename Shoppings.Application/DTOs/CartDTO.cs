using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shoppings.Application.DTOs
{
    public class CartDTO
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public DateTime? DeletedAt { get; set; }

        public virtual ICollection<CartItemWithDetailsDTO> CartItemsWithDetails { get; set; } = new List<CartItemWithDetailsDTO>();
    }
}
