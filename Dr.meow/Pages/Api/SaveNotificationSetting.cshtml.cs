using Dr.meow.Data;
using Dr.meow.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Dr.meow.Pages.API
{
    public class SaveNotificationSettingModel : PageModel
    {
        private readonly DrMeowDbContext _db;

        public SaveNotificationSettingModel(DrMeowDbContext db)
        {
            _db = db;
        }

        public class SaveRequest
        {
            public string TimeText { get; set; } = "09:00";

            public bool IsEnabled { get; set; }
        }

        public async Task<IActionResult> OnPostAsync(
            [FromBody] SaveRequest request)
        {
            if (!TimeSpan.TryParse(request.TimeText, out var time))
            {
                return new JsonResult(new
                {
                    success = false,
                    message = "時間格式錯誤"
                });
            }

            var setting = await _db.NotificationScheduleSettings
                .FirstOrDefaultAsync();

            if (setting == null)
            {
                setting = new NotificationScheduleSetting();

                _db.NotificationScheduleSettings.Add(setting);
            }

            setting.Hour = time.Hours;
            setting.Minute = time.Minutes;
            setting.IsEnabled = request.IsEnabled;
            setting.UpdatedAt = DateTime.Now;

            await _db.SaveChangesAsync();

            return new JsonResult(new
            {
                success = true
            });
        }
    }
}