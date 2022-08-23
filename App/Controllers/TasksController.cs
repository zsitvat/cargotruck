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
using Font = iTextSharp.text.Font;

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


        //closedXML needed
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
        //iTextSharp needed
        public Task<IActionResult> PDF()
        {
            var dt = from u in _context.Tasks select u;

            return null;
        }
    }
}
