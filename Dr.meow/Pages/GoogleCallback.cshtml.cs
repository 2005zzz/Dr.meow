using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace Dr.meow.Pages
{
    public class GoogleCallbackModel : PageModel
    {
        public IActionResult OnGet()
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToPage("/Login");

            // 取得 Google 回傳資料
            string? email = User.FindFirst(ClaimTypes.Email)?.Value;
            string? name = User.FindFirst(ClaimTypes.Name)?.Value;

            // 寫入 Session
            HttpContext.Session.SetString("Account", email ?? "");
            HttpContext.Session.SetString("Role", "user");   // 先預設一般使用者身份

            return RedirectToPage("/UserHome");
        }
    }
}
