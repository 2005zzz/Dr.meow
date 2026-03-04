using Dr.meow.Data;
using Dr.meow.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Dr.meow.Pages.Request
{
    public class DetailsModel : PageModel
    {
        private readonly DrMeowDbContext _db;

        public DetailsModel(DrMeowDbContext db)
        {
            _db = db;
        }

        public RequestForm Item { get; set; } = default!;

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
            var item = await _db.RequestForms.FirstOrDefaultAsync(x => x.Id == id);
            if (item == null) return NotFound();

            Item = item;

            // ✅ 如果已經有審核資料，帶回畫面（方便查看/二次修改）
            Review = new ReviewInput
            {
                Id = item.Id,
                AcceptanceContent = item.ReviewAcceptanceContent,
                AcceptanceDate = item.ReviewAcceptanceDate,
                SecurityAssessment = item.ReviewSecurityAssessment,
                SatisfactionNeed = item.ReviewSatisfactionNeed,
                SatisfactionStability = item.ReviewSatisfactionStability,
                SatisfactionOverall = item.ReviewSatisfactionOverall,
                BenefitManDays = item.ReviewBenefitManDays,
                BenefitRevenue = item.ReviewBenefitRevenue,
                RejectReason = item.ReviewRejectReason
            };

            return Page();
        }

        // ✅ 核准
        public async Task<IActionResult> OnPostApproveAsync()
        {
            var item = await _db.RequestForms.FindAsync(Review.Id);
            if (item == null) return NotFound();

            // 只允許「待審核」時核准（避免亂改）
            if (item.Status != "PendingDeptBoss")
                return RedirectToPage("/Request/Details", new { id = Review.Id });

            item.ReviewAcceptanceContent = Review.AcceptanceContent;
            item.ReviewAcceptanceDate = Review.AcceptanceDate ?? DateTime.Now;

            item.ReviewSecurityAssessment = Review.SecurityAssessment;

            item.ReviewSatisfactionNeed = Review.SatisfactionNeed;
            item.ReviewSatisfactionStability = Review.SatisfactionStability;
            item.ReviewSatisfactionOverall = Review.SatisfactionOverall;

            item.ReviewBenefitManDays = Review.BenefitManDays;
            item.ReviewBenefitRevenue = Review.BenefitRevenue;

            item.ReviewRejectReason = null;
            item.ReviewedBy = User?.Identity?.Name ?? "DeptBoss";
            item.ReviewedAt = DateTime.Now;

            item.Status = "Approved";

            await _db.SaveChangesAsync();
            return RedirectToPage("/AdminHome");
        }

        // ✅ 退回
        public async Task<IActionResult> OnPostRejectAsync()
        {
            if (string.IsNullOrWhiteSpace(Review.RejectReason))
            {
                ModelState.AddModelError("", "請填寫退回原因。");
                return await OnGetAsync(Review.Id);
            }

            var item = await _db.RequestForms.FindAsync(Review.Id);
            if (item == null) return NotFound();

            if (item.Status != "PendingDeptBoss")
                return RedirectToPage("/Request/Details", new { id = Review.Id });

            // 退回也可順便保存審核欄位（看你要不要）
            item.ReviewAcceptanceContent = Review.AcceptanceContent;
            item.ReviewSecurityAssessment = Review.SecurityAssessment;

            item.ReviewRejectReason = Review.RejectReason;
            item.ReviewedBy = User?.Identity?.Name ?? "DeptBoss";
            item.ReviewedAt = DateTime.Now;

            item.Status = "Rejected";

            await _db.SaveChangesAsync();
            return RedirectToPage("/AdminHome");
        }

        // ✅ 刪除（你原本的）
        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var item = await _db.RequestForms.FindAsync(id);
            if (item == null) return NotFound();

            _db.RequestForms.Remove(item);
            await _db.SaveChangesAsync();

            return RedirectToPage("/AdminHome");
        }
    }
}