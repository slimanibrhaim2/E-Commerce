using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payments.Application.DTOs
{
    public class CreatePaymentDTO
    {
        public Guid OrderId { get; set; }
        public decimal Amount { get; set; }
        public Guid PaymentMethodId { get; set; }
        public Guid StatusId { get; set; }
    }
}
