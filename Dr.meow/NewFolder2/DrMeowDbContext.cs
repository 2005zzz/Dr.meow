using Dr.meow.Models;
using Microsoft.EntityFrameworkCore;

namespace Dr.meow.Data
{
    public class DrMeowDbContext : DbContext
    {
        public DrMeowDbContext(DbContextOptions<DrMeowDbContext> options)
            : base(options)
        {
        }
        public DbSet<VulnerabilityAiDetail> VulnerabilityAiDetail { get; set; }
        // ==========================================
        // 1. 基礎帳號權限系統
        // ==========================================
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<Notifications> Notifications { get; set; }

        // ==========================================
        // 2. 弱點/變更管理系統 (Vulnerability System)
        // ==========================================
        public DbSet<Vulnerability> Vulnerability { get; set; }
        public DbSet<VulnerabilityLog> VulnerabilityLogs { get; set; }

        // ==========================================
        // 3. 需求申請系統 (Request System - 已正規化)
        // ==========================================
        public DbSet<RequestTicket> RequestTickets { get; set; }      // 主表
        public DbSet<RequestStatus> RequestStatuses { get; set; }    // 狀態查詢
        public DbSet<RequestAiDetail> RequestAiDetails { get; set; }  // AI 分析
        public DbSet<RequestAuditLog> RequestAuditLogs { get; set; }  // 審核軌跡
        public DbSet<RequestUserInput> RequestUserInputs { get; set; } // 使用者輸入
        public DbSet<AiConsultLogs> AiConsultLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 🔑 複合主鍵設定
            modelBuilder.Entity<UserRole>()
                .HasKey(ur => new { ur.UserId, ur.RoleId });

            // ------------------------------------------
            // 🚀 需求系統關聯設定 (Fluent API)
            // ------------------------------------------

            // A. Ticket <-> User (多對一：誰提的單)
            modelBuilder.Entity<RequestTicket>(entity =>
            {
                // 1. 先設定關聯 (Relationship)
                entity.HasOne(t => t.Requester)
                      .WithMany()
                      .HasForeignKey(t => t.RequesterId)
                      .OnDelete(DeleteBehavior.Restrict);

                // 2. 獨立設定屬性 (Property) 🚀 這樣才不會噴錯
                entity.Property(t => t.Description)
                      .HasColumnType("nvarchar(max)");

                // 如果有 Department 欄位也可以順便設定
                entity.Property(t => t.Department)
                      .HasMaxLength(50);
            });

            // B. Ticket <-> AiDetail (一對一：強關聯)
            modelBuilder.Entity<RequestAiDetail>()
                .HasOne(a => a.RequestTicket)
                .WithOne(t => t.AiDetail)
                .HasForeignKey<RequestAiDetail>(a => a.RequestId)
                .OnDelete(DeleteBehavior.Cascade);

            // C. Ticket <-> UserInput (一對一)
            modelBuilder.Entity<RequestUserInput>()
                .HasOne(u => u.RequestTicket)
                .WithOne(t => t.UserInput)
                .HasForeignKey<RequestUserInput>(u => u.RequestId)
                .OnDelete(DeleteBehavior.Cascade);

            // D. Ticket <-> AuditLogs (一對多)
            modelBuilder.Entity<RequestAuditLog>()
                .HasOne(l => l.RequestTicket)
                .WithMany(t => t.AuditLogs)
                .HasForeignKey(l => l.RequestId)
                .OnDelete(DeleteBehavior.Cascade);

            // E. AuditLog <-> User (多對一：誰操作的)
            modelBuilder.Entity<RequestAuditLog>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(l => l.ActorId)
                .OnDelete(DeleteBehavior.Restrict);

            // ------------------------------------------
            // 🛡️ 弱點管理系統關聯
            // ------------------------------------------
            modelBuilder.Entity<VulnerabilityLog>()
                .HasOne<Vulnerability>()
                .WithMany()
                .HasForeignKey(l => l.VulnerabilityId)
                .OnDelete(DeleteBehavior.Cascade);

