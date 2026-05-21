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
            var isAdmin = HttpContext.Session.GetString("IsAdmin");

            if (string.IsNullOrEmpty(userId))
            {
                return new JsonResult(new List<object>());
            }

            // ✅ 只有管理者可以看到通知
            if (isAdmin != "1")
            {
                return new JsonResult(new List<object>());
            }

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