using System;
using System.Collections.Generic;

namespace Infrastructure.Models;

public partial class ReviewDAO
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public Guid BaseItemId { get; set; }

    public Guid OrderId { get; set; }

    public string Title { get; set; } = null!;

    public string Content { get; set; } = null!;

    public bool IsVerifiedPurchase { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual UserDAO User { get; set; } = null!;

    public virtual BaseItemDAO BaseItem { get; set; } = null!;

    public virtual OrderDAO Order { get; set; } = null!;
} 