using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shoppings.Application.DTOs
{
    public class OrderStatusDTO
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;
        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public DateTime? DeletedAt { get; set; }
    }
}
