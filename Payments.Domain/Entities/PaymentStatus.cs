using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payments.Domain.Entities
{
    public class PaymentStatus
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;
        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public DateTime? DeletedAt { get; set; }
        public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }
}
