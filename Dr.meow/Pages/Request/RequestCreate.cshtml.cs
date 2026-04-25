<<<<<<< HEAD
=======
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.SemanticKernel.ChatCompletion;
using System.Security.Claims;
using System.Text.Json;
using System.ComponentModel.DataAnnotations;
>>>>>>> parent of b807ace (調整UI：移除sidebar、置中使用者顧問中心、header新增查看表單)
using Dr.meow.Data;
using Dr.meow.Models;
using Dr.meow.Services;
using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.SemanticKernel.ChatCompletion;
using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using System.IO;

namespace Dr.meow.Pages.Request
{
    // 提單專用的 ViewModel：負責畫面輸入與欄位驗證
    public class RequestCreateViewModel
    {
        [Required(ErrorMessage = "請輸入申請部門")]
        [Display(Name = "部門")]
        public string Department { get; set; } = "";

        [Display(Name = "職稱")]
        public string Role { get; set; } = "";

        [Display(Name = "聯絡方式")]
        public string Contact { get; set; } = "";

        [Required(ErrorMessage = "需求標題不能為空")]
        [StringLength(200, ErrorMessage = "標題長度不能超過 200 字")]
        [Display(Name = "需求標題")]
        public string Title { get; set; } = "";

        [Required(ErrorMessage = "請詳細描述您的需求內容")]
        [Display(Name = "需求內容")]
        public string Description { get; set; } = "";

        [Display(Name = "系統類別")]
        public string SystemCategory { get; set; } = "";

        [Display(Name = "需求類型")]
        public string RequestType { get; set; } = "";

        [Required(ErrorMessage = "請選擇優先等級")]
        [Display(Name = "優先等級")]
        public string Priority { get; set; } = "普通";

        [Display(Name = "預期效益")]
        public string? Benefit { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "期望完成日")]
        public DateTime? ExpectedDate { get; set; }

