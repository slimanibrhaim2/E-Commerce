// Users.Application/DTOs/UserDTO.cs
using System;
using System.Text.Json.Serialization;

namespace Users.Application.DTOs
{
    public class UserDTO
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; } = null!;
        public string? MiddleName { get; set; }
        public string LastName { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? ProfilePhoto { get; set; }
        public string? Description { get; set; }
    }
}
