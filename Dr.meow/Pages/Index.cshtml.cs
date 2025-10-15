using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RazorApp.Pages
{
    public class IndexModel : PageModel
    {
        // �ݩʡA�ΨӦs��Ƶ��e�����
        public string Message { get; set; }
        public string Time { get; set; }

        public void OnGet()
        {
            // �������
            Message = "�o�O�q ASP.NET Core ��ݶǨӪ���ơI";
            Time = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
        }
    }
}
