namespace Dr.meow.Models
{
    public class Notifications
    {
        public int Id { get; set; }
        public int UserId { get; set; } // 通知誰
        public string Title { get; set; } = null!;
        public string Content { get; set; } = null!;
        public bool IsRead { get; set; } = false; // 是否已讀
        public string? LinkUrl { get; set; } // 點擊通知後導向哪張表單
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
