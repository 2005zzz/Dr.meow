using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Dr.meow.Data;
using Dr.meow.Models;
using System.Diagnostics;

namespace Dr.meow.Pages.Vulnerabilities
{
    public class CreateModel : PageModel
    {
        private readonly DrMeowDbContext _context;

        public CreateModel(DrMeowDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            Vulnerability = new Vulnerability
            {
                FoundDate = DateTime.Today,
                ScheduledTime = "07:00"
            };

            return Page();
        }

        [BindProperty]
        public Vulnerability Vulnerability { get; set; } = default!;

        public async Task<IActionResult> OnPostAsync()
        {
            // 1. 移除不必要的驗證錯誤 (如果有的話)
            // 有時候 Id 或某些隱藏欄位會導致 ModelState 無效，這裡做個防呆
            ModelState.Remove("Vulnerability.AssignedTo");
            ModelState.Remove("Vulnerability.TicketNumber");

            if (!ModelState.IsValid)
            {
                // Debug 用：印出所有驗證錯誤
                foreach (var state in ModelState)
                {
                    foreach (var error in state.Value.Errors)
                    {
                        Debug.WriteLine($"[欄位錯誤] {state.Key}: {error.ErrorMessage}");
                    }
                }
                return Page();
            }

            // 2. 自動補全邏輯

            // A. 系統產生單號
 


            // B. 自動填入申請人 (若有登入系統)
            if (string.IsNullOrEmpty(Vulnerability.AssignedTo) && User.Identity.IsAuthenticated)
            {
                Vulnerability.AssignedTo = User.Identity.Name; // 抓取目前登入者的帳號
            }
            else if (string.IsNullOrEmpty(Vulnerability.AssignedTo))
            {
                Vulnerability.AssignedTo = "Guest"; // 訪客填寫
            }

            // C. 確保狀態為 Pending
            Vulnerability.Status = "Pending";
            Vulnerability.CreatedAt = DateTime.Now;

            // 3. 儲存至資料庫
            try
            {
                // 1️⃣ 先存，讓資料庫產生 Id
                _context.Vulnerability.Add(Vulnerability);
                await _context.SaveChangesAsync();

                // 2️⃣ 用「已產生的 Id」組單號（一定不會撞）
                Vulnerability.TicketNumber =
                    $"CHG-{DateTime.Now:yyyyMMdd}-{Vulnerability.Id:D5}";

                // 3️⃣ 再存一次，把單號寫回資料庫
                await _context.SaveChangesAsync();

                // 4️⃣ 成功訊息
                TempData["StatusMessage"] =
                    $"提交成功！你的單號是 <strong>{Vulnerability.TicketNumber}</strong>";
                // 清空表單
                Vulnerability = new Vulnerability
                {
                    FoundDate = DateTime.Today,
                    ScheduledTime = "07:00"
                };
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[Error] 資料庫存檔失敗: {ex.Message}");
                ModelState.AddModelError("", "存檔失敗，請聯繫管理員。");
                return Page();
            }
            return Page();
        }
    }
}