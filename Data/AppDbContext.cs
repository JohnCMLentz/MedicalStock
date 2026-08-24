using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using MedicalStock.Models;

namespace MedicalStock.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Product> Products {  get; set; }
        public DbSet<Batch> Batches { get; set; }
        public DbSet<Category> Categories { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=MedicalStock.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Category
            modelBuilder.Entity<Category>(entity =>
            {
                entity.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(100);
            });

            // Product
            modelBuilder.Entity<Product>(entity =>
            {
                entity.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(150);

                entity.Property(p => p.Barcode)
                .IsRequired()
                .HasMaxLength(50);

                entity.Property(p => p.Manufacturer)
                .IsRequired()
                .HasMaxLength(100);

                entity.Property(p => p.Price)
                .HasPrecision(18, 2);

                entity.HasIndex(p => p.Barcode)
                    .IsUnique();

                entity.HasOne(p => p.Category)
                    .WithMany(c => c.Products)
                    .HasForeignKey(p => p.CategoryId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Batch
            modelBuilder.Entity<Batch>(entity =>
            {
                entity.Property(b => b.Quantity)
                .IsRequired();

                entity.Property(b => b.ExpirationDate)
                .IsRequired();

                entity.Property(b => b.ReceivedAt)
                .IsRequired();

                entity.HasOne(b => b.Product)
                    .WithMany(p => p.Batches)
                    .HasForeignKey(b => b.ProductId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
