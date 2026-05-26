using Dr.meow.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Dr.meow.Pages.API
{
    public class NotificationsModel : PageModel
    {
        private readonly DrMeowDbContext _db;

        public NotificationsModel(DrMeowDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var userId = HttpContext.Session.GetString("UserId");

            // 沒登入
            if (string.IsNullOrEmpty(userId))
            {
                return new JsonResult(new List<object>());
            }

            // 撈目前登入者自己的未讀通知
            var list = await _db.Notifications
                .Where(x => x.UserId == userId && !x.IsRead)
                .OrderByDescending(x => x.CreatedAt)
                .Take(10)
                .Select(x => new
                {
                    x.Title,
                    x.Message,
                    Time = x.CreatedAt.ToString("yyyy/MM/dd HH:mm")
                })
                .ToListAsync();

            return new JsonResult(list);
        }
    }
}