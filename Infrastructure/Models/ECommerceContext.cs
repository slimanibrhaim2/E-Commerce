using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Models;

public partial class ECommerceContext : DbContext
{
    public ECommerceContext()
    {
    }

    public ECommerceContext(DbContextOptions<ECommerceContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AddressDAO> Addresses { get; set; }
    public virtual DbSet<AttachmentDAO> Attachments { get; set; }
    public virtual DbSet<AttachmentTypeDAO> AttachmentTypes { get; set; }
    public virtual DbSet<BaseContentDAO> BaseContents { get; set; }
    public virtual DbSet<BaseItemDAO> BaseItems { get; set; }
    public virtual DbSet<BrandDAO> Brands { get; set; }
    public virtual DbSet<CartDAO> Carts { get; set; }
    public virtual DbSet<CartItemDAO> CartItems { get; set; }
    public virtual DbSet<CategoryDAO> Categories { get; set; }
    public virtual DbSet<CommentDAO> Comments { get; set; }
    public virtual DbSet<ConversationDAO> Conversations { get; set; }
    public virtual DbSet<ConversationMemberDAO> ConversationMembers { get; set; }
    public virtual DbSet<CouponDAO> Coupons { get; set; }
    public virtual DbSet<DiscountTypeDAO> DiscountTypes { get; set; }
    public virtual DbSet<FavoriteDAO> Favorites { get; set; }
    public virtual DbSet<FollowerDAO> Followers { get; set; }
    public virtual DbSet<MediaTypeDAO> MediaTypes { get; set; }
    public virtual DbSet<MessageDAO> Messages { get; set; }
    public virtual DbSet<NotificationDAO> Notifications { get; set; }
    public virtual DbSet<OrderDAO> Orders { get; set; }
    public virtual DbSet<OrderActivityDAO> OrderActivities { get; set; }
    public virtual DbSet<OrderItemDAO> OrderItems { get; set; }
    public virtual DbSet<OrderStatusDAO> OrderStatuses { get; set; }
    public virtual DbSet<PaymentDAO> Payments { get; set; }
    public virtual DbSet<PaymentMethodDAO> PaymentMethods { get; set; }
    public virtual DbSet<PaymentStatusDAO> PaymentStatuses { get; set; }
    public virtual DbSet<ProductDAO> Products { get; set; }
    public virtual DbSet<ProductFeatureDAO> ProductFeatures { get; set; }
    public virtual DbSet<MediaDAO> Media { get; set; }
    public virtual DbSet<ServiceDAO> Services { get; set; }
    public virtual DbSet<ServiceFeatureDAO> ServiceFeatures { get; set; }
    public virtual DbSet<UserDAO> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Removed hardcoded connection string. Configuration is now handled via DI.
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseCollation("Arabic_CI_AS");


        modelBuilder.Entity<AddressDAO>(entity =>
        {
            entity.ToTable("Address");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Latitude).IsRequired();
            entity.Property(e => e.Longitude).IsRequired();

            entity.HasOne(d => d.User)
                .WithMany(p => p.Addresses)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Address_User");
        });

        modelBuilder.Entity<AttachmentDAO>(entity =>
        {
            entity.ToTable("Attachment");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.AttachmentUrl).HasMaxLength(500).IsRequired();
            entity.Property(e => e.AttachmentTypeId).IsRequired();

            entity.HasOne(d => d.BaseContent)
                .WithMany(p => p.Attachments)
                .HasForeignKey(d => d.BaseContentId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Attachment_BaseContent");
        });

