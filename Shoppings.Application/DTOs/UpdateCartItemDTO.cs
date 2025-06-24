using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shoppings.Application.DTOs
{
    public class UpdateCartItemDTO
    {
        public Guid ItemId { get; set; }
        public int Quantity { get; set; }
    }
} 