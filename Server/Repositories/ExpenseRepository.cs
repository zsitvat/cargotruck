using Cargotruck.Server.Data;
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
using Type = Cargotruck.Shared.Model.Type;

namespace Cargotruck.Server.Repositories
{
    public class ExpenseRepository : IExpenseRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IStringLocalizer<Resource> _localizer;
        private readonly IColumnNamesService _columnNameLists;
        private readonly IErrorHandlerService _errorHandler;

        public ExpenseRepository(ApplicationDbContext context, IStringLocalizer<Resource> localizer, IColumnNamesService columnNameLists, IErrorHandlerService errorHandler)
        {
            _context = context;
            _localizer = localizer;
            _columnNameLists = columnNameLists;
            _errorHandler = errorHandler;
        }

        private async Task<List<Expense>> GetDataAsync(string? searchString, string? filter, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate)
        {
            var data = await _context.Expenses.Where(s => (dateFilterStartDate != null ? (s.Date >= dateFilterStartDate) : true) && (dateFilterEndDate != null ? (s.Date <= dateFilterEndDate) : true)).ToListAsync();

            if (filter != null)
            {
                data = data.Where(data => data.Type.ToString() == filter).ToList();
            }

            searchString = searchString?.ToLower();
            if (searchString != null && searchString != "")
            {
                data = data.Where(s =>
               s.Type.ToString()!.ToLower().Contains(searchString)
            || s.TypeId.ToString()!.ToLower().Contains(searchString)
            || (s.Fuel != null && s.Fuel.ToString()!.Contains(searchString))
            || (s.RoadFees != null && s.RoadFees.ToString()!.Contains(searchString))
            || (s.Penalty != null && s.Fuel.ToString()!.Contains(searchString))
            || (s.DriverSpending != null && s.DriverSpending.ToString()!.Contains(searchString))
            || (s.DriverSalary != null && s.DriverSalary.ToString()!.Contains(searchString))
            || (s.RepairCost != null && s.RepairCost.ToString()!.Contains(searchString))
            || (s.RepairDescription != null && s.RepairDescription.ToString()!.Contains(searchString))
            || (s.CostOfStorage != null && s.CostOfStorage.ToString()!.Contains(searchString))
            || (s.Other != null && s.Other.ToString()!.Contains(searchString))
            || (s.TotalAmount != null && s.TotalAmount.ToString()!.Contains(searchString))
            ).ToList();
            }

            return data;
        }

