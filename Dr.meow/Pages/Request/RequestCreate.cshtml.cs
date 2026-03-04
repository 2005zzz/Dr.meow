using System;
using System.Threading.Tasks;
using Dr.meow.Data;
using Dr.meow.Models;
using Dr.meow.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dr.meow.Pages.Request
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
            // ✅ 1) 基本欄位空值保護（避免 NULL insert 直接炸）
            Department = (Department ?? "").Trim();
            Title = (Title ?? "").Trim();
            Description = (Description ?? "").Trim();
            Priority = (Priority ?? "").Trim();

            // ✅ 2) 最低限度必填檢查（不通過就留在原頁，顯示訊息）
            // 你 DB 欄位 Department 不允許 NULL，所以這裡一定要擋
            if (string.IsNullOrWhiteSpace(Department) ||
                string.IsNullOrWhiteSpace(Title) ||
                string.IsNullOrWhiteSpace(Description) ||
                string.IsNullOrWhiteSpace(Priority))
            {
                TempData["Message"] = "❌ 請至少填：提單人/部門、需求標題、需求內容、優先等級。";
                return Page(); // 留在原頁，不要 redirect（才看得到 TempData）
            }

            // ✅ 3) 先整理內容給 AI（但 AI 只提供建議，不決定通過/退回）
            var requestText = $@"
Department: {Department}
Role: {Role}
Contact: {Contact}
Title: {Title}
Description: {Description}
SystemCategory: {SystemCategory}
RequestType: {RequestType}
Priority: {Priority}
ExpectedDate: {ExpectedDate}
Benefit: {Benefit}
Note: {Note}
";

            // ✅ AI 改成只給「補充建議」，不要判 pass=false
            var prompt = $@"
你是醫院需求單的輔助審閱 AI。
請根據內容給我 1~2 句「補充建議」，例如：還缺哪些資訊、如何描述更清楚。
如果內容已經很清楚，就回覆：內容清楚。
請不要回 pass=true/false。

需求單內容：
{requestText}
";

            string aiReason = "內容清楚";
            bool aiPass = true;

            try
            {
                var ai = await _searchService.SearchAsync(prompt);
                var ans = (ai.Answer ?? "").Trim();

                if (!string.IsNullOrWhiteSpace(ans))
                    aiReason = ans.Length > 300 ? ans.Substring(0, 300) : ans; // 避免太長塞爆欄位
            }
            catch (Exception ex)
            {
                // AI 掛掉也不影響送出
                aiReason = "AI 暫時無法審核：" + ex.Message;
                aiPass = true;
            }

            // ✅ 4) 建立需求單（重點：Status 永遠 PendingDeptBoss）
            var form = new RequestForm
            {
                Department = Department,
                Title = Title,
                Description = Description,
                SystemCategory = SystemCategory,   
                Priority = Priority,
                CreatedAt = DateTime.Now,

                AiPass = aiPass,
                AiReason = aiReason,
                AiReviewedAt = DateTime.Now,

                // ✅ 不再讓 AI 退回你
                Status = "PendingDeptBoss"
            };

            _db.RequestForms.Add(form);
            await _db.SaveChangesAsync();

            TempData["Message"] = "✅ 已送出需求單（已進待審核）。AI 建議：" + aiReason;

            // 送出後回到自己頁面（清空表單）
            return RedirectToPage("/Request/RequestCreate");
        }
    }
}