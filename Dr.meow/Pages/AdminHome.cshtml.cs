using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;

namespace Dr.meow.Pages
{
    public class AdminHomeModel : PageModel
    {
        public string Account { get; set; } = "管理者";

        public void OnGet()
        {
            Account = HttpContext.Session.GetString("Account") ?? "管理者";
        }
    }
}
