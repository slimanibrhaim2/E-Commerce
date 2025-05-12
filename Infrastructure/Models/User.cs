using System;
using System.Collections.Generic;

namespace Infrastructure.Models;

public partial class User
{
    public Guid Id { get; set; }

    public string FirstName { get; set; } = null!;

    public string? MiddleName { get; set; }

    public string LastName { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public string Email { get; set; } = null!;

    public DateTime? ModificationDate { get; set; }

    public DateTime? CreationDate { get; set; }

    public double? Rate { get; set; }

    public string? ProfilePhoto { get; set; }

    public string? Description { get; set; }

    public virtual ICollection<BaseContent> BaseContents { get; set; } = new List<BaseContent>();

    public virtual ICollection<BaseItem> BaseItems { get; set; } = new List<BaseItem>();

    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();

    public virtual ICollection<Coupon> Coupons { get; set; } = new List<Coupon>();

    public virtual ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();

    public virtual ICollection<Follower> FollowerFollowerNavigations { get; set; } = new List<Follower>();

    public virtual ICollection<Follower> FollowerFollowings { get; set; } = new List<Follower>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
