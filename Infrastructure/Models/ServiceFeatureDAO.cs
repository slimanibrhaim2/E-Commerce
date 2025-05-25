using System;

namespace Infrastructure.Models;

public partial class ServiceFeatureDAO
{
    public Guid Id { get; set; }

    public Guid ServiceId { get; set; }

    public string Name { get; set; } = null!;

    public string Value { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual ServiceDAO Service { get; set; } = null!;
} 