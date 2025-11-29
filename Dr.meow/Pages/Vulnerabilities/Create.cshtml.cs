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
// *** 新增：郵件相關的命名空間 ***
using System.Net.Mail;
using System.Net;

namespace Dr.meow.Pages.Vulnerabilities
{
    // *** Power Automate 相關的類別 NewVulnerabilityPayload 已被移除 ***

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

        // === [ 1. 純 C# 郵件發送方法 ] ===
        private void SendEmailNotification(Vulnerability newVulnerability)
        {
            // 檢查收件人郵箱是否有效 (RequesterEmail 來自表單提交)
            if (string.IsNullOrWhiteSpace(newVulnerability.RequesterEmail))
            {
                System.Diagnostics.Debug.WriteLine("[Email Skip] 請求者信箱為空，跳過郵件發送。");
                return;
            }

            try
            {
                // ⚠️ 必填資訊：請【替換】成您自己的 SMTP 伺服器資訊
                // 如果使用 Gmail：Host=smtp.gmail.com, Port=587, 密碼請使用 App Password
                const string SmtpHost = "smtp.gmail.com"; // 例如: "smtp.gmail.com"
                const int SmtpPort = 587; // 標準連線埠
                const string SmtpUsername = "10932041@mail.hcsh.tp.edu.tw"; // <--- 【請替換】
                const string SmtpPassword = "wrbr mjgq dlbh qflu"; // <--- 【請替換】

                // 建立 MailMessage
                var mail = new MailMessage();
                mail.From = new MailAddress(SmtpUsername, "Dr. Meow 系統通知"); // 寄件人
                mail.To.Add(newVulnerability.RequesterEmail); // 收件人：填表人信箱
                mail.Subject = $"✅ 漏洞回報單已建立：{newVulnerability.Title} (ID: {newVulnerability.Id})";
                mail.IsBodyHtml = true;

                // 郵件內容
                mail.Body = $@"
                    <h2>您的漏洞回報單已成功建立</h2>
                    <p>感謝您提交此回報單。詳細資訊如下：</p>
                    <ul>
                        <li><strong>回報單編號:</strong> {newVulnerability.Id}</li>
                        <li><strong>標題:</strong> {newVulnerability.Title}</li>
                        <li><strong>狀態:</strong> {newVulnerability.Status}</li>
                        <li><strong>嚴重性:</strong> {newVulnerability.Severity}</li>
                        <li><strong>指派給:</strong> {newVulnerability.AssignedTo ?? "未指派"}</li>
                        <li><strong>詳細描述:</strong> {newVulnerability.Description}</li>
                    </ul>
                    <p>系統將會盡快處理您的回報。</p>
                ";

                // 設定 SmtpClient
                using (var smtpClient = new SmtpClient(SmtpHost, SmtpPort))
                {
                    smtpClient.EnableSsl = true; // 啟用 SSL/TLS
                    smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtpClient.UseDefaultCredentials = false;
                    smtpClient.Credentials = new NetworkCredential(SmtpUsername, SmtpPassword);

                    smtpClient.Send(mail); // 發送郵件
                    System.Diagnostics.Debug.WriteLine($"[Email Success] 郵件已發送至 {newVulnerability.RequesterEmail}");
                }
            }
            catch (Exception ex)
            {
                // 郵件發送失敗不會阻擋資料庫儲存，僅記錄錯誤
                System.Diagnostics.Debug.WriteLine($"[Email Failure] 郵件發送失敗，收件人: {newVulnerability.RequesterEmail}。錯誤: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"[Email Failure] 內層錯誤: {ex.InnerException?.Message}");
            }
        }

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

            // 2. 呼叫純 C# 郵件發送 (不使用 await，讓它在背景執行)
            // Task.Run 確保不會阻塞主線程，不影響使用者跳轉頁面
            await Task.Run(() => SendEmailNotification(Vulnerability));

            // 3. 導向列表頁面
            return RedirectToPage("./Index");
        }
    }
}