using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Dr.meow.Models
{
    public class RequestTicket
    {
        [Key]
        public int Id { get; set; }
        public string TicketNumber { get; set; } = "";
        public int RequesterId { get; set; }
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public byte Status { get; set; } = 0; // 0: PendingAI
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        // 導覽屬性 (Navigation Properties)
        public virtual RequestUserInput UserInput { get; set; }
        public virtual RequestAiDetail AiDetail { get; set; }
        public virtual ICollection<RequestAuditLog> AuditLogs { get; set; }
    }
}