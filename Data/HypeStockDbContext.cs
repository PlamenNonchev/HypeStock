using HypeStock.Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace HypeStock.Data
{
    public class HypeStockDbContext : IdentityDbContext<User>
    {
        public HypeStockDbContext(DbContextOptions<HypeStockDbContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<EditorsPick> EditorsPicks { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Retailer> Retailers { get; set; }
        public DbSet<RetailerProduct> RetailersProducts { get; set; }
        public DbSet<UserBrandLikes> UserBrandLikes { get; set;  }
        public DbSet<UserProductLikes> UserProductLikes { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder
                .Entity<Product>()
                .HasOne(p => p.Brand)
                .WithMany(b => b.Products)
                .HasForeignKey(p => p.BrandId)
                .OnDelete(DeleteBehavior.Restrict);


            builder
                .Entity<EditorsPick>()
                .HasOne(ep => ep.Product)
                .WithMany(p => p.EditorsPicks)
                .HasForeignKey(ep => ep.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .Entity<Brand>()
                .HasMany(b => b.Products)
                .WithOne(p => p.Brand)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .Entity<RetailerProduct>()
                .HasKey(rp => new { rp.RetailerId, rp.ProductId });

            builder
                .Entity<RetailerProduct>()
                .HasOne(rp => rp.Retailer)
                .WithMany(r => r.RetailerProducts)
                .HasForeignKey(rp => rp.RetailerId);

            builder
                .Entity<RetailerProduct>()
                .HasOne(rp => rp.Product)
                .WithMany(r => r.ProductRetailers)
                .HasForeignKey(rp => rp.ProductId);

            builder
                .Entity<UserBrandLikes>()
                .HasKey(ub => new { ub.UserId, ub.BrandId });

            builder
                .Entity<UserBrandLikes>()
                .HasOne(ub => ub.User)
                .WithMany(u => u.BrandLikes)
                .HasForeignKey(ub => ub.UserId);

            builder
                .Entity<UserBrandLikes>()
                .HasOne(ub => ub.Brand)
                .WithMany(b => b.LikesFromUsers)
                .HasForeignKey(ub => ub.BrandId);

            builder
                .Entity<UserProductLikes>()
                .HasKey(up => new { up.UserId, up.ProductId });

            builder
                .Entity<UserProductLikes>()
                .HasOne(up => up.User)
                .WithMany(u => u.ProductLikes)
                .HasForeignKey(up => up.UserId);

            builder
                .Entity<UserProductLikes>()
                .HasOne(up => up.Product)
                .WithMany(p => p.LikesFromUsers)
                .HasForeignKey(up => up.ProductId);

            base.OnModelCreating(builder);
        }
    }
}
