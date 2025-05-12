using Core;
namespace Users.Domain;

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
}