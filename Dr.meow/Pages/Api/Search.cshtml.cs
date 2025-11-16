using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Dr.meow.Services;

namespace Dr.meow.Pages.Api
{
    public class SearchModel : PageModel
    {
        private readonly ISearchService _searchService;

        public SearchModel(ISearchService searchService)
        {
            _searchService = searchService;
        }

        // GET /api/search?keyword=xxx
        public async Task<IActionResult> OnGetAsync(string keyword, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return new JsonResult(new { error = "keyword is required" });
            }

            // 呼叫同學的 SearchService（已經在 Program.cs 註冊過）
            var result = await _searchService.SearchAsync(keyword, ct);

            // 直接回傳 JSON 給前端 JS
            return new JsonResult(result);
        }
    }
}
