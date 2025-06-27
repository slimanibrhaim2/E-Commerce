using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
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
        public string? Description { set; get; }
        
        [JsonIgnore] // This will be set by the controller when handling file upload
        public string? ProfilePhoto { set; get; }
    }
}
