using System;
using System.ComponentModel.DataAnnotations;

namespace Dr.meow.Models
{
    public class RequestAuditLog
    {
        [Key]
        public int Id { get; set; }
        public int RequestId { get; set; }
        public int ActorId { get; set; }
        public string Action { get; set; } = "";
        public int? TargetUserId { get; set; }
        public string? Comment { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.Now;

        public virtual RequestTicket RequestTicket { get; set; }
    }
}