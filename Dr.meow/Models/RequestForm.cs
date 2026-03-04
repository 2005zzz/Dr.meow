using System;

namespace Dr.meow.Models
{
    public class RequestForm
    {
        public int Id { get; set; }

        // ===== 使用者填寫 =====
        public string Department { get; set; } = "";
        public string Contact { get; set; } = "";
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public string SystemCategory { get; set; } = "";
        public string Priority { get; set; } = "";
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public string Status { get; set; } = "PendingAI";

        // ===== AI審核 =====
        public bool? AiPass { get; set; }
        public string? AiReason { get; set; }
        public DateTime? AiReviewedAt { get; set; }

        // ===== 部門主管審核 (新增) =====

        // 驗收內容 / 審核意見
        public string? ReviewAcceptanceContent { get; set; }

        // 驗收日期
        public DateTime? ReviewAcceptanceDate { get; set; }

        // 資安評估
        public string? ReviewSecurityAssessment { get; set; }

        // 滿意度
        public int? ReviewSatisfactionNeed { get; set; }        // 需求一致性
        public int? ReviewSatisfactionStability { get; set; }   // 功能穩定性
        public int? ReviewSatisfactionOverall { get; set; }     // 整體評分

        // 效益
        public decimal? ReviewBenefitManDays { get; set; }      // 每月節省人力
        public decimal? ReviewBenefitRevenue { get; set; }      // 創造收益

        // 退回原因
        public string? ReviewRejectReason { get; set; }

        // 審核者資訊
        public string? ReviewedBy { get; set; }
        public DateTime? ReviewedAt { get; set; }
    }
}