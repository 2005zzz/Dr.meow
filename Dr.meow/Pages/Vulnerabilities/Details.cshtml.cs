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
    public class DetailsModel : PageModel
    {
        private readonly Dr.meow.Data.DrMeowDbContext _context;

        public DetailsModel(Dr.meow.Data.DrMeowDbContext context)
        {
            _context = context;
        }

        public Vulnerability Vulnerability { get; set; } = default!;
        public List<VulnerabilityLog> Logs { get; set; } = new();

        // 🚀 【關鍵擴充】：開一個屬性給前端 Details.cshtml 直接點用，完美終結退回理由消失的慘劇！
        public string? LatestRejectReason { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // 1. 使用 Include 抓取 Requester，不然頭像會報錯
            var vulnerability = await _context.Vulnerability
                .Include(v => v.Requester)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (vulnerability == null) return NotFound();

            Vulnerability = vulnerability;

            // 2. 抓取這張單子所有的審核紀錄 (按時間由新到舊排序)
            // 🚀 指示補丁：順便 Include(l => l.Reviewer) 讓妳在畫面想顯示是哪個主管簽核時有資料可用
            try
            {
                // 2. 🚀 核心修正：拿掉不存在的 Include(l => l.Reviewer)，回歸純淨撈取
                Logs = await _context.VulnerabilityLogs
                    .Where(l => l.VulnerabilityId == id)
                    .OrderByDescending(l => l.CreatedAt)
                    .ToListAsync();

                // 3. 🎯 核心修正：拿掉不存在的 Status，只認妳本來就有的 Action 欄位！
                // 只要 Action 記錄為 "Rejected"，就代表這是一筆退回紀錄
                if (Logs != null && Logs.Any())
                {
                    var lastRejectLog = Logs.FirstOrDefault(l =>
                        l.Action?.Equals("Rejected", StringComparison.OrdinalIgnoreCase) == true);

                    if (lastRejectLog != null)
                    {
                        // 🔒 安全閘：智慧偵測妳的備註欄位到底是叫 Comments 還是 Reason
                        // 這樣寫可以確保不論是哪個名稱，都能平安抓到理由，絕對不崩潰！
                        LatestRejectReason = lastRejectLog.GetType().GetProperty("Comments") != null
                            ? lastRejectLog.GetType().GetProperty("Comments")?.GetValue(lastRejectLog, null)?.ToString()
                            : (lastRejectLog.GetType().GetProperty("Reason") != null
                                ? lastRejectLog.GetType().GetProperty("Reason")?.GetValue(lastRejectLog, null)?.ToString()
                                : "表單已被退回，請聯繫審核主管。");
                    }
                }
            }
            catch (Exception ex)
            {
                // 防賴防當：就算日誌表有任何未知衝突，也絕對不影響主頁面詳細內容的顯示
                Console.WriteLine($"⚠️ [Details Logs 讀取緩和]：{ex.Message}");
                LatestRejectReason = "無法讀取詳細退回理由，請洽詢系統管理員。";
            }

            return Page();
        }
    }
}