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
            // 初始化一個預設物件，例如將日期設為今天
            Vulnerability = new Vulnerability
            {
                FoundDate = DateTime.Today,
                ScheduledTime = "07:00" // 預設時間
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

            // A. 若單號為空，自動產生 (格式: CHG-yyyyMMdd-亂數)
            if (string.IsNullOrEmpty(Vulnerability.TicketNumber))
            {
                var random = new Random();
                Vulnerability.TicketNumber = $"CHG-{DateTime.Now:yyyyMMdd}-{random.Next(1000, 9999)}";
            }

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
                _context.Vulnerability.Add(Vulnerability);
                await _context.SaveChangesAsync();

                Debug.WriteLine($"[Success] 表單建立成功 ID: {Vulnerability.Id}");

                // 設定成功訊息 (使用 TicketNumber 而非 Title)
                TempData["StatusMessage"] = $"單號 <strong>{Vulnerability.TicketNumber}</strong> 已提交成功！<br/>系統自動風險評估為：{Vulnerability.Severity}";
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[Error] 資料庫存檔失敗: {ex.Message}");
                ModelState.AddModelError("", "存檔失敗，請聯繫管理員。");
                return Page();
            }

            // 4. 導向到列表頁面 (假設您的卡片清單頁面是 Forms/FormsList)
            // 如果您的列表頁是 Index，請改為 RedirectToPage("./Index");
            return RedirectToPage("/Forms/FormsList");
        }
    }
}