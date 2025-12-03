using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

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

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // --- 模擬註冊邏輯 (未來這裡接資料庫) ---

            // 1. 檢查帳號是否重複 (模擬)
            if (Input.Account == "admin" || Input.Account == "user")
            {
                ErrorMessage = "此帳號已被使用，請更換一組。";
                return Page();
            }

            // 2. 模擬寫入資料庫成功
            // ... Save to DB ...

            // 3. 顯示成功訊息
            SuccessMessage = "註冊成功！請前往您的電子信箱收取驗證信。";

            // 清空表單，避免重複提交
            ModelState.Clear();
            Input = new InputModel();

            return Page();
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
        }
    }
}