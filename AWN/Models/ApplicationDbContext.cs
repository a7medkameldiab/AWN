using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AWN.Models
{
    public class ApplicationDbContext : IdentityDbContext<Account>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        public DbSet<DonateCase> donateCases { get; set; }
        public DbSet<Photos> photos { get; set; }   
        public DbSet<RequestJoin> requestJoins { get; set; }
        public DbSet<Notification> notifications { get; set; }
        public DbSet<Suggestion> suggestions { get; set; }
        public DbSet<Payment> payments { get; set; }
        public DbSet<Support> supports { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Account>().ToTable("Users", "AwnSc");
            modelBuilder.Entity<IdentityRole>().ToTable("Roles", "AwnSc");
            modelBuilder.Entity<IdentityUserRole<string>>().ToTable("UserRoles", "AwnSc");
            modelBuilder.Entity<IdentityUserClaim<string>>().ToTable("UserClaims", "AwnSc");
            modelBuilder.Entity<IdentityUserLogin<string>>().ToTable("UserLogins", "AwnSc");
            modelBuilder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaims", "AwnSc");
            modelBuilder.Entity<IdentityUserToken<string>>().ToTable("UserTokens", "AwnSc");

            modelBuilder.HasDefaultSchema("AwnSc");

            modelBuilder.Entity<DonateCase>()
                .Property(t => t.TimesTamp)
                .HasDefaultValueSql("GETDATE()");
            
            modelBuilder.Entity<Notification>()
                .Property(t => t.TimesTamp)
                .HasDefaultValueSql("GETDATE()");
            
            modelBuilder.Entity<Payment>()
                .Property(t => t.TimesTamp)
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<RequestJoin>()
               .HasOne(a => a.Account)
               .WithOne(r => r.requestJoins)
               .HasForeignKey<RequestJoin>(r => r.AccountId)
               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Account>()
                .HasMany(a => a.payments)
                .WithOne(p => p.Account)
                .HasForeignKey(p => p.AccountId);

            modelBuilder.Entity<DonateCase>()
               .Property(dc => dc.ExcessAmount)
               .HasComputedColumnSql("CASE WHEN [CurrentAmount] <= [TargetAmount] THEN 0 ELSE [CurrentAmount] - [TargetAmount] END");
        }
    }
}
