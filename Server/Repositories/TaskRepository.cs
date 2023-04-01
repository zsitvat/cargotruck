using Cargotruck.Server.Data;
using Cargotruck.Server.Repositories.Interfaces;
using Cargotruck.Server.Services.Interfaces;
using Cargotruck.Shared.Model;
using Cargotruck.Shared.Resources;
using ClosedXML.Excel;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System.Data;
using System.Globalization;
using System.Text;

namespace Cargotruck.Server.Repositories
{
    public class TaskRepository : ITaskRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IStringLocalizer<Resource> _localizer;
        private readonly IColumnNamesService _columnNameLists;
        private readonly IErrorHandlerService _errorHandler;

        public TaskRepository(ApplicationDbContext context, IStringLocalizer<Resource> localizer, IColumnNamesService columnNameLists, IErrorHandlerService errorHandler)
        {
            _context = context;
            _localizer = localizer;
            _columnNameLists = columnNameLists;
            _errorHandler = errorHandler;
        }

        private async Task<List<Task>> GetDataAsync(string? searchString, string? filter, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate)
        {
            var t = await _context.Tasks.Where(s => (dateFilterStartDate != null ? (s.Date >= dateFilterStartDate) : true) && (dateFilterEndDate != null ? (s.Date <= dateFilterEndDate) : true)).ToListAsync();

            if (filter == "Completed")
            {
                t = t.Where(data => data.Completed).ToList();
            }
            else if (filter == "Not_completed")
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

            return t;
        }

        public async Task<List<Task>> GetAsync(int page, int pageSize, string sortOrder, bool desc, string? searchString, string? filter, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate)
        {

            var t = await GetDataAsync(searchString, filter, dateFilterStartDate, dateFilterEndDate);

            sortOrder = sortOrder == "Partner" ? (desc ? "Partner_desc" : "Partner") : (sortOrder);
            sortOrder = sortOrder == "Description " ? (desc ? "Description_desc" : "Description") : (sortOrder);
            sortOrder = sortOrder == "Place_of_receipt" ? (desc ? "Place_of_receipt_desc" : "Place_of_receipt") : (sortOrder);
            sortOrder = sortOrder == "Time_of_receipt" ? (desc ? "Time_of_receipt_desc" : "Time_of_receipt") : (sortOrder);
            sortOrder = sortOrder == "Place_of_delivery" ? (desc ? "Place_of_delivery_desc" : "Place_of_delivery") : (sortOrder);
            sortOrder = sortOrder == "Time_of_delivery" ? (desc ? "Time_of_delivery_desc" : "Time_of_delivery") : (sortOrder);
            sortOrder = sortOrder == "Other_stops" ? (desc ? "Other_stops_desc" : "Other_stops") : (sortOrder);
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
                "Other_stops_desc" => t.OrderByDescending(s => s.Other_stops).ToList(),
                "Other_stops" => t.OrderBy(s => s.Other_stops).ToList(),
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

            return t.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        }

