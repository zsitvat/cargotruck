using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using App.Data;
using App.Models;
using Microsoft.AspNetCore.Http;
using System.Data;
using System.IO;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.InkML;
using System.Text;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Web.WebPages;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.AspNetCore.Hosting.Internal;
using Grpc.Core;
using Azure;
using static System.Net.WebRequestMethods;
using System.Security.AccessControl;
using System.Diagnostics;

namespace App.Controllers
{
    public class TasksController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TasksController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Tasks
        public async Task<IActionResult> Tasks_page(string searchString)
        {
            if (HttpContext.Session.GetString("Id") == null)
            {
                return RedirectToAction("Login_page", "Login");
            }
            else
            {
                var tasks = from u in _context.Tasks select u;
                if (!String.IsNullOrEmpty(searchString))
                {
                    ViewBag.search = searchString;
                    tasks = tasks.Where(s => s.Partner!.Contains(searchString) || s.Place_of_receipt!.Contains(searchString) || s.Place_of_delivery!.Contains(searchString) || s.Time_of_delivery.ToString()!.Contains(searchString) || (s.Id_cargo!.Contains(searchString) || s.Storage_time!.Contains(searchString) || s.Completion_time.ToString()!.Contains(searchString) || s.Payment!.Contains(searchString) || s.Final_Payment!.Contains(searchString) || s.Penalty!.Contains(searchString) || s.Date.ToString()!.Contains(searchString)));
                }
                else
                {
                    ViewBag.search = "Keresés...";
                }
                return View(await tasks.ToListAsync());
            }
        }

