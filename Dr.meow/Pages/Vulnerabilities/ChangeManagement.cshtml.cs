using Dr.meow.Data;
using Dr.meow.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

namespace Dr.meow.Pages.Vulnerabilities
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
        public List<VulnerabilityVM> Tracking { get; set; } = new();
        public List<VulnerabilityVM> Closed { get; set; } = new();
        public List<VulnerabilityVM> Rejected { get; set; } = new();

        public async Task OnGetAsync()
        {
            var myTeam = HttpContext.Session.GetString("UserTeam") ?? "";
            var userIdStr = HttpContext.Session.GetString("UserId") ?? "0";
            int currentUserId = int.Parse(userIdStr);

            Pending = await _db.Vulnerability
                .Include(v => v.Requester)
                .Where(x =>
                    (x.Department == myTeam && x.Status == "Pending") ||
                    (x.Department != myTeam && x.Status == "PendingCross") ||
                    (x.Status == "PendingCISO" && false)
                )
                .OrderByDescending(x => x.CreatedAt)
                .Select(x => new VulnerabilityVM
                {
                    Main = x,
                    FirstReviewComment = _db.VulnerabilityLogs
                        .Where(l => l.VulnerabilityId == x.Id
                            && l.StepName == "組長初審"
                            && l.Action == "Approved")
                        .OrderByDescending(l => l.CreatedAt)
                        .Select(l => l.Comment)
                        .FirstOrDefault()
                })
                .ToListAsync();

            Tracking = await _db.Vulnerability
                .Include(v => v.Requester)
                .Where(x =>
                    x.LastReviewerId == currentUserId &&
                    x.Status != "Approved" &&
                    x.Status != "Rejected")
                .OrderByDescending(x => x.CreatedAt)
                .Select(x => new VulnerabilityVM
                {
                    Main = x
                })
                .ToListAsync();

            Closed = await _db.Vulnerability
                .Include(v => v.Requester)
                .Where(x =>
                    x.Status == "Approved" &&
                    (x.Department == myTeam || x.LastReviewerId == currentUserId))
                .OrderByDescending(x => x.CreatedAt)
                .Select(x => new VulnerabilityVM
                {
                    Main = x
                })
                .ToListAsync();

            Rejected = await _db.Vulnerability
                .Include(v => v.Requester)
                .Where(x => x.Status == "Rejected")
                .Where(x =>
                    x.LastReviewerId == currentUserId ||
                    (x.Department == myTeam && x.UpdatedAt > x.CreatedAt) ||
                    (x.Department == myTeam && x.LastReviewerId == currentUserId)
                )
                .OrderByDescending(x => x.UpdatedAt ?? x.CreatedAt)
                .Select(x => new VulnerabilityVM
                {
                    Main = x,
                    LatestRejectReason = _db.VulnerabilityLogs
                        .Where(l => l.VulnerabilityId == x.Id
                            && l.Action == "Rejected")
                        .OrderByDescending(l => l.CreatedAt)
                        .Select(l => l.Comment)
                        .FirstOrDefault()
                })
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostApproveAsync(int id, string comment)
        {
            var item = await _db.Vulnerability
                .FirstOrDefaultAsync(x => x.Id == id);

            if (item == null)
                return NotFound();

            int currentUserId =
                int.Parse(HttpContext.Session.GetString("UserId") ?? "0");

            string stepName = "";

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

            var log = new VulnerabilityLog
            {
                VulnerabilityId = item.Id,
                ReviewerId = currentUserId,
                Action = "Approved",
                Comment = comment,
                StepName = stepName,
                CreatedAt = DateTime.Now
            };

            item.LastReviewerId = currentUserId;
            item.UpdatedAt = DateTime.Now;

            _db.VulnerabilityLogs.Add(log);

            await _db.SaveChangesAsync();

            return new OkResult();
        }

        public async Task<IActionResult> OnPostRejectAsync(int id, string reason)
        {
            var item = await _db.Vulnerability
                .FirstOrDefaultAsync(x => x.Id == id);

            if (item == null)
                return NotFound();

            int currentUserId =
                int.Parse(HttpContext.Session.GetString("UserId") ?? "0");

            item.Status = "Rejected";
            item.LastReviewerId = currentUserId;
            item.UpdatedAt = DateTime.Now;

            var log = new VulnerabilityLog
            {
                VulnerabilityId = item.Id,
                ReviewerId = currentUserId,
                Action = "Rejected",
                Comment = reason,
                StepName = "審核退回",
                CreatedAt = DateTime.Now
            };

            _db.VulnerabilityLogs.Add(log);

            await _db.SaveChangesAsync();

            return new OkResult();
        }
    }
}