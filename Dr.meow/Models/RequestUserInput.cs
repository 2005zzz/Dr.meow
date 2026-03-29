using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dr.meow.Models
{
    public class RequestUserInput
    {
        [Key, ForeignKey("RequestTicket")]
        public int RequestId { get; set; }
        public string Department { get; set; } = "";
        public string Role { get; set; } = "";
        public string Contact { get; set; } = "";
        public string Description { get; set; } = "";
        public string SystemCategory { get; set; } = "";
        public string RequestType { get; set; } = "";
        public string Priority { get; set; } = "";
        public string? Benefit { get; set; }
        public DateTime? ExpectedDate { get; set; }
        public string? Note { get; set; }

        public virtual RequestTicket RequestTicket { get; set; }
    }
}