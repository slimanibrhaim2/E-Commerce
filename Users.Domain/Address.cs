using Core;

namespace Users.Domain;

public class Address:BaseEntity
{
    public double Longitude { get; set; }

    public double Latitude { get; set; }

    public Guid UserId { get; set; }

    public string? Name { get; set; }

}