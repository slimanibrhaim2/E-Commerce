using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shoppings.Application.DTOs
{
    public class CreateOrderDTO
    {
        public Guid UserId { get; set; }

        public Guid OrderActivityId { get; set; }
    }
}
