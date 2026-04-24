using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace APPLICATION_BACKEND.Database;

public partial class CampusConnectDbContext : DbContext
{
    public CampusConnectDbContext(DbContextOptions<CampusConnectDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderItem> OrderItems { get; set; }

    public virtual DbSet<ProductCategory> ProductCategories { get; set; }

    public virtual DbSet<ProductCategoryItem> ProductCategoryItems { get; set; }

    public virtual DbSet<SystemRole> SystemRoles { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Order>(entity =>
        {
            entity.ToTable("ORDER");

            entity.Property(e => e.OrderId).HasColumnName("ORDER_ID");
            entity.Property(e => e.CustomerId).HasColumnName("CUSTOMER_ID");
            entity.Property(e => e.DelivererId).HasColumnName("DELIVERER_ID");
            entity.Property(e => e.Destination)
                .HasMaxLength(250)
                .HasColumnName("DESTINATION");
            entity.Property(e => e.EstimatedTimeMin).HasColumnName("ESTIMATED_TIME_MIN");
            entity.Property(e => e.OrderPickupType)
                .HasMaxLength(250)
                .HasColumnName("ORDER_PICKUP_TYPE");
            entity.Property(e => e.OrderStatus)
                .HasMaxLength(250)
                .HasColumnName("ORDER_STATUS");
            entity.Property(e => e.PaymentMethod)
                .HasMaxLength(250)
                .HasColumnName("PAYMENT_METHOD");
            entity.Property(e => e.PaymentStatus)
                .HasMaxLength(250)
                .HasColumnName("PAYMENT_STATUS");
            entity.Property(e => e.PickupPoint)
                .HasMaxLength(250)
                .HasColumnName("PICKUP_POINT");
            entity.Property(e => e.ShopkeeperId).HasColumnName("SHOPKEEPER_ID");
            entity.Property(e => e.SpecialNotes)
                .HasMaxLength(250)
                .HasColumnName("SPECIAL_NOTES");
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.ToTable("ORDER_ITEM");

            entity.Property(e => e.OrderItemId).HasColumnName("ORDER_ITEM_ID");
            entity.Property(e => e.Discount).HasColumnName("DISCOUNT");
            entity.Property(e => e.OrderId).HasColumnName("ORDER_ID");
            entity.Property(e => e.ProductCategoryItemId).HasColumnName("PRODUCT_CATEGORY_ITEM_ID");
            entity.Property(e => e.Quantity).HasColumnName("QUANTITY");
            entity.Property(e => e.UnitPrice).HasColumnName("UNIT_PRICE");
        });

        modelBuilder.Entity<ProductCategory>(entity =>
        {
            entity.ToTable("PRODUCT_CATEGORY");

            entity.Property(e => e.ProductCategoryId).HasColumnName("PRODUCT_CATEGORY_ID");
            entity.Property(e => e.DateAdded)
                .HasColumnType("datetime")
                .HasColumnName("DATE_ADDED");
            entity.Property(e => e.DateUpdated)
                .HasColumnType("datetime")
                .HasColumnName("DATE_UPDATED");
            entity.Property(e => e.Description).HasColumnName("DESCRIPTION");
            entity.Property(e => e.IsActive).HasColumnName("IS_ACTIVE");
            entity.Property(e => e.Name)
                .HasMaxLength(250)
                .HasColumnName("NAME");
        });

        modelBuilder.Entity<ProductCategoryItem>(entity =>
        {
            entity.ToTable("PRODUCT_CATEGORY_ITEM");

            entity.Property(e => e.ProductCategoryItemId).HasColumnName("PRODUCT_CATEGORY_ITEM_ID");
            entity.Property(e => e.DateAdded)
                .HasColumnType("datetime")
                .HasColumnName("DATE_ADDED");
            entity.Property(e => e.DateUpdated)
                .HasColumnType("datetime")
                .HasColumnName("DATE_UPDATED");
            entity.Property(e => e.ImageUrl)
                .HasMaxLength(250)
                .HasColumnName("IMAGE_URL");
            entity.Property(e => e.IsAvailable).HasColumnName("IS_AVAILABLE");
            entity.Property(e => e.Name)
                .HasMaxLength(250)
                .HasColumnName("NAME");
            entity.Property(e => e.PreperationTimeMinutes).HasColumnName("PREPERATION_TIME_MINUTES");
            entity.Property(e => e.Price).HasColumnName("PRICE");
            entity.Property(e => e.ProductCategoryId).HasColumnName("PRODUCT_CATEGORY_ID");
            entity.Property(e => e.Quantity).HasColumnName("QUANTITY");
        });

        modelBuilder.Entity<SystemRole>(entity =>
        {
            entity.HasKey(e => e.RoleId);

            entity.ToTable("SYSTEM_ROLE");

            entity.Property(e => e.RoleId).HasColumnName("ROLE_ID");
            entity.Property(e => e.RoleDescription).HasColumnName("ROLE_DESCRIPTION");
            entity.Property(e => e.RoleName)
                .HasMaxLength(100)
                .HasColumnName("ROLE_NAME");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("USER");

            entity.Property(e => e.UserId).HasColumnName("USER_ID");
            entity.Property(e => e.DateAdded)
                .HasColumnType("datetime")
                .HasColumnName("DATE_ADDED");
            entity.Property(e => e.DateUpdated)
                .HasColumnType("datetime")
                .HasColumnName("DATE_UPDATED");
            entity.Property(e => e.EmailAddres)
                .HasMaxLength(250)
                .HasColumnName("EMAIL_ADDRES");
            entity.Property(e => e.FirstName)
                .HasMaxLength(250)
                .HasColumnName("FIRST_NAME");
            entity.Property(e => e.IsActive).HasColumnName("IS_ACTIVE");
            entity.Property(e => e.LastName)
                .HasMaxLength(250)
                .HasColumnName("LAST_NAME");
            entity.Property(e => e.Password).HasColumnName("PASSWORD");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(250)
                .HasColumnName("PHONE_NUMBER");
            entity.Property(e => e.ProfilePictureUrl)
                .HasMaxLength(250)
                .HasColumnName("PROFILE_PICTURE_URL");
            entity.Property(e => e.RoleName)
                .HasMaxLength(250)
                .HasColumnName("ROLE_NAME");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
