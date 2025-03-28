using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Restaurant.Models.Core;
using Restaurant.Models.CoreModels;
using Restaurant.Models.Orders;
using Restaurant.Models.Reservations;
using Restaurant.Models.Users;

namespace Restaurant.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Table> Tables { get; set; }
        public DbSet<MenuCategory> MenuCategories { get; set; }
        public DbSet<MenuItem> MenuItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Reservation> Reservations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure all relationships and model properties

            // 1. MenuCategory -> MenuItem relationship
            modelBuilder.Entity<MenuItem>()
                .HasOne(m => m.Category)
                .WithMany(c => c.MenuItems)
                .HasForeignKey(m => m.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // 2. Order relationships
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Table)
                .WithMany(t => t.Orders)
                .HasForeignKey(o => o.TableId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.Staff)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.StaffId)
                .OnDelete(DeleteBehavior.Restrict);

            // 3. OrderDetail relationships
            modelBuilder.Entity<OrderDetail>()
                .HasOne(od => od.Order)
                .WithMany(o => o.OrderDetails)
                .HasForeignKey(od => od.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<OrderDetail>()
                .HasOne(od => od.MenuItem)
                .WithMany(mi => mi.OrderDetails)
                .HasForeignKey(od => od.ItemId)
                .OnDelete(DeleteBehavior.Restrict);

            // 4. Table configurations
            modelBuilder.Entity<Table>()
                .HasIndex(t => t.TableNumber)
                .IsUnique();

            modelBuilder.Entity<Table>()
                .HasMany(t => t.Orders)
                .WithOne(o => o.Table)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Table>()
                .HasMany(t => t.Reservations)
                .WithOne(r => r.Table)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Table>()
                .Property(t => t.Status)
                .HasConversion<string>();

            // 5. Reservation configurations
            modelBuilder.Entity<Reservation>()
                .HasIndex(r => r.ReservationDate);

            modelBuilder.Entity<Reservation>()
                .Property(r => r.Status)
                .HasConversion<string>();

            // 6. Additional indexes for performance
            modelBuilder.Entity<Order>()
                .HasIndex(o => o.OrderDate);

            modelBuilder.Entity<Order>()
                .Property(o => o.Status)
                .HasConversion<string>();

            modelBuilder.Entity<MenuItem>()
                .HasIndex(m => m.Name);

            modelBuilder.Entity<MenuItem>()
                .HasIndex(m => m.IsAvailable);

            // 7. Enum conversions for all status fields
            modelBuilder.Entity<Order>()
                .Property(o => o.Status)
                .HasConversion<string>();

            modelBuilder.Entity<Reservation>()
                .Property(r => r.Status)
                .HasConversion<string>();

            modelBuilder.Entity<Table>()
                .Property(t => t.Status)
                .HasConversion<string>();

            modelBuilder.Entity<OrderDetail>()
                .Property(od => od.Status)
                .HasConversion<string>();
        }
    }
}