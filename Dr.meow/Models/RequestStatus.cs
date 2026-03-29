using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dr.meow.Models
{
    public class RequestStatus
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)] // 👈 這裡很重要，因為我們手動指定 0, 1, 2...
        public byte StatusId { get; set; } // 👈 必須叫 StatusId

        [Required]
        [StringLength(30)]
        public string StatusName { get; set; } = ""; // 👈 必須叫 StatusName

        [StringLength(100)]
        public string? Description { get; set; } // 👈 必須叫 Description
    }
}