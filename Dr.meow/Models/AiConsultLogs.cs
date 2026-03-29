using System;
namespace Dr.meow.Models
{
    public class AiConsultLogs
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string UserMessage { get; set; } = null!;
        public string AiResponse { get; set; } = null!;
        public string SessionId { get; set; } = null!; // 用於區分當次登入的對話片段
        public DateTime Timestamp { get; set; } = DateTime.Now;
    }
}