        [Display(Name = "備註")]
        public string? Note { get; set; }
    }

    public class RequestCreateModel : PageModel
    {
        private readonly ISearchService _searchService;
        private readonly DrMeowDbContext _db;
        private readonly IBackgroundJobClient _backgroundJob;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IChatCompletionService _chatService;

        public RequestCreateModel(
            ISearchService searchService,
            DrMeowDbContext db,
            IBackgroundJobClient backgroundJob,
            IServiceScopeFactory scopeFactory,
            IChatCompletionService chatService)
        {
            _searchService = searchService;
            _db = db;
            _backgroundJob = backgroundJob;
            _scopeFactory = scopeFactory;
            _chatService = chatService;
        }

        [BindProperty]
        public RequestCreateViewModel FormModel { get; set; } = new();

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            // 1) 先做欄位驗證
            if (!ModelState.IsValid)
                return Page();

            // 2) 取得登入使用者 ID
            var userIdStr = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out var userId))
            {
                TempData["Message"] = "❌ 授權逾時，請重新登入";
                return RedirectToPage("/Login");
            }

            var ticketNumber = await GenerateUniqueTicketNumber();

            using var transaction = await _db.Database.BeginTransactionAsync();

            try
            {
                // A. 建立主表 Ticket
                var ticket = new RequestTicket
                {
                    TicketNumber = ticketNumber,
                    RequesterId = userId,
                    Title = FormModel.Title.Trim(),
                    Description = FormModel.Description.Trim(),
                    Status = 0, // PendingAI
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                _db.RequestTickets.Add(ticket);
                await _db.SaveChangesAsync();

                // B. 建立使用者輸入明細
                var userInput = new RequestUserInput
                {
                    RequestId = ticket.Id,
                    Department = FormModel.Department,
                    Role = FormModel.Role,
                    Contact = FormModel.Contact,
                    SystemCategory = FormModel.SystemCategory,
                    RequestType = FormModel.RequestType,
                    Priority = FormModel.Priority,
                    Benefit = FormModel.Benefit,
                    ExpectedDate = FormModel.ExpectedDate,
                    Note = FormModel.Note
                };

                _db.RequestUserInputs.Add(userInput);

                // C. 建立審核軌跡
                var auditLog = new RequestAuditLog
                {
                    RequestId = ticket.Id,
                    ActorId = userId,
                    Action = "Submitted",
                    Comment = "使用者送出需求單，進入 AI 自動分案階段",
                    Timestamp = DateTime.Now
                };

                _db.RequestAuditLogs.Add(auditLog);

                await _db.SaveChangesAsync();
                await transaction.CommitAsync();

<<<<<<< HEAD
                // D. 先直接執行 AI 分析（暫時繞過 Hangfire 做除錯）
                await ProcessAiAnalysisAsync(ticket.Id);
                TempData["Message"] = $"✅ 需求單 {ticketNumber} 已送出！AI 已完成分析。";
=======
                // D. 丟背景任務做 AI 分析
                _backgroundJob.Enqueue(() => ProcessAiAnalysisAsync(ticket.Id));

                TempData["Message"] = $"✅ 需求單 {ticketNumber} 已送出！AI 正在進行資安掃描與改寫中...";
>>>>>>> parent of b807ace (調整UI：移除sidebar、置中使用者顧問中心、header新增查看表單)
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                TempData["Message"] = "❌ 存檔失敗：" + ex.Message;
                return Page();
            }

            return RedirectToPage("/Request/RequestCreate");
        }

        private async Task<string> GenerateUniqueTicketNumber()
        {
            var datePart = DateTime.Now.ToString("yyyyMMdd");
            string ticketNumber;

            do
            {
                var randPart = Guid.NewGuid().ToString().Substring(0, 5).ToUpper();
                ticketNumber = $"REQ-{datePart}-{randPart}";
            }
            while (await _db.RequestTickets.AnyAsync(x => x.TicketNumber == ticketNumber));

            return ticketNumber;
        }

        [AutomaticRetry(Attempts = 2)]
        public async Task ProcessAiAnalysisAsync(int ticketId)
        {
<<<<<<< HEAD
            System.Diagnostics.Debug.WriteLine("🔥🔥🔥 AI任務有進來 🔥🔥🔥");
            System.IO.File.WriteAllText(@"C:\Dr.meow\Dr.meow\job_entered.txt", $"有進到 ProcessAiAnalysisAsync，ticketId={ticketId}，時間={DateTime.Now}");

=======
>>>>>>> parent of b807ace (調整UI：移除sidebar、置中使用者顧問中心、header新增查看表單)
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<DrMeowDbContext>();

            var ticket = await db.RequestTickets
                .Include(t => t.UserInput)
                .FirstOrDefaultAsync(x => x.Id == ticketId);

            if (ticket == null) return;

            try
            {
                var prompt = $@"
你是醫院需求單 AI 初審助手，負責判斷此需求單是否應進入人工審核，
若通過，也要幫主管先草擬審核頁欄位內容。

審核規則：
1. 標題不可過於隨意，例如：test、測試、123、aaa、ㄏㄏ。
2. 需求內容不可空泛、不可過短，必須能看出具體需求、使用情境、問題描述或預期功能。
3. 若內容明顯與資訊系統、報表、權限、流程改善、查詢、匯出、自動化、通知、資料整合無關，應判定不通過。
4. 若部門、角色、聯絡方式明顯亂填，應判定不通過。
5. 若內容大致合理但資訊略有不足，應優先判定為可進入人工審核，而不是直接退回。
6. AI 初審僅作第一層篩選，不負責最終決策。

請只回傳純 JSON，格式固定如下：
{{
    ""isApproved"": true,
    ""isITRelated"": true,
    ""refinedTitle"": ""整理後的正式標題"",
    ""refinedDescription"": ""整理後的正式需求描述"",
    ""securityAssessment"": ""只能填 符合 / 不適用 / 需補件 其中之一"",
    ""reason"": ""通過或退回原因"",
    ""aiReviewComment"": ""主管審核頁可直接看到的建議驗收內容/審核意見"",
    ""aiRequirementScore"": 4,
    ""aiStabilityScore"": 4,
    ""aiOverallScore"": 4,
    ""aiSavedManDays"": 2.5,
    ""aiRevenue"": 0
}}

欄位產出規則：
 1.aiReviewComment 請用主管審核草稿語氣撰寫，需具體、可執行、可驗收。
   請包含：
   - 建議確認重點
   - 驗收方式或驗收標準
   - 若有風險或限制，簡短提醒
   字數控制在 80~180 字。
<<<<<<< HEAD
2.securityAssessment 請根據需求內容的風險程度，自行判斷並輸出以下三個分類之一：
   - 符合：表示需求風險低，或不涉及資安疑慮
   - 不適用：表示此需求與資安無關
   - 需補件：表示需求可能存在風險、資訊不足，或需要進一步評估
   請務必根據語意自行判斷，不要輸出其他文字（例如：中、低、高、中風險等），
   最終結果只能是上述三個值之一。
=======
2.securityAssessment 只能輸出以下三種之一：
   - 符合
   - 不適用
   - 需補件
>>>>>>> parent of b807ace (調整UI：移除sidebar、置中使用者顧問中心、header新增查看表單)
3.aiRequirementScore（1~5）：
   1 = 幾乎看不懂需求
   2 = 有部分內容，但描述不清楚或邏輯混亂
   3 = 需求大致明確，但仍有缺漏
   4 = 需求清楚，僅有小部分細節需補充
   5 = 需求完整明確、問題與目標一致
4.aiStabilityScore（1~5）：
   1 = 風險高，可能影響系統穩定
   2 = 有明顯風險，需要大量測試或調整
   3 = 可行但仍需驗證
   4 = 風險低，大致穩定
   5 = 功能單純明確，預期穩定
5.aiOverallScore（1~5）：
   1 = 價值低或不可行
   2 = 價值有限或實作困難
   3 = 有一定價值但需再確認
   4 = 價值明確且可行
   5 = 具高價值、可行性高且建議優先處理
請依實際內容評分，1~5 每一分都可以使用，不要只使用 1、3、5。
三個分數不應全部相同，除非需求內容確實各面向表現一致。
6. aiSavedManDays 請保守估算：
   - 小型報表、查詢優化、簡單匯出功能：通常 0.5 ~ 3
   - 若無法判斷，填 0 或 0.5
7. aiRevenue：
   - 若屬內部流程優化，通常填 0
   - 只有有明確收入或成本回收時才填金額

需求單資料：
部門：{ticket.UserInput?.Department}
角色：{ticket.UserInput?.Role}
聯絡方式：{ticket.UserInput?.Contact}
需求標題：{ticket.Title}
需求內容：{ticket.Description}
系統類別：{ticket.UserInput?.SystemCategory}
需求類型：{ticket.UserInput?.RequestType}
優先級：{ticket.UserInput?.Priority}
預期效益：{ticket.UserInput?.Benefit}
備註：{ticket.UserInput?.Note}";

                var history = new ChatHistory();
                history.AddUserMessage(prompt);

                var result = await _chatService.GetChatMessageContentAsync(history);
                var aiText = result.Content ?? "";
                System.IO.File.WriteAllText(@"C:\Dr.meow\Dr.meow\last_ai_response.txt", aiText);

                var cleanJson = aiText
                 .Replace("```json", "")
                 .Replace("```", "")
                 .Trim();
<<<<<<< HEAD

                var aiData = JsonSerializer.Deserialize<AiResponseModel>(
                    cleanJson ?? "",
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                System.IO.File.WriteAllText(@"C:\Dr.meow\Dr.meow\before_review_ai.txt", "有走到第二段 AI 前");
                // ⭐ 新增：第二段 AI（審核評分）
                var reviewPrompt = $@"
你是醫院需求單審核評分助手，負責根據需求內容評估其明確性、穩定性、整體價值，
並保守估算可節省人力與可能收益。

請只回傳純 JSON，格式如下：
{{
    ""aiRequirementScore"": 4,
    ""aiStabilityScore"": 4,
    ""aiOverallScore"": 4,
    ""aiSavedManDays"": 2.5,
    ""aiRevenue"": 0
}}

評分規則：
1. aiRequirementScore（1~5）：需求清楚程度
2. aiStabilityScore（1~5）：系統風險與穩定性
3. aiOverallScore（1~5）：整體價值與可行性
4. aiSavedManDays：保守估算節省人力（0~3 常見）
5. aiRevenue：內部系統通常填 0

需求單資料：
標題：{ticket.Title}
內容：{ticket.Description}
";

                var reviewHistory = new ChatHistory();
                reviewHistory.AddUserMessage(reviewPrompt);

                var reviewResult = await _chatService.GetChatMessageContentAsync(reviewHistory);
                var reviewText = reviewResult.Content ?? "";
                System.IO.File.WriteAllText(@"C:\Dr.meow\Dr.meow\last_ai_reviewscore_response.txt", reviewText);

                var cleanReviewJson = reviewText
                    .Replace("```json", "")
                    .Replace("```", "")
                    .Trim();

                var reviewData = JsonSerializer.Deserialize<ReviewScoreAiResponseModel>(
                    cleanReviewJson,
=======

                var aiData = JsonSerializer.Deserialize<AiResponseModel>(
                    cleanJson ?? "",
>>>>>>> parent of b807ace (調整UI：移除sidebar、置中使用者顧問中心、header新增查看表單)
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (aiData == null)
                {
                    db.RequestAiDetails.Add(new RequestAiDetail
                    {
                        RequestId = ticketId,
                        ErrorMessage = "AI 回傳內容無法解析為 JSON",
                        AiReason = "AI 初審失敗，請人工確認",
                        IsProcessed = false
                    });

                    ticket.Status = 4; // 先退回，避免卡死
                    ticket.UpdatedAt = DateTime.Now;

                    await db.SaveChangesAsync();
                    return;
                }
                // ✅ 統一校正 SecurityAssessment，只允許三種值
                var rawSecurity = aiData.SecurityAssessment?.Trim() ?? "";
                Console.WriteLine($"[AI原始 SecurityAssessment] = {rawSecurity}");

<<<<<<< HEAD
                aiData.SecurityAssessment = rawSecurity switch
                {
                    "符合" => "符合",
                    "不適用" => "不適用",
                    "需補件" => "需補件",

                    "低" => "符合",
                    "低風險" => "符合",
                    "無明顯風險" => "符合",

                    "中" => "需補件",
                    "中風險" => "需補件",
                    "高" => "需補件",
                    "高風險" => "需補件",
                    "建議補強" => "需補件",

                    _ => "需補件"
                };

                System.Diagnostics.Debug.WriteLine($"[AI原始 SecurityAssessment] = {rawSecurity}");
                System.Diagnostics.Debug.WriteLine($"[校正後 SecurityAssessment] = {aiData.SecurityAssessment}");
                System.Diagnostics.Debug.WriteLine($"[準備存進DB的 SecurityAssessment] = {aiData.SecurityAssessment}");
                var aiDetail = new RequestAiDetail
                {
                    RequestId = ticketId,
                    IsITRelated = aiData.IsITRelated,
                    RefinedTitle = aiData.RefinedTitle,
                    RefinedDescription = aiData.RefinedDescription,
                    SecurityAssessment =
    aiData.SecurityAssessment == "符合" ||
    aiData.SecurityAssessment == "不適用" ||
    aiData.SecurityAssessment == "需補件"
        ? aiData.SecurityAssessment
        : "需補件",
                    AiReason = aiData.Reason,

                    AiReviewComment = aiData.AiReviewComment,
                    AiRequirementScore = reviewData?.AiRequirementScore,
                    AiStabilityScore = reviewData?.AiStabilityScore,
                    AiOverallScore = reviewData?.AiOverallScore,
                    AiSavedManDays = reviewData?.AiSavedManDays,
                    AiRevenue = reviewData?.AiRevenue,
=======
                var aiDetail = new RequestAiDetail
                {
                    RequestId = ticketId,
                    IsITRelated = aiData.IsITRelated,
                    RefinedTitle = aiData.RefinedTitle,
                    RefinedDescription = aiData.RefinedDescription,
                    SecurityAssessment = aiData.SecurityAssessment,
                    AiReason = aiData.Reason,

                    AiReviewComment = aiData.AiReviewComment,
                    AiRequirementScore = aiData.AiRequirementScore,
                    AiStabilityScore = aiData.AiStabilityScore,
                    AiOverallScore = aiData.AiOverallScore,
                    AiSavedManDays = aiData.AiSavedManDays,
                    AiRevenue = aiData.AiRevenue,
>>>>>>> parent of b807ace (調整UI：移除sidebar、置中使用者顧問中心、header新增查看表單)

                    ProcessedAt = DateTime.Now,
                    IsProcessed = true
                }; 

                db.RequestAiDetails.Add(aiDetail);

                // ✅ AI 初審分流
                if (aiData.IsApproved)
                {
                    ticket.Status = 1; // PendingReview = 待人工審核
                }
                else
                {
                    ticket.Status = 4; // Rejected = 已退回
                }

                ticket.UpdatedAt = DateTime.Now;

                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                db.RequestAiDetails.Add(new RequestAiDetail
                {
                    RequestId = ticketId,
                    ErrorMessage = ex.Message,
                    AiReason = "AI 初審發生錯誤，請稍後再試或人工確認",
                    IsProcessed = false
                });

                ticket.Status = 4;
                ticket.UpdatedAt = DateTime.Now;

                await db.SaveChangesAsync();
            }
        }
    }
    public class ReviewScoreAiResponseModel
    {
        public int? AiRequirementScore { get; set; }
        public int? AiStabilityScore { get; set; }
        public int? AiOverallScore { get; set; }
        public decimal? AiSavedManDays { get; set; }
        public decimal? AiRevenue { get; set; }
    }
    public class AiResponseModel
    {
        public bool IsApproved { get; set; }
        public bool IsITRelated { get; set; }

        public string RefinedTitle { get; set; } = "";
        public string RefinedDescription { get; set; } = "";
        public string SecurityAssessment { get; set; } = "";
        public string Reason { get; set; } = "";

        public string AiReviewComment { get; set; } = "";
        public int? AiRequirementScore { get; set; }
        public int? AiStabilityScore { get; set; }
        public int? AiOverallScore { get; set; }
        public decimal? AiSavedManDays { get; set; }
        public decimal? AiRevenue { get; set; }
    }
}