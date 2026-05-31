using Microsoft.EntityFrameworkCore;
using ResidenceMngSys.Services;

namespace ResidenceMngSys.Models
{
    public class AppDbContext : DbContext
    {
        private readonly int _tenantId;

        public AppDbContext(DbContextOptions<AppDbContext> options, ITenantService tenantService): base(options)
        {
            _tenantId = tenantService.GetTenantId();
        }

        public DbSet<User> Users { get; set; }
        public DbSet<UserRoles> UserRoles { get; set; }
        public DbSet<Apartment> Apartment { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Wallet> Wallet { get; set; }
        public DbSet<Dues> Dues { get; set; }
        public DbSet<Announcements> Announcements { get; set; }
        public DbSet<FaultAndRequest> FaultAndRequest { get; set; }
        public DbSet<BalanceRequests> BalanceRequests { get; set; }
        public DbSet<WalletTransactions> WalletTransactions { get; set; }
        public DbSet<GeneralSettings> GeneralSettings { get; set; }
        public DbSet<PaymentsInfo> PaymentsInfo { get; set; }
        public DbSet<Tenants> Tenants { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.NoAction;
            }


            // Mevcut ilişkiler
            modelBuilder.Entity<User>()
                .HasOne(u => u.Apartment)
                .WithOne(a => a.User)
                .HasForeignKey<User>(u => u.Apartment_Id);

            modelBuilder.Entity<Wallet>()
                .HasOne(w => w.apartment)
                .WithOne()
                .HasForeignKey<Wallet>(w => w.Apartment_Id);


            // Global Query Filters — her sorguya otomatik WHERE TenantId = x ekler
            modelBuilder.Entity<User>().HasQueryFilter(x => x.TenantId == _tenantId);
            modelBuilder.Entity<Apartment>().HasQueryFilter(x => x.TenantId == _tenantId);
            modelBuilder.Entity<Dues>().HasQueryFilter(x => x.TenantId == _tenantId);
            modelBuilder.Entity<Announcements>().HasQueryFilter(x => x.TenantId == _tenantId);
            modelBuilder.Entity<FaultAndRequest>().HasQueryFilter(x => x.TenantId == _tenantId);
            modelBuilder.Entity<Wallet>().HasQueryFilter(x => x.TenantId == _tenantId);
            modelBuilder.Entity<WalletTransactions>().HasQueryFilter(x => x.TenantId == _tenantId);
            modelBuilder.Entity<BalanceRequests>().HasQueryFilter(x => x.TenantId == _tenantId);
            modelBuilder.Entity<GeneralSettings>().HasQueryFilter(x => x.TenantId == _tenantId);
            modelBuilder.Entity<PaymentsInfo>().HasQueryFilter(x => x.TenantId == _tenantId);
        }

        // INSERT işlemlerinde otomatik TenantId ataması
        public override int SaveChanges()
        {
            SetTenantId();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            SetTenantId();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void SetTenantId()
        {
            foreach (var entry in ChangeTracker.Entries<ITenantEntity>())
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.TenantId = _tenantId;
                }
            }
        }
    }
}