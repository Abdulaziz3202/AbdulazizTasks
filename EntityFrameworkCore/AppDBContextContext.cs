using Microsoft.EntityFrameworkCore;
using KPMGTask.Models;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace KPMGTask.EntityFrameworkCore
{
    public class AppDBContextContext : IdentityDbContext<User, Role, Guid>
    {

        public AppDBContextContext(): base(){}

        public AppDBContextContext(DbContextOptions<AppDBContextContext> opt) : base(opt) { }
        public DbSet<Permission> Permissions { get; set; }

        public DbSet<RolePermissions> RolePermissions { get; set; }
        public DbSet<Role> Roles { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<TransactionHistory> TransactionHistory { get; set; }

        public DbSet<TransactionType> TransactionType { get; set; }
     
        public DbSet<FinancialAccount> FinancialAccount { get; set; }
        public DbSet<UserNotification> UserNotification { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
        

    }
}