        public async Task<Task?> GetByIdAsync(int id)
        {
            return await _context.Tasks.FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<List<Task>> GetTasksAsync()
        {
            return await _context.Tasks.ToListAsync();
        }

        public async Task<int[]> GetChartDataAsync()
        {
            var data = await _context.Tasks.ToListAsync();
            int[] columnsHeight = new int[24];
            for (int i = 0; i < 12; i++)
            {
                columnsHeight[i] = data.Where(x => x.Date.Year == DateTime.Now.Year && x.Date.Month == i + 1 && x.Completed == false).Count();
            }
            for (int i = 0; i < 12; i++)
            {
                columnsHeight[i + 12] = data.Where(x => x.Date.Year == DateTime.Now.Year && x.Date.Month == i + 1 && x.Completed).Count();
            }
            return columnsHeight;
        }

        public async Task<int> CountAsync(bool all)
        {
            if (all)
            {
                return await _context.Tasks.CountAsync();
            }
            else
            {
                return await _context.Tasks.Where(x => x.Completed == false).CountAsync();
            }
        }

        public async Task<int> PageCountAsync(string? searchString, string? filter, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate)
        {
            var t = await GetDataAsync(searchString, filter, dateFilterStartDate, dateFilterEndDate);
            return t.Count;
        }

        public async System.Threading.Tasks.Task PostAsync(Shared.Model.Task t)
        {
            t.Final_Payment = (t.Payment != null ? t.Payment : 0) - (t.Penalty != null ? t.Penalty : 0);

            if (t.Completed)
            {
                t.Completion_time = DateTime.Now;
            }
            else
            {
                t.Completion_time = null;
            }

            _context.Add(t);
            await _context.SaveChangesAsync();

            var cargo = _context.Cargoes.FirstOrDefault(a => a.Id == t.Id_cargo);
            if (cargo != null)
            {
                cargo.Task_id = t.Id;
                _context.Entry(cargo).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
        }

        public async System.Threading.Tasks.Task PutAsync(Shared.Model.Task t)
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
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var t = _context.Tasks.FirstOrDefault(x => x.Id == id);
            if (t != null)
            {
                _context.Tasks.Remove(t);
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }

        public async System.Threading.Tasks.Task ChangeCompletionAsync(Shared.Model.Task t)
        {
            t.Completed = !t.Completed;

            if (t.Completed)
            {
                t.Completion_time = DateTime.Now;
            }
            else { t.Completion_time = null; }

            _context.Entry(t).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }


        public string ExportToExcel(CultureInfo lang, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate)
        {
            var tasks = _context.Tasks.Where(s => (dateFilterStartDate != null ? (s.Date >= dateFilterStartDate) : true) && (dateFilterEndDate != null ? (s.Date <= dateFilterEndDate) : true));

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Tasks");
            var currentRow = 1;

            CultureInfo.CurrentUICulture = lang;
            List<string> columnNames = _columnNameLists.GetTasksColumnNames().Select(x => _localizer[x].Value).ToList();

            for (var i = 0; i < columnNames.Count; i++)
            {
                worksheet.Cell(currentRow, i + 1).Value = columnNames[i];
                worksheet.Cell(currentRow, i + 1).Style.Font.SetBold();
            }

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
                worksheet.Cell(currentRow, 14).Value = task.Payment + (task.Payment != null ? " HUF" : "");
                worksheet.Cell(currentRow, 15).Value = task.Final_Payment + (task.Final_Payment != null ? " HUF" : "");
                worksheet.Cell(currentRow, 16).Value = task.Penalty + (task.Penalty != null ? " HUF" : "");
                worksheet.Cell(currentRow, 17).Value = task.Date;
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            var content = stream.ToArray();

            return Convert.ToBase64String(content);
        }

        public async Task<string> ExportToPdfAsync(CultureInfo lang, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate)
        {
            var tasks = _context.Tasks.Where(s => (dateFilterStartDate != null ? (s.Date >= dateFilterStartDate) : true) && (dateFilterEndDate != null ? (s.Date <= dateFilterEndDate) : true));

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

            System.Type type = typeof(Shared.Model.Task);
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



            //copy column names to a list based on language
            CultureInfo.CurrentUICulture = lang;
            List<string> columnNames = _columnNameLists.GetTasksColumnNames().Select(x => _localizer[x].Value).ToList();

            var title = new Paragraph(15, _localizer["Tasks"].Value)
            {
                Alignment = Element.ALIGN_CENTER
            };

            document.Add(title);
            document.Add(new Paragraph("\n"));

            if (tasks.Any())
            {
                foreach (var name in columnNames.Take(column_number))
                {
                    table.AddCell(new PdfPCell(new Phrase(name, font1))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    });
                }

                foreach (Shared.Model.Task task in tasks)
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
                    if (!string.IsNullOrEmpty(task.Other_stops)) { s = task.Other_stops.ToString(); }
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

                table2.AddCell(new PdfPCell(new Phrase(_localizer["Id"].Value, font1))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                });

                foreach (var name in columnNames.Skip(column_number).Take(column_number))
                {
                    table2.AddCell(new PdfPCell(new Phrase(name, font1))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    });
                }

                table2.HeaderRows = 1;


                foreach (Shared.Model.Task task in tasks)
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
                var noContent = new Paragraph(_localizer["No_records"])
                {
                    Alignment = Element.ALIGN_CENTER
                };

                document.Add(noContent);
            }

            document.Close();
            writer.Close();
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

