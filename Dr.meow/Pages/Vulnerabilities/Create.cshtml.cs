using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Dr.meow.Data;
using Dr.meow.Models;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;

namespace Dr.meow.Pages.Vulnerabilities
{
    public class CreateModel : PageModel
    {
        private readonly DrMeowDbContext _context;

        public CreateModel(DrMeowDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            Vulnerability = new Vulnerability
            {
                ScheduledTime = DateTime.Today.AddDays(1).AddHours(9)
            };

            return Page();
        }

        [BindProperty]
        public Vulnerability Vulnerability { get; set; } = default!;

        public async Task<IActionResult> OnPostAsync()
        {
            ModelState.Remove("Vulnerability.TicketNumber");
            ModelState.Remove("Vulnerability.Department");

            var userIdStr = HttpContext.Session.GetString("UserId");
            var userTeam = HttpContext.Session.GetString("UserTeam");

            if (string.IsNullOrEmpty(userIdStr))
            {
                return RedirectToPage("/Login");
            }

            if (!ModelState.IsValid)
            {
                foreach (var state in ModelState)
                {
                    foreach (var error in state.Value.Errors)
                    {
                        Debug.WriteLine($"[欄位錯誤] {state.Key}: {error.ErrorMessage}");
                    }
                }
                return Page();
            }

            Vulnerability.RequesterId = int.Parse(userIdStr);
            Vulnerability.Department = userTeam ?? "未分配部門";
            Vulnerability.Status = "Pending";
            Vulnerability.CreatedAt = DateTime.Now;
            Vulnerability.FoundDate = DateTime.Now;
            Vulnerability.FormType = "Change";

            try
            {
                _context.Vulnerability.Add(Vulnerability);
                await _context.SaveChangesAsync();

                string teamShort = (userTeam == "Team1") ? "T1" : "T2";
                Vulnerability.TicketNumber =
                    $"CHG-{teamShort}-{DateTime.Now:yyyyMMdd}-{Vulnerability.Id:D5}";

                await _context.SaveChangesAsync();

                TempData["StatusMessage"] = $"提交成功！你的單號是 <strong>{Vulnerability.TicketNumber}</strong>";

                return RedirectToPage();
            }
            catch (DbUpdateException dbEx)
            {
                var innerMsg = dbEx.InnerException?.Message ?? dbEx.Message;
                Debug.WriteLine($"[DB Error] {innerMsg}");
                ModelState.AddModelError("", "資料庫寫入失敗：" + innerMsg);
                return Page();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[General Error] {ex.Message}");
                ModelState.AddModelError("", "發生未知錯誤：" + ex.Message);
                return Page();
            }
        }
    }
}