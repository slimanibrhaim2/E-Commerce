using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shoppings.Application.DTOs
{
    public class CreateCartItemDTO
    {
        public Guid CartId { get; set; }

        public Guid BaseItemId { get; set; }

        public int Quantity { get; set; }
    }
}
