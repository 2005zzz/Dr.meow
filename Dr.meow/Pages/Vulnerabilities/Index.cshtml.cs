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
    public class IndexModel : PageModel
    {
        private readonly Dr.meow.Data.DrMeowDbContext _context;

        public IndexModel(Dr.meow.Data.DrMeowDbContext context)
        {
            _context = context;
        }

        public IList<Vulnerability> Vulnerability { get;set; } = default!;

        public async Task OnGetAsync()
        {
            Vulnerability = await _context.Vulnerability.ToListAsync();
        }
    }
}
