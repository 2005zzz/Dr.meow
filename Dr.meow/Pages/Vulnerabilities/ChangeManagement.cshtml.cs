using Dr.meow.Data;
using Dr.meow.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Dr.meow.Pages.Forms
{
    public class ChangeManagementModel : PageModel
    {
        private readonly DrMeowDbContext _db;

        public ChangeManagementModel(DrMeowDbContext db)
        {
            _db = db;
        }

        public List<Vulnerability> Pending { get; set; } = new();
        public List<Vulnerability> Approved { get; set; } = new();
        public List<Vulnerability> Rejected { get; set; } = new();

        public async Task OnGetAsync()
        {
            // ✅ 這三個狀態要跟你存進 DB 的字串一致
            Pending = await _db.Vulnerability
                .Where(x => x.Status == "Pending")
                .OrderByDescending(x => x.FoundDate)
                .ToListAsync();

            Approved = await _db.Vulnerability
                .Where(x => x.Status == "Approved")
                .OrderByDescending(x => x.FoundDate)
                .ToListAsync();

            Rejected = await _db.Vulnerability
                .Where(x => x.Status == "Rejected")
                .OrderByDescending(x => x.FoundDate)
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostApproveAsync(int id)
        {
            var item = await _db.Vulnerability.FirstOrDefaultAsync(x => x.Id == id);
            if (item == null) return RedirectToPage();

            item.Status = "Approved";
            await _db.SaveChangesAsync();
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostRejectAsync(int id, string reason)
        {
            var item = await _db.Vulnerability.FirstOrDefaultAsync(x => x.Id == id);
            if (item == null) return RedirectToPage();

            item.Status = "Rejected";

            // ✅ 你的 Vulnerability 目前沒有 RejectReason 欄位，所以先不要寫入
            // item.RejectReason = reason;

            await _db.SaveChangesAsync();
            return RedirectToPage();
        }
    }
}