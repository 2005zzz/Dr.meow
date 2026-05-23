using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Dr.meow.Models
{
    public class User
    {
        [Key]
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

        // ⭐ 新增：所屬組別（例如：Team1, Team2, IT_Engineers）
        // 這樣組員提交時，系統才能根據此欄位判斷「第一關」要給哪個組長
        public string? Department { get; set; }

        // ⭐ 新增：真實姓名（用於表單顯示，Google 登入時抓取 Name）
        public string? UserName { get; set; }

        // 新增日期追蹤
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
