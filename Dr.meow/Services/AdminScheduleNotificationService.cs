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
                await RunSchedules();

                // 每分鐘檢查一次時間
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }

        private async Task RunSchedules()
        {
            var now = DateTime.Now;

            // ✅ 測試用：每分鐘前 10 秒產生一次通知
            // 測完後請改回：now.Hour == 9 && now.Minute == 0
            if (now.Hour == 9 && now.Minute == 0)
            {
                await NotifyAdmins(
                    "表單處理提醒",
                    "今日待辦：請檢查逾期未處理案件，並確認每日待審核清單。",
                    "TEST_0900");
            }

            // 每日 13:00
            if (now.Hour == 13 && now.Minute == 0)
            {
                await NotifyAdmins(
                    "資料同步檢查",
                    "請確認各系統資料一致性，並記錄今日同步狀態。",
                    "Daily_1300");
            }

            // 每週一 08:00
            if (now.DayOfWeek == DayOfWeek.Monday && now.Hour == 8 && now.Minute == 0)
            {
                await NotifyAdmins(
                    "上週績效報告",
                    "請檢視上週表單處理時效、熱門查詢主題與系統使用情況報告。",
                    "Weekly_Mon_0800");
            }

            // 每週六 02:00
            if (now.DayOfWeek == DayOfWeek.Saturday && now.Hour == 2 && now.Minute == 0)
            {
                await NotifyAdmins(
                    "知識庫更新",
                    "請檢查 SharePoint 文件異動、重新索引更新內容，並確認向量資料庫優化狀態。",
                    "Weekly_Sat_0200");
            }

            // 每月 25 日 09:00
            if (now.Day == 25 && now.Hour == 9 && now.Minute == 0)
            {
                await NotifyAdmins(
                    "每月合規報告",
                    "請執行文件時效性檢查、政策遵循狀態評估，並產生管理階層報告。",
                    "Monthly_25_0900");
            }
        }

        private async Task NotifyAdmins(string title, string message, string scheduleKey)
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<DrMeowDbContext>();

            // ✅ 用 yyyyMMddHHmm，確保同一分鐘不會重複新增
            var notificationKey = $"{scheduleKey}_{DateTime.Now:yyyyMMddHHmm}";

            // 找管理者帳號
            var adminIds = await db.Users
                .Where(u => u.AccountType == "Admin")
                .Select(u => u.UserId.ToString())
                .ToListAsync();

            foreach (var adminId in adminIds)
            {
                bool exists = await db.Notifications.AnyAsync(n =>
                    n.UserId == adminId &&
                    n.Title == title &&
                    n.Message.Contains(notificationKey));

                if (!exists)
                {
                    db.Notifications.Add(new Notification
                    {
                        UserId = adminId,
                        Title = title,
                        Message = $"{message}（排程代碼：{notificationKey}）",
                        IsRead = false,
                        CreatedAt = DateTime.Now
                    });
                }
            }

            await db.SaveChangesAsync();
        }
    }
}