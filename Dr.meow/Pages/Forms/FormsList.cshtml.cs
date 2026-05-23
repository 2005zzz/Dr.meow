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

        public class VulnerabilityVM
        {
            public Vulnerability Main { get; set; } = default!;
            public string? LatestRejectReason { get; set; }
        }
        public class RequestTicketVM
        {
            public RequestTicket Main { get; set; } = default!;
            public string? LatestRejectReason { get; set; }
        }

        public IList<VulnerabilityVM> VulnerabilityList { get; set; } = new List<VulnerabilityVM>();
        public IList<RequestTicketVM> RequestList { get; set; } = new List<RequestTicketVM>();

        // 🚀 關鍵修改：讓 OnGetAsync 接收前端傳來的 formType 參數
        public async Task OnGetAsync(string? formType)
        {
            var userIdStr = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdStr)) return;

            int currentUserId = int.Parse(userIdStr);

            // 🎯 條件分流 A：如果前端要的是變更單（或者沒傳），才去撈變更單資料
            if (string.IsNullOrEmpty(formType) || formType == "Vulnerability")
            {
                VulnerabilityList = await _context.Vulnerability
                    .Include(v => v.Requester)
                    .Where(v => v.RequesterId == currentUserId)
                    .OrderByDescending(v => v.Id)
                    .Select(v => new VulnerabilityVM
                    {
                        Main = v,
                        LatestRejectReason = _context.VulnerabilityLogs
                            .Where(l => l.VulnerabilityId == v.Id && l.Action == "Rejected")
                            .OrderByDescending(l => l.CreatedAt)
                            .Select(l => l.Comment)
                            .FirstOrDefault()
                    })
                    .ToListAsync();
            }

            // 🎯 條件分流 B：如果前端要的是需求單（或者沒傳），才去撈需求單資料
            if (string.IsNullOrEmpty(formType) || formType == "Request")
            {
                RequestList = await _context.RequestTickets
                    .Include(r => r.Requester)
                    .Where(r => r.RequesterId == currentUserId)
                    .OrderByDescending(r => r.Id)
                    .Select(r => new RequestTicketVM
                    {
                        Main = r,
                        LatestRejectReason = _context.RequestAuditLogs
                            .Where(l => l.RequestId == r.Id && l.Action == "Rejected")
                            .OrderByDescending(l => l.Timestamp)
                            .Select(l => l.Comment ?? "")
                            .FirstOrDefault()
                    })
                    .ToListAsync();
            }
        }
    }
}