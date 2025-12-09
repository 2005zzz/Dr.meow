using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;

namespace Dr.meow.Pages
{
    public class UserHomeModel : PageModel
    {
        public string Account { get; set; } = "使用者";

        public IActionResult OnGet()
        {
            // 🔸讀取 Session
            Account = HttpContext.Session.GetString("Account");

            // ❗如果沒有登入，強制退回 Login 頁面
            if (string.IsNullOrEmpty(Account))
            {
                return RedirectToPage("/Login");
            }

            return Page();
        }
    }
}
