namespace Dr.meow.Models
{
    public class User
    {
        public int Id { get; set; }

        public string Account { get; set; } = "";

        public string PasswordHash { get; set; } = "";

        // "user" 或 "admin"
        public string Role { get; set; } = "user";

        public string? Email { get; set; }

        public DateTime CreateDate { get; set; } = DateTime.Now;
    }
}
