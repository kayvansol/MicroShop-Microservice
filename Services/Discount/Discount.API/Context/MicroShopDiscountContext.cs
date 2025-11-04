using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Discount.API.Context;

public partial class MicroShopDiscountContext : DbContext
{
    public MicroShopDiscountContext()
    {
    }

    public MicroShopDiscountContext(DbContextOptions<MicroShopDiscountContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Coupon> Coupons { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Data Source=.;Initial Catalog=MicroShopDiscount;Integrated Security=True;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Coupon>(entity =>
        {
            entity.ToTable("Coupon");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Amount).HasDefaultValueSql("((0))");
            entity.Property(e => e.Description).HasMaxLength(100);
            entity.Property(e => e.ProductId).HasColumnName("ProductID");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
