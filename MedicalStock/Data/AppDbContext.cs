using Microsoft.EntityFrameworkCore;
using MedicalStock.Models;

namespace MedicalStock.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Product> Products {  get; set; }
        public DbSet<Batch> Batches { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<StockMovement> StockMovements { get; set; }
        public AppDbContext()
        {
        }

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var projectDirectory =
                    Directory.GetParent(AppContext.BaseDirectory)!
                    .Parent!
                    .Parent!
                    .Parent!
                    .FullName;

                var dbPath = Path.Combine(projectDirectory, "MedicalStock.db");

                optionsBuilder.UseSqlite($"Data Source={dbPath}");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Category
            modelBuilder.Entity<Category>(entity =>
            {
                entity.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(100);

                entity.HasIndex(c => c.Name)
                .IsUnique();
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

            // StockMovement
            modelBuilder.Entity<StockMovement>(entity =>
            {
                entity.Property(s => s.Quantity)
                .IsRequired();

                entity.Property(s => s.Type)
                .IsRequired();

                entity.Property(s => s.MovementDate)
                .IsRequired();

                entity.HasOne(s => s.Batch)
                    .WithMany(p => p.StockMovements)
                    .HasForeignKey(s => s.BatchId)
                    .OnDelete(DeleteBehavior.Restrict);

            });
        }
    }
}
