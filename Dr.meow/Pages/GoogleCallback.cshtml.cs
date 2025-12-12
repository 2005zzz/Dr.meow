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

            // --- 🚨 權限檢查邏輯開始 ---
            string userRole = "user"; // 預設為一般使用者

            // 📢 請將 'your.admin.email@gmail.com' 替換成您的真實 Google 郵箱！
            const string AdminEmail = "roupg123456@gmail.com";

            if (email is not null && email.Equals(AdminEmail, StringComparison.OrdinalIgnoreCase))
            {
                userRole = "admin"; // 如果郵箱匹配，則升級為管理者
            }
            // --- 權限檢查邏輯結束 ---

            // 寫入 Session
            HttpContext.Session.SetString("Account", email ?? "");
            HttpContext.Session.SetString("Role", userRole);

            // 🎯 根據身份導向不同首頁 
            if (userRole == "admin")
            {
                // 管理者導向 AdminHome
                return RedirectToPage("/AdminHome");
            }
            else
            {
                // 一般使用者導向 UserHome
                return RedirectToPage("/UserHome");
            }
        }
    }
}