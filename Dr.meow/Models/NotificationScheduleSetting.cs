using System.ComponentModel.DataAnnotations;

namespace Dr.meow.Models
{
    public class NotificationScheduleSetting
    {
        [Key]
        public int Id { get; set; }

        public int Hour { get; set; } = 9;

        public int Minute { get; set; } = 0;

        public bool IsEnabled { get; set; } = true;

        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}