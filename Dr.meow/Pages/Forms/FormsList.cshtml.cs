using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Dr.meow.Data;
using Dr.meow.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dr.meow.Pages.Forms
{
    public class FormsListModel : PageModel
    {
        private readonly DrMeowDbContext _context;

        public FormsListModel(DrMeowDbContext context)
        {
            _context = context;
        }

        // 🚀 建立一個內嵌類別 (ViewModel)，專門用來裝卡片需要的資料
        public class VulnerabilityVM
        {
            public Vulnerability Main { get; set; } = default!;
            public string? LatestRejectReason { get; set; }
        }

        // 修正屬性型別：現在我們改用 VM 清單
        public IList<VulnerabilityVM> VulnerabilityList { get; set; } = new List<VulnerabilityVM>();

        public async Task OnGetAsync()
        {
            var userIdStr = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdStr)) return;

            int currentUserId = int.Parse(userIdStr);

            // 🚀 核心邏輯：使用 .Select 一次把主表、關聯人、以及最後一筆退回理由勾出來
            VulnerabilityList = await _context.Vulnerability
                .Include(v => v.Requester) // 抓提單人
                .Where(v => v.RequesterId == currentUserId)
                .OrderByDescending(v => v.Id)
                .Select(v => new VulnerabilityVM
                {
                    Main = v,
                    // 🎯 子查詢：去 VulnerabilityLogs 找這張單子最新的「Rejected」留言
                    LatestRejectReason = _context.VulnerabilityLogs
                        .Where(l => l.VulnerabilityId == v.Id && l.Action == "Rejected")
                        .OrderByDescending(l => l.CreatedAt)
                        .Select(l => l.Comment)
                        .FirstOrDefault()
                })
                .ToListAsync();
        }
    }
}