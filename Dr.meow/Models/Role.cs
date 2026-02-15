namespace Dr.meow.Models
{
    public class Role
    {
        public int RoleId { get; set; }

        // 權限名稱：單位主管 / team1 / team2 / 最高階主管
        public string RoleName { get; set; } = null!;

        // 關聯到哪些使用者
        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }
}
