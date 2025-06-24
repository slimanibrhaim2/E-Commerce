using System;
using System.Collections.Generic;

namespace Infrastructure.Models;

public partial class BaseItemDAO
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public bool IsAvailable { get; set; }

    public double Price { get; set; }

    public Guid? CategoryId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual ICollection<CartItemDAO> CartItems { get; set; } = new List<CartItemDAO>();

    public virtual CategoryDAO? Category { get; set; }

    public virtual ICollection<CommentDAO> Comments { get; set; } = new List<CommentDAO>();


    public virtual ICollection<FavoriteDAO> Favorites { get; set; } = new List<FavoriteDAO>();

    public virtual ICollection<OrderItemDAO> OrderItems { get; set; } = new List<OrderItemDAO>();

    public virtual ICollection<MediaDAO> ProductMedia { get; set; } = new List<MediaDAO>();

    public virtual ICollection<ProductDAO> Products { get; set; } = new List<ProductDAO>();

    public virtual ICollection<ReviewDAO> Reviews { get; set; } = new List<ReviewDAO>();

    public virtual ICollection<ServiceDAO> Services { get; set; } = new List<ServiceDAO>();

    public virtual UserDAO User { get; set; } = null!;
}
