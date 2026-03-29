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
    }
}