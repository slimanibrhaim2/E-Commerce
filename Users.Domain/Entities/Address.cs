namespace Users.Domain.Entities;

public class Address 
{
    public Guid Id { get; set; }

    public double Longitude { get; set; }

    public double Latitude { get; set; }

    public Guid UserId { get; set; }

    public string Name { get; set; }

    public User User { get; set; }
    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }
}