using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Dr.meow.Pages
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public string Account { get; set; } = "";

        [BindProperty]
        public string Password { get; set; } = "";

        [BindProperty]
        public string Role { get; set; } = "user";

        public string? ErrorMessage { get; set; }

        public void OnGet()
        {
            // 登入頁先清 Session（避免殘留上一位使用者）
            HttpContext.Session.Clear();
        }

        public IActionResult OnPost()
        {
            if (string.IsNullOrWhiteSpace(Account) || Password != "1234")
            {
                ErrorMessage = "帳號或密碼錯誤。";
                return Page();
            }

            // 寫入 Session
            HttpContext.Session.SetString("Account", Account);
            HttpContext.Session.SetString("Role", Role);

            // 依身分導到不同首頁
            if (Role == "admin")
            {
                return RedirectToPage("/AdminHome");
            }
            else
            {
                return RedirectToPage("/UserHome");
            }
        }
        // =======================
        // ⭐ Google 一鍵登入（你新增的）
        // =======================
        public IActionResult OnPostGoogleLogin()
        {
            // Google OAuth 登入完成後會回到這頁
            var props = new AuthenticationProperties
            {
                RedirectUri = Url.Page("/GoogleCallback")  // 登入成功後的跳轉頁
            };

            // 觸發 Google OAuth 流程
            return Challenge(props, GoogleDefaults.AuthenticationScheme);
        }
    }
}