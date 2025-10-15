using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RazorApp.Pages
{
    public class IndexModel : PageModel
    {
        // 屬性，用來存資料給前端顯示
        public string Message { get; set; }
        public string Time { get; set; }

        public void OnGet()
        {
            // 模擬資料
            Message = "這是從 ASP.NET Core 後端傳來的資料！";
            Time = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
        }
    }
}
