using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Dr.meow.Services;

namespace Dr.meow.Pages.Api
{
    // ⚠️ 修正 1：加上這個屬性，忽略 CSRF 驗證 (否則前端 fetch POST 會被擋下 400 Bad Request)
    [IgnoreAntiforgeryToken]
    public class SearchModel : PageModel
    {
        private readonly ISearchService _searchService;

        public SearchModel(ISearchService searchService)
        {
            _searchService = searchService;
        }

        // ⚠️ 修正 2：定義一個內部類別，用來對應前端傳來的 JSON { "query": "..." }
        public class SearchRequestBody
        {
            public string Query { get; set; } = "";
        }

        // ⚠️ 修正 3：改成 OnPostAsync (對應前端 method: 'POST')
        // ⚠️ 修正 4：使用 [FromBody] 來接收 JSON 物件
        public async Task<IActionResult> OnPostAsync([FromBody] SearchRequestBody request, CancellationToken ct)
        {
            // 檢查接收到的資料
            if (request == null || string.IsNullOrWhiteSpace(request.Query))
            {
                return new JsonResult(new { error = "Query cannot be empty" });
            }

            // 呼叫 SearchService (傳入 request.Query)
            // ✅ 指定名稱，明確告訴它第二個參數是模式，第三個是 Token
            var result = await _searchService.SearchAsync(request.Query, mode: "consult", ct: ct);

            if (result == null)
            {
                return new JsonResult(new { error = "後端沒有回傳資料" });
            }

            // 回傳 JSON
            return new JsonResult(result);
        }
    }
}