using Dr.meow.Data;
using Dr.meow.Models;
using Microsoft.EntityFrameworkCore;

namespace Dr.meow.Services
{
    public class AdminScheduleNotificationService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public AdminScheduleNotificationService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await RunScheduleAsync();

                // 每分鐘檢查一次
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }

        private async Task RunScheduleAsync()
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<DrMeowDbContext>();

            var now = DateTime.Now;

            // ✅ 從資料庫讀取管理者設定的通知時間
            var setting = await db.NotificationScheduleSettings
                .FirstOrDefaultAsync();

            // ✅ 如果資料庫沒有設定，先自動建立一筆預設 09:00
            if (setting == null)
            {
                setting = new NotificationScheduleSetting
                {
                    Hour = 9,
                    Minute = 0,
                    IsEnabled = true,
                    UpdatedAt = DateTime.Now
                };

                db.NotificationScheduleSettings.Add(setting);
                await db.SaveChangesAsync();
            }

            // ✅ 如果管理者關閉通知，就不執行
            if (!setting.IsEnabled)
            {
                return;
            }

            // ✅ 時間不符合就不執行
            if (now.Hour != setting.Hour || now.Minute != setting.Minute)
            {
                return;
            }

            // ✅ 檢查是否有未審核需求單
            var pendingRequestCount = await db.RequestTickets
                .CountAsync(x => x.Status == 1);

            // ✅ 檢查是否有未審核變更單
            var pendingVulnerabilityCount = await db.Vulnerability
                .CountAsync(x => x.Status == "Pending");

            var totalPending = pendingRequestCount + pendingVulnerabilityCount;

            // ✅ 沒有未審核表單就不通知
            if (totalPending <= 0)
            {
                return;
            }

            await NotifyAdmins(
                db,
                "表單處理提醒",
                $"目前有 {totalPending} 筆未審核表單，請至管理者後台查看。"
            );
        }

        private async Task NotifyAdmins(DrMeowDbContext db, string title, string message)
        {
            var now = DateTime.Now;

            // ✅ 同一分鐘內避免重複新增
            var notificationKey = $"PendingForms_{now:yyyyMMddHHmm}";

            var adminIds = await db.Users
                .Where(u => u.AccountType == "Admin")
                .Select(u => u.UserId.ToString())
                .ToListAsync();

            foreach (var adminId in adminIds)
            {
                var exists = await db.Notifications.AnyAsync(n =>
                    n.UserId == adminId &&
                    n.Message.Contains(notificationKey));

                if (!exists)
                {
                    db.Notifications.Add(new Notification
                    {
                        UserId = adminId,
                        Title = title,
                        Message = $"{message}（通知代碼：{notificationKey}）",
                        IsRead = false,
                        CreatedAt = now
                    });
                }
            }

            await db.SaveChangesAsync();
        }
    }
}