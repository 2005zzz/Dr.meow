using Dr.meow.Data;
using Dr.meow.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Dr.meow.Pages.Request
{
    public class DetailsModel : PageModel
    {
        private readonly DrMeowDbContext _db;

        public DetailsModel(DrMeowDbContext db)
        {
            _db = db;
        }

        public RequestTicket Item { get; set; } = default!;

        // ✅ 審核表單輸入（會從畫面 Post 回來）
        [BindProperty]
        public ReviewInput Review { get; set; } = new();

        public class ReviewInput
        {
            public int Id { get; set; }

            public string? AcceptanceContent { get; set; }
            public DateTime? AcceptanceDate { get; set; }

            public string? SecurityAssessment { get; set; }

            public int? SatisfactionNeed { get; set; }
            public int? SatisfactionStability { get; set; }
            public int? SatisfactionOverall { get; set; }

            public decimal? BenefitManDays { get; set; }
            public decimal? BenefitRevenue { get; set; }

            public string? RejectReason { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            // ✅ 關鍵：使用 Include 抓取所有相關表的資料
            var item = await _db.RequestTickets
                .Include(t => t.AiDetail)
                .Include(t => t.UserInput)
                .Include(t => t.AuditLogs)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (item == null) return NotFound();
            Item = item;

            // ✅ 從不同的子表撈回資料填入 ReviewInput
            Review = new ReviewInput
            {
                Id = item.Id,
                // ✅ AI 自動預填主管審核欄位
                AcceptanceContent =
                   item.AiDetail?.AiReviewComment
                   ?? item.AiDetail?.RefinedDescription
                   ?? "（AI 尚未提供建議）",
                SecurityAssessment = item.AiDetail?.SecurityAssessment switch
                {
                    "符合" => "符合",
                    "不適用" => "不適用",
                    "需補件" => "需補件",
                    "低風險" => "符合",
                    "無明顯風險" => "符合",
                    "中" => "需補件",
                    "高" => "需補件",
                    "建議補強" => "需補件",
                    _ => ""
                },
                SatisfactionNeed = item.AiDetail?.AiRequirementScore,
                SatisfactionStability = item.AiDetail?.AiStabilityScore,
                SatisfactionOverall = item.AiDetail?.AiOverallScore,
                BenefitManDays = item.AiDetail?.AiSavedManDays,
                BenefitRevenue = item.AiDetail?.AiRevenue,

                // ✅ 退回原因仍抓最後一筆人工退回紀錄
                RejectReason = item.AuditLogs?
                 .OrderByDescending(l => l.Timestamp)
                 .FirstOrDefault(l => l.Action == "Rejected")?.Comment
            };

            return Page();
        }

        // ✅ 核准
        public async Task<IActionResult> OnPostApproveAsync()
        {
            using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                var item = await _db.RequestTickets.Include(t => t.AiDetail).FirstOrDefaultAsync(x => x.Id == Review.Id);
                if (item == null) return NotFound();

                // 狀態檢查：1 代表 PendingReview
                if (item.Status != 1) return RedirectToPage("/Request/Details", new { id = Review.Id });

                // 1. 更新主表狀態
                item.Status = 2; // 2: InDevelopment
                item.UpdatedAt = DateTime.Now;

                // 2. 更新 AI 細節表的資安評估 (如果主管有修改)
                // 2. 更新 AI 細節表（主管可修改 AI 草稿後，按核准時覆蓋成最終版本）
                if (item.AiDetail == null)
                {
                    item.AiDetail = new RequestAiDetail
                    {
                        RequestId = item.Id,
                        ProcessedAt = DateTime.Now,
                        IsProcessed = true
                    };
                }

                item.AiDetail.AiReviewComment = Review.AcceptanceContent;
                item.AiDetail.SecurityAssessment = Review.SecurityAssessment;
                item.AiDetail.AiRequirementScore = Review.SatisfactionNeed;
                item.AiDetail.AiStabilityScore = Review.SatisfactionStability;
                item.AiDetail.AiOverallScore = Review.SatisfactionOverall;
                item.AiDetail.AiSavedManDays = Review.BenefitManDays;
                item.AiDetail.AiRevenue = Review.BenefitRevenue;

                // 3. 寫入審核軌跡 (Audit Log)
                var userIdStr = HttpContext.Session.GetString("UserId");
                if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out var userId))
                {
                    TempData["Message"] = "❌ 授權逾時，請重新登入";
                    return RedirectToPage("/Login");
                }
                var auditLog = new RequestAuditLog
                {
                    RequestId = item.Id,
                    ActorId = userId,
                    Action = "Approved",
                    Comment = "主管核准通過，進入開發階段",
                    Timestamp = DateTime.Now
                };
                _db.RequestAuditLogs.Add(auditLog);

                // ✅ 新增通知
                _db.Notifications.Add(new Notification
                {
                    UserId = item.RequesterId.ToString(),
                    Title = "變更單通知",
                    Message = "您的變更單已審核完成",
                    IsRead = false,
                    CreatedAt = DateTime.Now
                });

                await _db.SaveChangesAsync();
                await transaction.CommitAsync();

                TempData["Message"] = "✅ 已核准該需求單。";
                return RedirectToPage("/AdminHome");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                ModelState.AddModelError("", "核准過程出錯：" + ex.Message);
                return await OnGetAsync(Review.Id);
            }
        }

        // ✅ 退回
        public async Task<IActionResult> OnPostRejectAsync()
        {
            if (string.IsNullOrWhiteSpace(Review.RejectReason))
            {
                ModelState.AddModelError("", "請務必填寫退回原因。");
                return await OnGetAsync(Review.Id);
            }

            using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                var item = await _db.RequestTickets.FindAsync(Review.Id);
                if (item == null) return NotFound();

                // 1. 更新主表狀態
                item.Status = 4; // 4: Rejected
                item.UpdatedAt = DateTime.Now;

                // 2. 寫入審核軌跡 (包含退回理由)
                var userIdStr = HttpContext.Session.GetString("UserId");
                if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out var userId))
                {
                    TempData["Message"] = "❌ 授權逾時，請重新登入";
                    return RedirectToPage("/Login");
                }
                var auditLog = new RequestAuditLog
                {
                    RequestId = item.Id,
                    ActorId = userId,
                    Action = "Rejected",
                    Comment = Review.RejectReason,
                    Timestamp = DateTime.Now
                };
                _db.RequestAuditLogs.Add(auditLog);

                await _db.SaveChangesAsync();
                await transaction.CommitAsync();

                TempData["Message"] = "❌ 需求單已退回。";
                return RedirectToPage("/AdminHome");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return await OnGetAsync(Review.Id);
            }
        }

        // ✅ 刪除（你原本的）
        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var item = await _db.RequestTickets.FindAsync(id);
            if (item == null) return NotFound();

            _db.RequestTickets.Remove(item);
            await _db.SaveChangesAsync();

            return RedirectToPage("/AdminHome");
        }
    }
}