        modelBuilder.Entity<BaseContentDAO>(entity =>
        {
            entity.ToTable("BaseContent");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Description).HasMaxLength(2000);

            entity.HasOne(d => d.User)
                .WithMany(p => p.BaseContents)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_BaseContent_User");
        });

        modelBuilder.Entity<BaseItemDAO>(entity =>
        {
            entity.ToTable("BaseItem");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Description).HasMaxLength(500);

            entity.HasOne(d => d.Category)
                .WithMany(p => p.BaseItems)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_BaseItem_Category");

            entity.HasOne(d => d.User)
                .WithMany(p => p.BaseItems)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_BaseItem_User");
        });

        modelBuilder.Entity<CartDAO>(entity =>
        {
            entity.ToTable("Cart");
            entity.HasKey(e => e.Id);

            entity.HasOne(d => d.User)
                .WithMany(p => p.Carts)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Cart_User");
        });

        modelBuilder.Entity<CartItemDAO>(entity =>
        {
            entity.ToTable("CartItem");
            entity.HasKey(e => e.Id);

            entity.HasOne(d => d.Cart)
                .WithMany(p => p.CartItems)
                .HasForeignKey(d => d.CartId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_CartItem_Cart");

            entity.HasOne(d => d.BaseItem)
                .WithMany(p => p.CartItems)
                .HasForeignKey(d => d.BaseItemId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_CartItem_BaseItem");
        });

        modelBuilder.Entity<CategoryDAO>(entity =>
        {
            entity.ToTable("Category");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Description).HasMaxLength(200);

            entity.HasOne(d => d.Parent)
                .WithMany(p => p.InverseParent)
                .HasForeignKey(d => d.ParentId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Category_Parent");
        });

        modelBuilder.Entity<CommentDAO>(entity =>
        {
            entity.ToTable("Comment");
            entity.HasKey(e => e.Id);

            entity.HasOne(d => d.BaseContent)
                .WithMany()
                .HasForeignKey(d => d.BaseContentId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Comment_BaseContent");

            entity.HasOne(d => d.BaseItem)
                .WithMany(p => p.Comments)
                .HasForeignKey(d => d.BaseItemId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Comment_BaseItem");
        });

        modelBuilder.Entity<ConversationDAO>(entity =>
        {
            entity.ToTable("Conversation");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).HasMaxLength(100).IsRequired();
        });

        modelBuilder.Entity<ConversationMemberDAO>(entity =>
        {
            entity.ToTable("ConversationMember");
            entity.HasKey(e => e.Id);

            entity.HasOne(d => d.Conversation)
                .WithMany(p => p.ConversationMembers)
                .HasForeignKey(d => d.ConversationId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_ConversationMember_Conversation");

            entity.HasOne(d => d.User)
                .WithMany()
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_ConversationMember_User");
        });

        modelBuilder.Entity<CouponDAO>(entity =>
        {
            entity.ToTable("Coupon");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Code).HasMaxLength(50).IsRequired();

            entity.HasOne(d => d.User)
                .WithMany(p => p.Coupons)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Coupon_User");
        });

        modelBuilder.Entity<FavoriteDAO>(entity =>
        {
            entity.ToTable("Favorite");
            entity.HasKey(e => e.Id);

            entity.HasOne(d => d.User)
                .WithMany(p => p.Favorites)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Favorite_User");

            entity.HasOne(d => d.Product)
                .WithMany()
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Favorite_Product");
        });

        modelBuilder.Entity<FollowerDAO>(entity =>
        {
            entity.ToTable("Follower");
            entity.HasKey(e => e.Id);

            entity.HasOne(d => d.Follower)
                .WithMany()
                .HasForeignKey(d => d.FollowerId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Follower_Follower");

            entity.HasOne(d => d.Following)
                .WithMany(p => p.Followees)
                .HasForeignKey(d => d.FollowingId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Follower_Following");
        });

        modelBuilder.Entity<MessageDAO>(entity =>
        {
            entity.ToTable("Message");
            entity.HasKey(e => e.Id);

            entity.HasOne(d => d.BaseContent)
                .WithMany()
                .HasForeignKey(d => d.BaseContentId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Message_BaseContent");

            entity.HasOne(d => d.Conversation)
                .WithMany(p => p.Messages)
                .HasForeignKey(d => d.ConversationId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Message_Conversation");

            entity.HasOne(d => d.Sender)
                .WithMany()
                .HasForeignKey(d => d.SenderId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Message_Sender");
        });

        modelBuilder.Entity<NotificationDAO>(entity =>
        {
            entity.ToTable("Notification");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Message).HasMaxLength(500).IsRequired();

            entity.HasOne(d => d.User)
                .WithMany(p => p.Notifications)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Notification_User");
        });

        modelBuilder.Entity<OrderDAO>(entity =>
        {
            entity.ToTable("Order");
            entity.HasKey(e => e.Id);

            entity.HasOne(d => d.User)
                .WithMany(p => p.Orders)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Order_User");

            entity.HasOne(d => d.OrderActivity)
                .WithMany(p => p.Orders)
                .HasForeignKey(d => d.OrderActivityId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Order_OrderActivity");

            entity.Property(e => e.TotalAmount)
                .HasPrecision(18, 2);
        });

        modelBuilder.Entity<OrderActivityDAO>(entity =>
        {
            entity.ToTable("OrderActivity");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.StatusId).IsRequired();
        });

        modelBuilder.Entity<OrderItemDAO>(entity =>
        {
            entity.ToTable("OrderItem");
            entity.HasKey(e => e.Id);

            entity.HasOne(d => d.Order)
                .WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_OrderItem_Order");

            entity.HasOne(d => d.BaseItem)
                .WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.BaseItemId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_OrderItem_BaseItem");
        });

        modelBuilder.Entity<PaymentDAO>(entity =>
        {
            entity.ToTable("Payment");
            entity.HasKey(e => e.Id);

            entity.HasOne(d => d.Order)
                .WithMany(p => p.Payments)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Payment_Order");

            entity.Property(e => e.Amount)
                .HasPrecision(18, 2);
        });

        modelBuilder.Entity<ProductDAO>(entity =>
        {
            entity.ToTable("Product");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.SKU).HasMaxLength(50).IsRequired();

            entity.HasOne(d => d.BaseItem)
                .WithMany(p => p.Products)
                .HasForeignKey(d => d.BaseItemId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Product_BaseItem");
        });

        modelBuilder.Entity<ProductFeatureDAO>(entity =>
        {
            entity.ToTable("ProductFeature");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Value).HasMaxLength(500).IsRequired();

            entity.HasOne(d => d.Product)
                .WithMany(p => p.ProductFeatures)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_ProductFeature_Product");
        });

        modelBuilder.Entity<MediaDAO>(entity =>
        {
            entity.ToTable("Media");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.MediaUrl).HasMaxLength(500).IsRequired();
            entity.Property(e => e.MediaTypeId).IsRequired();

            entity.HasOne(d => d.BaseItem)
                .WithMany(p => p.ProductMedia)
                .HasForeignKey(d => d.BaseItemId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_ProductMedia_BaseItem");
        });

        modelBuilder.Entity<ServiceDAO>(entity =>
        {
            entity.ToTable("Service");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ServiceType).HasMaxLength(50).IsRequired();

            entity.HasOne(d => d.BaseItem)
                .WithMany(p => p.Services)
                .HasForeignKey(d => d.BaseItemId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Service_BaseItem");
        });

        modelBuilder.Entity<ServiceFeatureDAO>(entity =>
        {
            entity.ToTable("ServiceFeature");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Value).HasMaxLength(500).IsRequired();

            entity.HasOne(d => d.Service)
                .WithMany(p => p.ServiceFeatures)
                .HasForeignKey(d => d.ServiceId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_ServiceFeatures_Services");
        });

        modelBuilder.Entity<UserDAO>(entity =>
        {
            entity.ToTable("User");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FirstName).HasMaxLength(50).IsRequired();
            entity.Property(e => e.MiddleName).HasMaxLength(50);
            entity.Property(e => e.LastName).HasMaxLength(50).IsRequired();
            entity.Property(e => e.PhoneNumber).HasMaxLength(20).IsRequired();
            entity.Property(e => e.Email).HasMaxLength(100).IsRequired();
            entity.Property(e => e.ProfilePhoto).HasMaxLength(500);
            entity.Property(e => e.Description).HasMaxLength(1000);
        });

        modelBuilder.Entity<UserDAO>()
            .HasMany(u => u.Addresses)
            .WithOne(a => a.User)
            .HasForeignKey(a => a.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
