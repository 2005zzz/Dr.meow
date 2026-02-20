using Dr.meow.Models;
using Microsoft.EntityFrameworkCore;

namespace Dr.meow.Data
{
    // 繼承自 DbContext
    public class DrMeowDbContext : DbContext
    {
        public DrMeowDbContext(DbContextOptions<DrMeowDbContext> options)
            : base(options)
        {
        }

        // 定義一個 Dbset，對應到 SQL Server 中的 Vulnerability 表格
        public DbSet<Vulnerability> Vulnerability { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<RequestForm> RequestForms { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserRole>()
                .HasKey(ur => new { ur.UserId, ur.RoleId });
            modelBuilder.Entity<Role>().HasData(
        new Role { RoleId = 1, RoleName = "單位主管" },
        new Role { RoleId = 2, RoleName = "team1" },
        new Role { RoleId = 3, RoleName = "team2" },
        new Role { RoleId = 4, RoleName = "最高階主管" }
    );

            modelBuilder.Entity<User>().HasData(
    new User
    {
        UserId = 1,
        Account = "deptboss",//需求單單位主管
        PasswordHash = "123456",
        Email = "dept@drmeow.com",
        GoogleId = null,
        AccountType = "Admin",
        IsActive = true
    },
    new User
    {
        UserId = 2,
        Account = "team1boss",//變更單team1主管
        PasswordHash = "123456",
        Email = "team1@drmeow.com",
        GoogleId = null,
        AccountType = "Admin",
        IsActive = true
    },
    new User
    {
        UserId = 3,
        Account = "team2boss",//變更單team2主管
        PasswordHash = "123456",
        Email = "team2@drmeow.com",
        GoogleId = null,
        AccountType = "Admin",
        IsActive = true
    },
    new User
    {
        UserId = 4,
        Account = "superboss",//變更單最高主管
        PasswordHash = "123456",
        Email = "super@drmeow.com",
        GoogleId = null,
        AccountType = "Admin",
        IsActive = true
    }
);
            modelBuilder.Entity<UserRole>().HasData(
    new UserRole { UserId = 1, RoleId = 1 }, // 單位主管（需求單）
    new UserRole { UserId = 2, RoleId = 2 }, // team1 主管（變更單）
    new UserRole { UserId = 3, RoleId = 3 }, // team2 主管（變更單）
    new UserRole { UserId = 4, RoleId = 4 }  // 最高階主管（變更單）
);
        }

        // 您未來可以將 QueryLog.cs 等其他模型加到這裡
    }
}