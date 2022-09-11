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
using X.PagedList;

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
        public IActionResult Tasks_page(string searchString, string sortOrder, string currentFilter, int? page)
        {
            if (HttpContext.Session.GetString("Id") == null)
            {
                return RedirectToAction("Login_page", "Login");
            }
            else
            {
                ViewBag.CurrentSort = sortOrder;
                if (searchString != null)
                {
                    page = 1;
                }
                else
                {
                    searchString = currentFilter;
                }

                ViewBag.CurrentFilter = searchString;

                var tasks = from t in _context.Tasks select t;
                // which column to sort 
                ViewBag.PartnerSortParm = sortOrder == "Partner" ? "Partner_desc" : "Partner";
                ViewBag.DescriptionSortParm = sortOrder == "Description" ? "Description_desc" : "Description";
                ViewBag.Place_of_receiptSortParm = sortOrder == "Place_of_receipt" ? "Place_of_receipt_desc" : "Place_of_receipt";
                ViewBag.Time_of_receiptSortParm = sortOrder == "Time_of_receipt" ? "Time_of_receipt_desc" : "Time_of_receipt";
                ViewBag.Place_of_deliverySortParm = sortOrder == "Place_of_delivery" ? "Place_of_delivery_desc" : "Place_of_delivery";
                ViewBag.Time_of_deliverySortParm = sortOrder == "Time_of_delivery" ? "Time_of_delivery_desc" : "Time_of_delivery";
                ViewBag.Other_stopsSortParm = sortOrder == "Other_stops" ? "Other_stops_desc" : "Other_stops";
                ViewBag.Id_cargoSortParm = sortOrder == "Id_cargo" ? "Id_cargo_desc" : "Id_cargo";
                ViewBag.Storage_timeSortParm = sortOrder == "Storage_time" ? "Storage_time_desc" : "Storage_time";
                ViewBag.CompletedSortParm = sortOrder == "Completed" ? "Completed_desc" : "Completed";
                ViewBag.Completion_timeSortParm = sortOrder == "Completion_time" ? "Completion_time_desc" : "Completion_time";
                ViewBag.Time_of_delaySortParm = sortOrder == "Time_of_delay" ? "Time_of_delay_desc" : "Time_of_delay";
                ViewBag.PaymentSortParm = sortOrder == "Payment" ? "Payment_desc" : "Payment";
                ViewBag.Final_PaymentSortParm = sortOrder == "Final_Payment" ? "Final_Payment_desc" : "Final_Payment";
                ViewBag.PenaltySortParm = sortOrder == "Penalty" ? "Penalty_desc" : "Penalty";
                ViewBag.DateSortParm = String.IsNullOrEmpty(sortOrder) ? "Date_desc" : "";
                //search
                if (!String.IsNullOrEmpty(searchString))
                {
                    ViewBag.searchPlaceHolder = searchString;
                    tasks = tasks.Where(s => s.Partner!.Contains(searchString) || s.Place_of_receipt!.Contains(searchString) || s.Place_of_delivery!.Contains(searchString) || s.Time_of_delivery.ToString()!.Contains(searchString) || (s.Id_cargo!.Contains(searchString) || s.Storage_time!.Contains(searchString) || s.Completion_time.ToString()!.Contains(searchString) || s.Payment!.Contains(searchString) || s.Final_Payment!.Contains(searchString) || s.Penalty!.Contains(searchString) || s.Date.ToString()!.Contains(searchString)));
                }
                else
                {
                    ViewBag.searchPlaceHolder = "Keresés...";
                }
                // sort order
                switch (sortOrder)
                {
                    case "Partner_desc":
                        tasks = tasks.OrderByDescending(s => s.Partner);
                        break;
                    case "Partner":
                        tasks = tasks.OrderBy(s => s.Partner);
                        break;
                    case "Description_desc":
                        tasks = tasks.OrderByDescending(s => s.Description);
                        break;
                    case "Description":
                        tasks = tasks.OrderBy(s => s.Description);
                        break;
                    case "Place_of_receipt_desc":
                        tasks = tasks.OrderByDescending(s => s.Place_of_receipt);
                        break;
                    case "Place_of_receipt":
                        tasks = tasks.OrderBy(s => s.Place_of_receipt);
                        break;
                    case "Time_of_receipt_desc":
                        tasks = tasks.OrderByDescending(s => s.Time_of_receipt);
                        break;
                    case "Time_of_receipt":
                        tasks = tasks.OrderBy(s => s.Time_of_receipt);
                        break;
                    case "Place_of_delivery_desc":
                        tasks = tasks.OrderByDescending(s => s.Place_of_delivery);
                        break;
                    case "Place_of_delivery":
                        tasks = tasks.OrderBy(s => s.Place_of_delivery);
                        break;
                    case "Time_of_delivery_desc":
                        tasks = tasks.OrderByDescending(s => s.Time_of_delivery);
                        break;
                    case "Time_of_delivery":
                        tasks = tasks.OrderBy(s => s.Time_of_delivery);
                        break;
                    case "Other_stops_desc":
                        tasks = tasks.OrderByDescending(s => s.Other_stops);
                        break;
                    case "Other_stops":
                        tasks = tasks.OrderBy(s => s.Other_stops);
                        break;
                    case "Id_cargo_desc":
                        tasks = tasks.OrderByDescending(s => s.Id_cargo);
                        break;
                    case "Id_cargo":
                        tasks = tasks.OrderBy(s => s.Id_cargo);
                        break;
                    case "Storage_time_desc":
                        tasks = tasks.OrderByDescending(s => s.Storage_time);
                        break;
                    case "Storage_time":
                        tasks = tasks.OrderBy(s => s.Storage_time);
                        break;
                    case "Completed_desc":
                        tasks = tasks.OrderByDescending(s => s.Completed);
                        break;
                    case "Completed":
                        tasks = tasks.OrderBy(s => s.Completed);
                        break;
                    case "Completion_time_desc":
                        tasks = tasks.OrderByDescending(s => s.Completion_time);
                        break;
                    case "Completion_time":
                        tasks = tasks.OrderBy(s => s.Completion_time);
                        break;
                    case "Time_of_delay_desc":
                        tasks = tasks.OrderByDescending(s => s.Time_of_delay);
                        break;
                    case "Time_of_delay":
                        tasks = tasks.OrderBy(s => s.Time_of_delay);
                        break;
                    case "Payment_desc":
                        tasks = tasks.OrderByDescending(s => s.Payment);
                        break;
                    case "Payment":
                        tasks = tasks.OrderBy(s => s.Payment);
                        break;
                    case "Final_Payment_desc":
                        tasks = tasks.OrderByDescending(s => s.Final_Payment);
                        break;
                    case "Final_Payment":
                        tasks = tasks.OrderBy(s => s.Final_Payment);
                        break;
                    case "Penalty_desc":
                        tasks = tasks.OrderByDescending(s => s.Penalty);
                        break;
                    case "Penalty":
                        tasks = tasks.OrderBy(s => s.Penalty);
                        break;
                    case "Date_desc":
                        tasks = tasks.OrderByDescending(s => s.Date);
                        break;
                    default:
                        tasks = tasks.OrderBy(s => s.Date);
                        break;
                }

                int pageSize = 10; //Show x rows every time
                int pageNumber = (page ?? 1);
                return View(tasks.ToPagedList(pageNumber, pageSize));
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
        public async Task<IActionResult> Create([Bind("Id,User_id,Partner,Description,Place_of_receipt,Time_of_receipt,Place_of_delivery,Time_of_delivery,Other_stops,Id_cargo,Storage_time,Completed,Completion_time,Time_of_delay,Payment,Final_Payment,Penalty,Date")] Tasks tasks)
        {
            if (ModelState.IsValid)
            {
                tasks.User_id= (int)HttpContext.Session.GetInt32("Id");
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
                    tasks.User_id = (int)HttpContext.Session.GetInt32("Id");
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


        //closedXML needed !!!
        public async Task<IActionResult> Excel()
        {
            var tasks = from u in _context.Tasks select u;
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Tasks");
                var currentRow = 1;
                worksheet.Cell(currentRow, 1).Value = "Id";
                worksheet.Cell(currentRow, 1).Style.Font.SetBold();
                worksheet.Cell(currentRow, 2).Value = "User_id";
                worksheet.Cell(currentRow, 2).Style.Font.SetBold();
                worksheet.Cell(currentRow, 3).Value = "Partner";
                worksheet.Cell(currentRow, 3).Style.Font.SetBold();
                worksheet.Cell(currentRow, 4).Value = "Leírás";
                worksheet.Cell(currentRow, 4).Style.Font.SetBold();
                worksheet.Cell(currentRow, 5).Value = "Átvétel helye";
                worksheet.Cell(currentRow, 5).Style.Font.SetBold();
                worksheet.Cell(currentRow, 6).Value = "Átvétel ideje";
                worksheet.Cell(currentRow, 6).Style.Font.SetBold();
                worksheet.Cell(currentRow, 7).Value = "Leadás helye";
                worksheet.Cell(currentRow, 7).Style.Font.SetBold();
                worksheet.Cell(currentRow, 8).Value = "Leadás ideje";
                worksheet.Cell(currentRow, 8).Style.Font.SetBold();
                worksheet.Cell(currentRow, 9).Value = "Egyéb megállóhelyek";
                worksheet.Cell(currentRow, 9).Style.Font.SetBold();
                worksheet.Cell(currentRow, 10).Value = "Rakomány ID";
                worksheet.Cell(currentRow, 10).Style.Font.SetBold();
                worksheet.Cell(currentRow, 11).Value = "Raktározás ideje";
                worksheet.Cell(currentRow, 11).Style.Font.SetBold();
                worksheet.Cell(currentRow, 12).Value = "Teljesítve";
                worksheet.Cell(currentRow, 12).Style.Font.SetBold();
                worksheet.Cell(currentRow, 13).Value = "Teljesítés ideje";
                worksheet.Cell(currentRow, 13).Style.Font.SetBold();
                worksheet.Cell(currentRow, 14).Value = "Késés";
                worksheet.Cell(currentRow, 14).Style.Font.SetBold();
                worksheet.Cell(currentRow, 15).Value = "Igért összeg";
                worksheet.Cell(currentRow, 15).Style.Font.SetBold();
                worksheet.Cell(currentRow, 16).Value = "Végleges összeg";
                worksheet.Cell(currentRow, 16).Style.Font.SetBold();
                worksheet.Cell(currentRow, 17).Value = "Büntetés összege";
                worksheet.Cell(currentRow, 17).Style.Font.SetBold();
                worksheet.Cell(currentRow, 18).Value = "Dátum";
                worksheet.Cell(currentRow, 18).Style.Font.SetBold();
                foreach (var task in tasks)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = task.Id;
                    worksheet.Cell(currentRow, 2).Value = task.User_id;
                    worksheet.Cell(currentRow, 3).Value = task.Partner;
                    worksheet.Cell(currentRow, 4).Value = task.Description;
                    worksheet.Cell(currentRow, 5).Value = task.Place_of_receipt;
                    worksheet.Cell(currentRow, 6).Value = task.Time_of_receipt;
                    worksheet.Cell(currentRow, 7).Value = task.Place_of_delivery;
                    worksheet.Cell(currentRow, 8).Value = task.Time_of_delivery;
                    worksheet.Cell(currentRow, 9).Value = task.Other_stops;
                    worksheet.Cell(currentRow, 10).Value = task.Id_cargo;
                    worksheet.Cell(currentRow, 11).Value = task.Storage_time;
                    worksheet.Cell(currentRow, 12).Value = task.Completed;
                    worksheet.Cell(currentRow, 13).Value = task.Completion_time;
                    worksheet.Cell(currentRow, 14).Value = task.Time_of_delay;
                    worksheet.Cell(currentRow, 15).Value = task.Payment;
                    worksheet.Cell(currentRow, 16).Value = task.Final_Payment;
                    worksheet.Cell(currentRow, 17).Value = task.Penalty;
                    worksheet.Cell(currentRow, 18).Value = task.Date;
                }

                await using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    Random rnd = new Random();
                    int random = rnd.Next(1000000, 9999999);
                    return File(
                        content,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                       "Tasks" + random + "_" + DateTime.Now.ToString("yyyy-MM-dd") + ".xlsx");
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
                Random rnd = new Random();
                int random = rnd.Next(1000000, 9999999);
                string filename = "Tasks" + random + "_" + DateTime.Now.ToString("dd-MM-yyyy");
                string filepath = "/" + filename + ".pdf";

                Document document = new Document(PageSize.A4, 5f, 5f, 10f, 10f);
                FileStream fs = new FileStream(filepath, FileMode.Create);
                PdfWriter writer = PdfWriter.GetInstance(document, fs);
                document.Open();
               
                Font font1 = FontFactory.GetFont(FontFactory.TIMES_BOLD, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 12);
                Font font2 = FontFactory.GetFont(FontFactory.TIMES_ROMAN, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 10);

                Type type = typeof(Tasks);
                var column_number = (type.GetProperties().Length)/2;
                var columnDefinitionSize = new float[column_number];
                for (int i = 0; i < column_number; i++) columnDefinitionSize[i] = 1F;

                PdfPTable table,table2;
                PdfPCell cell;

                table = new PdfPTable(columnDefinitionSize)
                {
                    WidthPercentage = 80
                };
                table2 = new PdfPTable(columnDefinitionSize)
                {
                    WidthPercentage = 80
                };
                cell = new PdfPCell
                {
                    BackgroundColor = new BaseColor(0xC0, 0xC0, 0xC0),
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_CENTER
                };

                var title = new Paragraph( 15,"Megbízások");
                title.Alignment = Element.ALIGN_CENTER;
                

                document.Add(title);
                document.Add( new Paragraph("\n"));

                table.AddCell(new PdfPCell(new Phrase("Id", font1)) {
                    HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE 
                });
                table.AddCell(new PdfPCell(new Phrase("Partner", font1))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                });
                table.AddCell(new PdfPCell(new Phrase("User id", font1))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                });
                table.AddCell(new PdfPCell(new Phrase("Leírás", font1))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                });
                table.AddCell(new PdfPCell(new Phrase("Átvétel helye", font1))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                });
                table.AddCell(new PdfPCell(new Phrase("Átvétel ideje", font1))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                });
                table.AddCell(new PdfPCell(new Phrase("Leadás helye", font1))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                });
                table.AddCell(new PdfPCell(new Phrase("Leadás ideje", font1))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                });
                table.AddCell(new PdfPCell(new Phrase("Egyéb megálló", font1))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                });


                foreach (Tasks task in tasks)
                {
                    var s="";                  
                    if (!string.IsNullOrEmpty(task.Id.ToString())){ s = task.Id.ToString(); }
                    else { s = "-"; }    
                    table.AddCell(new PdfPCell(new Phrase(s.ToString(), font2))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    });
                    if (!string.IsNullOrEmpty(task.User_id.ToString())){ s = task.User_id.ToString(); }
                    else { s = "-"; }    
                    table.AddCell(new PdfPCell(new Phrase(s.ToString(), font2))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    });
                    if (!string.IsNullOrEmpty(task.Partner.ToString())){ s = task.Partner.ToString(); }
                    else { s = "-"; }    
                    table.AddCell(new PdfPCell(new Phrase(s.ToString(), font2))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    });
                    if (!string.IsNullOrEmpty(task.Description.ToString())) { s = task.Description.ToString(); }
                    else { s = "-"; }
                    table.AddCell(new PdfPCell(new Phrase(s.ToString(), font2))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    });
                    if (!string.IsNullOrEmpty(task.Place_of_receipt.ToString())) { s = task.Place_of_receipt.ToString(); }
                    else { s = "-"; }
                    table.AddCell(new PdfPCell(new Phrase(s.ToString(), font2))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    });
                    if (!string.IsNullOrEmpty(task.Time_of_receipt.ToString())) { s = task.Time_of_receipt.ToString(); }
                    else { s = "-"; }
                    table.AddCell(new PdfPCell(new Phrase(s.ToString(), font2))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    });
                    if (!string.IsNullOrEmpty(task.Place_of_delivery.ToString())) { s = task.Place_of_delivery.ToString(); }
                    else { s = "-"; }
                    table.AddCell(new PdfPCell(new Phrase(s.ToString(), font2))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    });
                    if (!string.IsNullOrEmpty(task.Time_of_delivery.ToString())) { s = task.Time_of_delivery.ToString(); }
                    else { s = "-"; }
                    table.AddCell(new PdfPCell(new Phrase(s.ToString(), font2))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    });
                    if (!string.IsNullOrEmpty(task.Other_stops)) { s = task.Other_stops.ToString(); }
                    else { s = "-"; }
                    table.AddCell(new PdfPCell(new Phrase(s.ToString(), font2))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    });
                    pdfRowIndex++;
                }

                document.Add(table);
                table=null;
                document.Add(new Paragraph("\n"));

                table2.AddCell(new PdfPCell(new Phrase("Rakomány Id", font1))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                });
                table2.AddCell(new PdfPCell(new Phrase("Raktározás ideje", font1))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                });
                table2.AddCell(new PdfPCell(new Phrase("Teljesítve", font1))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                });
                table2.AddCell(new PdfPCell(new Phrase("Teljesítés ideje", font1))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                });
                table2.AddCell(new PdfPCell(new Phrase("Késés", font1))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                });
                table2.AddCell(new PdfPCell(new Phrase("Igért összeg", font1))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                });
                table2.AddCell(new PdfPCell(new Phrase("Végleges összeg", font1))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                });
                table2.AddCell(new PdfPCell(new Phrase("Büntetés összege", font1))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                });
                table2.AddCell(new PdfPCell(new Phrase("Dátum", font1))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                });
                table2.HeaderRows = 1;


                foreach (Tasks task in tasks)
                {
                    var s = "";
                    if (!string.IsNullOrEmpty(task.Id_cargo)) { s = task.Id_cargo.ToString(); }
                    else { s = "-"; }
                    table2.AddCell(new PdfPCell(new Phrase(s.ToString(), font2))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    });
                    if (!string.IsNullOrEmpty(task.Storage_time.ToString())) { s = task.Storage_time.ToString(); }
                    else { s = "-"; }
                    table2.AddCell(new PdfPCell(new Phrase(s.ToString(), font2))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    });
                    if (!string.IsNullOrEmpty(task.Completed.ToString())) { s = task.Completed.ToString(); }
                    else { s = "-"; }
                    table2.AddCell(new PdfPCell(new Phrase(s.ToString(), font2))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    });
                    if (!string.IsNullOrEmpty(task.Completion_time.ToString())) { s = task.Completion_time.ToString(); }
                    else { s = "-"; }
                    table2.AddCell(new PdfPCell(new Phrase(s.ToString(), font2))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    });
                    if (!string.IsNullOrEmpty(task.Time_of_delay)) { s = task.Time_of_delay.ToString(); }
                    else { s = "-"; }
                    table2.AddCell(new PdfPCell(new Phrase(s.ToString(), font2))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    });
                    if (!string.IsNullOrEmpty(task.Payment)) { s = task.Payment.ToString(); }
                    else { s = "-"; }
                    table2.AddCell(new PdfPCell(new Phrase(s.ToString(), font2))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    });
                    if (!string.IsNullOrEmpty(task.Final_Payment)) { s = task.Final_Payment.ToString(); }
                    else { s = "-"; }
                    table2.AddCell(new PdfPCell(new Phrase(s.ToString(), font2))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    });
                    if (!string.IsNullOrEmpty(task.Penalty)) { s = task.Penalty.ToString(); }
                    else { s = "-"; }
                    table2.AddCell(new PdfPCell(new Phrase(s.ToString(), font2))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    });
                    if (!string.IsNullOrEmpty(task.Date.ToString())) { s = task.Date.ToString(); }
                    else { s = "-"; }
                    table2.AddCell(new PdfPCell(new Phrase(s.ToString(), font2))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    });

                    pdfRowIndex++;
                }
                document.Add(table2);
              
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
