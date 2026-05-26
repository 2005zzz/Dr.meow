using Dr.meow.Data;
using Dr.meow.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Dr.meow.Pages.API
{
    public class GetNotificationSettingModel : PageModel
    {
        private readonly DrMeowDbContext _db;

        public GetNotificationSettingModel(DrMeowDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var setting = await _db.NotificationScheduleSettings.FirstOrDefaultAsync();

            if (setting == null)
            {
                setting = new NotificationScheduleSetting
                {
                    Hour = 9,
                    Minute = 0,
                    IsEnabled = true,
                    UpdatedAt = DateTime.Now
                };

                _db.NotificationScheduleSettings.Add(setting);
                await _db.SaveChangesAsync();
            }

            return new JsonResult(new
            {
                hour = setting.Hour,
                minute = setting.Minute,
                isEnabled = setting.IsEnabled
            });
        }
    }
}