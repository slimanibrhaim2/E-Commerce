using System;
using System.Collections.Generic;

namespace Infrastructure.Models;

public partial class Address
{
    public Guid Id { get; set; }

    public double Longitude { get; set; }

    public double Latitude { get; set; }

    public Guid UserId { get; set; }

    public string? Name { get; set; }

    public virtual User User { get; set; } = null!;
}
