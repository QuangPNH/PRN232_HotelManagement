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

            modelBuilder.Entity<Contract>()
                .Property(c => c.Price)
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

            // --- DATA SEEDING (15 items per table, except Role) ---

            // 1. Roles
            modelBuilder.Entity<Role>().HasData(
                new Role { RoleId = 1, RoleName = "Admin" },
                new Role { RoleId = 2, RoleName = "Staff" },
                new Role { RoleId = 3, RoleName = "Tenant" }
            );

            // 2. Accounts
            var accounts = new List<Account>();
            for (int i = 1; i <= 15; i++)
            {
                accounts.Add(new Account
                {
                    AccountId = i,
                    Email = $"user{i}@smarthotel.vn",
                    PasswordHash = "$2b$10$fQGVdsimRD4F3yZPlarSs.ZMQdlZteNs/B863ks8JLnemuU.rFBLy", //Default password: "12345678"
                    IsActive = true,
                    CreatedAt = DateTime.Now.AddDays(-30)
                });
            }
            modelBuilder.Entity<Account>().HasData(accounts);

            // 3. AccountRoles
            var accountRoles = new List<AccountRole>();
            for (int i = 1; i <= 15; i++)
            {
                accountRoles.Add(new AccountRole { AccountId = i, RoleId = (i <= 2 ? 1 : (i <= 5 ? 2 : 3)) });
            }
            modelBuilder.Entity<AccountRole>().HasData(accountRoles);

            // 4. Users (Staff/Admins)
            var users = new List<User>();
            for (int i = 1; i <= 5; i++)
            {
                users.Add(new User { UserId = i, AccountId = i, FullName = $"Employee {i}" });
            }
            modelBuilder.Entity<User>().HasData(users);

            // 5. Tenants
            var tenants = new List<Tenant>();
            for (int i = 1; i <= 10; i++) // Accounts 6-15
            {
                tenants.Add(new Tenant { TenantId = i, AccountId = i + 5, FullName = $"Tenant {i}", CCCD = $"00120300{1000 + i}", Phone = "0897908343" });
            }
            modelBuilder.Entity<Tenant>().HasData(tenants);

            // 6. Rooms
            var rooms = new List<Room>();
            for (int i = 1; i <= 15; i++)
            {
                rooms.Add(new Room
                {
                    RoomId = i,
                    RoomNumber = $"P.{100 + i}",
                    Floor = (i / 5) + 1,
                    Price = 2000000m + (i * 100000),
                    Capacity = 2,
                    Status = "Available",
                    ImageUrl = "default.jpg"
                });
            }
            modelBuilder.Entity<Room>().HasData(rooms);

            // 7. Services
            var services = new List<Service>();
            string[] sUnits = { "kWh", "m3", "Month", "Times", "Person" };
            for (int i = 1; i <= 15; i++)
            {
                services.Add(new Service
                {
                    ServiceId = i,
                    ServiceName = $"Service {i}",
                    UnitPrice = 5000m * i,
                    Unit = sUnits[i % 5],
                    IsMeterBased = (i <= 2), // Electricity & Water
                    IsActive = true
                });
            }
            modelBuilder.Entity<Service>().HasData(services);

            // 8. Contracts
            var contracts = new List<Contract>();
            for (int i = 1; i <= 15; i++)
            {
                contracts.Add(new Contract
                {
                    ContractId = i,
                    TenantId = (i % 10) + 1,
                    RoomId = (i % 15) + 1,
                    StartDate = DateTime.Now.AddMonths(-2),
                    EndDate = DateTime.Now.AddMonths(10),
                    DepositAmount = 5000000m,
                    Price = 2500000m,
                    CreatedAt = DateTime.Now.AddMonths(-2)
                });
            }
            modelBuilder.Entity<Contract>().HasData(contracts);

            // 9. MeterReadings
            var readings = new List<MeterReading>();
            for (int i = 1; i <= 15; i++)
            {
                readings.Add(new MeterReading
                {
                    MeterReadingId = i,
                    ContractId = i,
                    ServiceId = (i % 2) + 1, // Linking to Electricity or Water
                    OldIndex = 100,
                    NewIndex = 150 + (i * 2),
                    ReadingDate = DateTime.Now
                });
            }
            modelBuilder.Entity<MeterReading>().HasData(readings);

            // 10. Invoices
            var invoices = new List<Invoice>();
            for (int i = 1; i <= 15; i++)
            {
                invoices.Add(new Invoice
                {
                    InvoiceId = i,
                    ContractId = i,
                    InvoiceMonth = new DateTime(2026, 03, 01),
                    TotalAmount = 3000000m,
                    IsPaid = (i % 2 == 0)
                });
            }
            modelBuilder.Entity<Invoice>().HasData(invoices);

            // 11. InvoiceDetails
            var details = new List<InvoiceDetail>();
            for (int i = 1; i <= 15; i++)
            {
                details.Add(new InvoiceDetail
                {
                    InvoiceDetailId = i,
                    InvoiceId = i,
                    ServiceId = (i % 15) + 1,
                    Quantity = 1,
                    UnitPrice = 50000m,
                    Amount = 50000m
                });
            }
            modelBuilder.Entity<InvoiceDetail>().HasData(details);

            // 12. RefreshTokens
            var tokens = new List<RefreshToken>();
            for (int i = 1; i <= 15; i++)
            {
                tokens.Add(new RefreshToken
                {
                    RefreshTokenId = i,
                    AccountId = i,
                    Token = Guid.NewGuid().ToString(),
                    ExpiredAt = DateTime.Now.AddDays(7), // Consistent with your model
                    IsRevoked = false
                });
            }
            modelBuilder.Entity<RefreshToken>().HasData(tokens);
        }
    }
}