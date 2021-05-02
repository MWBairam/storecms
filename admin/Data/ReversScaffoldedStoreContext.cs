using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using admin.Models;

#nullable disable

namespace admin.Data
{
    public partial class ReversScaffoldedStoreContext : DbContext
    {
        public ReversScaffoldedStoreContext()
        {
        }

        public ReversScaffoldedStoreContext(DbContextOptions<ReversScaffoldedStoreContext> options)
            : base(options)
        {
        }

        public virtual DbSet<DeliveryMethod> DeliveryMethods { get; set; }
        public virtual DbSet<Message> Messages { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<OrderItem> OrderItems { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<ProductBrand> ProductBrands { get; set; }
        public virtual DbSet<ProductType> ProductTypes { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseNpgsql("Server=localhost; Port=5432;User Id=appuser; Password=secret; Database=skinet");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "en_US.utf8");

            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.Property(e => e.Price).HasPrecision(18, 2);

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.OrderItems)
                    .HasForeignKey(d => d.OrderId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.Property(e => e.Price).HasPrecision(18, 2);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
