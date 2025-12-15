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
                // 列表頁：依 ID 倒序，最新的在最上面
                Vulnerability = await _context.Vulnerability
                    .OrderByDescending(v => v.Id)
                    .ToListAsync();
            }
        }

        // === 匯出 Excel 功能 ===
        public async Task<IActionResult> OnGetExportToExcel()
        {
            // 1. EPPlus 授權 (必須設定)
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            // 2. 撈取資料：取消 Take(10) 限制，匯出完整資料，並依日期排序
            var vulnerabilities = await _context.Vulnerability
                .OrderByDescending(v => v.FoundDate)
                .ToListAsync();

            // --- 內部清洗函式 ---
            string SanitizeText(string? text)
            {
                if (string.IsNullOrEmpty(text)) return string.Empty;
                // 移除控制字元與換行，避免 Excel 格式跑版
                string safeText = Regex.Replace(text, @"[\x00-\x08\x0B\x0C\x0E-\x1F\x7F]", string.Empty);
                return safeText.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ");
            }
            // --------------------

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("變更申請報表");

                // --- 3. 設定標題列 (對應新的 12 欄位架構) ---
                var headers = new string[]
                {
                    "ID", "單號", "系統類別", "工單類別",
                    "變更類型", "流程狀態",
                    "風險等級", "影響程度", "依賴性",
                    "預計實施日期", "預計時間",
                    "測試計畫", "回復計畫",
                    "指派對象", "變更內容描述"
                };

                for (int i = 0; i < headers.Length; i++)
                {
                    worksheet.Cells[1, i + 1].Value = headers[i];
                }

                // 標題樣式美化
                using (var range = worksheet.Cells[1, 1, 1, headers.Length])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                    range.Style.Border.Bottom.Style = ExcelBorderStyle.Thick;
                }

                // --- 4. 填入內容 ---
                var currentRow = 2;
                foreach (var v in vulnerabilities)
                {
                    try
                    {
                        int col = 1;
                        worksheet.Cells[currentRow, col++].Value = v.Id;
                        worksheet.Cells[currentRow, col++].Value = SanitizeText(v.TicketNumber); // 單號

                        // 分類
                        worksheet.Cells[currentRow, col++].Value = SanitizeText(v.SystemCategory);
                        worksheet.Cells[currentRow, col++].Value = SanitizeText(v.TicketCategory);

                        // 狀態與類型
                        worksheet.Cells[currentRow, col++].Value = SanitizeText(v.ChangeType); // 緊急/例行
                        worksheet.Cells[currentRow, col++].Value = SanitizeText(v.Status);     // Pending/Approved

                        // 風險評估
                        worksheet.Cells[currentRow, col++].Value = SanitizeText(v.Severity);
                        worksheet.Cells[currentRow, col++].Value = SanitizeText(v.ImpactLevel);
                        worksheet.Cells[currentRow, col++].Value = SanitizeText(v.Dependency);

                        // 時間 (日期格式化)
                        worksheet.Cells[currentRow, col++].Value = v.FoundDate;
                        worksheet.Cells[currentRow, col - 1].Style.Numberformat.Format = "yyyy/MM/dd";
                        worksheet.Cells[currentRow, col++].Value = SanitizeText(v.ScheduledTime);

                        // ✨ 重點：計畫檢核 (AI 自動填寫的結果)
                        worksheet.Cells[currentRow, col++].Value = SanitizeText(v.TestPlan);
                        worksheet.Cells[currentRow, col++].Value = SanitizeText(v.RecoveryPlan);

                        // 人員與描述
                        worksheet.Cells[currentRow, col++].Value = SanitizeText(v.AssignedTo);
                        worksheet.Cells[currentRow, col++].Value = SanitizeText(v.Description);

                        currentRow++;
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Export Error ID {v.Id}: {ex.Message}");
                    }
                }

                // 自動調整欄寬 (最後一欄描述除外)
                worksheet.Cells[1, 1, currentRow, 14].AutoFitColumns();
                worksheet.Column(15).Width = 60; // 描述欄位給寬一點
                worksheet.Column(15).Style.WrapText = true;

                // 產生檔案
                var fileBytes = package.GetAsByteArray();
                var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                var fileName = $"資安變更報表_{DateTime.Now:yyyyMMdd_HHmm}.xlsx";

                return File(fileBytes, contentType, fileName);
            }
        }
    }
}