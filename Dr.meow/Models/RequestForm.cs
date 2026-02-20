using System;

namespace Dr.meow.Models
{
    public class RequestForm
    {
        public int Id { get; set; }

        public string Department { get; set; } = "";
        public string Contact { get; set; } = "";
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public string SystemCategory { get; set; } = "";
        public string Priority { get; set; } = "";
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public string Status { get; set; } = "PendingAI";

        public bool? AiPass { get; set; }
        public string? AiReason { get; set; }
        public DateTime? AiReviewedAt { get; set; }
    }
}
