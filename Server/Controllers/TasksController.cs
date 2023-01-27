using Cargotruck.Data;
using Tasks = Cargotruck.Shared.Models.Tasks;
using iTextSharp.text.pdf;
using iTextSharp.text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using ClosedXML.Excel;
using Document = iTextSharp.text.Document;
using Microsoft.Data.SqlClient;
using System.Text;
using Cargotruck.Shared.Models;
using DocumentFormat.OpenXml.Bibliography;
using Microsoft.ClearScript.JavaScript;

namespace Cargotruck.Server.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public TasksController(ApplicationDbContext context )
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Get(int page, int pageSize, string sortOrder, bool desc, string? searchString, string? filter, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate)
        {
            var t = await _context.Tasks.Where(s=>(dateFilterStartDate != null ? (s.Date >= dateFilterStartDate) : true) && (dateFilterEndDate != null ? (s.Date <= dateFilterEndDate) : true)).ToListAsync();

            if (filter == "completed")
            {
                t = t.Where(data => data.Completed).ToList();
            }
            else if (filter == "not_completed")
            {
                t = t.Where(data => data.Completed == false).ToList();
            }

            searchString = searchString?.ToLower();
            if (searchString != null && searchString != "") 
            { 
                    t = t.Where(s => (
                    s.Partner != null && s.Partner.ToLower()!.Contains(searchString))
                || (s.Description != null && s.Description.ToLower()!.Contains(searchString))
                || (s.Place_of_receipt != null && s.Place_of_receipt.ToLower()!.Contains(searchString))
                || (s.Place_of_delivery != null && s.Place_of_delivery.ToLower()!.Contains(searchString))
                || (s.Time_of_delivery.ToString()!.Contains(searchString))
                || (s.Id_cargo != null && s.Id_cargo.ToString()!.Contains(searchString))
                || (s.Storage_time != null && s.Storage_time.ToLower()!.Contains(searchString))
                || (s.Completion_time != null && s.Completion_time.ToString()!.Contains(searchString))
                || (s.Payment != null && s.Payment.ToString()!.Contains(searchString))
                || (s.Final_Payment != null && s.Final_Payment.ToString()!.Contains(searchString))
                || (s.Penalty != null && s.Penalty.ToString()!.Contains(searchString))
                || (s.Date.ToString()!.Contains(searchString))
                ).ToList(); 
            }

            
            sortOrder = sortOrder == "Partner" ? ( desc ? "Partner_desc" : "Partner" ) : (sortOrder);
            sortOrder = sortOrder == "Description " ? (desc ? "Description_desc" : "Description") : (sortOrder);
            sortOrder = sortOrder == "Place_of_receipt" ? (desc ? "Place_of_receipt_desc" : "Place_of_receipt") : (sortOrder);
            sortOrder = sortOrder == "Time_of_receipt" ? (desc ? "Time_of_receipt_desc" : "Time_of_receipt") : (sortOrder);
            sortOrder = sortOrder == "Place_of_delivery" ? (desc ? "Place_of_delivery_desc" : "Place_of_delivery") : (sortOrder);
            sortOrder = sortOrder == "Time_of_delivery" ? (desc ? "Time_of_delivery_desc" : "Time_of_delivery") : (sortOrder);
            sortOrder = sortOrder == "other_stops" ? (desc ? "other_stops_desc" : "other_stops") : (sortOrder);
            sortOrder = sortOrder == "Id_cargo" ? (desc ? "Id_cargo_desc" : "Id_cargo") : (sortOrder);
            sortOrder = sortOrder == "Storage_time" ? (desc ? "Storage_time_desc" : "Storage_time") : (sortOrder);
            sortOrder = sortOrder == "Completed" ? (desc ? "Completed_desc" : "Completed") : (sortOrder);
            sortOrder = sortOrder == "Completion_time" ? (desc ? "Completion_time_desc" : "Completion_time") : (sortOrder);
            sortOrder = sortOrder == "Time_of_delay" ? (desc ? "Time_of_delay_desc" : "Time_of_delay") : (sortOrder);
            sortOrder = sortOrder == "Payment" ? (desc ? "Payment_desc" : "Payment") : (sortOrder);
            sortOrder = sortOrder == "Final_Payment" ? (desc ? "Final_Payment_desc" : "Final_Payment") : (sortOrder);
            sortOrder = sortOrder == "Penalty" ? (desc ? "Penalty_desc" : "Penalty") : (sortOrder);
            sortOrder = sortOrder == "Date" || String.IsNullOrEmpty(sortOrder) ? (desc ? "Date_desc" : "") : (sortOrder);

            t = sortOrder switch
            {
                "Partner_desc" => t.OrderByDescending(s => s.Partner).ToList(),
                "Partner" => t.OrderBy(s => s.Partner).ToList(),
                "Description_desc" => t.OrderByDescending(s => s.Description).ToList(),
                "Description" => t.OrderBy(s => s.Description).ToList(),
                "Place_of_receipt_desc" => t.OrderByDescending(s => s.Place_of_receipt).ToList(),
                "Place_of_receipt" => t.OrderBy(s => s.Place_of_receipt).ToList(),
                "Time_of_receipt_desc" => t.OrderByDescending(s => s.Time_of_receipt).ToList(),
                "Time_of_receipt" => t.OrderBy(s => s.Time_of_receipt).ToList(),
                "Place_of_delivery_desc" => t.OrderByDescending(s => s.Place_of_delivery).ToList(),
                "Place_of_delivery" => t.OrderBy(s => s.Place_of_delivery).ToList(),
                "Time_of_delivery_desc" => t.OrderByDescending(s => s.Time_of_delivery).ToList(),
                "Time_of_delivery" => t.OrderBy(s => s.Time_of_delivery).ToList(),
                "other_stops_desc" => t.OrderByDescending(s => s.other_stops).ToList(),
                "other_stops" => t.OrderBy(s => s.other_stops).ToList(),
                "Id_cargo_desc" => t.OrderByDescending(s => s.Id_cargo).ToList(),
                "Id_cargo" => t.OrderBy(s => s.Id_cargo).ToList(),
                "Storage_time_desc" => t.OrderByDescending(s => s.Storage_time).ToList(),
                "Storage_time" => t.OrderBy(s => s.Storage_time).ToList(),
                "Completed_desc" => t.OrderByDescending(s => s.Completed).ToList(),
                "Completed" => t.OrderBy(s => s.Completed).ToList(),
                "Completion_time_desc" => t.OrderByDescending(s => s.Completion_time).ToList(),
                "Completion_time" => t.OrderBy(s => s.Completion_time).ToList(),
                "Time_of_delay_desc" => t.OrderByDescending(s => s.Time_of_delay).ToList(),
                "Time_of_delay" => t.OrderBy(s => s.Time_of_delay).ToList(),
                "Payment_desc" => t.OrderByDescending(s => s.Payment).ToList(),
                "Payment" => t.OrderBy(s => s.Payment).ToList(),
                "Final_Payment_desc" => t.OrderByDescending(s => s.Final_Payment).ToList(),
                "Final_Payment" => t.OrderBy(s => s.Final_Payment).ToList(),
                "Penalty_desc" => t.OrderByDescending(s => s.Penalty).ToList(),
                "Penalty" => t.OrderBy(s => s.Penalty).ToList(),
                "Date_desc" => t.OrderByDescending(s => s.Date).ToList(),
                _ => t.OrderBy(s => s.Date).ToList(),
            };
            t = t.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            return Ok(t);
        }

        [HttpGet]
        public async Task<IActionResult> GetTasks()
        {
            var t = await _context.Tasks.ToListAsync();
            return Ok(t);
        }

        [HttpGet]
        public async Task<IActionResult> Count(bool all)
        {
            if (all) 
            {
                return Ok(await _context.Tasks.CountAsync()); 
            }
            else
            {
                return Ok(await _context.Tasks.Where(x => x.Completed == false).CountAsync());
            }
        }

        [HttpGet]
        public async Task<IActionResult> PageCount(string? filter)
        {
            var t = await _context.Tasks.ToListAsync();
            if (filter == "completed")
            {
                t = t.Where(data => data.Completed).ToList();
            }
            else if (filter == "not_completed")
            {
                t = t.Where(data => data.Completed == false).ToList();
            }
            int PageCount = t.Count;
            return Ok(PageCount);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var t = await _context.Tasks.FirstOrDefaultAsync(a => a.Id == id);
            return Ok(t);
        }

        [HttpPost]
        public async Task<IActionResult> Post(Tasks t)
        {
            t.Final_Payment = (t.Payment != null ? t.Payment : 0) - (t.Penalty != null ? t.Penalty : 0);
            t.User_id = _context.Users.FirstOrDefault(a => a.UserName == User.Identity!.Name)?.Id;
            if (t.Completed)
            {
                t.Completion_time = DateTime.Now;
            }
            else { t.Completion_time = null; }
            _context.Add(t);
            await _context.SaveChangesAsync();

            var cargo = _context.Cargoes.FirstOrDefault(a => a.Id == t.Id_cargo);
            if (cargo!=null) { 
                cargo.Task_id = t.Id;
                _context.Entry(cargo).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            return Ok(t.Id);
        }

        [HttpPut]
        public async Task<IActionResult> Put(Tasks t)
        {
            t.Final_Payment = (t.Payment != null ? t.Payment : 0) - (t.Penalty != null ? t.Penalty : 0);
            if (t.Completed) 
            {
                t.Completion_time = DateTime.Now;
            }
            else { t.Completion_time = null; }
            _context.Entry(t).State = EntityState.Modified;

            var cargo = _context.Cargoes.FirstOrDefault(a => a.Id == t.Id_cargo);
            if (cargo != null)
            {
                cargo.Task_id = t.Id;
                _context.Entry(cargo).State = EntityState.Modified;
            }
            await _context.SaveChangesAsync();
            return NoContent();
        }


        [HttpPut]
        public async Task<IActionResult> ChangeCompletion(Tasks t)
        {
            t.Completed = !t.Completed;
            if (t.Completed)
            {
                t.Completion_time = DateTime.Now;
            }
            else { t.Completion_time = null; }
            _context.Entry(t).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var t = new Tasks { Id = id };
            _context.Remove(t);
            await _context.SaveChangesAsync();
            return NoContent();
        }


        //closedXML needed !!!
        [HttpGet]
        public string Excel(string lang)
        {
            var tasks = from t in _context.Tasks select t;
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Tasks");
            var currentRow = 1;
            worksheet.Cell(currentRow, 1).Value = "Id";
            worksheet.Cell(currentRow, 1).Style.Font.SetBold();
            worksheet.Cell(currentRow, 2).Value = lang == "hu" ? Cargotruck.Shared.Resources.Resource.User_id : "User ID";
            worksheet.Cell(currentRow, 2).Style.Font.SetBold();
            worksheet.Cell(currentRow, 3).Value = lang == "hu" ? Cargotruck.Shared.Resources.Resource.Partner : "Partner";
            worksheet.Cell(currentRow, 3).Style.Font.SetBold();
            worksheet.Cell(currentRow, 4).Value = lang == "hu" ? Cargotruck.Shared.Resources.Resource.Description : "Description";
            worksheet.Cell(currentRow, 4).Style.Font.SetBold();
            worksheet.Cell(currentRow, 5).Value = lang == "hu" ? Cargotruck.Shared.Resources.Resource.Place_of_receipt : "Place of receipt";
            worksheet.Cell(currentRow, 5).Style.Font.SetBold();
            worksheet.Cell(currentRow, 6).Value = lang == "hu" ? Cargotruck.Shared.Resources.Resource.Time_of_receipt : "Time of receipt";
            worksheet.Cell(currentRow, 6).Style.Font.SetBold();
            worksheet.Cell(currentRow, 7).Value = lang == "hu" ? Cargotruck.Shared.Resources.Resource.Place_of_delivery : "Place of delivery";
            worksheet.Cell(currentRow, 7).Style.Font.SetBold();
            worksheet.Cell(currentRow, 8).Value = lang == "hu" ? Cargotruck.Shared.Resources.Resource.Time_of_delivery : "Time of delivery";
            worksheet.Cell(currentRow, 8).Style.Font.SetBold();
            worksheet.Cell(currentRow, 9).Value = lang == "hu" ? Cargotruck.Shared.Resources.Resource.other_stops : "other stops";
            worksheet.Cell(currentRow, 9).Style.Font.SetBold();
            worksheet.Cell(currentRow, 10).Value = lang == "hu" ? Cargotruck.Shared.Resources.Resource.Id_cargo : "Id cargo";
            worksheet.Cell(currentRow, 10).Style.Font.SetBold();
            worksheet.Cell(currentRow, 11).Value = lang == "hu" ? Cargotruck.Shared.Resources.Resource.Storage_time : "Storage time";
            worksheet.Cell(currentRow, 11).Style.Font.SetBold();
            worksheet.Cell(currentRow, 12).Value = lang == "hu" ? Cargotruck.Shared.Resources.Resource.Completed : "Completed";
            worksheet.Cell(currentRow, 12).Style.Font.SetBold();
            worksheet.Cell(currentRow, 13).Value = lang == "hu" ? Cargotruck.Shared.Resources.Resource.Completion_time : "Completion time";
            worksheet.Cell(currentRow, 13).Style.Font.SetBold();
            worksheet.Cell(currentRow, 14).Value = lang == "hu" ? Cargotruck.Shared.Resources.Resource.Time_of_delay : "Time of delay";
            worksheet.Cell(currentRow, 14).Style.Font.SetBold();
            worksheet.Cell(currentRow, 15).Value = lang == "hu" ? Cargotruck.Shared.Resources.Resource.Payment : "Payment";
            worksheet.Cell(currentRow, 15).Style.Font.SetBold();
            worksheet.Cell(currentRow, 16).Value = lang == "hu" ? Cargotruck.Shared.Resources.Resource.Final_Payment : "Final Payment";
            worksheet.Cell(currentRow, 16).Style.Font.SetBold();
            worksheet.Cell(currentRow, 17).Value = lang == "hu" ? Cargotruck.Shared.Resources.Resource.Penalty : "Penalty";
            worksheet.Cell(currentRow, 17).Style.Font.SetBold();
            worksheet.Cell(currentRow, 18).Value = lang == "hu" ? Cargotruck.Shared.Resources.Resource.Date : "Date";
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
                worksheet.Cell(currentRow, 9).Value = task.other_stops;
                worksheet.Cell(currentRow, 10).Value = task.Id_cargo;
                worksheet.Cell(currentRow, 11).Value = task.Storage_time;
                worksheet.Cell(currentRow, 12).Value = task.Completed;
                worksheet.Cell(currentRow, 13).Value = task.Completion_time;
                worksheet.Cell(currentRow, 14).Value = task.Time_of_delay;
                worksheet.Cell(currentRow, 15).Value = task.Payment + (task.Payment != null ? " HUF" : "");
                worksheet.Cell(currentRow, 16).Value = task.Final_Payment + (task.Final_Payment != null ? " HUF" : "");
                worksheet.Cell(currentRow, 17).Value = task.Penalty + (task.Penalty != null ? " HUF" : "");
                worksheet.Cell(currentRow, 18).Value = task.Date;
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            var content = stream.ToArray();
            return Convert.ToBase64String(content);
        }

        //iTextSharp needed !!!
        [HttpGet]
         public async Task<string> PDF(string lang)
        {
            var tasks = from t in _context.Tasks select t;

            int pdfRowIndex = 1;
            Random rnd = new();
            int random = rnd.Next(1000000, 9999999);
            string filename = "Tasks" + random + "_" + DateTime.Now.ToString("dd-MM-yyyy");
            string filepath = "Files/" + filename + ".pdf";

            Document document = new(PageSize.A4, 5f, 5f, 10f, 10f);
            FileStream fs = new(filepath, FileMode.Create);
            PdfWriter writer = PdfWriter.GetInstance(document, fs);
            document.Open();

            Font font1 = FontFactory.GetFont(FontFactory.TIMES_BOLD, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 11);
            Font font2 = FontFactory.GetFont(FontFactory.TIMES_ROMAN, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 10);

            System.Type type = typeof(Tasks);
            var column_number = (type.GetProperties().Length) / 2;
            var columnDefinitionSize = new float[column_number];
            for (int i = 0; i < column_number; i++) columnDefinitionSize[i] = 1F;

            PdfPTable table, table2;
            PdfPCell cell;

            table = new PdfPTable(columnDefinitionSize)
            {
                WidthPercentage = 90
            };
            table2 = new PdfPTable(columnDefinitionSize)
            {
                WidthPercentage = 90
            };
            cell = new PdfPCell
            {
                BackgroundColor = new BaseColor(0xC0, 0xC0, 0xC0),
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_CENTER
            };

            var title = new Paragraph(15, lang == "hu" ? Cargotruck.Shared.Resources.Resource.Tasks : "Tasks")
            {
                Alignment = Element.ALIGN_CENTER
            };


            document.Add(title);
            document.Add(new Paragraph("\n"));

            if (tasks.Any())
            {
                table.AddCell(new PdfPCell(new Phrase("Id", font1))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                });
                table.AddCell(new PdfPCell(new Phrase(lang == "hu" ? Cargotruck.Shared.Resources.Resource.Partner : "Partner", font1))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                });
                table.AddCell(new PdfPCell(new Phrase(lang == "hu" ? Cargotruck.Shared.Resources.Resource.Description : "Description", font1))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                });
                table.AddCell(new PdfPCell(new Phrase(lang == "hu" ? Cargotruck.Shared.Resources.Resource.Place_of_receipt : "Place of receipt", font1))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                });
                table.AddCell(new PdfPCell(new Phrase(lang == "hu" ? Cargotruck.Shared.Resources.Resource.Time_of_receipt : "Time of receipt", font1))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                });
                table.AddCell(new PdfPCell(new Phrase(lang == "hu" ? Cargotruck.Shared.Resources.Resource.Place_of_delivery : "Place of delivery", font1))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                });
                table.AddCell(new PdfPCell(new Phrase(lang == "hu" ? Cargotruck.Shared.Resources.Resource.Time_of_delivery : "Time of delivery", font1))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                });
                table.AddCell(new PdfPCell(new Phrase(lang == "hu" ? Cargotruck.Shared.Resources.Resource.other_stops : "other stops", font1))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                });
                table.AddCell(new PdfPCell(new Phrase(lang == "hu" ? Cargotruck.Shared.Resources.Resource.Id_cargo : "Id cargo", font1))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                });


                foreach (Tasks task in tasks)
                {
                    var s = "";
                    if (!string.IsNullOrEmpty(task.Id.ToString())) { s = task.Id.ToString(); }
                    else { s = "-"; }
                    table.AddCell(new PdfPCell(new Phrase(s.ToString(), font2))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    });
                    if (!string.IsNullOrEmpty(task.Partner?.ToString())) { s = task.Partner.ToString(); }
                    else { s = "-"; }
                    table.AddCell(new PdfPCell(new Phrase(s.ToString(), font2))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    });
                    if (!string.IsNullOrEmpty(task.Description?.ToString())) { s = task.Description.ToString(); }
                    else { s = "-"; }
                    table.AddCell(new PdfPCell(new Phrase(s.ToString(), font2))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    });
                    if (!string.IsNullOrEmpty(task.Place_of_receipt?.ToString())) { s = task.Place_of_receipt.ToString(); }
                    else { s = "-"; }
                    table.AddCell(new PdfPCell(new Phrase(s.ToString(), font2))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    });
                    if (!string.IsNullOrEmpty(task.Time_of_receipt.ToString())) { s = task.Time_of_receipt.ToString(); }
                    else { s = "-"; }
                    table.AddCell(new PdfPCell(new Phrase(s?.ToString(), font2))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    });
                    if (!string.IsNullOrEmpty(task.Place_of_delivery?.ToString())) { s = task.Place_of_delivery.ToString(); }
                    else { s = "-"; }
                    table.AddCell(new PdfPCell(new Phrase(s.ToString(), font2))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    });
                    if (!string.IsNullOrEmpty(task.Time_of_delivery.ToString())) { s = task.Time_of_delivery.ToString(); }
                    else { s = "-"; }
                    table.AddCell(new PdfPCell(new Phrase(s?.ToString(), font2))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    });
                    if (!string.IsNullOrEmpty(task.other_stops)) { s = task.other_stops.ToString(); }
                    else { s = "-"; }
                    table.AddCell(new PdfPCell(new Phrase(s?.ToString(), font2))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    });
                    if (!string.IsNullOrEmpty(task.Id_cargo.ToString())) { s = task.Id_cargo.ToString(); }
                    else { s = "-"; }
                    table.AddCell(new PdfPCell(new Phrase(s?.ToString(), font2))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    });
                    pdfRowIndex++;
                }

                document.Add(table);
                document.Add(new Paragraph("\n"));
                table2.AddCell(new PdfPCell(new Phrase("Id", font1))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                });
                table2.AddCell(new PdfPCell(new Phrase(lang == "hu" ? Cargotruck.Shared.Resources.Resource.Storage_time : "Storage time", font1))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                });
                table2.AddCell(new PdfPCell(new Phrase(lang == "hu" ? Cargotruck.Shared.Resources.Resource.Completed : "Completed", font1))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                });
                table2.AddCell(new PdfPCell(new Phrase(lang == "hu" ? Cargotruck.Shared.Resources.Resource.Completion_time : "Completion time", font1))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                });
                table2.AddCell(new PdfPCell(new Phrase(lang == "hu" ? Cargotruck.Shared.Resources.Resource.Time_of_delay : "Time of delay", font1))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                });
                table2.AddCell(new PdfPCell(new Phrase(lang == "hu" ? Cargotruck.Shared.Resources.Resource.Payment : "Payment", font1))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                });
                table2.AddCell(new PdfPCell(new Phrase(lang == "hu" ? Cargotruck.Shared.Resources.Resource.Final_Payment : "Final Payment", font1))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                });
                table2.AddCell(new PdfPCell(new Phrase(lang == "hu" ? Cargotruck.Shared.Resources.Resource.Penalty : "Penalty", font1))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                });
                table2.AddCell(new PdfPCell(new Phrase(lang == "hu" ? Cargotruck.Shared.Resources.Resource.Date : "Date", font1))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                });
                table2.HeaderRows = 1;


                foreach (Tasks task in tasks)
                {
                    var s = "";
                    if (!string.IsNullOrEmpty(task.Id.ToString())) { s = task.Id.ToString(); }
                    else { s = "-"; }
                    table2.AddCell(new PdfPCell(new Phrase(s.ToString(), font2))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    });
                    if (!string.IsNullOrEmpty(task.Storage_time)) { s = task.Storage_time.ToString(); }
                    else { s = "-"; }
                    table2.AddCell(new PdfPCell(new Phrase(s.ToString(), font2))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    });
                    if (!string.IsNullOrEmpty(task.Completed.ToString())) { s = task.Completed == true ? Cargotruck.Shared.Resources.Resource.True : Cargotruck.Shared.Resources.Resource.False; }
                    else { s = "-"; }
                    table2.AddCell(new PdfPCell(new Phrase(s.ToString(), font2))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    });
                    if (!string.IsNullOrEmpty(task.Completion_time.ToString())) { s = task.Completion_time.ToString(); }
                    else { s = "-"; }
                    table2.AddCell(new PdfPCell(new Phrase(s?.ToString(), font2))
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
                    if (!string.IsNullOrEmpty(task.Payment.ToString())) { s = task.Payment.ToString() + " HUF"; }
                    else { s = "-"; }
                    table2.AddCell(new PdfPCell(new Phrase(s.ToString(), font2))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    });
                    if (!string.IsNullOrEmpty(task.Final_Payment.ToString())) { s = task.Final_Payment.ToString() + " HUF"; }
                    else { s = "-"; }
                    table2.AddCell(new PdfPCell(new Phrase(s.ToString(), font2))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    });
                    if (!string.IsNullOrEmpty(task.Penalty.ToString())) { s = task.Penalty.ToString() + " HUF"; }
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
            }
            else
            {
                var noContent = new Paragraph(lang == "hu" ? "Nem található adat!" : "No content found!")
                {
                    Alignment = Element.ALIGN_CENTER
                };
                document.Add(noContent);
            }
            document.Close();
            //document.CloseDocument();
            //document.Dispose();
            writer.Close();
            //writer.Dispose();
            fs.Close();
            fs.Dispose();

            FileStream sourceFile = new(filepath, FileMode.Open);
            MemoryStream memoryStream = new();
            await sourceFile.CopyToAsync(memoryStream);
            var buffer = memoryStream.ToArray();
            var pdf = Convert.ToBase64String(buffer);
            sourceFile.Dispose();
            sourceFile.Close();
            System.IO.File.Delete(filepath); // delete the file in the app folder

            return pdf;
        }

        //iTextSharp needed !!!
        [HttpGet]
        public async Task<string> CSV(string lang)
        {
            var tasks = from t in _context.Tasks select t;

            Random rnd = new();
            int random = rnd.Next(1000000, 9999999);
            string filename = "Tasks" + random + "_" + DateTime.Now.ToString("dd-MM-yyyy");
            string filepath = "Files/" + filename + ".txt";

            StreamWriter txt = new(filepath);
            txt.Write("Id" + ";");
            txt.Write((lang == "hu" ? Cargotruck.Shared.Resources.Resource.User_id : "User ID") + ";");
            txt.Write((lang == "hu" ? Cargotruck.Shared.Resources.Resource.Partner : "Partner") + ";");
            txt.Write((lang == "hu" ? Cargotruck.Shared.Resources.Resource.Description : "Description") + "; ");
            txt.Write((lang == "hu" ? Cargotruck.Shared.Resources.Resource.Place_of_receipt : "Place of receipt") + "; ");
            txt.Write((lang == "hu" ? Cargotruck.Shared.Resources.Resource.Time_of_receipt : "Time of receipt") + "; ");
            txt.Write((lang == "hu" ? Cargotruck.Shared.Resources.Resource.Place_of_delivery : "Place of delivery") + "; ");
            txt.Write((lang == "hu" ? Cargotruck.Shared.Resources.Resource.Time_of_delivery : "Time of delivery") + "; ");
            txt.Write((lang == "hu" ? Cargotruck.Shared.Resources.Resource.other_stops : "other stops") + "; ");
            txt.Write((lang == "hu" ? Cargotruck.Shared.Resources.Resource.Id_cargo : "Id cargo") + "; ");
            txt.Write((lang == "hu" ? Cargotruck.Shared.Resources.Resource.Storage_time : "Storage time") + "; ");
            txt.Write((lang == "hu" ? Cargotruck.Shared.Resources.Resource.Completed : "Completed") + "; ");
            txt.Write((lang == "hu" ? Cargotruck.Shared.Resources.Resource.Completion_time : "Completion time") + "; ");
            txt.Write((lang == "hu" ? Cargotruck.Shared.Resources.Resource.Time_of_delay : "Time of delay") + "; ");
            txt.Write((lang == "hu" ? Cargotruck.Shared.Resources.Resource.Payment : "Payment") + "; ");
            txt.Write((lang == "hu" ? Cargotruck.Shared.Resources.Resource.Final_Payment : "Final Payment") + "; ");
            txt.Write((lang == "hu" ? Cargotruck.Shared.Resources.Resource.Penalty : "Penalty") + "; ");
            txt.Write((lang == "hu" ? Cargotruck.Shared.Resources.Resource.Date : "Date") + "; ");
            txt.Write("\n");
            foreach (var task in tasks)
            {
                txt.Write(task.Id + ";");
                txt.Write(task.Partner + ";");
                txt.Write(task.Description + ";");
                txt.Write(task.Place_of_receipt + ";");
                txt.Write(task.Time_of_receipt + ";");
                txt.Write(task.Place_of_delivery + ";");
                txt.Write(task.Time_of_delivery + ";");
                txt.Write(task.other_stops + ";");
                txt.Write(task.Id_cargo + ";");
                txt.Write(task.Storage_time + ";");
                txt.Write(task.Completed + ";");
                txt.Write(task.Completion_time + ";");
                txt.Write(task.Time_of_delay + ";");
                txt.Write(task.Payment + (task.Payment != null ? " HUF" : "") + ";");
                txt.Write(task.Final_Payment + (task.Final_Payment != null ? " HUF" : "") + ";");
                txt.Write(task.Penalty + (task.Penalty != null ? " HUF" : "") + "; ");
                txt.Write(task.Date + "; ");
                txt.Write("\n");
            }
            txt.Close();
            txt.Dispose();

            //change the encoding of the file content
            string csvFileContents = System.IO.File.ReadAllText(filepath);
            Encoding utf8Encoding = Encoding.UTF8;
            byte[] csvFileContentsAsBytes = Encoding.Default.GetBytes(csvFileContents);
            byte[] convertedCsvFileContents = Encoding.Convert(Encoding.Default, utf8Encoding, csvFileContentsAsBytes);
            string convertedCsvFileContentsAsString = utf8Encoding.GetString(convertedCsvFileContents);
            System.IO.File.WriteAllText(filepath, convertedCsvFileContentsAsString, utf8Encoding);


            FileStream sourceFile = new(filepath, FileMode.Open);
            MemoryStream memoryStream = new();
            await sourceFile.CopyToAsync(memoryStream);
            var buffer = memoryStream.ToArray();
            var file = Convert.ToBase64String(buffer);
            sourceFile.Dispose();
            sourceFile.Close();
            if (!sourceFile.CanWrite) {
                System.IO.File.Delete(filepath); // delete the file in the app folder
            }
            return file;
        }

        public async Task<IActionResult> Import(string file, string lang)
        {
            var error = "";
            var haveColumns = false;
            if (file != null)
            {
                string path = Path.Combine("Files/", file);
                //Checking file content length and Extension must be .xlsx  
                if (file != null && System.IO.File.Exists(path) && file.ToLower().Contains(".xlsx"))
                {             

                    //Started reading the Excel file.  
                    XLWorkbook workbook = new(path);
                    
                    IXLWorksheet worksheet = workbook.Worksheet(1);
                    //Loop through the Worksheet rows.
                    DataTable? dt = new();
                    bool firstRow = true;
                    if (worksheet.Row(2).CellsUsed().Count() > 1 && worksheet.Row(2).Cell(worksheet.Row(1).CellsUsed().Count()) !=null) {
                        int l = 0;
                        foreach (IXLRow row in worksheet.Rows())
                        {

                        
                        
                            //Use the first row to add columns to DataTable with column names check.
                            if (firstRow)
                            {
                                List<string?> titles = new() {
                                     "Id",
                                    lang == "hu" ? Cargotruck.Shared.Resources.Resource.User_id : "User ID",
                                    lang == "hu" ? Cargotruck.Shared.Resources.Resource.Partner : "Partner",
                                    lang == "hu" ? Cargotruck.Shared.Resources.Resource.Description : "Description",
                                    lang == "hu" ? Cargotruck.Shared.Resources.Resource.Place_of_receipt : "Place of receipt",
                                    lang == "hu" ? Cargotruck.Shared.Resources.Resource.Time_of_receipt : "Time of receipt",
                                    lang == "hu" ? Cargotruck.Shared.Resources.Resource.Place_of_delivery : "Place of delivery",
                                    lang == "hu" ? Cargotruck.Shared.Resources.Resource.Time_of_delivery : "Time of delivery",
                                    lang == "hu" ? Cargotruck.Shared.Resources.Resource.other_stops : "other stops",
                                    lang == "hu" ? Cargotruck.Shared.Resources.Resource.Id_cargo : "Id cargo",
                                    lang == "hu" ? Cargotruck.Shared.Resources.Resource.Storage_time : "Storage time",
                                    lang == "hu" ? Cargotruck.Shared.Resources.Resource.Completed : "Completed",
                                    lang == "hu" ? Cargotruck.Shared.Resources.Resource.Completion_time : "Completion time",
                                    lang == "hu" ? Cargotruck.Shared.Resources.Resource.Time_of_delay : "Time of delay",
                                    lang == "hu" ? Cargotruck.Shared.Resources.Resource.Payment : "Payment",
                                    lang == "hu" ? Cargotruck.Shared.Resources.Resource.Final_Payment : "Final Payment",
                                    lang == "hu" ? Cargotruck.Shared.Resources.Resource.Penalty : "Penalty",
                                    lang == "hu" ? Cargotruck.Shared.Resources.Resource.Date : "Date"
                                 };

                                foreach (IXLCell cell in row.Cells())
                                {
                                    if (titles.Contains(cell.Value.ToString())) 
                                    {
                                        titles.Remove(cell.Value.ToString());
                                        dt.Columns.Add(cell.Value.ToString()); 
                                    }
                                    else 
                                    {
                                        error = lang == "hu" ? @Cargotruck.Shared.Resources.Resource.Not_match_col : "Wrong column names!";
                                        System.IO.File.Delete(path); // delete the file
                                        return BadRequest(error);
                                    }
                               
                                }
                                firstRow = false;
                                if (titles.Count == 0) 
                                { 
                                    haveColumns = true; 
                                        l += 1;
                                }
                                else if (titles.Count == 1 &&  titles.Contains("Id")) 
                                {
                                    haveColumns = true;
                                }
                            }
                            else if(haveColumns)
                            {
                                List<object?> list = new();
                                int nulls = 0;
                                //Add rows to DataTable.
                                dt.Rows.Add();
                                foreach (IXLCell cell in row.Cells(1, dt.Columns.Count))
                                {
                                    if (cell.Value != null && cell.Value.ToString()!="") { list.Add(cell.Value); }
                                    else 
                                    { 
                                        list.Add(System.DBNull.Value);
                                        nulls += 1;
                                    }
                                }

                                list[l + 13] = new String(list[l + 13]?.ToString()?.Where(Char.IsDigit).ToArray());
                                list[l + 14] = new String(list[l + 14]?.ToString()?.Where(Char.IsDigit).ToArray());
                                list[l + 15] = new String(list[l + 15]?.ToString()?.Where(Char.IsDigit).ToArray());

                                if (nulls != list.Count)
                                {
                                    var sql = @"Insert Into Tasks (User_id,Partner,Description,Place_of_receipt,Time_of_receipt,Place_of_delivery,Time_of_delivery,other_stops,Id_cargo,Storage_time,Completed,Completion_time,Time_of_delay,Payment,Final_Payment,Penalty,Date ) 
                                        Values (@User_id,@Partner,@Description,@Place_of_receipt,@Time_of_receipt, @Place_of_delivery,@Time_of_delivery,@other_stops,@Id_cargo,@Storage_time,@Completed,@Completion_time,@Time_of_delay,@Payment,@Final_Payment,@Penalty,@Date)";
                                    var insert = await _context.Database.ExecuteSqlRawAsync(sql,
                                        new SqlParameter("@User_id", list[l]),
                                        new SqlParameter("@Partner", list[l + 1]),
                                        new SqlParameter("@Description", list[l + 2]),
                                        new SqlParameter("@Place_of_receipt", list[l + 3]),
                                        new SqlParameter("@Time_of_receipt", list[l + 4] == System.DBNull.Value || list[l + 4] == null ? System.DBNull.Value : DateTime.Parse(list[l + 4]?.ToString()!)),
                                        new SqlParameter("@Place_of_delivery", list[l + 5]),
                                        new SqlParameter("@Time_of_delivery", list[l + 6] == System.DBNull.Value || list[l + 6] == null ? System.DBNull.Value : DateTime.Parse(list[l + 6]?.ToString()!)),
                                        new SqlParameter("@other_stops", list[l + 7]),
                                        new SqlParameter("@Id_cargo", list[l + 8]),
                                        new SqlParameter("@Storage_time", list[l + 9]),
                                        new SqlParameter("@Completed", list[l + 10]),
                                        new SqlParameter("@Completion_time", list[l + 11]),
                                        new SqlParameter("@Time_of_delay", list[l + 12]),
                                        new SqlParameter("@Payment", list[l + 13]),
                                        new SqlParameter("@Final_Payment", list[l + 14]),
                                        new SqlParameter("@Penalty", list[l + 15]),
                                        new SqlParameter("@Date", DateTime.Now)
                                        );

                                    if (insert > 0)
                                    {
                                        error = "";
                                        var lastId = await _context.Tasks.OrderBy(x => x.Id).LastOrDefaultAsync();

                                        if (lastId?.Id_cargo != null)
                                        {
                                            var WithNewIds = await _context.Tasks.Where(x => x.Id_cargo == lastId.Id_cargo).ToListAsync();
                                            Cargoes? cargo = await _context.Cargoes.FirstOrDefaultAsync(x => x.Id == lastId.Id_cargo);

                                            foreach (var item in WithNewIds)
                                            {
                                                if (item != null)
                                                {
                                                    if (item.Id != lastId?.Id)
                                                    {
                                                        item.Id_cargo = null;
                                                        _context.Entry(item).State = EntityState.Modified;
                                                        await _context.SaveChangesAsync();
                                                    }
                                                    else if (cargo == null)
                                                    {
                                                        item.Id_cargo = null;
                                                        _context.Entry(item).State = EntityState.Modified;
                                                        await _context.SaveChangesAsync();
                                                    }
                                                }
                                            }

                                            if (cargo != null)
                                            {
                                                cargo.Task_id = lastId.Id;
                                                _context.Entry(cargo).State = EntityState.Modified;
                                                await _context.SaveChangesAsync();
                                            }
                                        }
                                    }
                                    else if (insert <= 0)
                                    {
                                        System.IO.File.Delete(path); // delete the file
                                    }
                                }
                            }
                            else
                            {
                                error = lang == "hu" ? @Cargotruck.Shared.Resources.Resource.Not_match_col_count : "Missing columns in the datatable";
                                return BadRequest(error);
                            }
                            //If no data in Excel file  
                            if (firstRow)
                            {
                                error = lang == "hu" ? @Cargotruck.Shared.Resources.Resource.Empty_excel : "Empty excel file!";
                                System.IO.File.Delete(path); // delete the file
                                return BadRequest(error);
                            }
                        }
                    }
                    else
                    {
                        error = lang == "hu" ? @Cargotruck.Shared.Resources.Resource.Missing_data_rows : "No datarows in the file!";
                        System.IO.File.Delete(path); // delete the file
                        return BadRequest(error);
                    }
                }
                else
                {
                    //If file extension of the uploaded file is different then .xlsx  
                    error = lang == "hu" ?  @Cargotruck.Shared.Resources.Resource.Not_excel : "Bad format! The file is not an excel.";
                    System.IO.File.Delete(path); // delete the file
                    return BadRequest(error);
                }
            }
            else
            {
                error = lang == "hu" ?  @Cargotruck.Shared.Resources.Resource.No_excel : "File not found!";
                return BadRequest(error);
            }
            return NoContent();
        }
    }
}