using Core;
namespace Users.Domain.Entities;

public class User
{
    public Guid Id { get; set; }

    public string FirstName { get; set; } = null!;

    public string? MiddleName { get; set; }

    public string LastName { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public string Email { get; set; } = null!;
    
    public double? Rate { get; set; }

    public string? ProfilePhoto { get; set; }

    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public List<Follower>? Followees { get; set; }
    public List<Address>? Addresses { get; set; }
    public List<Notification>? Notifications { get; set; }

}