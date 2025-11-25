using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dr.meow.Models;
using Dr.meow.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dr.meow.Pages.Api
{
    public class SearchModel : PageModel
    {
        private readonly ISearchService _searchService;

        public SearchModel(ISearchService searchService)
        {
            _searchService = searchService;
        }

        public async Task<IActionResult> OnGetAsync(string keyword, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return new JsonResult(Array.Empty<SearchItem>());
            }

            var items = await _searchService.SearchAsync(keyword, ct);
            return new JsonResult(items);
        }
    }
}
