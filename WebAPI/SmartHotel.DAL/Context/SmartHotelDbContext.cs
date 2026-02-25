using Microsoft.EntityFrameworkCore;
using SmartHotel.DAL.Models;

namespace SmartHotel.DAL.Data
{
    public class SmartHotelDbContext : DbContext
    {
        public SmartHotelDbContext(DbContextOptions<SmartHotelDbContext> options) : base(options)
        {
        }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<AccountRole> AccountRoles { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Tenant> Tenants { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Contract> Contracts { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<MeterReading> MeterReadings { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<InvoiceDetail> InvoiceDetails { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

 
            modelBuilder.Entity<AccountRole>()
                .HasKey(ar => new { ar.AccountId, ar.RoleId });

            modelBuilder.Entity<AccountRole>()
                .HasOne(ar => ar.Account)
                .WithMany(a => a.AccountRoles)
                .HasForeignKey(ar => ar.AccountId);

            modelBuilder.Entity<AccountRole>()
                .HasOne(ar => ar.Role)
                .WithMany(r => r.AccountRoles)
                .HasForeignKey(ar => ar.RoleId);

            modelBuilder.Entity<Account>()
                .HasIndex(a => a.Email)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasOne(u => u.Account)
                .WithOne(a => a.User)
                .HasForeignKey<User>(u => u.AccountId)
                .OnDelete(DeleteBehavior.Cascade); 

            modelBuilder.Entity<Tenant>()
                .HasOne(t => t.Account)
                .WithOne(a => a.Tenant)
                .HasForeignKey<Tenant>(t => t.AccountId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Tenant>()
                .HasIndex(t => t.CCCD)
                .IsUnique();

            modelBuilder.Entity<Room>()
                .Property(r => r.Price)
                .HasColumnType("decimal(18, 2)");

            modelBuilder.Entity<Room>()
                .HasIndex(r => r.RoomNumber)
                .IsUnique(); 

            modelBuilder.Entity<Contract>()
                .Property(c => c.DepositAmount)
                .HasColumnType("decimal(18, 2)");

            modelBuilder.Entity<Service>()
                .Property(s => s.UnitPrice)
                .HasColumnType("decimal(18, 2)");

            modelBuilder.Entity<Invoice>()
                .Property(i => i.TotalAmount)
                .HasColumnType("decimal(18, 2)");

            modelBuilder.Entity<InvoiceDetail>()
                .Property(id => id.UnitPrice)
                .HasColumnType("decimal(18, 2)");

            modelBuilder.Entity<InvoiceDetail>()
                .Property(id => id.Amount)
                .HasColumnType("decimal(18, 2)");

            modelBuilder.Entity<InvoiceDetail>()
                .HasOne(id => id.Service)
                .WithMany(s => s.InvoiceDetails)
                .HasForeignKey(id => id.ServiceId)
                .OnDelete(DeleteBehavior.Restrict);

 
            modelBuilder.Entity<Role>().HasData(
                new Role { RoleId = 1, RoleName = "Admin" },
                new Role { RoleId = 2, RoleName = "Staff" },
                new Role { RoleId = 3, RoleName = "Tenant" }
            );
        }
    }
}