using Dr.meow.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Dr.meow.Pages.Login
{
    public class ForgotPasswordModel : PageModel
    {
        private readonly DrMeowDbContext _db;

        public ForgotPasswordModel(DrMeowDbContext db)
        {
            _db = db;
        }

        [BindProperty]
        public ForgotPasswordInput Input { get; set; } = new();

        public string? ErrorMessage { get; set; }
        public string? SuccessMessage { get; set; }

        public class ForgotPasswordInput
        {
            public string Account { get; set; } = "";
            public string Email { get; set; } = "";
            public string NewPassword { get; set; } = "";
            public string ConfirmPassword { get; set; } = "";
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrWhiteSpace(Input.Account) ||
                string.IsNullOrWhiteSpace(Input.Email) ||
                string.IsNullOrWhiteSpace(Input.NewPassword) ||
                string.IsNullOrWhiteSpace(Input.ConfirmPassword))
            {
                ErrorMessage = "請完整填寫所有欄位。";
                return Page();
            }

            if (Input.NewPassword != Input.ConfirmPassword)
            {
                ErrorMessage = "兩次輸入的新密碼不一致。";
                return Page();
            }

            var user = await _db.Users
                .FirstOrDefaultAsync(u =>
                    u.Account == Input.Account &&
                    u.Email == Input.Email);

            if (user == null)
            {
                ErrorMessage = "查無此帳號或 Email 不符合。";
                return Page();
            }

            user.PasswordHash = Input.NewPassword;

            await _db.SaveChangesAsync();

            TempData["Message"] = "✅ 密碼已重設，請使用新密碼登入。";

            return RedirectToPage("/Login");
        }
    }
}