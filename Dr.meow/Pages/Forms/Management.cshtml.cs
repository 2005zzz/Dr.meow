using Dr.meow.Data;      // 你的 DbContext 命名空間（依你專案實際）
using Dr.meow.Models;    // RequestForm 在這裡
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
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

        public List<RequestForm> Pending { get; set; } = new();
        public List<RequestForm> Approved { get; set; } = new();
        public List<RequestForm> Rejected { get; set; } = new();

        public void OnGet()
        {
            Pending = _db.RequestForms
                .Where(x => x.Status == "PendingDeptBoss")
                .OrderByDescending(x => x.CreatedAt)
                .ToList();

            Approved = _db.RequestForms
                .Where(x => x.Status == "Approved")
                .OrderByDescending(x => x.CreatedAt)
                .ToList();

            Rejected = _db.RequestForms
                .Where(x => x.Status == "RejectedByAI" || x.Status == "RejectedByDeptBoss")
                .OrderByDescending(x => x.CreatedAt)
                .ToList();
        }

    }
}