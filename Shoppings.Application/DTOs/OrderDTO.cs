using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shoppings.Application.DTOs
{
    public class OrderDTO
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public Guid OrderActivityId { get; set; }

        public decimal TotalAmount { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public DateTime? DeletedAt { get; set; }

    }
}
