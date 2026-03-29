using System.Collections.Generic;
using System.Linq;
using Dr.meow.Data;
using Dr.meow.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dr.meow.Pages.Forms
{
    public class ManagementModel : PageModel
    {
        private readonly DrMeowDbContext _db;

        public ManagementModel(DrMeowDbContext db)
        {
            _db = db;
        }

        public List<RequestTicket> Pending { get; set; } = new();
        public List<RequestTicket> Approved { get; set; } = new();
        public List<RequestTicket> Rejected { get; set; } = new();

        public async Task OnGetAsync() // ✅ 改為非同步提升效能
        {
            // 💡 預載入關聯資料，方便列表顯示細節
            var allTickets = await _db.RequestTickets
                .Include(t => t.AiDetail)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();

            // ✅ 1. 待審核 (包含 0:等待AI, 1:等待主管)
            Pending = allTickets
                .Where(x => x.Status == 0 || x.Status == 1)
                .ToList();

            // ✅ 2. 已核准 (2:開發中, 3:已結案)
            Approved = allTickets
                .Where(x => x.Status == 2 || x.Status == 3)
                .ToList();

            // ✅ 3. 已退回 (4:已拒絕)
            Rejected = allTickets
                .Where(x => x.Status == 4)
                .ToList();
        }
    }
}