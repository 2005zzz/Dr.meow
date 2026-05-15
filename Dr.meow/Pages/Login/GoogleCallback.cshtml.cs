using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies; // 假設您使用 Cookie 認證作為本地方案

namespace Dr.meow.Pages
{
    public class GoogleCallbackModel : PageModel
    {
        // 將 OnGet 改為 OnGetAsync 以支援異步操作
        public async Task<IActionResult> OnGetAsync(string returnUrl = null)
        {
            // 1. 嘗試讀取外部登入 (Google) 驗證結果
            // "External" 是外部驗證方案使用的暫時 Cookie 名稱
            var authenticateResult = await HttpContext.AuthenticateAsync("Google");

            if (!authenticateResult.Succeeded)
            {
                // 如果外部驗證失敗（例如使用者取消或超時），導向回登入頁面
                return RedirectToPage("/Login");
            }

            var externalUser = authenticateResult.Principal;
            string? email = externalUser.FindFirst(ClaimTypes.Email)?.Value;
            string? name = externalUser.FindFirst(ClaimTypes.Name)?.Value;

            // --- 🚨 權限檢查邏輯開始 ---
            string userRole = "user"; // 預設為一般使用者
            const string AdminEmail = "roupg123456@gmail.com";

            if (email is not null && email.Equals(AdminEmail, StringComparison.OrdinalIgnoreCase))
            {
                userRole = "admin"; // 如果郵箱匹配，則升級為管理者
            }
            // --- 權限檢查邏輯結束 ---

            // 2. 建立本地 Claims (包含 Role)
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, email ?? ""),
                new Claim(ClaimTypes.Name, name ?? "Google User"),
                new Claim(ClaimTypes.Role, userRole)
            };

            // 3. 建立本地身分 (使用應用程式主要的 Cookie 認證方案)
            var claimsIdentity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme // 假設您使用預設的 Cookie 認證
            );
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            // 4. 執行本地登入，這將創建 Session Cookie
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                claimsPrincipal
            );

            // 6. 寫入 Session (雖然已經有 Claim，但為了與舊程式碼保持一致性)
            HttpContext.Session.SetString("Account", email ?? "");
            HttpContext.Session.SetString("Role", userRole);


            // 7. 根據身份導向不同首頁
            if (userRole == "admin")
            {
                return RedirectToPage("/AdminHome");
            }
            else
            {
                // 如果 returnUrl 不為空，則導向原目標，否則導向一般首頁
                return LocalRedirect(returnUrl ?? "/UserHome");
            }
        }
    }
}