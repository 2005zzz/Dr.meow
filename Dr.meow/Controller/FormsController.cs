using Microsoft.AspNetCore.Mvc;
using Dr.meow.Data;
using Dr.meow.Models;
using Microsoft.EntityFrameworkCore;

namespace Dr.meow.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FormsController : ControllerBase
    {
        private readonly DrMeowDbContext _context;

        public FormsController(DrMeowDbContext context)
        {
            _context = context;
        }

        // 🔥 【核心入口】萬用存檔方法
        [HttpPost("SaveAll")]
        public async Task<IActionResult> SaveAll([FromBody] SaveFormRequest data)
        {
            if (data == null) return BadRequest("請求資料不可為空");

            // 🔍 關鍵補丁：如果前端/轉發端已經傳了 ID，就以它為準
            // 如果沒有傳，才去試著抓本地 Session (用於網頁直接操作時)
            if (data.RequesterId == 0)
            {
                var sessionUserId = HttpContext.Session.GetString("UserId");
                if (!string.IsNullOrEmpty(sessionUserId))
                {
                    data.RequesterId = int.Parse(sessionUserId);
                }
            }

            // 🔍 同理，確保 Department 也有值
            if (string.IsNullOrEmpty(data.Department))
            {
                data.Department = HttpContext.Session.GetString("UserTeam") ?? "Team1";
            }

            // 根據 FormType 判斷要走哪一條路 (不分大小寫)
            string type = data.FormType?.ToLower() ?? "general";

            if (type == "vulnerability" || type == "change")
            {
                return await ProcessVulnerability(data);
            }
            else
            {
                return await ProcessRequestTicket(data);
            }
        }

        // 🛡️ 內部邏輯：處理變更單 (Vulnerability)
        private async Task<IActionResult> ProcessVulnerability(SaveFormRequest data)
        {

            var connection = _context.Database.GetDbConnection();
            Console.WriteLine($"🔍 [寫入方] 資料庫: {connection.Database}, 伺服器: {connection.DataSource}");

            // 建立一個 Transaction 確保兩張表同時寫入成功或同時失敗
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                Console.WriteLine($"🔍 [診斷] 前端送來的完整資料: {System.Text.Json.JsonSerializer.Serialize(data)}");
                var userTeam = HttpContext.Session.GetString("UserTeam");
                var userIdStr = HttpContext.Session.GetString("UserId");

                // 1. 先存主表 Vulnerability
                var vuln = new Vulnerability
                {
                    TicketNumber = "CHG-" + DateTime.Now.ToString("yyyyMMddHHmm"),
                    Description = data.Description,
                    Department = userTeam ?? data.Department ?? "Unknown",
                    RequesterId = !string.IsNullOrEmpty(userIdStr) ? int.Parse(userIdStr) : (data.RequesterId != 0 ? data.RequesterId : 5),
                    SystemCategory = data.SystemCategory ?? "Other",
                    TicketCategory = data.TicketCategory ?? "DevOps",
                    ChangeType = data.ChangeType ?? "標準",
                    Severity = data.Priority ?? "低風險",
                    ImpactLevel = data.ImpactLevel ?? "低",
                    Dependency = data.Dependency ?? "無",
                    TestPlan = data.TestPlan ?? "依標準程序測試",
                    RecoveryPlan = data.RecoveryPlan ?? "執行備份還原",
                    ScheduledTime = DateTime.TryParse(data.ExpectedCompletionDate, out var dt) ? dt : DateTime.Now,
                    Status = "Pending",
                    CreatedAt = DateTime.Now
                };

                _context.Vulnerability.Add(vuln);
                await _context.SaveChangesAsync(); // 這一行執行完，vuln.Id 就會有值了！

                

                // 2. 再存 AI 詳細資料 VulnerabilityAiDetail (現在它有家了！)
                var aiDetail = new VulnerabilityAiDetail
                {
                    VulnerabilityId = vuln.Id, // 關聯主表 Id
                    AiReviewComment = data.AiReviewComment ?? data.AiReason ?? "自動生成的資安評估...",
                    AiOverallScore = data.AiOverallScore,
                    AiRequirementScore = data.AiRequirementScore,
                    AiStabilityScore = data.AiStabilityScore,
                    SecurityAssessment = data.SecurityAssessment ?? "需補件",
                    IsProcessed = true,
                    ProcessedAt = DateTime.Now,
                    ComplianceStatus = data.ComplianceStatus ?? "Review",
                    PriorityScore = data.PriorityScore ?? 50
                };

                Console.WriteLine($"🔍 準備寫入 AI Detail, ID: {vuln.Id}, Comment: {aiDetail.AiReviewComment}");
                Console.WriteLine($"🔍 [最終除錯] 準備存入的 PriorityScore 是: {aiDetail.PriorityScore}");
                Console.WriteLine($"🔍 [存檔排查] 這張主單的 ID 是: {vuln.Id}");

                _context.VulnerabilityAiDetail.Add(aiDetail);
                var entry = _context.Entry(aiDetail);
                Console.WriteLine($"🔍 [追蹤狀態] 欄位 PriorityScore 的狀態: {entry.Property(x => x.PriorityScore).CurrentValue}");
                int rowsAffected = await _context.SaveChangesAsync();
                Console.WriteLine($"🔍 已寫入列數: {rowsAffected}");

                await transaction.CommitAsync(); // 兩張表都寫入成功才提交

                return Ok(new { success = true, ticketNumber = vuln.TicketNumber, id = vuln.Id });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                var error = ex.InnerException?.Message ?? ex.Message;
                return StatusCode(500, $"Vulnerability Error: {error}");
            }
        }

        // 📄 內部邏輯：處理需求單 (RequestTicket 三表關聯)
        private async Task<IActionResult> ProcessRequestTicket(SaveFormRequest data)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                System.Diagnostics.Debug.WriteLine($"==== [ProcessRequestTicket] ====");
                if (data.RequesterId == 0)
                {
                    return BadRequest("RequesterId 不可為 0");
                }

                // 🚀 【核心修正】：在 new RequestTicket 時，正式且確實地將欄位指派給實體！
                var ticket = new RequestTicket
                {
                    TicketNumber = "REQ-" + DateTime.Now.ToString("yyyyMMddHHmm"),
                    RequesterId = data.RequesterId,
                    Title = data.Title ?? "未命名需求",
                    Description = data.Description ?? "",
                    Status = 1, // PendingAI
                    Department = data.Department ?? "Team1",
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,

                    // 🎯 修正 1：日期字串安全解碼轉換，並賦值給主表的 ExpectedCompletionDate
                    ExpectedCompletionDate = string.IsNullOrEmpty(data.ExpectedCompletionDate)
                        ? (DateTime?)null
                        : (DateTime.TryParse(data.ExpectedCompletionDate, out var parsedMainDate) ? parsedMainDate : null),

                    // 🎯 修正 2：將獨立的需求單資安擴充屬性直接寫入 RequestTicket 實體欄位！
                    SystemCategory = data.SystemCategory ?? "Other",
                    RequestType = data.RequestType ?? "一般需求",
                    Priority = data.Priority ?? "中",
                    ExpectedBenefits = data.ExpectedBenefits ?? ""
                };

                _context.RequestTickets.Add(ticket);
                await _context.SaveChangesAsync();

                // B. RequestUserInput (三表關聯：關聯資料同步對齊補強)
                var userInput = new RequestUserInput
                {
                    RequestId = ticket.Id,
                    Department = data.Department ?? "未知部門",
                    Contact = data.Extension ?? "",
                    Description = data.Description,
                    Priority = data.Priority ?? "中",
                    Role = "User",
                    SystemCategory = data.SystemCategory ?? "Other",
                    RequestType = data.RequestType ?? "General",
                    ExpectedDate = string.IsNullOrEmpty(data.ExpectedCompletionDate)
                        ? DateTime.Now.AddDays(7)
                        : (DateTime.TryParse(data.ExpectedCompletionDate, out var d) ? d : DateTime.Now.AddDays(7))
                };
                _context.RequestUserInputs.Add(userInput);

                // C. RequestAiDetail
                var aiDetail = new RequestAiDetail
                {
                    RequestId = ticket.Id,
                    IsITRelated = true,
                    RefinedTitle = data.RefinedTitle ?? data.Title,
                    RefinedDescription = data.RefinedDescription ?? data.Description,
                    SecurityAssessment = data.SecurityAssessment ?? "需補件",
                    AiReason = data.AiReason ?? "AI 自動分析存檔",
                    AiReviewComment = data.AiReviewComment,

                    AiRequirementScore = data.AiRequirementScore,
                    AiStabilityScore = data.AiStabilityScore,
                    AiOverallScore = data.AiOverallScore,
                    AiSavedManDays = data.AiSavedManDays,
                    AiRevenue = data.AiRevenue,

                    ProcessedAt = DateTime.Now,
                    IsProcessed = true
                };
                _context.RequestAiDetails.Add(aiDetail);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(new { success = true, ticketNumber = ticket.TicketNumber, id = ticket.Id });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                var innerError = ex.InnerException?.Message ?? ex.Message;
                Console.WriteLine($"❌ 資料庫寫入失敗詳情: {innerError}");
                return StatusCode(500, $"Request DB Error: {innerError}");
            }
        }
    }

    // 🚀 【全量強型別萬用 DTO】：100% 同步 7068 封包契約
    public class SaveFormRequest
    {
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public string? TicketNumber { get; set; }
        public int RequesterId { get; set; }
        public string? Department { get; set; }
        public string? Extension { get; set; }


        // 🎯 導正傳輸對域變數
        public string? ExpectedCompletionDate { get; set; }
        public string? Priority { get; set; }
        public string? FormType { get; set; }

        // 🚀 補齊這三個擴充欄位，讓 7186 控制器能平安接收到傳過來的物件欄位
        public string? SystemCategory { get; set; }
        public string? RequestType { get; set; }
        public string? ExpectedBenefits { get; set; }

        public string? Severity { get; set; }
        public string? RefinedTitle { get; set; }
        public string? RefinedDescription { get; set; }
        public string? SecurityAssessment { get; set; }
        public string? AiReason { get; set; }
        public string? AiReviewComment { get; set; }
        public int? AiRequirementScore { get; set; }
        public int? AiStabilityScore { get; set; }
        public int? AiOverallScore { get; set; }
        public decimal? AiSavedManDays { get; set; }
        public decimal? AiRevenue { get; set; }

        // 變更單額外擴充
        public string? TicketCategory { get; set; }
        public string? ChangeType { get; set; }
        public string? ImpactLevel { get; set; }
        public string? Dependency { get; set; }
        public string? TestPlan { get; set; }
        public string? RecoveryPlan { get; set; }
        public string? ChatSnapshot { get; set; }
        public string? ComplianceStatus { get; set; }
        public int? PriorityScore { get; set; }
    }
}