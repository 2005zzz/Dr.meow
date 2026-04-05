using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Dr.meow.Models;

namespace Dr.meow.Models
{
    public class RequestAiDetail
    {
        [Key, ForeignKey("RequestTicket")]
        public int RequestId { get; set; }
        public bool IsITRelated { get; set; }
        public string? RefinedTitle { get; set; }
        public string? RefinedDescription { get; set; }
        public string? SecurityAssessment { get; set; }
        public string? AiReason { get; set; } // ✅ 補上這個欄位
        public DateTime ProcessedAt { get; set; }
        public string? ErrorMessage { get; set; }
        public bool IsProcessed { get; set; }

        public virtual RequestTicket RequestTicket { get; set; }

        public string? AiReviewComment { get; set; }      // AI 建議的驗收內容 / 審核意見
        public int? AiRequirementScore { get; set; }      // 需求一致性 1~5
        public int? AiStabilityScore { get; set; }        // 功能穩定性 1~5
        public int? AiOverallScore { get; set; }          // 整體 1~5
        public decimal? AiSavedManDays { get; set; }      // 每月節省人力
        public decimal? AiRevenue { get; set; }           // 創造收益
    }
}