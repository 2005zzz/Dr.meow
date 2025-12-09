using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;

namespace Dr.meow.Pages
{
    public class AdminHomeModel : PageModel
    {
        public string Account { get; set; } = "管理者";

        public IActionResult OnGet()
        {
            // 讀取 Session
            Account = HttpContext.Session.GetString("Account");
            var role = HttpContext.Session.GetString("Role");

            // ❗尚未登入 → 退回登入頁
            if (string.IsNullOrEmpty(Account))
            {
                return RedirectToPage("/Login");
            }

            // ❗登入但不是 admin → 禁止進入
            if (role != "admin")
            {
                return Content("🚫 你沒有權限進入管理後台，請使用管理者帳號登入！");
                // 或可改成 👉 return RedirectToPage("/UserHome");
            }

            return Page();
        }
    }
}
