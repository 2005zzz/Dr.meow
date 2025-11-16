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
           
                Vulnerability = await _context.Vulnerability.ToListAsync();
                //Vulnerability = new List<Vulnerability>(); // 暫時給空清單

            }
        }

      
        public async Task<IActionResult> OnGetExportToExcel()
        {
           
            var vulnerabilities = await _context.Vulnerability.ToListAsync();

            using (var workbook = new XLWorkbook())
            {
                
                var worksheet = workbook.Worksheets.Add("�|�}�l�ܳ��i");


                var currentRow = 1;
                worksheet.Cell(currentRow, 1).Value = "ID";
                worksheet.Cell(currentRow, 2).Value = "�t��/�u�����O";
                worksheet.Cell(currentRow, 3).Value = "���A/�ܧ�����";
                worksheet.Cell(currentRow, 4).Value = "�Y����/���I";
             
                worksheet.Cell(currentRow, 6).Value = "�渹/������H";
                worksheet.Cell(currentRow, 7).Value = "���e/�y�z";
              
              
                worksheet.Range(currentRow, 1, currentRow, 9).Style.Font.Bold = true;
                worksheet.Range(currentRow, 1, currentRow, 9).Style.Fill.BackgroundColor = XLColor.LightGray;

           
                foreach (var vulnerability in vulnerabilities)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = vulnerability.Id;
                    worksheet.Cell(currentRow, 2).Value = vulnerability.Title;
                    worksheet.Cell(currentRow, 3).Value = vulnerability.Status;
                    worksheet.Cell(currentRow, 4).Value = vulnerability.Severity;
                    worksheet.Cell(currentRow, 5).Value = vulnerability.FoundDate.ToString("yyyy/MM/dd"); // �榡�Ƥ��
                    worksheet.Cell(currentRow, 6).Value = vulnerability.AssignedTo;
                    worksheet.Cell(currentRow, 7).Value = vulnerability.Description;
                    // worksheet.Cell(currentRow, 8).Value = vulnerability.TestPlan;
                    // worksheet.Cell(currentRow, 9).Value = vulnerability.RecoveryPlan;
                }

             
                worksheet.Columns().AdjustToContents();

               
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    var fileName = $"�|��l�ܳ��i_{DateTime.Now:yyyyMMddHHmmss}.xlsx";

                    return File(content, contentType, fileName);
                }
            }
        }
    }
}