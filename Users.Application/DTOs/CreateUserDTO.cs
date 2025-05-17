using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Users.Application.DTOs
{
    public class CreateUserDTO
    {
        public string FirstName { set; get; }
        public string? MiddleName { set; get; }
        public string LastName { set; get; }
        public string PhoneNumber { set; get; }
        public string Email { set; get; }
        public string? ProfilePhoto { set; get; }
        public string? Description { set; get; }
    }
}
