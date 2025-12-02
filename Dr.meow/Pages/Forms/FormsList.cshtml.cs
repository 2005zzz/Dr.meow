using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Dr.meow.Data;
using Dr.meow.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dr.meow.Pages.Forms
{
    // 請確保此處的命名空間與您的專案結構一致
    public class FormsListModel : PageModel
    {
        // 宣告資料庫上下文
        private readonly DrMeowDbContext _context;

        // 這是儲存從資料庫讀取出來的漏洞/變更清單的屬性
        public IList<Vulnerability> VulnerabilityList { get; set; } = default!;

        // 構造函式：透過依賴注入取得資料庫上下文
        public FormsListModel(DrMeowDbContext context)
        {
            _context = context;
        }

        // 處理 HTTP GET 請求
        public async Task OnGetAsync()
        {
            // 從資料庫中讀取所有的 Vulnerability 紀錄
            // 如果您的資料表名稱有變動，請在這裡修正
            VulnerabilityList = await _context.Vulnerability
                                                .OrderByDescending(v => v.Id) // 依照 Id 降冪排序，顯示最新的紀錄
                                                .ToListAsync();
        }
    }
}