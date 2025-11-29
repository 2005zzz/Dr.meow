using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;

namespace Dr.meow.Pages
{
    public class UserHomeModel : PageModel
    {
        public string Account { get; set; } = "使用者";

        public void OnGet()
        {
            Account = HttpContext.Session.GetString("Account") ?? "使用者";
        }
    }
}
