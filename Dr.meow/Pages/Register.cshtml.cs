using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Dr.meow.Data;
using Dr.meow.Helpers;
using Dr.meow.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Dr.meow.Pages
{
    public class RegisterModel : PageModel
    {
        private readonly DrMeowDbContext _context;

        public RegisterModel(DrMeowDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();

        public string? ErrorMessage { get; set; }
        public string? SuccessMessage { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var account = Input.Account.Trim();

            // 1️⃣ 檢查帳號是否已存在
            var exists = await _context.Users.AnyAsync(u => u.Account == account);
            if (exists)
            {
                ErrorMessage = "此帳號已被使用，請更換一組。";
                return Page();
            }

            // 2️⃣ 建立 User 實體，密碼做雜湊後存進 DB
            var user = new User
            {
                Account = account,
                Email = Input.Email.Trim(),
                PasswordHash = PasswordHelper.HashPassword(Input.Password),
                Role = "user",                 // 預設一般使用者
                CreateDate = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // 3️⃣ 顯示成功訊息
            SuccessMessage = "註冊成功！請使用此帳號登入系統。";

            // 4️⃣ 清空表單，避免重複送出
            ModelState.Clear();
            Input = new InputModel();

            return Page();
        }

        public class InputModel
        {
            [Required(ErrorMessage = "姓名是必填的")]
            [Display(Name = "姓名")]
            public string Name { get; set; } = string.Empty;

            [Required(ErrorMessage = "電子郵件是必填的")]
            [EmailAddress(ErrorMessage = "請輸入有效的電子郵件格式")]
            [Display(Name = "電子郵件")]
            public string Email { get; set; } = string.Empty;

            [Required(ErrorMessage = "帳號是必填的")]
            [StringLength(20, MinimumLength = 4, ErrorMessage = "帳號長度需為 4-20 個字元")]
            [Display(Name = "帳號")]
            public string Account { get; set; } = string.Empty;

            [Required(ErrorMessage = "密碼是必填的")]
            [DataType(DataType.Password)]
            [StringLength(100, MinimumLength = 6, ErrorMessage = "密碼長度至少需 6 個字元")]
            [Display(Name = "密碼")]
            public string Password { get; set; } = string.Empty;

            [DataType(DataType.Password)]
            [Display(Name = "確認密碼")]
            [Compare("Password", ErrorMessage = "兩次輸入的密碼不一致")]
            public string ConfirmPassword { get; set; } = string.Empty;
        }
    }
}
