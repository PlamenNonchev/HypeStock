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
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Retailer> Retailers { get; set; }
        public DbSet<RetailerProduct> RetailersProducts { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder
                .Entity<Product>()
                .HasOne(p => p.Brand)
                .WithMany(b => b.Products)
                .HasForeignKey(p => p.BrandId)
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

            base.OnModelCreating(builder);
        }
    }
}
