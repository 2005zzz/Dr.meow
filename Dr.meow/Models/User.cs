using System.Collections.Generic;

namespace Dr.meow.Models
{
    public class User
    {
        public int UserId { get; set; }

        // ⭐ 系統帳號登入用（你負責的）
        public string? Account { get; set; }        // 允許 null → Google 用戶沒有帳號
        public string? PasswordHash { get; set; }   // Google 用戶沒有密碼

        // ⭐ 共用識別資訊
        public string Email { get; set; } = null!;　// Google 一定會給 Email

        // ⭐ Google 登入用（組員會用到）
        public string? GoogleId { get; set; }       // Google OAuth 回傳的唯一 ID

        // ⭐ 登入來源類型
        public string AccountType { get; set; } = "System";// System = 系統帳號註冊　Google = Google OAuth 用戶

        public bool IsActive { get; set; } = true;

        // ⭐ 權限角色（你之前做的）
        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }
}
