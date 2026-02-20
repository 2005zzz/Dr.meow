using Dr.meow.Data;
using Dr.meow.Models;
using Dr.meow.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dr.meow.Pages.Forms
{
    public class RequestCreateModel : PageModel
    {
        private readonly ISearchService _searchService;
        private readonly DrMeowDbContext _db;

        public RequestCreateModel(ISearchService searchService, DrMeowDbContext db)
        {
            _searchService = searchService;
            _db = db;
        }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync(
            string Department,
            string Role,
            string Contact,
            string Title,
            string Description,
            string SystemCategory,
            string RequestType,
            string Priority,
            string Benefit,
            string ExpectedDate,
            string Note

        )
        {
            var aiTest = _searchService.SearchAsync("請回覆：AI測試成功").Result;
            TempData["Message"] = aiTest.Answer;

            // 先做可用版：不接 DB 也不會出錯
            // ===== 將需求單內容整理給 AI =====
            var requestText = $@"
Title: {Title}
Description: {Description}
SystemCategory: {SystemCategory}
RequestType: {RequestType}
Priority: {Priority}
ExpectedDate: {ExpectedDate}
Benefit: {Benefit}
Note: {Note}
";

            // ===== AI 審核規則 =====
            var prompt = $@"
你是需求單的第一關 AI 審核，只做「通過/退回」判斷。

通過條件：
- 需求描述清楚
- 基本欄位完整（SystemCategory / RequestType / Priority / ExpectedDate）
- 有理由（Benefit 或 Note）

退回條件：
- 描述空泛看不出具體需求
- 關鍵資訊不足

請嚴格只回兩行：
pass=true/false
reason=一句理由

需求單內容：
{requestText}
";

            // 呼叫 AI
            var ai = await _searchService.SearchAsync(prompt);

            // 解析 AI 回覆
            var text = (ai.Answer ?? "").Replace("\r", "");
            var pass = text.Contains("pass=true", StringComparison.OrdinalIgnoreCase);

            var reason = "AI 未提供原因";
            foreach (var line in text.Split('\n'))
            {
                if (line.TrimStart().StartsWith("reason=", StringComparison.OrdinalIgnoreCase))
                {
                    reason = line.Split("=", 2)[1].Trim();
                    break;
                }
            }
            // ===== 建立需求單物件 =====
            var form = new RequestForm
            {
                Department = Department,
                Title = Title,
                Description = Description,
                Priority = Priority,
                CreatedAt = DateTime.Now,

                AiPass = pass,
                AiReason = reason,
                AiReviewedAt = DateTime.Now,

                Status = pass ? "PendingDeptBoss" : "RejectedByAI"
            };

            // 存入資料庫
            _db.RequestForms.Add(form);
            await _db.SaveChangesAsync();

            // 暫時顯示結果（測試用）
            TempData["Message"] = pass
                ? $"✅ AI審核通過 → 送主管審核\n原因：{reason}"
                : $"❌ AI退回（不進主管）\n原因：{reason}";

            return RedirectToPage("/Forms/RequestCreate");
        }
    }
}
