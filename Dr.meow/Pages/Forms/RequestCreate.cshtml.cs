using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dr.meow.Pages.Forms
{
    public class RequestCreateModel : PageModel
    {
        public void OnGet() { }

        public IActionResult OnPost(
            string Department,
            string Role,
            string Contact,
            string Title,
            string Description,
            string SystemCategory,
            string RequestType,
            string Priority,
            string Benefit,
            string ExpectedDate,
            string Note
        )
        {
            // 先做可用版：不接 DB 也不會出錯
            TempData["Message"] = "✅ 需求單已送出（示範），資訊人員將進行接收審核。";
            return RedirectToPage("/Forms/RequestCreate");
        }
    }
}
