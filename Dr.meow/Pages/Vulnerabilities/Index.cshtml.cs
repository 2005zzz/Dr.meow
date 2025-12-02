using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Dr.meow.Data;
using Dr.meow.Models;
using ClosedXML.Excel;
using System.IO;

namespace Dr.meow.Pages.Vulnerabilities
{
    public class IndexModel : PageModel
    {
        private readonly DrMeowDbContext _context;

        public IndexModel(DrMeowDbContext context)
        {
            _context = context;
        }

        public IList<Vulnerability> Vulnerability { get; set; } = default!;

        public async Task OnGetAsync()
        {
            if (_context.Vulnerability != null)
            {
                Vulnerability = await _context.Vulnerability.ToListAsync();
            }
        }

        // 匯出 Excel
        public async Task<IActionResult> OnGetExportToExcel()
        {
            var vulnerabilities = await _context.Vulnerability.ToListAsync();

            using (var workbook = new XLWorkbook())
            {
                // 工作表名稱
                var worksheet = workbook.Worksheets.Add("變更 ／ 漏洞報表");

                // 標題列
                var currentRow = 1;
                worksheet.Cell(currentRow, 1).Value = "ID";
                worksheet.Cell(currentRow, 2).Value = "標題 / 系統名稱";
                worksheet.Cell(currentRow, 3).Value = "狀態";
                worksheet.Cell(currentRow, 4).Value = "嚴重程度";
                worksheet.Cell(currentRow, 5).Value = "發現日期";
                worksheet.Cell(currentRow, 6).Value = "指派對象";
                worksheet.Cell(currentRow, 7).Value = "描述";

                // 標題格式
                worksheet.Range(currentRow, 1, currentRow, 7).Style.Font.Bold = true;
                worksheet.Range(currentRow, 1, currentRow, 7).Style.Fill.BackgroundColor = XLColor.LightGray;

                // 內容列
                foreach (var v in vulnerabilities)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = v.Id;
                    worksheet.Cell(currentRow, 2).Value = v.Title;
                    worksheet.Cell(currentRow, 3).Value = v.Status;
                    worksheet.Cell(currentRow, 4).Value = v.Severity;
                    worksheet.Cell(currentRow, 5).Value = v.FoundDate.ToString("yyyy/MM/dd");
                    worksheet.Cell(currentRow, 6).Value = v.AssignedTo;
                    worksheet.Cell(currentRow, 7).Value = v.Description;
                }

                worksheet.Columns().AdjustToContents();

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    var fileName = $"漏洞清單_{DateTime.Now:yyyyMMddHHmmss}.xlsx";

                    return File(content, contentType, fileName);
                }
            }
        }
    }
}