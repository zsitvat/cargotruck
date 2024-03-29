﻿using Cargotruck.Server.Data;
using Cargotruck.Server.Repositories.Interfaces;
using Cargotruck.Server.Services.Interfaces;
using Cargotruck.Shared.Model;
using Cargotruck.Shared.Resources;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Bibliography;
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

        //this method gets the data from db and filter it
        private async Task<List<DeliveryTask>> GetDataAsync(string? searchString, string? filter, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate)
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
            || (s.PlaceOfReceipt != null && s.PlaceOfReceipt.ToLower()!.Contains(searchString))
            || (s.PlaceOfDelivery != null && s.PlaceOfDelivery.ToLower()!.Contains(searchString))
            || (s.TimeOfDelivery.ToString()!.Contains(searchString))
            || (s.Cargo != null && s.CargoId.ToString()!.Contains(searchString))
            || (s.StorageTime != null && s.StorageTime.ToLower()!.Contains(searchString))
            || (s.CompletionTime != null && s.CompletionTime.ToString()!.Contains(searchString))
            || (s.Payment != null && s.Payment.ToString()!.Contains(searchString))
            || (s.FinalPayment != null && s.FinalPayment.ToString()!.Contains(searchString))
            || (s.Penalty != null && s.Penalty.ToString()!.Contains(searchString))
            || (s.Date.ToString()!.Contains(searchString))
            ).ToList();
            }

            return t;
        }

        public async Task<List<DeliveryTask>> GetAsync(int page, int pageSize, string sortOrder, bool desc, string? searchString, string? filter, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate)
        {

            var t = await GetDataAsync(searchString, filter, dateFilterStartDate, dateFilterEndDate);

            sortOrder = sortOrder == "Partner" ? (desc ? "Partner_desc" : "Partner") : (sortOrder);
            sortOrder = sortOrder == "Description" ? (desc ? "Description_desc" : "Description") : (sortOrder);
            sortOrder = sortOrder == "PlaceOfReceipt" ? (desc ? "PlaceOfReceipt_desc" : "PlaceOfReceipt") : (sortOrder);
            sortOrder = sortOrder == "TimeOfReceipt" ? (desc ? "TimeOfReceipt_desc" : "TimeOfReceipt") : (sortOrder);
            sortOrder = sortOrder == "PlaceOfDelivery" ? (desc ? "PlaceOfDelivery_desc" : "PlaceOfDelivery") : (sortOrder);
            sortOrder = sortOrder == "TimeOfDelivery" ? (desc ? "TimeOfDelivery_desc" : "TimeOfDelivery") : (sortOrder);
            sortOrder = sortOrder == "OtherStops" ? (desc ? "OtherStops_desc" : "OtherStops") : (sortOrder);
            sortOrder = sortOrder == "CargoId" ? (desc ? "CargoId_desc" : "CargoId") : (sortOrder);
            sortOrder = sortOrder == "StorageTime" ? (desc ? "StorageTime_desc" : "StorageTime") : (sortOrder);
            sortOrder = sortOrder == "Completed" ? (desc ? "Completed_desc" : "Completed") : (sortOrder);
            sortOrder = sortOrder == "CompletionTime" ? (desc ? "CompletionTime_desc" : "CompletionTime") : (sortOrder);
            sortOrder = sortOrder == "TimeOfDelay" ? (desc ? "TimeOfDelay_desc" : "TimeOfDelay") : (sortOrder);
            sortOrder = sortOrder == "Payment" ? (desc ? "Payment_desc" : "Payment") : (sortOrder);
            sortOrder = sortOrder == "FinalPayment" ? (desc ? "FinalPayment_desc" : "FinalPayment") : (sortOrder);
            sortOrder = sortOrder == "Penalty" ? (desc ? "Penalty_desc" : "Penalty") : (sortOrder);
            sortOrder = sortOrder == "Date" || String.IsNullOrEmpty(sortOrder) ? (desc ? "Date_desc" : "") : (sortOrder);

            t = sortOrder switch
            {
                "Partner_desc" => t.OrderByDescending(s => s.Partner).ToList(),
                "Partner" => t.OrderBy(s => s.Partner).ToList(),
                "Description_desc" => t.OrderByDescending(s => s.Description).ToList(),
                "Description" => t.OrderBy(s => s.Description).ToList(),
                "PlaceOfReceipt_desc" => t.OrderByDescending(s => s.PlaceOfReceipt).ToList(),
                "PlaceOfReceipt" => t.OrderBy(s => s.PlaceOfReceipt).ToList(),
                "TimeOfReceipt_desc" => t.OrderByDescending(s => s.TimeOfReceipt).ToList(),
                "TimeOfReceipt" => t.OrderBy(s => s.TimeOfReceipt).ToList(),
                "PlaceOfDelivery_desc" => t.OrderByDescending(s => s.PlaceOfDelivery).ToList(),
                "PlaceOfDelivery" => t.OrderBy(s => s.PlaceOfDelivery).ToList(),
                "TimeOfDelivery_desc" => t.OrderByDescending(s => s.TimeOfDelivery).ToList(),
                "TimeOfDelivery" => t.OrderBy(s => s.TimeOfDelivery).ToList(),
                "OtherStops_desc" => t.OrderByDescending(s => s.OtherStops).ToList(),
                "OtherStops" => t.OrderBy(s => s.OtherStops).ToList(),
                "CargoId_desc" => t.OrderByDescending(s => s.CargoId).ToList(),
                "CargoId" => t.OrderBy(s => s.CargoId).ToList(),
                "StorageTime_desc" => t.OrderByDescending(s => s.StorageTime).ToList(),
                "StorageTime" => t.OrderBy(s => s.StorageTime).ToList(),
                "Completed_desc" => t.OrderByDescending(s => s.Completed).ToList(),
                "Completed" => t.OrderBy(s => s.Completed).ToList(),
                "CompletionTime_desc" => t.OrderByDescending(s => s.CompletionTime).ToList(),
                "CompletionTime" => t.OrderBy(s => s.CompletionTime).ToList(),
                "TimeOfDelay_desc" => t.OrderByDescending(s => s.TimeOfDelay).ToList(),
                "TimeOfDelay" => t.OrderBy(s => s.TimeOfDelay).ToList(),
                "Payment_desc" => t.OrderByDescending(s => s.Payment).ToList(),
                "Payment" => t.OrderBy(s => s.Payment).ToList(),
                "FinalPayment_desc" => t.OrderByDescending(s => s.FinalPayment).ToList(),
                "FinalPayment" => t.OrderBy(s => s.FinalPayment).ToList(),
                "Penalty_desc" => t.OrderByDescending(s => s.Penalty).ToList(),
                "Penalty" => t.OrderBy(s => s.Penalty).ToList(),
                "Date_desc" => t.OrderByDescending(s => s.Date).ToList(),
                _ => t.OrderBy(s => s.Date).ToList(),
            };

            return t.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        }

        public async Task<DeliveryTask?> GetByIdAsync(int id)
        {
            return await _context.Tasks.FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<List<DeliveryTask>> GetTasksAsync()
        {
            return await _context.Tasks.ToListAsync();
        }

        //gets the data of the charts
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

        public async Task PostAsync(DeliveryTask t)
        {
            t.FinalPayment = (t.Payment != null ? t.Payment : 0) - (t.Penalty != null ? t.Penalty : 0);

            if (t.Completed)
            {
                t.CompletionTime = DateTime.Now;
            }
            else
            {
                t.CompletionTime = null;
            }

            t.Cargo = await _context.Cargoes.FirstOrDefaultAsync(x => x.Id == t.CargoId);

            _context.Add(t);
            await _context.SaveChangesAsync();
        }

        public async Task PutAsync(DeliveryTask t)
        {
            t.FinalPayment = (t.Payment != null ? t.Payment : 0) - (t.Penalty != null ? t.Penalty : 0);

            if (t.Completed)
            {
                t.CompletionTime = DateTime.Now;
            }
            else { t.CompletionTime = null; }

            _context.Entry(t).State = EntityState.Modified;

            var cargo = _context.Cargoes.FirstOrDefault(a => a.Id == t.CargoId);

            if (cargo != null)
            {
                cargo.TaskId = t.Id;
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

        //change task completion to the opposite
        public async Task ChangeCompletionAsync(DeliveryTask t)
        {
            t.Completed = !t.Completed;

            if (t.Completed)
            {
                t.CompletionTime = DateTime.Now;
            }
            else { t.CompletionTime = null; }

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
                worksheet.Cell(currentRow, 4).Value = task.PlaceOfReceipt;
                worksheet.Cell(currentRow, 5).Value = task.TimeOfReceipt;
                worksheet.Cell(currentRow, 6).Value = task.PlaceOfDelivery;
                worksheet.Cell(currentRow, 7).Value = task.TimeOfDelivery;
                worksheet.Cell(currentRow, 8).Value = task.OtherStops;
                worksheet.Cell(currentRow, 9).Value = task.Cargo;
                worksheet.Cell(currentRow, 10).Value = task.StorageTime;
                worksheet.Cell(currentRow, 11).Value = task.Completed;
                worksheet.Cell(currentRow, 12).Value = task.CompletionTime;
                worksheet.Cell(currentRow, 13).Value = task.TimeOfDelay;
                worksheet.Cell(currentRow, 14).Value = task.Payment + (task.Payment != null ? " HUF" : "");
                worksheet.Cell(currentRow, 15).Value = task.FinalPayment + (task.FinalPayment != null ? " HUF" : "");
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

            System.Type type = typeof(DeliveryTask);
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

                foreach (Shared.Model.DeliveryTask task in tasks)
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
                    if (!string.IsNullOrEmpty(task.PlaceOfReceipt?.ToString())) { s = task.PlaceOfReceipt.ToString(); }
                    else { s = "-"; }
                    table.AddCell(new PdfPCell(new Phrase(s.ToString(), font2))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    });
                    if (!string.IsNullOrEmpty(task.TimeOfReceipt.ToString())) { s = task.TimeOfReceipt.ToString(); }
                    else { s = "-"; }
                    table.AddCell(new PdfPCell(new Phrase(s?.ToString(), font2))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    });
                    if (!string.IsNullOrEmpty(task.PlaceOfDelivery?.ToString())) { s = task.PlaceOfDelivery.ToString(); }
                    else { s = "-"; }
                    table.AddCell(new PdfPCell(new Phrase(s.ToString(), font2))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    });
                    if (!string.IsNullOrEmpty(task.TimeOfDelivery.ToString())) { s = task.TimeOfDelivery.ToString(); }
                    else { s = "-"; }
                    table.AddCell(new PdfPCell(new Phrase(s?.ToString(), font2))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    });
                    if (!string.IsNullOrEmpty(task.OtherStops)) { s = task.OtherStops.ToString(); }
                    else { s = "-"; }
                    table.AddCell(new PdfPCell(new Phrase(s?.ToString(), font2))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    });
                    if (!string.IsNullOrEmpty(task.CargoId.ToString())) { s = task.CargoId.ToString(); }
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


                foreach (DeliveryTask task in tasks)
                {
                    var s = "";
                    if (!string.IsNullOrEmpty(task.Id.ToString())) { s = task.Id.ToString(); }
                    else { s = "-"; }
                    table2.AddCell(new PdfPCell(new Phrase(s.ToString(), font2))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    });
                    if (!string.IsNullOrEmpty(task.StorageTime)) { s = task.StorageTime.ToString(); }
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
                    if (!string.IsNullOrEmpty(task.CompletionTime.ToString())) { s = task.CompletionTime.ToString(); }
                    else { s = "-"; }
                    table2.AddCell(new PdfPCell(new Phrase(s?.ToString(), font2))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    });
                    if (!string.IsNullOrEmpty(task.TimeOfDelay)) { s = task.TimeOfDelay.ToString(); }
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
                    if (!string.IsNullOrEmpty(task.FinalPayment.ToString())) { s = task.FinalPayment.ToString() + " HUF"; }
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

            //read the file as base64
            byte[] fileBytes = await File.ReadAllBytesAsync(filepath);
            string base64String = Convert.ToBase64String(fileBytes);
            File.Delete(filepath);

            return base64String;
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
                txt.Write((task.PlaceOfReceipt ?? ifNull) + separator);
                txt.Write((task.TimeOfReceipt != null ? task.TimeOfReceipt : ifNull) + separator);
                txt.Write((task.PlaceOfDelivery ?? ifNull) + separator);
                txt.Write((task.TimeOfDelivery != null ? task.TimeOfDelivery : ifNull) + separator);
                txt.Write((task.OtherStops ?? ifNull) + separator);
                txt.Write((task.Cargo != null ? task.Cargo : ifNull) + separator);
                txt.Write((task.StorageTime ?? ifNull) + separator);
                txt.Write(task.Completed + separator);
                txt.Write((task.CompletionTime != null ? task.CompletionTime : ifNull) + separator);
                txt.Write((task.TimeOfDelay ?? ifNull) + separator);
                txt.Write((task.Payment + "HUF" ?? ifNull) + separator);
                txt.Write((task.FinalPayment + "HUF" ?? ifNull) + separator);
                txt.Write((task.Penalty + "HUF" ?? ifNull) + separator);
                txt.Write(task.Date + separator);

                txt.Write("\n");

            }

            txt.Close();
            txt.Dispose();

            //change the encoding of the file
            string csvFileContents = await File.ReadAllTextAsync(filepath);
            byte[] buffer = Encoding.Convert(Encoding.Default, Encoding.UTF8, Encoding.Default.GetBytes(csvFileContents));
            string convertedCsvFileContents = Encoding.UTF8.GetString(buffer);
            await File.WriteAllTextAsync(filepath, convertedCsvFileContents, Encoding.UTF8);

            //read the file as base64
            byte[] fileBytes = await File.ReadAllBytesAsync(filepath);
            string base64String = Convert.ToBase64String(fileBytes);
            File.Delete(filepath);

            return base64String;
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
                        int rowNumber = 0;
                        foreach (IXLRow row in worksheet.Rows())
                        {

                            //Use the first row to add columns to DataTable with column names check.
                            if (firstRow)
                            {
                                //copy column names to a list
                                CultureInfo.CurrentUICulture = lang;
                                List<string> columnNames = _columnNameLists.GetTasksColumnNames().Select(x => _localizer[x].Value).ToList();

                                var cellValues = row.Cells().Select(c => c.Value.ToString()).ToList();
                                if (!columnNames.Where(cname  => cname != _localizer["Id"]).SequenceEqual(cellValues.Where(cname => cname != _localizer["Id"])))
                                {
                                    error = _localizer["Not_match_col"].Value;
                                    File.Delete(path); // delete the file
                                    return error;
                                }
                                dt.Columns.AddRange(cellValues.Select(cname => new DataColumn(cname)).ToArray());
                                columnNames.RemoveAll(cname => cellValues.Contains(cname));

                                firstRow = false;
                                if (columnNames.Count == 0 || (columnNames.Count == 1 && columnNames.Contains(_localizer["Id"])))
                                {
                                    haveColumns = true;
                                    if (columnNames.Count == 0)
                                    {
                                        l += 1;
                                    }
                                }

                            }
                            else if (haveColumns)
                            {
                                List<string?> list = new();
                                int nulls = 0;
                                //Add rows to DataTable.
                                dt.Rows.Add();
                                foreach (IXLCell cell in row.Cells(1, dt.Columns.Count))
                                {
                                    if (cell.Value != null && cell.Value.ToString() != "") 
                                    { 
                                        list.Add(cell.Value.ToString()); 
                                    }
                                    else
                                    {
                                        list.Add(null);
                                        nulls += 1;
                                    }
                                }

                                list[l + 12] = new String(list[l + 12]?.ToString()?.Where(Char.IsDigit).ToArray());
                                list[l + 13] = new String(list[l + 13]?.ToString()?.Where(Char.IsDigit).ToArray());
                                list[l + 14] = new String(list[l + 14]?.ToString()?.Where(Char.IsDigit).ToArray());

                                try
                                {
                                    if (nulls != list.Count)
                                    {
                                        ++rowNumber;

                                        var task = new DeliveryTask
                                        {
                                            UserId = "Imported",
                                            Partner = list[l],
                                            Description = list[l + 1],
                                            PlaceOfReceipt = list[l + 2],
                                            TimeOfReceipt =  !string.IsNullOrWhiteSpace(list[l + 3]) ? DateTime.Parse(list[l + 3] ?? "0") : null,
                                            PlaceOfDelivery = list[l + 4],
                                            TimeOfDelivery = !string.IsNullOrWhiteSpace(list[l + 5]) ? DateTime.Parse(list[l + 5] ?? "0") : null,
                                            OtherStops = list[l + 6],
                                            CargoId = !string.IsNullOrWhiteSpace(list[l + 7]) ? int.Parse(list[l + 7] ?? "0") : null,
                                            StorageTime = list[l + 8],
                                            Completed = bool.Parse(list[l + 9] ?? "False"),
                                            CompletionTime = !string.IsNullOrWhiteSpace(list[l + 10]) ? DateTime.Parse(list[l + 10] ?? "0") : null,
                                            TimeOfDelay = list[l + 11],
                                            Payment = !string.IsNullOrWhiteSpace(list[l + 12]) ? long.Parse(list[l + 12] ?? "0") : null,
                                            FinalPayment = !string.IsNullOrWhiteSpace(list[l + 13]) ? long.Parse(list[l + 13] ?? "0") : null,
                                            Penalty = !string.IsNullOrWhiteSpace(list[l + 14]) ? long.Parse(list[l + 14] ?? "0") : null,
                                            Date = DateTime.Now
                                        };

                                        if (task != null)
                                        {
                                            if (task?.CargoId != null)
                                            {
                                                Cargo? cargo = await _context.Cargoes.FirstOrDefaultAsync(x => x.Id == task.CargoId);
                                                
                                                if (cargo != null)
                                                { 
                                                    var WithNewId = await _context.Tasks.Where(x => x.CargoId == task.CargoId).ToListAsync();

                                                    if (WithNewId.Count == 0)
                                                    {
                                                        task.Cargo = cargo;
                                                        await _context.Tasks.AddAsync(task);

                                                        cargo.TaskId = task.Id;
                                                        _context.Entry(cargo).State = EntityState.Modified;
                                                        await _context.SaveChangesAsync();
                                                    }
                                                    else
                                                    {
                                                        error += "\n " + _localizer["Deleted_duplicate_id"] + " " + rowNumber + ".";
                                                    }
                                                }
                                                else
                                                {
                                                    error += "\n " + _localizer["Deleted_wrong_id"] + " " + rowNumber + ".";
                                                }
                                            }
                                            else
                                            {
                                                await _context.Tasks.AddAsync(task!);
                                                await _context.SaveChangesAsync();
                                            }
                                        }
                                        else if (task == null)
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
            return error.TrimStart('\r', '\n');
        }
    }
}

