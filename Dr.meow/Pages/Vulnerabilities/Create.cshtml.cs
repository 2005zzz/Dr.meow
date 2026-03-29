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
using Microsoft.EntityFrameworkCore;

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
            // 🚀 初始化：預設時間設為「明天的早上 9 點」
            Vulnerability = new Vulnerability
            {
                // 因為資料庫現在是 DateTime?，我們直接給它一個完整的日期時間
                ScheduledTime = DateTime.Today.AddDays(1).AddHours(9)
            };

            return Page();
        }

        [BindProperty]
        public Vulnerability Vulnerability { get; set; } = default!;

        public async Task<IActionResult> OnPostAsync()
        {
            //移除不需要的驗證錯誤（避免因 ID 或單號未填導致失敗）
            ModelState.Remove("Vulnerability.TicketNumber");
            ModelState.Remove("Vulnerability.Department"); // 如果 Model 裡有這欄位也記得移除

            // 1️ 從 Session 抓取「本人」資訊
            var userIdStr = HttpContext.Session.GetString("UserId");
            var userTeam = HttpContext.Session.GetString("UserTeam"); // "Team1" 或 "Team2"

            // 🔒 安全門禁
            if (string.IsNullOrEmpty(userIdStr))
            {
                return RedirectToPage("/Login");
            }


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



            // 3️⃣ 強制身分綁定 (身分隔離的核心)
            // 我們改用 UserId 來對應，並記錄他所屬的 Team
            Vulnerability.RequesterId = int.Parse(userIdStr); // 💡 確保你的 Vulnerability 模型有這欄位
            Vulnerability.Department = userTeam ?? "未分配部門";

            Vulnerability.Status = "Pending";
            Vulnerability.CreatedAt = DateTime.Now;
            Vulnerability.FormType = "Change"; // 確保類別正確

            // 4️⃣ 儲存至資料庫並產生單號
            try
            {
                // 1️⃣ 先存，讓資料庫產生 Id
                _context.Vulnerability.Add(Vulnerability);
                await _context.SaveChangesAsync();

                // 2️⃣ 用「Team 識別碼 + 產生的 Id」組單號 (例如 CHG-T1-20260312-00001)
                string teamShort = (userTeam == "Team1") ? "T1" : "T2";
                Vulnerability.TicketNumber =
                    $"CHG-{teamShort}-{DateTime.Now:yyyyMMdd}-{Vulnerability.Id:D5}";

                // 3️⃣ 再存一次，把單號寫回資料庫
                await _context.SaveChangesAsync();

                // 4️⃣ 成功訊息
                TempData["StatusMessage"] = $"提交成功！你的單號是 <strong>{Vulnerability.TicketNumber}</strong>";

                // 清空表單回到初始狀態
                return RedirectToPage();
            }
            catch (DbUpdateException dbEx)
            {
                // 🎯 這是專門抓資料庫報錯的（例如：欄位太長、必填沒填）
                var innerMsg = dbEx.InnerException?.Message ?? dbEx.Message;
                Debug.WriteLine($"[DB Error] {innerMsg}");
                ModelState.AddModelError("", "資料庫寫入失敗：" + innerMsg);
                return Page();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[General Error] {ex.Message}");
                ModelState.AddModelError("", "發生未知錯誤：" + ex.Message);
                return Page();
            }
        }
    }
}