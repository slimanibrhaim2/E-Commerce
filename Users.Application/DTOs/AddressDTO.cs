using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Users.Application.DTOs
{
    public class AddressDTO
    {
        public Guid? Id { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string? Name { get; set; }
    }
}