        public async Task<string> ExportToCSVAsync(CultureInfo lang, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate, bool isTextDocument)
        {
            var tasks = _context.Tasks.Where(s => (dateFilterStartDate != null ? (s.Date >= dateFilterStartDate) : true) && (dateFilterEndDate != null ? (s.Date <= dateFilterEndDate) : true));

            Random rnd = new();
            int random = rnd.Next(1000000, 9999999);
            string filename = "Tasks" + random + "_" + DateTime.Now.ToString("dd-MM-yyyy");
            string filepath = "Files/" + filename + ".txt";

            StreamWriter txt = new(filepath);

            //copy column names to a list based on language
            CultureInfo.CurrentUICulture = lang;
            List<string> columnNames = _columnNameLists.GetTasksColumnNames().Select(x => _localizer[x].Value).ToList();

            string separator = isTextDocument ? "\t" : ";";
            string ifNull = isTextDocument ? " --- " : "";

            foreach (var name in columnNames)
            {
                txt.Write(name + (isTextDocument ? "  " : separator));
            }

            txt.Write("\n");

            foreach (var task in tasks)
            {

                txt.Write(task.Id + separator);
                txt.Write((task.Partner ?? ifNull) + separator);
                txt.Write((task.Description ?? ifNull) + separator);
                txt.Write((task.Place_of_receipt ?? ifNull) + separator);
                txt.Write((task.Time_of_receipt != null ? task.Time_of_receipt : ifNull) + separator);
                txt.Write((task.Place_of_delivery ?? ifNull) + separator);
                txt.Write((task.Time_of_delivery != null ? task.Time_of_delivery : ifNull) + separator);
                txt.Write((task.Other_stops ?? ifNull) + separator);
                txt.Write((task.Id_cargo != null ? task.Id_cargo : ifNull) + separator);
                txt.Write((task.Storage_time ?? ifNull) + separator);
                txt.Write(task.Completed + separator);
                txt.Write((task.Completion_time != null ? task.Completion_time : ifNull) + separator);
                txt.Write((task.Time_of_delay ?? ifNull) + separator);
                txt.Write((task.Payment + "HUF" ?? ifNull) + separator);
                txt.Write((task.Final_Payment + "HUF" ?? ifNull) + separator);
                txt.Write((task.Penalty + "HUF" ?? ifNull) + separator);
                txt.Write(task.Date + separator);

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

            if (!sourceFile.CanWrite)
            {
                System.IO.File.Delete(filepath); // delete the file in the app folder
            }

            return file;
        }

        public async Task<string?> ImportAsync([FromBody] string file, CultureInfo lang)
        {
            var error = "";
            var haveColumns = false;

            if (file != null)
            {
                string path = Path.Combine("Files/", file);
                //Checking file content length and Extension must be .xlsx  
                if (file != null && System.IO.File.Exists(path) && file.ToLower().Contains(".xlsx"))
                {

                    //Started reading the ExportToExcel file.  
                    XLWorkbook workbook = new(path);

                    IXLWorksheet worksheet = workbook.Worksheet(1);
                    //Loop through the Worksheet rows.
                    DataTable? dt = new();
                    bool firstRow = true;

                    if (worksheet.Row(2).CellsUsed().Count() > 1 && worksheet.Row(2).Cell(worksheet.Row(1).CellsUsed().Count()) != null)
                    {
                        int l = 0;
                        foreach (IXLRow row in worksheet.Rows())
                        {

                            //Use the first row to add columns to DataTable with column names check.
                            if (firstRow)
                            {
                                //copy column names to a list
                                CultureInfo.CurrentUICulture = lang;
                                List<string> columnNames = _columnNameLists.GetTasksColumnNames().Select(x => _localizer[x].Value).ToList();

                                foreach (IXLCell cell in row.Cells())
                                {
                                    if (columnNames.Contains(cell.Value.ToString()!))
                                    {
                                        columnNames.Remove(cell.Value.ToString()!);
                                        dt.Columns.Add(cell.Value.ToString());
                                    }
                                    else
                                    {
                                        error = _localizer["Not_match_col"].Value;
                                        System.IO.File.Delete(path); // delete the file
                                        return error;
                                    }

                                }
                                firstRow = false;
                                if (columnNames.Count == 0)
                                {
                                    haveColumns = true;
                                    l += 1;
                                }
                                else if (columnNames.Count == 1 && columnNames.Contains(_localizer["Id"].Value))
                                {
                                    haveColumns = true;
                                }
                            }
                            else if (haveColumns)
                            {
                                List<object?> list = new();
                                int nulls = 0;
                                //Add rows to DataTable.
                                dt.Rows.Add();
                                foreach (IXLCell cell in row.Cells(1, dt.Columns.Count))
                                {
                                    if (cell.Value != null && cell.Value.ToString() != "") { list.Add(cell.Value); }
                                    else
                                    {
                                        list.Add(System.DBNull.Value);
                                        nulls += 1;
                                    }
                                }

                                list[l + 12] = new String(list[l + 13]?.ToString()?.Where(Char.IsDigit).ToArray());
                                list[l + 13] = new String(list[l + 14]?.ToString()?.Where(Char.IsDigit).ToArray());
                                list[l + 14] = new String(list[l + 15]?.ToString()?.Where(Char.IsDigit).ToArray());

                                try
                                {

                                    if (nulls != list.Count)
                                    {
                                        var sql = @"Insert Into Tasks (User_id,Partner,Description,Place_of_receipt,Time_of_receipt,Place_of_delivery,Time_of_delivery,Other_stops,Id_cargo,Storage_time,Completed,Completion_time,Time_of_delay,Payment,Final_Payment,Penalty,Date ) 
                                            Values (@User_id,@Partner,@Description,@Place_of_receipt,@Time_of_receipt, @Place_of_delivery,@Time_of_delivery,@Other_stops,@Id_cargo,@Storage_time,@Completed,@Completion_time,@Time_of_delay,@Payment,@Final_Payment,@Penalty,@Date)";
                                        var insert = await _context.Database.ExecuteSqlRawAsync(sql,
                                            new SqlParameter("@User_id", "Imported"),
                                            new SqlParameter("@Partner", list[l]),
                                            new SqlParameter("@Description", list[l + 1]),
                                            new SqlParameter("@Place_of_receipt", list[l + 2]),
                                            new SqlParameter("@Time_of_receipt", list[l + 3] == System.DBNull.Value || list[l + 3] == null ? System.DBNull.Value : DateTime.Parse(list[l + 3]?.ToString()!)),
                                            new SqlParameter("@Place_of_delivery", list[l + 4]),
                                            new SqlParameter("@Time_of_delivery", list[l + 5] == System.DBNull.Value || list[l + 5] == null ? System.DBNull.Value : DateTime.Parse(list[l + 5]?.ToString()!)),
                                            new SqlParameter("@Other_stops", list[l + 6]),
                                            new SqlParameter("@Id_cargo", list[l + 7]),
                                            new SqlParameter("@Storage_time", list[l + 8]),
                                            new SqlParameter("@Completed", list[l + 9]),
                                            new SqlParameter("@Completion_time", list[l + 10]),
                                            new SqlParameter("@Time_of_delay", list[l + 11]),
                                            new SqlParameter("@Payment", list[l + 12]),
                                            new SqlParameter("@Final_Payment", list[l + 13]),
                                            new SqlParameter("@Penalty", list[l + 14]),
                                            new SqlParameter("@Date", DateTime.Now)
                                            );

                                        if (insert > 0)
                                        {
                                            error = "";
                                            var lastId = await _context.Tasks.OrderBy(x => x.Id).LastOrDefaultAsync();

                                            if (lastId?.Id_cargo != null)
                                            {
                                                var WithNewIds = await _context.Tasks.Where(x => x.Id_cargo == lastId.Id_cargo).ToListAsync();
                                                Cargo? cargo = await _context.Cargoes.FirstOrDefaultAsync(x => x.Id == lastId.Id_cargo);

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
                                catch (Exception ex)
                                {
                                    return _errorHandler.GetErrorMessageAsString(ex);
                                }

                            }
                            else
                            {
                                error = _localizer["Not_match_col_count"];
                                return error;
                            }
                            //If no data in ExportToExcel file  
                            if (firstRow)
                            {
                                error = _localizer["Empty_excel"];
                                System.IO.File.Delete(path); // delete the file
                                return error;
                            }
                        }
                    }
                    else
                    {
                        error = _localizer["Missing_data_rows"];
                        System.IO.File.Delete(path); // delete the file
                        return error;
                    }
                }
                else
                {
                    //If file extension of the uploaded file is different then .xlsx  
                    error = _localizer["Not_excel"];
                    System.IO.File.Delete(path); // delete the file
                    return error;
                }
            }
            else
            {
                error = _localizer["No_excel"];
                return error;
            }
            return null;
        }
    }
}

