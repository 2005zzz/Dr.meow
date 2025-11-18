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
        public async Task<IActionResult> OnGetAsync(string? keyword, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return new JsonResult(new { error = "keyword is required" });
            }

            // 呼叫 SearchService，搜尋 RAG 後端
            var result = await _searchService.SearchAsync(keyword, ct);

            if (result == null)
            {
                return new JsonResult(new { error = "後端沒有回傳資料" });
            }

            // 回傳 JSON
            return new JsonResult(result);
        }
    }
}
