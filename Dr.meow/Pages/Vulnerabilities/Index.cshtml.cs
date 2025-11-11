using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Dr.meow.Data;
using Dr.meow.Models;
using ClosedXML.Excel; // 確保有這一行!

namespace Dr.meow.Pages.Vulnerabilities
{
    public class IndexModel : PageModel
    {
        private readonly Dr.meow.Data.DrMeowDbContext _context;

        public IndexModel(Dr.meow.Data.DrMeowDbContext context)
        {
            _context = context;
        }

        public IList<Vulnerability> Vulnerability { get; set; } = default!;

        public async Task OnGetAsync()
        {
            if (_context.Vulnerability != null)
            {
                // 讀取所有漏洞資料到列表頁模型中
                Vulnerability = await _context.Vulnerability.ToListAsync();
            }
        }

        // ***** 新增匯出 Excel 的 Action Method *****
        // 這是使用者點擊「匯出 Excel 報告」按鈕時會執行的方法
        public async Task<IActionResult> OnGetExportToExcel()
        {
            // 1. 從資料庫讀取所有數據
            var vulnerabilities = await _context.Vulnerability.ToListAsync();

            // 2. 建立 Excel 活頁簿
            using (var workbook = new XLWorkbook())
            {
                // 3. 新增工作表
                var worksheet = workbook.Worksheets.Add("漏洞追蹤報告");

                // 4. 設定標題行 (包含您最新新增的欄位)
                var currentRow = 1;
                worksheet.Cell(currentRow, 1).Value = "ID";
                worksheet.Cell(currentRow, 2).Value = "系統/工單類別";
                worksheet.Cell(currentRow, 3).Value = "狀態/變更類型";
                worksheet.Cell(currentRow, 4).Value = "嚴重度/風險";
                // worksheet.Cell(currentRow, 5).Value = "實施日期";
                worksheet.Cell(currentRow, 6).Value = "單號/指派對象";
                worksheet.Cell(currentRow, 7).Value = "內容/描述";
                // worksheet.Cell(currentRow, 8).Value = "測試計劃";
                // worksheet.Cell(currentRow, 9).Value = "回復計劃";

                // 將標題行設為粗體
                worksheet.Range(currentRow, 1, currentRow, 9).Style.Font.Bold = true;
                worksheet.Range(currentRow, 1, currentRow, 9).Style.Fill.BackgroundColor = XLColor.LightGray;

                // 5. 寫入資料
                foreach (var vulnerability in vulnerabilities)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = vulnerability.Id;
                    worksheet.Cell(currentRow, 2).Value = vulnerability.Title;
                    worksheet.Cell(currentRow, 3).Value = vulnerability.Status;
                    worksheet.Cell(currentRow, 4).Value = vulnerability.Severity;
                    worksheet.Cell(currentRow, 5).Value = vulnerability.FoundDate.ToString("yyyy/MM/dd"); // 格式化日期
                    worksheet.Cell(currentRow, 6).Value = vulnerability.AssignedTo;
                    worksheet.Cell(currentRow, 7).Value = vulnerability.Description;
                    // worksheet.Cell(currentRow, 8).Value = vulnerability.TestPlan;
                    // worksheet.Cell(currentRow, 9).Value = vulnerability.RecoveryPlan;
                }

                // 6. 自動調整欄寬
                worksheet.Columns().AdjustToContents();

                // 7. 將活頁簿寫入記憶體流並傳回
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    var fileName = $"漏洞追蹤報告_{DateTime.Now:yyyyMMddHHmmss}.xlsx";

                    return File(content, contentType, fileName);
                }
            }
        }
    }
}