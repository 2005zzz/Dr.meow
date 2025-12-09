using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;

namespace Dr.meow.Pages
{
    public class LoginModel : PageModel
    {
        // 🔐 Demo 用帳號密碼（之後你們可以改成查資料庫）
        private const string UserAccount = "user";
        private const string AdminAccount = "admin";
        private const string DemoPassword = "1234";

        [BindProperty]
        public string Account { get; set; } = string.Empty;

        [BindProperty]
        public string Password { get; set; } = string.Empty;

        // "user" or "admin"（前端 radio 綁這個）
        [BindProperty]
        public string Role { get; set; } = "user";

        public string? ErrorMessage { get; set; }

        public void OnGet()
        {
            // 進到登入頁先清掉 Session，避免上一個使用者殘留
            HttpContext.Session.Clear();
        }

        public IActionResult OnPost()
        {
            // 簡單欄位檢查
            if (string.IsNullOrWhiteSpace(Account) ||
                string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "請輸入帳號與密碼。";
                return Page();
            }

            // 根據選擇的角色，做對應帳號檢查
            bool isValid = false;

            if (Role == "admin")
            {
                // 管理者：admin / 1234
                isValid = (Account == AdminAccount && Password == DemoPassword);
            }
            else // 預設當作一般使用者
            {
                // 使用者：user / 1234
                isValid = (Account == UserAccount && Password == DemoPassword);
                Role = "user";   // 保險起見，強制寫回 user
            }

            if (!isValid)
            {
                ErrorMessage = "帳號或密碼錯誤。";
                return Page();
            }

            // ✅ 登入成功 → 寫入 Session
            HttpContext.Session.SetString("Account", Account);
            HttpContext.Session.SetString("Role", Role);

            // 依身分導向不同首頁
            if (Role == "admin")
            {
                return RedirectToPage("/AdminHome");
            }
            else
            {
                return RedirectToPage("/UserHome");
            }
        }
    }
}
