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
    public class DeleteModel : PageModel
    {
        private readonly Dr.meow.Data.DrMeowDbContext _context;

        public DeleteModel(Dr.meow.Data.DrMeowDbContext context)
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

            var vulnerability = await _context.Vulnerability.FirstOrDefaultAsync(m => m.Id == id);

            if (vulnerability == null)
            {
                return NotFound();
            }
            else
            {
                Vulnerability = vulnerability;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vulnerability = await _context.Vulnerability.FindAsync(id);
            if (vulnerability != null)
            {
                Vulnerability = vulnerability;
                _context.Vulnerability.Remove(Vulnerability);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
