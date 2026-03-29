using Dr.meow.Data;
using Dr.meow.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

namespace Dr.meow.Pages.Forms
{
    [IgnoreAntiforgeryToken]
    public class ChangeManagementModel : PageModel
    {
        private readonly DrMeowDbContext _db;

        public ChangeManagementModel(DrMeowDbContext db)
        {
            _db = db;
        }
        public class VulnerabilityVM
        {
            public Vulnerability Main { get; set; } = default!;
            public string? LatestRejectReason { get; set; }
            public string? FirstReviewComment { get; set; }
        }

        public List<VulnerabilityVM> Pending { get; set; } = new();
        public List<VulnerabilityVM> Tracking { get; set; } = new(); // 處理中
        public List<VulnerabilityVM> Closed { get; set; } = new();   // 已結案
        public List<VulnerabilityVM> Rejected { get; set; } = new();

        public async Task OnGetAsync()
        {
            // 1. 取得目前主管的組別 (例如: Team1)
            var myTeam = HttpContext.Session.GetString("UserTeam") ?? "";
            var userIdStr = HttpContext.Session.GetString("UserId") ?? "0";
            int currentUserId = int.Parse(userIdStr);
            System.Diagnostics.Debug.WriteLine($"🔍 目前登入者 ID: {currentUserId}");
            var baseQuery = _db.Vulnerability.Include(v => v.Requester);

            // --- 1. ⏳ 待處理 (Pending) ---
            Pending = await _db.Vulnerability
                .Include(v => v.Requester) // 🚀 重要：沒有這一行，卡片上的提單人頭像會抓不到資料
                .Where(x =>
                    (x.Department == myTeam && x.Status == "Pending") ||
                    (x.Department != myTeam && x.Status == "PendingCross") ||
                    (x.Status == "PendingCISO" && /* 這裡判斷是否為資安長角色 */ false)
                )
                .OrderByDescending(x => x.CreatedAt)
                .Select(x => new VulnerabilityVM
                {
                    Main = x,
                    // 💡 待處理單據通常不需要 LatestRejectReason，除非妳想在初審時看到「之前的退回紀錄」
                    // 如果需要，可以比照 Rejected 的寫法把子查詢加在這裡
                    FirstReviewComment = _db.VulnerabilityLogs
                    .Where(l => l.VulnerabilityId == x.Id && l.StepName == "組長初審" && l.Action == "Approved")
                    .OrderByDescending(l => l.CreatedAt)
                    .Select(l => l.Comment)
                    .FirstOrDefault()
                })
                .ToListAsync();

            // --- 2. 🔄 處理中 (Tracking) ---
            // 💡 邏輯：我審過(我的ID在裡面)，但狀態還不是最終的 Approved 或 Rejected
            // --- 2. 🔄 處理中 (Tracking) ---
            // 邏輯：我審過(ID在裡面)，但還在流程中(尚未 Approved 或 Rejected)
            Tracking = await _db.Vulnerability
                .Include(v => v.Requester) // 🚀 必加：顯示申請人資訊
                .Where(x =>
                    (x.LastReviewerId == currentUserId) &&
                    x.Status != "Approved" && x.Status != "Rejected")
                .OrderByDescending(x => x.CreatedAt)
                .Select(x => new VulnerabilityVM { Main = x }) // 🚀 裝箱
                .ToListAsync();

            // --- 3. ✅ 已結案 (Closed) ---
            // 邏輯：最終資安長簽完變成 Approved，且跟我的組別有關或是由我審核過的單子
            Closed = await _db.Vulnerability
                .Include(v => v.Requester) // 🚀 必加
                .Where(x => x.Status == "Approved" && (x.Department == myTeam || x.LastReviewerId == currentUserId))
                .OrderByDescending(x => x.CreatedAt)
                .Select(x => new VulnerabilityVM { Main = x }) // 🚀 裝箱
                .ToListAsync();

            // --- 4. ❌ 已退回 (Rejected) ---
            Rejected = await _db.Vulnerability
                .Include(v => v.Requester) // 🚀 記得 Include，頭像第一個字才跑得出來
                .Where(x => x.Status == "Rejected")
                .Where(x =>
                    // 條件 1：我是最後一個退單的人
                    x.LastReviewerId == currentUserId ||
                    // 條件 2：我這組的單子且已被審過 (可能被跨組或資安長退)
                    (x.Department == myTeam && x.UpdatedAt > x.CreatedAt) ||
                    // 條件 3：我這組的單子且由我親手退回
                    (x.Department == myTeam && x.LastReviewerId == currentUserId)
                )
                .OrderByDescending(x => x.UpdatedAt ?? x.CreatedAt)
                .Select(x => new VulnerabilityVM
                {
                    Main = x,
                    // 🎯 核心補丁：去 Log 表幫我翻出最後一筆退回評語
                    LatestRejectReason = _db.VulnerabilityLogs
                        .Where(l => l.VulnerabilityId == x.Id && l.Action == "Rejected")
                        .OrderByDescending(l => l.CreatedAt)
                        .Select(l => l.Comment)
                        .FirstOrDefault()
                })
                .ToListAsync();
        }
        public async Task<IActionResult> OnPostApproveAsync(int id, string comment)
        {
            var item = await _db.Vulnerability.FirstOrDefaultAsync(x => x.Id == id);
            if (item == null) return NotFound();

            var currentUserId = int.Parse(HttpContext.Session.GetString("UserId") ?? "0");
            string stepName = "";

            // 💡 邏輯切換：更新狀態並決定「這張單子現在在哪一關」
            if (item.Status == "Pending")
            {
                item.Status = "PendingCross";
                stepName = "組長初審";
            }
            else if (item.Status == "PendingCross")
            {
                item.Status = "PendingCISO";
                stepName = "跨組覆核";
            }

            // 🚀 核心改動：建立一筆新的 Log 紀錄
            var log = new VulnerabilityLog
            {
                VulnerabilityId = item.Id,
                ReviewerId = currentUserId,
                Action = "Approved",
                Comment = comment,        // 以前存進 item.TeamBossComment，現在存這裡
                StepName = stepName,
                CreatedAt = DateTime.Now
            };

            // 更新主表的基本稽核欄位
            item.LastReviewerId = currentUserId;
            item.UpdatedAt = DateTime.Now;

            _db.VulnerabilityLogs.Add(log); // 告訴 EF：我要新增這筆 Log
            await _db.SaveChangesAsync();   // 一次 SaveChanges 會同時處理「更新主表」跟「新增 Log」

            return new OkResult();
        }

        public async Task<IActionResult> OnPostRejectAsync(int id, string reason)
        {
            var item = await _db.Vulnerability.FirstOrDefaultAsync(x => x.Id == id);
            if (item == null) return NotFound();

            int currentUserId = int.Parse(HttpContext.Session.GetString("UserId") ?? "0");

            // 1. 更新主表狀態
            item.Status = "Rejected";
            item.LastReviewerId = currentUserId;
            item.UpdatedAt = DateTime.Now;

            // 2. 🚀 建立退回紀錄 Log
            var log = new VulnerabilityLog
            {
                VulnerabilityId = item.Id,
                ReviewerId = currentUserId,
                Action = "Rejected",
                Comment = reason,         // 以前存進 item.RejectReason，現在存這裡
                StepName = "審核退回",
                CreatedAt = DateTime.Now
            };

            _db.VulnerabilityLogs.Add(log);
            await _db.SaveChangesAsync();

            return new OkResult();
        }
    }
}