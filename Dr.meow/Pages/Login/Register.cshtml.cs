using Dr.meow.Data;
using Dr.meow.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims; // 為了使用 Challenge 函式

namespace Dr.meow.Pages
{
    public class RegisterModel : PageModel
    {
        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();

        public string ErrorMessage { get; set; }
        public string SuccessMessage { get; set; }

        public void OnGet()
        {
        }

        private readonly DrMeowDbContext _context;

        public RegisterModel(DrMeowDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // 1️⃣ 檢查帳號是否重複（用 Account）
            bool accountExists = await _context.Users
                .AnyAsync(u => u.Email == Input.Email);

            if (accountExists)
            {
                ErrorMessage = "此電子郵件已被註冊。";
                return Page();
            }

            // 2️⃣ 建立 User
            var user = new User
            {
                Account = Input.Account,
                Email = Input.Email,
                PasswordHash = Input.Password, // 先明文，之後再補 Hash
                AccountType = "User",
                IsActive = true
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync(); // 先存，拿 UserId

            // 3️⃣ 找對應的 Role（只能是 team1 / team2）
            var role = await _context.Roles
                .FirstAsync(r => r.RoleName == Input.Role);

            // 4️⃣ 建立 UserRole
            var userRole = new UserRole
            {
                UserId = user.UserId,
                RoleId = role.RoleId
            };

            _context.UserRoles.Add(userRole);
            await _context.SaveChangesAsync();

            // 5️⃣ 成功訊息
            SuccessMessage = "註冊成功！請前往登入。";

            ModelState.Clear();
            Input = new InputModel();

            return Page();
        }


        // 🎯 新增的處理函式：處理 Google 註冊的 POST 請求
        public IActionResult OnPostGoogleRegistration(string returnUrl = null)
        {
            // 外部登入提供者的名稱，這必須和您在 Program.cs 中設定的名稱一致 (即 "Google")
            var provider = "Google";

            // 設定回傳 URL。Google 驗證成功後，會被導向 GoogleCallback 頁面
            var redirectUrl = Url.Page("/GoogleCallback", new { returnUrl });

            // 啟動 Challenge 流程，將使用者導向 Google 登入頁面
            var properties = new Microsoft.AspNetCore.Authentication.AuthenticationProperties { RedirectUri = redirectUrl };

            // 使用 Challenge() 函式觸發 Google OAuth 流程
            return Challenge(properties, provider);
        }
        public class InputModel
        {
            [Required(ErrorMessage = "姓名是必填的")]
            [Display(Name = "姓名")]
            public string Name { get; set; }

            [Required(ErrorMessage = "電子郵件是必填的")]
            [EmailAddress(ErrorMessage = "請輸入有效的電子郵件格式")]
            [Display(Name = "電子郵件")]
            public string Email { get; set; }

            [Required(ErrorMessage = "帳號是必填的")]
            [StringLength(20, MinimumLength = 4, ErrorMessage = "帳號長度需為 4-20 個字元")]
            [Display(Name = "帳號")]
            public string Account { get; set; }

            [Required(ErrorMessage = "密碼是必填的")]
            [DataType(DataType.Password)]
            [StringLength(100, MinimumLength = 6, ErrorMessage = "密碼長度至少需 6 個字元")]
            [Display(Name = "密碼")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "確認密碼")]
            [Compare("Password", ErrorMessage = "兩次輸入的密碼不一致")]
            public string ConfirmPassword { get; set; }

            [Required(ErrorMessage = "請選擇單位")]
            [Display(Name = "角色")]
            public string Role { get; set; } // team1 / team2
        }
    }
}