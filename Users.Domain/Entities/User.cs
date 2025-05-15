using Core;
using Core.Abstraction;
namespace Users.Domain.Entities;

public class User : Aggregate
{

    public string FirstName { get; set; } = null!;

    public string? MiddleName { get; set; }

    public string LastName { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public string Email { get; set; } = null!;
    
    public double? Rate { get; set; }

    public string? ProfilePhoto { get; set; }

    public string? Description { get; set; }

    public List<Follower>? Followees { get; set; }
    public List<Address>? Addresses { get; set; }
    public List<Notification>? Notifications { get; set; }

}