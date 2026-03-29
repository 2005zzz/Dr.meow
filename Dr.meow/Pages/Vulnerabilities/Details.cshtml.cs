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

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // ?? 2. 使用 Include 抓取 Requester，不然頭像會報錯
            var vulnerability = await _context.Vulnerability
                .Include(v => v.Requester)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (vulnerability == null) return NotFound();

            Vulnerability = vulnerability;

            // ?? 3. 順便抓取這張單子所有的審核紀錄 (按時間排序)
            Logs = await _context.VulnerabilityLogs
                //.Include(l => l.Reviewer) // 如果妳想顯示是哪個主管簽的，就要 Include Reviewer
                .Where(l => l.VulnerabilityId == id)
                .OrderByDescending(l => l.CreatedAt)
                .ToListAsync();

            return Page();
        }
    }
}
