using Microsoft.EntityFrameworkCore;
using Dr.meow.Models;

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

        // 您未來可以將 QueryLog.cs 等其他模型加到這裡
    }
}