        public async Task<List<Expense>> GetAsync(int page, int pageSize, string sortOrder, bool desc, string? searchString, string? filter, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate)
        {

            var data = await GetDataAsync(searchString, filter, dateFilterStartDate, dateFilterEndDate);

            sortOrder = sortOrder == "Type" ? (desc ? "Type_desc" : "Type") : (sortOrder);
            sortOrder = sortOrder == "TypeId" ? (desc ? "TypeId_desc" : "TypeId") : (sortOrder);
            sortOrder = sortOrder == "Fuel" ? (desc ? "Fuel_desc" : "Fuel") : (sortOrder);
            sortOrder = sortOrder == "RoadFees" ? (desc ? "RoadFees_desc" : "RoadFees") : (sortOrder);
            sortOrder = sortOrder == "Penalty" ? (desc ? "Penalty_desc" : "Penalty") : (sortOrder);
            sortOrder = sortOrder == "DriverSpending" ? (desc ? "DriverSpending_desc" : "DriverSpending") : (sortOrder);
            sortOrder = sortOrder == "DriverSalary" ? (desc ? "DriverSalary_desc" : "DriverSalary") : (sortOrder);
            sortOrder = sortOrder == "RepairCost" ? (desc ? "RepairCost_desc" : "RepairCost") : (sortOrder);
            sortOrder = sortOrder == "RepairDescription" ? (desc ? "RepairDescription_desc" : "RepairDescription") : (sortOrder);
            sortOrder = sortOrder == "CostOfStorage" ? (desc ? "CostOfStorage_desc" : "CostOfStorage") : (sortOrder);
            sortOrder = sortOrder == "Other" ? (desc ? "Other_desc" : "Other") : (sortOrder);
            sortOrder = sortOrder == "TotalAmount" ? (desc ? "TotalAmount_desc" : "TotalAmount") : (sortOrder);
            sortOrder = sortOrder == "Date" || String.IsNullOrEmpty(sortOrder) ? (desc ? "Date_desc" : "") : (sortOrder);

            data = sortOrder switch
            {
                "Type_desc" => data.OrderByDescending(s => s.Type).ToList(),
                "Type" => data.OrderBy(s => s.Type).ToList(),
                "TypeId_desc" => data.OrderByDescending(s => s.TypeId).ToList(),
                "TypeId" => data.OrderBy(s => s.TypeId).ToList(),
                "Fuel_desc" => data.OrderByDescending(s => s.Fuel).ToList(),
                "Fuel" => data.OrderBy(s => s.Fuel).ToList(),
                "RoadFees_desc" => data.OrderByDescending(s => s.RoadFees).ToList(),
                "RoadFees" => data.OrderBy(s => s.RoadFees).ToList(),
                "Penalty_desc" => data.OrderByDescending(s => s.Penalty).ToList(),
                "Penalty" => data.OrderBy(s => s.Penalty).ToList(),
                "DriverSpending_desc" => data.OrderByDescending(s => s.DriverSpending).ToList(),
                "DriverSpending" => data.OrderBy(s => s.DriverSpending).ToList(),
                "DriverSalary_desc" => data.OrderByDescending(s => s.DriverSalary).ToList(),
                "DriverSalary" => data.OrderBy(s => s.DriverSalary).ToList(),
                "RepairCost_desc" => data.OrderByDescending(s => s.RepairCost).ToList(),
                "RepairCost" => data.OrderBy(s => s.RepairCost).ToList(),
                "RepairDescription_desc" => data.OrderByDescending(s => s.RepairDescription).ToList(),
                "RepairDescription" => data.OrderBy(s => s.RepairDescription).ToList(),
                "CostOfStorage_desc" => data.OrderByDescending(s => s.CostOfStorage).ToList(),
                "CostOfStorage" => data.OrderBy(s => s.CostOfStorage).ToList(),
                "Other_desc" => data.OrderByDescending(s => s.Other).ToList(),
                "Other" => data.OrderBy(s => s.Other).ToList(),
                "TotalAmount_desc" => data.OrderByDescending(s => s.TotalAmount).ToList(),
                "TotalAmount" => data.OrderBy(s => s.TotalAmount).ToList(),
                "Date_desc" => data.OrderByDescending(s => s.Date).ToList(),
                _ => data.OrderBy(s => s.Date).ToList(),
            };

            return data.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        }

