using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.Cookies;
using Dr.meow.Data;
using Microsoft.EntityFrameworkCore;


namespace Dr.meow.Pages.Login
{
    public class LoginModel : PageModel
    {
        private readonly DrMeowDbContext _context;
        public LoginModel(DrMeowDbContext context)
        {
            _context = context;
        }
        [BindProperty]
        public string Account { get; set; } = "";

        [BindProperty]
        public string Password { get; set; } = "";

        [BindProperty]
        public string Role { get; set; } = "user";

        public string? ErrorMessage { get; set; }
        public void OnGet()
        {
            // 只清掉你自己用的登入資訊，別把 OAuth 流程用到的狀態也清掉
            HttpContext.Session.Remove("Account");
            HttpContext.Session.Remove("Role");
            HttpContext.Session.Remove("UserId");
            HttpContext.Session.Remove("IsAdmin");
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrWhiteSpace(Account) || string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "請輸入帳號與密碼。";
                Role = "user"; // 強制回 user
                return Page();
            }
            var user = await _context.Users
    　　　　　　.Include(u => u.UserRoles)
    　　　　　　.ThenInclude(ur => ur.Role)
    　　　　　　.FirstOrDefaultAsync(u => u.Account == Account && u.IsActive);

            if (user == null)
            {
                ErrorMessage = "帳號不存在或已停用。";
                Role = "user";
                return Page();
            }
            if (user.PasswordHash != Password)
            {
                ErrorMessage = "密碼錯誤。";
                Role = "user";
                return Page();
            }
            var roleNames = user.UserRoles.Select(ur => ur.Role.RoleName).ToList();

            bool isAdmin = user.AccountType == "Admin";

            // 使用者勾選管理者入口，但他不是管理者 → 不准進
            if (Role == "admin" && !isAdmin)
            {
                ErrorMessage = "此帳號不是管理者，已切換為使用者登入。";

                ModelState.Remove("Role");// ⭐把使用者剛剛POST上來的 Role(admin)從 ModelState 清掉
                Role = "user";
                return Page(); // 你說「無法登入」→ 這裡直接擋下來
            }

            // 取得使用者的主要角色名稱 (假設一個人目前只有一個主要角色)
            var userRoleName = user.UserRoles.FirstOrDefault()?.Role?.RoleName ?? "一般人員";

            // 寫入 Session
            HttpContext.Session.SetString("Account", user.Account ?? "");
            HttpContext.Session.SetString("UserId", user.UserId.ToString());
            HttpContext.Session.SetString("IsAdmin", isAdmin ? "1" : "0");
            // ⭐ 加入這一行，把資料庫裡的 Department 存進名為 "UserTeam" 的 Session 裡
            HttpContext.Session.SetString("UserTeam", user.Department ?? "未分配部門");
            HttpContext.Session.SetString("UserRoleName", userRoleName);      // ⭐ 新增：存入 "Team1主管" 或 "工程師"

            // 管理者可以選 admin 或 user 入口
            if (Role == "admin" && isAdmin)
                return RedirectToPage("/AdminHome");

            return RedirectToPage("/UserHome");
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