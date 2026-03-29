using System;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using System.Text.Json; // ✅ 使用高效能 JSON 解析
using Dr.meow.Data;
using Dr.meow.Models;
using Dr.meow.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Hangfire; // ✅ 確保引用 Hangfire
using System.ComponentModel.DataAnnotations;
using Dr.meow.Models;  // 妳剛建的那四個 RequestTicket 等類別
using Dr.meow.Data;    // 妳的 DbContext 所在地

namespace Dr.meow.Pages.Request
{
    // ✅ 提單專用的 ViewModel：負責「畫面輸入」與「欄位驗證」
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

        public RequestCreateModel(
            ISearchService searchService,
            DrMeowDbContext db,
            IBackgroundJobClient backgroundJob,
            IServiceScopeFactory scopeFactory)
        {
            _searchService = searchService;
            _db = db;
            _backgroundJob = backgroundJob;
            _scopeFactory = scopeFactory;
        }

        [BindProperty] // ✅ 讓表單能自動綁定回此 Model
        public RequestCreateViewModel FormModel { get; set; }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            // ✅ 1) Model Validation (檢查 ViewModel 上的特性)
            if (!ModelState.IsValid) return Page();

            // ✅ 2) 取得真正的登入使用者 ID
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                TempData["Message"] = "❌ 授權逾時，請重新登入";
                return RedirectToPage("/Account/Login");
            }

            var ticketNumber = await GenerateUniqueTicketNumber();

            using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                // A. 建立主表 Ticket (只存核心搜尋資訊)
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

                // B. 儲存使用者輸入的完整詳細資訊 (正規化拆分)
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

                // C. 紀錄審核軌跡 (Audit Log)
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

                // ✅ 4) 非同步處理 AI 分析 (傳入 ID，讓背景任務去查 DB，避免參數過長)
                _backgroundJob.Enqueue(() => ProcessAiAnalysisAsync(ticket.Id));

                TempData["Message"] = $"✅ 需求單 {ticketNumber} 已送出！AI 正在進行資安掃描與改寫中...";
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                TempData["Message"] = "❌ 存檔失敗：" + ex.Message;
                return Page();
            }

            return RedirectToPage("/Request/RequestCreate");
        }

        // 生成不重複單號
        private async Task<string> GenerateUniqueTicketNumber()
        {
            var datePart = DateTime.Now.ToString("yyyyMMdd");
            string ticketNumber;
            do
            {
                var randPart = Guid.NewGuid().ToString().Substring(0, 5).ToUpper();
                ticketNumber = $"REQ-{datePart}-{randPart}";
            } while (await _db.RequestTickets.AnyAsync(x => x.TicketNumber == ticketNumber));
            return ticketNumber;
        }

        // 🔥 背景任務：AI 深度分析
        [AutomaticRetry(Attempts = 2)] // Hangfire 失敗自動重試
        public async Task ProcessAiAnalysisAsync(int ticketId)
        {
            // 注意：背景任務需要獨立的 DB Context Scope
            // 這裡假設妳已經在 DI 容器中設定好，或者手動建立
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<DrMeowDbContext>();

            var ticket = await db.RequestTickets.Include(t => t.UserInput).FirstOrDefaultAsync(x => x.Id == ticketId);
            if (ticket == null) return;

            try
            {
                var prompt = $@"
                    你是醫院資安與需求分析 AI。分析以下需求並只回傳純 JSON：
                    {{
                        ""isITRelated"": true/false,
                        ""refinedTitle"": ""專業標題"",
                        ""refinedDescription"": ""專業需求補全"",
                        ""securityAssessment"": ""風險等級與建議"",
                        ""reason"": ""分析理由""
                    }}
                    內容：{ticket.Title} - {ticket.UserInput.Description}";

                var aiResult = await _searchService.SearchAsync(prompt);

                // ✅ 使用 System.Text.Json 解析 (處理 AI 可能噴出的 JSON Markdown)
                var cleanJson = aiResult.Answer?.Replace("```json", "").Replace("```", "").Trim();
                var aiData = JsonSerializer.Deserialize<AiResponseModel>(cleanJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                var aiDetail = new RequestAiDetail
                {
                    RequestId = ticketId,
                    IsITRelated = aiData.IsITRelated,
                    RefinedTitle = aiData.RefinedTitle,
                    RefinedDescription = aiData.RefinedDescription,
                    SecurityAssessment = aiData.SecurityAssessment,
                    AiReason = aiData.Reason,
                    ProcessedAt = DateTime.Now,
                    IsProcessed = true
                };

                db.RequestAiDetails.Add(aiDetail);
                ticket.Status = 1; // 切換為 PendingReview
                ticket.UpdatedAt = DateTime.Now;

                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // 存入錯誤訊息到 AiDetail 表，方便排錯
                db.RequestAiDetails.Add(new RequestAiDetail
                {
                    RequestId = ticketId,
                    ErrorMessage = ex.Message,
                    IsProcessed = false
                });
                await db.SaveChangesAsync();
            }
        }
    }

    // AI 回傳 JSON 的專用模型
    public class AiResponseModel
    {
        public bool IsITRelated { get; set; }
        public string RefinedTitle { get; set; }
        public string RefinedDescription { get; set; }
        public string SecurityAssessment { get; set; }
        public string Reason { get; set; }
    }
}