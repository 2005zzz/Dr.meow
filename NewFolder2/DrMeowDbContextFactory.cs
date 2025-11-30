using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Dr.meow.Data;

// 這個類別專門讓 EF Core 工具使用
public class DrMeowDbContextFactory : IDesignTimeDbContextFactory<DrMeowDbContext>
{
    public DrMeowDbContext CreateDbContext(string[] args)
    {
        // *** 這裡我們手動將連線字串硬編碼，這是唯一能繞過配置載入失敗的方式 ***
        // 請確保這個連線字串與您的 appsettings.json 中的完全一致
        var connectionString = "Server=(localdb)\\mssqllocaldb;Database=DrMeowDB;Trusted_Connection=True;MultipleActiveResultSets=true";

        var builder = new DbContextOptionsBuilder<DrMeowDbContext>();

        // 強制指定使用 SQL Server
        builder.UseSqlServer(connectionString);

        return new DrMeowDbContext(builder.Options);
    }
}