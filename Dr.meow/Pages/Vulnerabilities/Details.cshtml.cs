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
    }
}