        // GET: Tasks/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Tasks/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Partner,Description,Place_of_receipt,Time_of_receipt,Place_of_delivery,Time_of_delivery,Other_stops,Id_cargo,Storage_time,Completed,Completion_time,Time_of_delay,Payment,Final_Payment,Penalty,Date")] Tasks tasks)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tasks);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Tasks_page));
            }
            return View(tasks);
        }

        // GET: Tasks/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tasks = await _context.Tasks.FindAsync(id);
            if (tasks == null)
            {
                return NotFound();
            }
            return View(tasks);
        }

        // POST: Tasks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,Partner,Description,Place_of_receipt,Time_of_receipt,Place_of_delivery,Time_of_delivery,Other_stops,Id_cargo,Storage_time,Completed,Completion_time,Time_of_delay,Payment,Final_Payment,Penalty,Date")] Tasks tasks)
        {
            if (id != tasks.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tasks);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TasksExists(tasks.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Tasks_page));
            }
            return View(tasks);
        }

        // GET: Tasks/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tasks = await _context.Tasks
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tasks == null)
            {
                return NotFound();
            }

            return View(tasks);
        }

        // POST: Tasks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var tasks = await _context.Tasks.FindAsync(id);
            _context.Tasks.Remove(tasks);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Tasks_page));
        }

        private bool TasksExists(long id)
        {
            return _context.Tasks.Any(e => e.Id == id);
        }


        //closedXML needed !!!!!
        public async Task<IActionResult> Excel()
        {
            var tasks = from u in _context.Tasks select u;
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Tasks");
                var currentRow = 1;
                worksheet.Cell(currentRow, 1).Value = "Id";
                worksheet.Cell(currentRow, 2).Value = "Partner";
                worksheet.Cell(currentRow, 3).Value = "Leírás";
                worksheet.Cell(currentRow, 4).Value = "Átvétel helye";
                worksheet.Cell(currentRow, 5).Value = "Átvétel ideje";
                worksheet.Cell(currentRow, 6).Value = "Leadás helye";
                worksheet.Cell(currentRow, 7).Value = "Leadás ideje";
                worksheet.Cell(currentRow, 8).Value = "Egyéb megállóhelyek";
                worksheet.Cell(currentRow, 9).Value = "Rakomány ID";
                worksheet.Cell(currentRow, 10).Value = "Raktározás ideje";
                worksheet.Cell(currentRow, 11).Value = "Teljesítve";
                worksheet.Cell(currentRow, 12).Value = "Teljesítés ideje";
                worksheet.Cell(currentRow, 13).Value = "Késés";
                worksheet.Cell(currentRow, 14).Value = "Igért összeg";
                worksheet.Cell(currentRow, 15).Value = "Végleges összeg";
                worksheet.Cell(currentRow, 16).Value = "Büntetés összege";
                worksheet.Cell(currentRow, 17).Value = "Dátum";
                foreach (var task in tasks)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = task.Id;
                    worksheet.Cell(currentRow, 2).Value = task.Partner;
                    worksheet.Cell(currentRow, 3).Value = task.Description;
                    worksheet.Cell(currentRow, 4).Value = task.Place_of_receipt;
                    worksheet.Cell(currentRow, 5).Value = task.Time_of_receipt;
                    worksheet.Cell(currentRow, 6).Value = task.Place_of_delivery;
                    worksheet.Cell(currentRow, 7).Value = task.Time_of_delivery;
                    worksheet.Cell(currentRow, 8).Value = task.Other_stops;
                    worksheet.Cell(currentRow, 9).Value = task.Id_cargo;
                    worksheet.Cell(currentRow, 10).Value = task.Storage_time;
                    worksheet.Cell(currentRow, 11).Value = task.Completed;
                    worksheet.Cell(currentRow, 12).Value = task.Completion_time;
                    worksheet.Cell(currentRow, 13).Value = task.Time_of_delay;
                    worksheet.Cell(currentRow, 14).Value = task.Payment;
                    worksheet.Cell(currentRow, 15).Value = task.Final_Payment;
                    worksheet.Cell(currentRow, 16).Value = task.Penalty;
                    worksheet.Cell(currentRow, 17).Value = task.Date;
                }

                await using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(
                        content,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                       "Tasks" + DateTime.Now.ToString("yyyy/MM/dd") + ".xlsx");
                }
            }
        }

        //iTextSharp needed !!!
        public void PDF()
        {
            var tasks = from u in _context.Tasks select u;

            if (tasks.Count() > 0)
            {
                int pdfRowIndex = 1;

                string filename = "Tasks-" + DateTime.Now.ToString("dd-MM-yyyy");
                string filepath = "/" + filename + ".pdf";

                Document document = new Document(PageSize.A4, 5f, 5f, 10f, 10f);
                FileStream fs = new FileStream(filepath, FileMode.Create);
                PdfWriter writer = PdfWriter.GetInstance(document, fs);
                document.Open();

                Font font1 = FontFactory.GetFont(FontFactory.COURIER_BOLD, 8);
                Font font2 = FontFactory.GetFont(FontFactory.COURIER, 6);

                float[] columnDefinitionSize = { 1F, 1F, 1F, 1F };
                PdfPTable table;
                PdfPCell cell;

                table = new PdfPTable(columnDefinitionSize)
                {
                    WidthPercentage = 100
                };

                cell = new PdfPCell
                {
                    BackgroundColor = new BaseColor(0xC0, 0xC0, 0xC0)
                };

                table.AddCell(new Phrase("Id", font1));
                table.AddCell(new Phrase("Partner", font1));
                table.AddCell(new Phrase("Leírás", font1));
                table.AddCell(new Phrase("Átvétel helye", font1));
                table.AddCell(new Phrase("Átvétel ideje", font1));
                table.AddCell(new Phrase("Leadás helye", font1));
                table.AddCell(new Phrase("Leadás ideje", font1));
                table.AddCell(new Phrase("Egyéb megállóhelyek", font1));
                table.AddCell(new Phrase("Rakomány ID", font1));
                table.AddCell(new Phrase("Raktározás ideje", font1));
                table.AddCell(new Phrase("Teljesítve", font1));
                table.AddCell(new Phrase("Teljesítés ideje", font1));
                table.AddCell(new Phrase("Késés", font1));
                table.AddCell(new Phrase("Igért összeg", font1));
                table.AddCell(new Phrase("Végleges összeg", font1));
                table.AddCell(new Phrase("Büntetés összege", font1)); 
                table.AddCell(new Phrase("Date", font1));
                table.HeaderRows = 1;

                foreach (Tasks task in tasks)
                {
                     if  (!task.Id.ToString().IsEmpty())
                    {
                        table.AddCell(new Phrase(task.Id.ToString(), font2));
                    }else { table.AddCell(new Phrase("-", font2)); }

                    if (!task.Partner.IsEmpty())
                    {
                        table.AddCell(new Phrase(task.Partner.ToString(), font2));
                    }else { table.AddCell(new Phrase("-", font2)); }

                    if (!task.Description.IsEmpty())
                    {
                        table.AddCell(new Phrase(task.Description.ToString(), font2));
                    }else { table.AddCell(new Phrase("-", font2)); }

                    if (!task.Place_of_receipt.IsEmpty())
                    {
                        table.AddCell(new Phrase(task.Place_of_receipt.ToString(), font2));
                    }else { table.AddCell(new Phrase("-", font2)); }

                    if (!task.Time_of_receipt.ToString().IsEmpty())
                    {
                        table.AddCell(new Phrase(task.Time_of_receipt.ToString(), font2));
                    }else { table.AddCell(new Phrase("-", font2)); }

                    if (!task.Place_of_delivery.IsEmpty())
                    {
                        table.AddCell(new Phrase(task.Place_of_delivery.ToString(), font2));

                    }else { table.AddCell(new Phrase("-", font2)); }

                    if (!task.Time_of_delivery.ToString().IsEmpty())
                    {
                        table.AddCell(new Phrase(task.Time_of_delivery.ToString(), font2));
                    }else { table.AddCell(new Phrase("-", font2)); }

                    if (!task.Other_stops.IsEmpty()) {  
                        table.AddCell(new Phrase(task.Other_stops.ToString(), font2)) ;
                    }else { table.AddCell(new Phrase("-", font2)); }

                    if (!task.Id_cargo.IsEmpty())
                    {
                        table.AddCell(new Phrase(task.Id_cargo.ToString(), font2));
                    } else { table.AddCell(new Phrase("-", font2)); }

                    if (!task.Storage_time.IsEmpty())
                    {
                        table.AddCell(new Phrase(task.Storage_time.ToString(), font2));
                    }else { table.AddCell(new Phrase("-", font2)); }

                    if (!task.Completed.ToString().IsEmpty())
                    {
                        table.AddCell(new Phrase(task.Completed.ToString(), font2));
                    }else { table.AddCell(new Phrase("-", font2)); }

                    if (!task.Completion_time.ToString().IsEmpty())
                    {
                        table.AddCell(new Phrase(task.Completion_time.ToString(), font2));
                    } else { table.AddCell(new Phrase("-", font2)); }

                    if (!task.Time_of_delay.IsEmpty())
                    {
                        table.AddCell(new Phrase(task.Time_of_delay.ToString(), font2));
                    }else { table.AddCell(new Phrase("-", font2)); }

                    if (!task.Payment.IsEmpty())
                    {
                        table.AddCell(new Phrase(task.Payment.ToString(), font2));
                    }else { table.AddCell(new Phrase("-", font2)); }

                    if (!task.Final_Payment.IsEmpty())
                    {
                        table.AddCell(new Phrase(task.Final_Payment.ToString(), font2));
                    }else { table.AddCell(new Phrase("-", font2)); }

                    if (!task.Penalty.IsEmpty())
                    {
                        table.AddCell(new Phrase(task.Penalty.ToString(), font2));
                    }else { table.AddCell(new Phrase("-", font2)); }

                    if (!task.Date.ToString().IsEmpty())
                    {
                        table.AddCell(new Phrase(task.Date.ToString(), font2));
                    }else { table.AddCell(new Phrase("-", font2)); }

                    pdfRowIndex++;
                }

                document.Add(table);
                document.Close();
                document.CloseDocument();
                document.Dispose();
                writer.Close();
                writer.Dispose();
                fs.Close();
                fs.Dispose();

                FileStream sourceFile = new FileStream(filepath, FileMode.Open);
                float fileSize = 0;
                fileSize = sourceFile.Length;
                byte[] getContent = new byte[Convert.ToInt32(Math.Truncate(fileSize))];
                sourceFile.Read(getContent, 0, Convert.ToInt32(sourceFile.Length)); 
                sourceFile.Close();
                Response.Clear();
                Response.Headers.Clear();
                Response.ContentType = "application/pdf";
                Response.Headers.Add("Content-Disposition", "attachment; filename=" + filename + ".pdf");
                Response.Headers.Add("Content-Length", getContent.Length.ToString());
                Response.Body.WriteAsync(getContent);
                Response.Body.Flush();
            }
        }

    }
}
