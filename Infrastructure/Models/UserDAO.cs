using System;
using System.Collections.Generic;

namespace Infrastructure.Models;

public partial class UserDAO
{
    public Guid Id { get; set; }

    public string FirstName { get; set; } = null!;

    public string? MiddleName { get; set; }

    public string LastName { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public string Email { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public double? Rate { get; set; }

    public string? ProfilePhoto { get; set; }

    public string? Description { get; set; }

    public virtual ICollection<AddressDAO> Addresses { get; set; } = new List<AddressDAO>();

    public virtual ICollection<FollowerDAO> Followees { get; set; } = new List<FollowerDAO>();

    public virtual ICollection<NotificationDAO> Notifications { get; set; } = new List<NotificationDAO>();

    public virtual ICollection<BaseContentDAO> BaseContents { get; set; } = new List<BaseContentDAO>();

    public virtual ICollection<BaseItemDAO> BaseItems { get; set; } = new List<BaseItemDAO>();

    public virtual ICollection<CartDAO> Carts { get; set; } = new List<CartDAO>();

    public virtual ICollection<CouponDAO> Coupons { get; set; } = new List<CouponDAO>();

    public virtual ICollection<FavoriteDAO> Favorites { get; set; } = new List<FavoriteDAO>();

    public virtual ICollection<OrderDAO> Orders { get; set; } = new List<OrderDAO>();
}
