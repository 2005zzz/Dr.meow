using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Dr.meow.Data;
using Dr.meow.Models;

namespace Dr.meow.Pages.Vulnerabilities
{
    public class EditModel : PageModel
    {
        private readonly Dr.meow.Data.DrMeowDbContext _context;

        public EditModel(Dr.meow.Data.DrMeowDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Vulnerability Vulnerability { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vulnerability =  await _context.Vulnerability.FirstOrDefaultAsync(m => m.Id == id);
            if (vulnerability == null)
            {
                return NotFound();
            }
            Vulnerability = vulnerability;
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // ⭐【加在這】先抓資料庫原本那筆
            var dbVulnerability = await _context.Vulnerability
                .AsNoTracking()
                .FirstOrDefaultAsync(v => v.Id == Vulnerability.Id);

            if (dbVulnerability == null)
            {
                return NotFound();
            }

            dbVulnerability.Description = Vulnerability.Description;
            dbVulnerability.SystemCategory = Vulnerability.SystemCategory;
            dbVulnerability.TicketCategory = Vulnerability.TicketCategory;
            dbVulnerability.ChangeType = Vulnerability.ChangeType;
            dbVulnerability.Severity = Vulnerability.Severity;
            dbVulnerability.ImpactLevel = Vulnerability.ImpactLevel;
            dbVulnerability.Dependency = Vulnerability.Dependency;
            dbVulnerability.TestPlan = Vulnerability.TestPlan;
            dbVulnerability.RecoveryPlan = Vulnerability.RecoveryPlan;
            dbVulnerability.ScheduledTime = Vulnerability.ScheduledTime; // 這裡現在是 DateTime? 了
            dbVulnerability.Summary = Vulnerability.Summary;
            dbVulnerability.UpdatedAt = DateTime.Now;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VulnerabilityExists(Vulnerability.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            // ⭐ 修改成功通知
            TempData["StatusMessage"] = "✅ 申請單修改成功！";

            // ⭐ 回到同一筆 Edit 頁顯示通知
            return RedirectToPage(new { id = Vulnerability.Id });
        }


        private bool VulnerabilityExists(int id)
        {
            return _context.Vulnerability.Any(e => e.Id == id);
        }
    }
}