            // 🛡️ 弱點管理系統關聯設定
            modelBuilder.Entity<VulnerabilityAiDetail>(entity =>
            {
                // 強制設定關聯：Vulnerability 擁有一個 VulnerabilityAiDetail
                entity.HasOne(a => a.Vulnerability)
                      .WithOne(v => v.AiDetail) // ⚠️ 請確保 Vulnerability.cs 內有這行屬性
                      .HasForeignKey<VulnerabilityAiDetail>(a => a.VulnerabilityId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // ------------------------------------------
            // 🌱 Seed Data (種子資料)
            // ------------------------------------------
            SeedSystemData(modelBuilder);
        }

        private void SeedSystemData(ModelBuilder modelBuilder)
        {
            // --- 1. 角色資料 (Roles) ---
            modelBuilder.Entity<Role>().HasData(
                new Role { RoleId = 1, RoleName = "工程師" },
                new Role { RoleId = 2, RoleName = "Team1主管" },
                new Role { RoleId = 3, RoleName = "Team2主管" },
                new Role { RoleId = 4, RoleName = "最高階主管" },
                new Role { RoleId = 5, RoleName = "Team1組員" },
                new Role { RoleId = 6, RoleName = "Team2組員" }
            );

            // --- 2. 帳號資料 (Users) ---
            // 注意：密碼建議在正式環境加密，這裡維持妳原本的測試設定
            modelBuilder.Entity<User>().HasData(
                new User { UserId = 1, Account = "enginee", PasswordHash = "123456", Email = "dept@drmeow.com", AccountType = "Admin", IsActive = true, CreatedAt = DateTime.Now },
                new User { UserId = 2, Account = "team1boss", PasswordHash = "123456", Email = "team1@drmeow.com", Department = "Team1", AccountType = "Admin", IsActive = true, CreatedAt = DateTime.Now },
                new User { UserId = 3, Account = "team2boss", PasswordHash = "123456", Email = "team2@drmeow.com", Department = "Team2", AccountType = "Admin", IsActive = true, CreatedAt = DateTime.Now },
                new User { UserId = 4, Account = "superboss", PasswordHash = "123456", Email = "super@drmeow.com", AccountType = "Admin", IsActive = true, CreatedAt = DateTime.Now },
                new User { UserId = 5, Account = "gmember1", PasswordHash = "123456", Email = "user1@drmeow.com", Department = "Team1", AccountType = "User", IsActive = true, CreatedAt = DateTime.Now },
                new User { UserId = 6, Account = "gmember2", PasswordHash = "123456", Email = "user2@drmeow.com", Department = "Team2", AccountType = "User", IsActive = true, CreatedAt = DateTime.Now }
            );

            // --- 3. 帳號與角色對照 (UserRoles) ---
            modelBuilder.Entity<UserRole>().HasData(
                new UserRole { UserId = 1, RoleId = 1 }, // 工程師
                new UserRole { UserId = 2, RoleId = 2 }, // Team1主管
                new UserRole { UserId = 3, RoleId = 3 }, // Team2主管
                new UserRole { UserId = 4, RoleId = 4 }, // 最高階主管
                new UserRole { UserId = 5, RoleId = 5 }, // Team1組員
                new UserRole { UserId = 6, RoleId = 6 }  // Team2組員
            );

            // --- 4. 需求單狀態定義 (RequestStatus) ---
            // 這是對應妳正規化後 RequestTickets.Status (TINYINT) 的來源
            modelBuilder.Entity<RequestStatus>().HasData(
                new RequestStatus { StatusId = 0, StatusName = "PendingAI", Description = "等待 AI 分析" },
                new RequestStatus { StatusId = 1, StatusName = "PendingReview", Description = "等待主管審核" },
                new RequestStatus { StatusId = 2, StatusName = "InDevelopment", Description = "工程師開發中" },
                new RequestStatus { StatusId = 3, StatusName = "Completed", Description = "需求已結案" },
                new RequestStatus { StatusId = 4, StatusName = "Rejected", Description = "需求已被拒絕" }
            );
        }
    }
}