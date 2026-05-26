using Dr.meow.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Dr.meow.Pages.API
{
    [IgnoreAntiforgeryToken(Order = 1001)]
    public class ReadNotificationsModel : PageModel
    {
        private readonly DrMeowDbContext _db;

        public ReadNotificationsModel(DrMeowDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var userId = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userId))
            {
                return new JsonResult(new { success = false });
            }

            var notifications = await _db.Notifications
                .Where(x => x.UserId == userId && x.IsRead == false)
                .ToListAsync();

            foreach (var n in notifications)
            {
                n.IsRead = true;
            }

            await _db.SaveChangesAsync();

            return new JsonResult(new
            {
                success = true,
                count = notifications.Count
            });
        }
    }
}