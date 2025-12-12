using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Dr.meow.Data;
using Dr.meow.Models;
using System.IO;
using System.Diagnostics;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Text.RegularExpressions;
using System.Drawing;

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

        // 匯出 Excel (使用 EPPlus 實現)
        public async Task<IActionResult> OnGetExportToExcel()
        {
            // EPPlus 授權設置 (必須)
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            // 數據過濾：過濾掉 Title 為 Null 或空字串的記錄
            var vulnerabilities = await _context.Vulnerability
                .Where(v => v.Title != null && v.Title != "")
                .Take(10)
                .ToListAsync();

            // --- 數據清洗函式：移除所有非法 XML 字符與換行符 ---
            string SanitizeText(string? text)
            {
                if (string.IsNullOrEmpty(text))
                    return string.Empty;
                // 移除所有非法 XML 字符（控制字符）
                string safeText = Regex.Replace(text, @"[\x00-\x08\x0B\x0C\x0E-\x1F\x7F]", string.Empty);
                // 清理換行符
                return safeText.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ");
            }
            // ------------------------------------

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("變更單漏洞報表");

                // 標題列
                var currentRow = 1;

                // 設置標題
                worksheet.Cells[currentRow, 1].Value = "ID";
                worksheet.Cells[currentRow, 2].Value = "標題 / 系統名稱";
                worksheet.Cells[currentRow, 3].Value = "狀態";
                worksheet.Cells[currentRow, 4].Value = "嚴重程度";
                worksheet.Cells[currentRow, 5].Value = "發現日期";
                worksheet.Cells[currentRow, 6].Value = "指派對象";
                worksheet.Cells[currentRow, 7].Value = "描述";

                // 標題格式
                using (var range = worksheet.Cells[currentRow, 1, currentRow, 7])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                }

                // 內容列
                foreach (var v in vulnerabilities)
                {
                    try
                    {
                        currentRow++;

                        // 寫入數據
                        worksheet.Cells[currentRow, 1].Value = v.Id;
                        worksheet.Cells[currentRow, 2].Value = SanitizeText(v.Title);
                        worksheet.Cells[currentRow, 3].Value = SanitizeText(v.Status); // 強化清洗
                        worksheet.Cells[currentRow, 4].Value = SanitizeText(v.Severity);

                        // 修正點 1: 直接寫入 DateTime，並讓 Excel 處理格式
                        worksheet.Cells[currentRow, 5].Value = v.FoundDate;
                        worksheet.Cells[currentRow, 5].Style.Numberformat.Format = "yyyy/MM/dd"; // 設置格式

                        worksheet.Cells[currentRow, 6].Value = SanitizeText(v.AssignedTo);
                        worksheet.Cells[currentRow, 7].Value = SanitizeText(v.Description);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Error exporting vulnerability ID {v.Id}: {ex.Message}");
                        currentRow--;
                        continue;
                    }
                }

                // 自動調整欄寬
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();


                // 輸出檔案
                var fileBytes = package.GetAsByteArray();
                var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                var fileName = $"漏洞清單_{DateTime.Now:yyyyMMddHHmmss}.xlsx";

                return File(fileBytes, contentType, fileName);
            }
        }
    }
}