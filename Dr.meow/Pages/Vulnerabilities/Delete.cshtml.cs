using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Dr.meow.Data;
using Dr.meow.Models;

namespace Dr.meow.Pages.Vulnerabilities
{
    public class DeleteModel : PageModel
    {
        private readonly Dr.meow.Data.DrMeowDbContext _context;

        public DeleteModel(Dr.meow.Data.DrMeowDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Vulnerability Vulnerability { get; set; } = default!;

        // ⭐ 新增屬性：用於前端判斷是否顯示「刪除成功」訊息
        [ViewData]
        public bool IsDeletedSuccess { get; set; } = false;
        // 🎯 輔助方法：創建一個安全的空 Vulnerability 實例，避免前端 Razor 崩潰 (即「清空欄位」)
        private Vulnerability CreateBlankVulnerability()
        {
            return new Vulnerability
            {
                Id = -1,
                TicketNumber = "",
                SystemCategory = "",
                TicketCategory = "",
                ChangeType = "",
                Severity = "",
                Description = "",
                Status = "",
                CreatedAt = DateTime.MinValue
                // 💡 其他如 AssignedTo, TestPlan, RecoveryPlan 已經不在 Model 裡了，所以不必設值
            };
        }


        public async Task<IActionResult> OnGetAsync(int? id)
        {
            // ⭐ 1. 處理刪除成功後的頁面狀態
            // 如果沒有 ID (id == null)，但 TempData 裡面有成功訊息，
            // 則表示這是從 OnPost 刪除成功後重定向回來的頁面，應該顯示成功狀態。
            if (id == null)
            {
          
                if (TempData.ContainsKey("StatusMessage"))
                {
                    // 設定為成功狀態，讓前端 Razor 顯示成功畫面
                    IsDeletedSuccess = true;
                    // 我們不需要再從資料庫抓取 VULNERABILITY，直接返回 Page
                    Vulnerability = CreateBlankVulnerability();
                    return Page();
                }

                // 如果沒有 ID 且沒有成功訊息，則認為請求無效 (讓前端顯示無效提示)
                return RedirectToPage("/Forms/FormsList");
            }

            // 2. 正常顯示刪除確認頁面
            var vulnerability = await _context.Vulnerability.FirstOrDefaultAsync(m => m.Id == id);

            if (vulnerability == null)
            {
                // 如果找不到項目，將錯誤訊息存入 TempData (可選：如果想顯示特定錯誤)
                var dateText = DateTime.Today.ToString("yyyy/MM/dd"); // fallback 用

                TempData["StatusMessage"] =
                    $"錯誤❌ 找不到 {dateText} ID 為 {id} 的變更申請單。";
                Vulnerability = CreateBlankVulnerability();
                return Page();
            }
            else
            {
                Vulnerability = vulnerability;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                // 如果沒有 ID，理論上不會發生，但作為安全檢查
                return NotFound();
            }

            var vulnerability = await _context.Vulnerability.FindAsync(id);

            if (vulnerability != null)
            {
                try
                {
                    // 💡 注意：因為資料庫設有 ON DELETE CASCADE，
                    // 刪除這筆 Vulnerability 時，相關的 VulnerabilityLogs 會被自動清掉。
                    _context.Vulnerability.Remove(vulnerability);
                    await _context.SaveChangesAsync();

                    TempData["StatusMessage"] = "✅ 申請單已成功刪除！";
                    // 重定向回當前頁面，但不帶 ID
                    return RedirectToPage(new { id = (int?)null });
                }
                catch (DbUpdateException)
                {
                    // 處理資料庫刪除失敗的錯誤
                    TempData["StatusMessage"] = "❌ 資料庫錯誤：刪除申請單時發生錯誤。";
                    // 重新加載頁面以顯示錯誤，並保留當前 VULNERABILITY 資訊
                    return RedirectToPage(new { id = id });
                }
            }
            else
            {
                // 如果 post 時找不到資料 (可能已被其他用戶刪除)
                TempData["StatusMessage"] = "❌ 錯誤：該申請單可能已被刪除或不存在。";
                return RedirectToPage(null, new { id = (int?)null });
            }
        }
    }
}