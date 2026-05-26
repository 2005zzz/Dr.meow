using Dr.meow.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Dr.meow.Pages.API
{
    [IgnoreAntiforgeryToken]
    public class ClearNotificationsModel : PageModel
    {
        private readonly DrMeowDbContext _db;

        public ClearNotificationsModel(DrMeowDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                var userId = HttpContext.Session.GetString("UserId");

                if (string.IsNullOrEmpty(userId))
                {
                    return new JsonResult(new
                    {
                        success = false,
                        message = "Session 沒有 UserId"
                    });
                }

                await _db.Notifications
                    .Where(x => x.UserId == userId)
                    .ExecuteDeleteAsync();

                return new JsonResult(new
                {
                    success = true
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }
    }
}