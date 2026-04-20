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
            try
            {
                var vuln = new Vulnerability
                {
                    TicketNumber = "CHG-" + DateTime.Now.ToString("yyyyMMddHHmm"),
                    Title = data.Title,
                    Description = data.Description,
                    Department = data.Department ?? "資訊中心",
                    RequesterId = data.RequesterId != 0 ? data.RequesterId : 5, // 預設 5

                    // 映射 AI 分析出的資安欄位
                    SystemCategory = data.SystemCategory ?? "Other",
                    TicketCategory = data.TicketCategory ?? "DevOps",
                    ChangeType = data.ChangeType ?? "標準",
                    Severity = data.Priority ?? "低風險",
                    ImpactLevel = data.ImpactLevel ?? "低",
                    Dependency = data.Dependency ?? "無",
                    TestPlan = data.TestPlan ?? "依標準程序測試",
                    RecoveryPlan = data.RecoveryPlan ?? "執行備份還原",

                    ScheduledTime = DateTime.TryParse(data.ExpectedDate, out var dt) ? dt : DateTime.Now,
                    Status = "Pending",
                    CreatedAt = DateTime.Now
                };

                _context.Vulnerability.Add(vuln);
                await _context.SaveChangesAsync();
                return Ok(new { success = true, ticketNumber = vuln.TicketNumber, id = vuln.Id });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Vulnerability Error: {ex.Message}");
            }
        }

        // 📄 內部邏輯：處理需求單 (RequestTicket 三表關聯)
        private async Task<IActionResult> ProcessRequestTicket(SaveFormRequest data)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // A. RequestTicket
                var ticket = new RequestTicket
                {
                    TicketNumber = "REQ-" + DateTime.Now.ToString("yyyyMMddHHmm"),
                    RequesterId = data.RequesterId != 0 ? data.RequesterId : 5,
                    Title = data.Title,
                    Description = data.Description,
                    Status = 1,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };
                _context.RequestTickets.Add(ticket);
                await _context.SaveChangesAsync();

                // B. RequestUserInput
                var userInput = new RequestUserInput
                {
                    RequestId = ticket.Id,
                    Department = data.Department ?? "未知部門",
                    Contact = data.Extension ?? "",
                    Description = data.Description,
                    Priority = data.Priority ?? "中",
                    Role = "User",
                    SystemCategory = "Other",
                    RequestType = data.FormType ?? "General",
                    ExpectedDate = DateTime.TryParse(data.ExpectedDate, out var d) ? d : DateTime.Now.AddDays(7)
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
                return StatusCode(500, $"Request DB Error: {ex.Message}");
            }
        }
    }

    // 萬用 DTO：包含所有可能傳過來的欄位
    public class SaveFormRequest
    {
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public string? TicketNumber { get; set; }
        public int RequesterId { get; set; }
        public string? Department { get; set; }
        public string? Extension { get; set; }
        public string? ExpectedDate { get; set; }
        public string? Priority { get; set; }
        public string? FormType { get; set; }
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
        public string? SystemCategory { get; set; }
        public string? TicketCategory { get; set; }
        public string? ChangeType { get; set; }
        public string? ImpactLevel { get; set; }
        public string? Dependency { get; set; }
        public string? TestPlan { get; set; }
        public string? RecoveryPlan { get; set; }
    }
}