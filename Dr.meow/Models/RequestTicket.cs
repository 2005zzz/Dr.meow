using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dr.meow.Models
{
    public class RequestTicket
    {
        [Key]
        public int Id { get; set; }
        public string TicketNumber { get; set; } = "";
        public int RequesterId { get; set; }
        // 🚀 補上這行，建立與 User 表的關聯
        [ForeignKey(nameof(RequesterId))]
        public virtual User? Requester { get; set; }

        // 🚀 新增這個欄位，確保主管能根據部門過濾單據
        [MaxLength(50)]
        public string? Department { get; set; } = "";
        public string Title { get; set; } = "";

        [Column(TypeName = "nvarchar(max)")] // 🚀 確保長度足夠
        public string? Description { get; set; } = "";

        // 🚀 新增：期望完成日期 (非必填，所以用 DateTime?)
        public DateTime? ExpectedCompletionDate { get; set; }

        // 🚀 新增：資訊評估區塊欄位
        [MaxLength(100)]
        public string? SystemCategory { get; set; } = ""; // 系統類別 (例如：掛號系統)

        [MaxLength(50)]
        public string? RequestType { get; set; } = "";    // 需求類型 (例如：新功能、bug修復)

        [MaxLength(20)]
        public string? Priority { get; set; } = "中";     // 優先等級

        [Column(TypeName = "nvarchar(max)")]
        public string? ExpectedBenefits { get; set; } = ""; // 預期效益 / 原因
        public byte Status { get; set; } = 0; // 0: PendingAI
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        // 導覽屬性 (Navigation Properties)
        public virtual RequestUserInput UserInput { get; set; }
        public virtual RequestAiDetail AiDetail { get; set; }
        public virtual ICollection<RequestAuditLog> AuditLogs { get; set; }
    }
}