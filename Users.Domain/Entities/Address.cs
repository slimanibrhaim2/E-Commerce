using Core.Abstraction;

namespace Users.Domain.Entities;

public class Address : BaseEntity
{
    public double Longitude { get; set; }

    public double Latitude { get; set; }

    public Guid UserId { get; set; }

    public string? Name { get; set; }

}