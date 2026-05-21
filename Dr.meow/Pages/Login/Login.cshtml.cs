using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
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
                Role = "user";
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

            bool isAdmin = user.AccountType == "Admin";

            if (Role == "admin" && !isAdmin)
            {
                ErrorMessage = "此帳號不是管理者，已切換為使用者登入。";
                ModelState.Remove("Role");
                Role = "user";
                return Page();
            }

            var userRoleName = user.UserRoles.FirstOrDefault()?.Role?.RoleName ?? "一般人員";

            HttpContext.Session.SetString("Account", user.Account ?? "");
            HttpContext.Session.SetString("UserId", user.UserId.ToString());
            HttpContext.Session.SetString("IsAdmin", isAdmin ? "1" : "0");
            HttpContext.Session.SetString("Role", isAdmin ? "admin" : "user");
            HttpContext.Session.SetString("UserTeam", user.Department ?? "未分配部門");
            HttpContext.Session.SetString("UserRoleName", userRoleName);

            if (Role == "admin" && isAdmin)
            {
                return RedirectToPage("/AdminHome");
            }

            return RedirectToPage("/UserHome");
        }

        public IActionResult OnPostGoogleLogin()
        {
            var props = new AuthenticationProperties
            {
                RedirectUri = Url.Page("/GoogleCallback")
            };

            return Challenge(props, GoogleDefaults.AuthenticationScheme);
        }
    }
}