        public async Task<Expense?> GetByIdAsync(int id)
        {
            return await _context.Expenses.FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<List<Expense>> GetExpensesAsync()
        {
            return await _context.Expenses.ToListAsync();
        }

        public async Task<int> CountAsync()
        {
           return await _context.Expenses.CountAsync();
        }

        public async Task<int> PageCountAsync(string? searchString, string? filter, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate)
        {
            var data = await GetDataAsync(searchString, filter, dateFilterStartDate, dateFilterEndDate);
            return data.Count;
        }

        public async Task PostAsync(Expense data)
        {
            data.TotalAmount = GetTotalAmount(data);
            _context?.Add(data);
            await _context?.SaveChangesAsync()!;

            if (data.Type.ToString() == Type.repair.ToString())
            {
                var road = _context.Roads.FirstOrDefault(a => a.Id == data.TypeId);
                if (road != null)
                {
                    road.ExpensesId = data.Id;
                    _context.Entry(road).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                }
            }
        }

        public async Task PutAsync(Expense data)
        {
            data.TotalAmount = GetTotalAmount(data);
            _context.Entry(data).State = EntityState.Modified;
            if (data.Type.ToString() == Type.repair.ToString())
            {
                var road = _context.Roads.FirstOrDefault(a => a.Id == data.TypeId);
                if (road != null)
                {
                    road.ExpensesId = data.Id;
                    _context.Entry(road).State = EntityState.Modified;
                }
            }
            await _context.SaveChangesAsync();
        }

        private int? GetTotalAmount(Expense data)
        {
            var totalAmount = (data.Fuel != null ? data.Fuel : 0) + (data.RoadFees != null ? data.RoadFees : 0) + (data.Penalty != null ? data.Penalty : 0) +
                (data.CostOfStorage != null ? data.CostOfStorage : 0) + (data.DriverSalary != null ? data.DriverSalary : 0) +
                (data.DriverSpending != null ? data.DriverSpending : 0) + (data.Other != null ? data.Other : 0) + (data.RepairCost != null ? data.RepairCost : 0);
            return totalAmount != null ? totalAmount : 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var t = _context.Expenses.FirstOrDefault(x => x.Id == id);
            if (t != null)
            {
                _context.Expenses.Remove(t);
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }


        public string ExportToExcel(CultureInfo lang, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate)
        {
            var expenses = _context.Expenses.Where(s => (dateFilterStartDate != null ? (s.Date >= dateFilterStartDate) : true) && (dateFilterEndDate != null ? (s.Date <= dateFilterEndDate) : true));

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Expenses");
            var currentRow = 1;


            CultureInfo.CurrentUICulture = lang;
            List<string> columnNames = _columnNameLists.GetExpensesColumnNames().Select(x => _localizer[x].Value).ToList();

            for (var i = 0; i < columnNames.Count; i++)
            {
                worksheet.Cell(currentRow, i + 1).Value = columnNames[i];
                worksheet.Cell(currentRow, i + 1).Style.Font.SetBold();
            }

            foreach (var expense in expenses)
            {
                currentRow++;
                worksheet.Cell(currentRow, 1).Value = expense.Id;
                worksheet.Cell(currentRow, 2).Value = expense.Type;
                worksheet.Cell(currentRow, 3).Value = expense.TypeId;
                worksheet.Cell(currentRow, 4).Value = expense.Fuel + (expense.Fuel != null ? " HUF" : "");
                worksheet.Cell(currentRow, 5).Value = expense.RoadFees + (expense.RoadFees != null ? " HUF" : "");
                worksheet.Cell(currentRow, 6).Value = expense.Penalty + (expense.Penalty != null ? " HUF" : "");
                worksheet.Cell(currentRow, 7).Value = expense.DriverSpending + (expense.DriverSpending != null ? " HUF" : "");
                worksheet.Cell(currentRow, 8).Value = expense.DriverSalary + (expense.DriverSalary != null ? " HUF" : "");
                worksheet.Cell(currentRow, 9).Value = expense.RepairCost + (expense.RepairCost != null ? " HUF" : "");
                worksheet.Cell(currentRow, 10).Value = expense.RepairDescription;
                worksheet.Cell(currentRow, 11).Value = expense.CostOfStorage + (expense.CostOfStorage != null ? " HUF" : "");
                worksheet.Cell(currentRow, 12).Value = expense.Other + (expense.Other != null ? " HUF" : "");
                worksheet.Cell(currentRow, 13).Value = expense.TotalAmount + (expense.TotalAmount != null ? " HUF" : "");
                worksheet.Cell(currentRow, 14).Value = expense.Date;
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            var content = stream.ToArray();

            return Convert.ToBase64String(content);
        }

        public async Task<string> ExportToPdfAsync(CultureInfo lang, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate)
        {
            var expenses = _context.Expenses.Where(s => (dateFilterStartDate != null ? (s.Date >= dateFilterStartDate) : true) && (dateFilterEndDate != null ? (s.Date <= dateFilterEndDate) : true));

            int pdfRowIndex = 1;
            Random rnd = new();
            int random = rnd.Next(1000000, 9999999);
            string filename = "Expenses" + random + "_" + DateTime.Now.ToString("dd-MM-yyyy");
            string filepath = "Files/" + filename + ".pdf";

            Document document = new(PageSize.A4, 5f, 5f, 10f, 10f);
            FileStream fs = new(filepath, FileMode.Create);
            PdfWriter writer = PdfWriter.GetInstance(document, fs);
            document.Open();

            Font font1 = FontFactory.GetFont(FontFactory.TIMES_BOLD, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 12);
            Font font2 = FontFactory.GetFont(FontFactory.TIMES_ROMAN, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 10);

            System.Type type = typeof(Expense);
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
            List<string> columnNames = _columnNameLists.GetExpensesColumnNames().Where(x => x != "Repair_description").Select(x => _localizer[x].Value).ToList();

            var title = new Paragraph(15, _localizer["Expenses"].Value)
            {
                Alignment = Element.ALIGN_CENTER
            };


            document.Add(title);
            document.Add(new Paragraph("\n"));

            if (expenses.Any())
            {
                foreach (var name in columnNames.Take(column_number))
                {
                    table.AddCell(new PdfPCell(new Phrase(name, font1))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    });
                }

                foreach (Expense expense in expenses)
                {
                    var s = "";
                    if (!string.IsNullOrEmpty(expense.Id.ToString())) { s = expense.Id.ToString(); }
                    else { s = "-"; }
                    table.AddCell(new PdfPCell(new Phrase(s?.ToString(), font2))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    });
                    if (!string.IsNullOrEmpty(expense.Type.ToString())) { s = expense.Type.ToString(); }
                    else { s = "-"; }
                    table.AddCell(new PdfPCell(new Phrase(s?.ToString(), font2))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    });
                    if (!string.IsNullOrEmpty(expense.TypeId.ToString())) { s = expense.TypeId.ToString(); }
                    else { s = "-"; }
                    table.AddCell(new PdfPCell(new Phrase(s?.ToString(), font2))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    });
                    if (!string.IsNullOrEmpty(expense.Fuel.ToString())) { s = expense.Fuel.ToString() + " HUF"; }
                    else { s = "-"; }
                    table.AddCell(new PdfPCell(new Phrase(s?.ToString(), font2))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    });
                    if (!string.IsNullOrEmpty(expense.RoadFees.ToString())) { s = expense.RoadFees.ToString() + " HUF"; }
                    else { s = "-"; }
                    table.AddCell(new PdfPCell(new Phrase(s?.ToString(), font2))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    });
                    if (!string.IsNullOrEmpty(expense.Penalty.ToString())) { s = expense.Penalty.ToString() + " HUF"; }
                    else { s = "-"; }
                    table.AddCell(new PdfPCell(new Phrase(s?.ToString(), font2))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    });
                    if (!string.IsNullOrEmpty(expense.DriverSpending.ToString())) { s = expense.DriverSpending.ToString() + " HUF"; }
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


                foreach (Expense expense in expenses)
                {
                    var s = "";
                    if (!string.IsNullOrEmpty(expense.Id.ToString())) { s = expense.Id.ToString(); }
                    else { s = "-"; }
                    table2.AddCell(new PdfPCell(new Phrase(s?.ToString(), font2))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    });
                    if (!string.IsNullOrEmpty(expense.DriverSalary.ToString())) { s = expense.DriverSalary.ToString() + " HUF"; }
                    else { s = "-"; }
                    table2.AddCell(new PdfPCell(new Phrase(s?.ToString(), font2))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    });
                    if (!string.IsNullOrEmpty(expense.RepairCost.ToString())) { s = expense.RepairCost.ToString() + " HUF"; }
                    else { s = "-"; }
                    table2.AddCell(new PdfPCell(new Phrase(s?.ToString(), font2))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    });
                    if (!string.IsNullOrEmpty(expense.CostOfStorage.ToString())) { s = expense.CostOfStorage.ToString() + " HUF"; }
                    else { s = "-"; }
                    table2.AddCell(new PdfPCell(new Phrase(s?.ToString(), font2))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    });
                    if (!string.IsNullOrEmpty(expense.Other.ToString())) { s = expense.Other.ToString() + " HUF"; }
                    else { s = "-"; }
                    table2.AddCell(new PdfPCell(new Phrase(s?.ToString(), font2))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    });
                    if (!string.IsNullOrEmpty(expense.Other.ToString())) { s = expense.TotalAmount.ToString() + " HUF"; }
                    else { s = "-"; }
                    table2.AddCell(new PdfPCell(new Phrase(s?.ToString(), font2))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    });
                    if (!string.IsNullOrEmpty(expense.Date.ToString())) { s = expense.Date.ToString(); }
                    else { s = "-"; }
                    table2.AddCell(new PdfPCell(new Phrase(s?.ToString(), font2))
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

            byte[] fileBytes = await File.ReadAllBytesAsync(filepath);
            string base64String = Convert.ToBase64String(fileBytes);
            File.Delete(filepath);

            return base64String;
        }

        public async Task<string> ExportToCSVAsync(CultureInfo lang, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate, bool isTextDocument)
        {
            var expenses = _context.Expenses.Where(s => (dateFilterStartDate != null ? (s.Date >= dateFilterStartDate) : true) && (dateFilterEndDate != null ? (s.Date <= dateFilterEndDate) : true));

            Random rnd = new();
            int random = rnd.Next(1000000, 9999999);
            string filename = "Expenses" + random + "_" + DateTime.Now.ToString("dd-MM-yyyy");
            string filepath = "Files/" + filename + ".csv";

            StreamWriter txt = new(filepath);
            //copy column names to a list based on language
            CultureInfo.CurrentUICulture = lang;
            List<string> columnNames = _columnNameLists.GetExpensesColumnNames().Select(x => _localizer[x].Value).ToList();

            string separator = isTextDocument ? "\t" : ";";
            string ifNull = isTextDocument ? " --- " : "";

            foreach (var name in columnNames)
            {
                 txt.Write(name + (isTextDocument ? "  " : separator));
            }

            txt.Write("\n");

            foreach (var expense in expenses)
            {
                txt.Write(expense.Id + separator);
                txt.Write((expense.Type != null ? expense.Type : ifNull) + separator);
                txt.Write((expense.TypeId != null ? expense.TypeId : ifNull) + separator);
                txt.Write(expense.Fuel + (expense.Fuel != null ? " HUF" : ifNull) + separator);
                txt.Write(expense.RoadFees + (expense.RoadFees != null ? " HUF" : ifNull) + separator);
                txt.Write(expense.Penalty + (expense.Penalty != null ? " HUF" : ifNull) + separator);
                txt.Write(expense.DriverSpending + (expense.DriverSpending != null ? " HUF" : ifNull) + separator);
                txt.Write(expense.DriverSalary + (expense.DriverSalary != null ? " HUF" : ifNull) + separator);
                txt.Write(expense.RepairCost + (expense.RepairCost != null ? " HUF" : ifNull) + separator);
                txt.Write((expense.RepairDescription != null ? expense.RepairDescription : ifNull) + separator);
                txt.Write(expense.CostOfStorage + (expense.CostOfStorage != null ? " HUF" : ifNull) + separator);
                txt.Write(expense.Other + (expense.Other != null ? " HUF" : ifNull) + separator);
                txt.Write(expense.Other + (expense.TotalAmount != null ? " HUF" : ifNull) + separator);
                txt.Write(expense.Date + separator);
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

        [HttpPost]
        public async Task<string?> ImportAsync([FromBody] string file, CultureInfo lang)
        {
            CultureInfo.CurrentUICulture = lang;
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
                                List<string> columnNames = _columnNameLists.GetExpensesColumnNames().Select(x => _localizer[x].Value).ToList();

                                var cellValues = row.Cells().Select(c => c.Value.ToString()).ToList();
                                if (!columnNames.SequenceEqual(cellValues))
                                {
                                    error = _localizer["Not_match_col"].Value;
                                    File.Delete(path); // delete the file
                                    return error;
                                }
                                dt.Columns.AddRange(cellValues.Select(c => new DataColumn(c)).ToArray());
                                columnNames.Clear();

                                firstRow = false;
                                if (columnNames.Count == 0 || (columnNames.Count == 1 && columnNames.Contains("Id")))
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

                                try
                                {
                                    if (nulls != list.Count)
                                    {

                                        list[l] = list[l] switch
                                        {
                                            "task" => "0",
                                            "repair" => "1",
                                            "storage" => "2",
                                            "salary" => "3",
                                            "othertype" => "4",
                                            _ => throw new ArgumentNullException(_localizer["Error_type"])
                                        };

                                        //count the totalamount
                                        var totalAmount = 0;
                                        for (int i = l + 2; i < list.Count - 1; i++)
                                        {
                                            if (i != (l + 8) && list[i] != null && list[i] != null)
                                            {
                                                list[i] = new String(list[i]?.ToString()?.Where(Char.IsDigit).ToArray());
                                                if (list[i] != null && i < l + 11)
                                                {
                                                    totalAmount += Int32.Parse(list[i]?.ToString()!);
                                                }
                                            }
                                        }

                                        ++rowNumber;

                                        var expense = new Expense
                                        {
                                            UserId = "Imported",
                                            Type = !string.IsNullOrWhiteSpace(list[l]) ? (Type)Enum.Parse(typeof(Type), list[l]!) : Type.othertype,
                                            TypeId = !string.IsNullOrWhiteSpace(list[l + 1]) ? int.Parse(list[l + 1] ?? "0") : null,
                                            Fuel = !string.IsNullOrWhiteSpace(list[l + 2]) ? int.Parse(list[l + 2] ?? "0") : null,
                                            RoadFees = !string.IsNullOrWhiteSpace(list[l + 3]) ? int.Parse(list[l + 3] ?? "0") : null,
                                            Penalty = !string.IsNullOrWhiteSpace(list[l + 4]) ? int.Parse(list[l + 4] ?? "0") : null,
                                            DriverSpending = !string.IsNullOrWhiteSpace(list[l + 5]) ? int.Parse(list[l + 5] ?? "0") : null,
                                            DriverSalary = !string.IsNullOrWhiteSpace(list[l + 6]) ? int.Parse(list[l + 6] ?? "0") : null,
                                            RepairCost = !string.IsNullOrWhiteSpace(list[l + 7]) ? int.Parse(list[l + 7] ?? "0") : null,
                                            RepairDescription = list[l + 8],
                                            CostOfStorage = !string.IsNullOrWhiteSpace(list[l + 9]) ? int.Parse(list[l + 9] ?? "0") : null,
                                            Other = !string.IsNullOrWhiteSpace(list[l + 10]) ? int.Parse(list[l + 10] ?? "0") : null,
                                            TotalAmount = totalAmount,
                                            Date = DateTime.Now
                                        };
                                      

                                        if (expense != null && expense.Type != null)
                                        {
                                            bool saveable = true;
                                            DeliveryTask? task = await _context.Tasks.FirstOrDefaultAsync(x => x.Id == expense.TypeId && expense.Type == Type.task);
                                            Road? road = await _context.Roads.FirstOrDefaultAsync(x => x.Id == expense.TypeId && expense.Type == Type.repair);
                                            Cargo? cargo = await _context.Cargoes.FirstOrDefaultAsync(x => x.Id == expense.TypeId && expense.Type == Type.storage);

                                            if (saveable && expense?.Type == Type.task && expense?.TypeId != null)
                                            {
                                                var withNewId = await _context.Expenses.Where(x => x.Type == Type.task && x.TypeId == expense.TypeId).ToListAsync();

                                                if (withNewId.Count != 0)
                                                {
                                                    saveable = false;
                                                    error += "\n " + _localizer["Deleted_duplicate_id"] + " " + rowNumber + ".";
                                                }
                                                else if (task == null)
                                                {
                                                    saveable = false;
                                                    error += "\n " + _localizer["Deleted_wrong_id"] + " " + rowNumber + ".";
                                                }
                                            }

                                            if (saveable && expense?.Type == Type.repair && expense.TypeId != null)
                                            {
                                                var withNewId = await _context.Expenses.Where(x => x.Type == Type.repair && x.Id == expense.TypeId).ToListAsync();

                                                if (withNewId.Count != 0)
                                                {
                                                    saveable = false;
                                                    error += "\n " + _localizer["Deleted_duplicate_id"] + " " + rowNumber + ".";
                                                }
                                                else if (road == null)
                                                {
                                                    saveable = false;
                                                    error += "\n " + _localizer["Deleted_wrong_id"] + " " + rowNumber + ".";
                                                }
                                            }

                                            if (saveable && expense?.Type == Type.storage && expense?.TypeId != null)
                                            {
                                                var withNewId = await _context.Expenses.Where(x => x.Type == Type.storage && x.Id == expense.TypeId).ToListAsync();

                                                if (withNewId.Count != 0)
                                                {
                                                    saveable = false;
                                                    error += "\n " + _localizer["Deleted_duplicate_id"] + " " + rowNumber + ".";
                                                }
                                                else if (cargo == null)
                                                {
                                                    saveable = false;
                                                    error += "\n " + _localizer["Deleted_wrong_id"] + " " + rowNumber + ".";
                                                }
                                            }

                                            if (saveable)
                                            {
                                                if (expense?.TypeId != null && road != null)
                                                {
                                                    road.ExpensesId = expense.Id;
                                                    _context.Entry(road).State = EntityState.Modified;
                                                }

                                                await _context.Expenses.AddAsync(expense!);
                                                await _context.SaveChangesAsync();
                                            }
                                        }
                                        else if (expense == null)
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

