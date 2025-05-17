using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Users.Application.DTOs
{
    public class AddAddressDTO
    {
        public double Latitude { set; get; }
        public double Longitude { set; get; }
        public string Name { set; get; }
    }
}
