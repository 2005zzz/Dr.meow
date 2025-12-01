using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Dr.meow.Data;
using Dr.meow.Models;
using System.Diagnostics;
// *** 郵件相關的 using 已被刪除 ***

namespace Dr.meow.Pages.Vulnerabilities
{
    public class CreateModel : PageModel
    {
        private readonly DrMeowDbContext _context;

        // 建構子
        public CreateModel(DrMeowDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Vulnerability Vulnerability { get; set; } = default!;

        // === [ 1. 純 C# 郵件發送方法 - 已移除 ] ===
        // 相關方法已移除，以符合您的要求。

        // === [ 2. OnPostAsync - 核心邏輯 ] ===
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                // 記錄驗證失敗的詳細錯誤
                foreach (var state in ModelState)
                {
                    if (state.Value?.Errors.Any() == true)
                    {
                        System.Diagnostics.Debug.WriteLine($"[Validation Failure] - 欄位 '{state.Key}' 錯誤: {state.Value.Errors.First().ErrorMessage}");
                    }
                }
                return Page();
            }

            // 1. 儲存變更申請單至資料庫
            try
            {
                _context.Vulnerability.Add(Vulnerability);
                await _context.SaveChangesAsync();
                System.Diagnostics.Debug.WriteLine("[DB Success] 資料庫儲存成功。ID: " + Vulnerability.Id);
            }
            catch (Exception dbEx)
            {
                System.Diagnostics.Debug.WriteLine($"[DB Failure] 儲存資料庫時發生嚴重錯誤: {dbEx.Message}");
                ModelState.AddModelError(string.Empty, "資料庫儲存失敗，請檢查欄位是否超出長度或資料庫是否已更新 (Migration)。詳細錯誤已記錄在 Output 視窗。");
                return Page();
            }

            // 2. 郵件發送步驟已移除。

            // 3. 導向列表頁面
            return RedirectToPage("Index");
        }
    }
}