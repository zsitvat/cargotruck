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

        private async Task<List<Expenses>> GetDataAsync(string? searchString, string? filter, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate)
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
            || s.Type_id.ToString()!.ToLower().Contains(searchString)
            || (s.Fuel != null && s.Fuel.ToString()!.Contains(searchString))
            || (s.Road_fees != null && s.Road_fees.ToString()!.Contains(searchString))
            || (s.Penalty != null && s.Fuel.ToString()!.Contains(searchString))
            || (s.Driver_spending != null && s.Driver_spending.ToString()!.Contains(searchString))
            || (s.Driver_salary != null && s.Driver_salary.ToString()!.Contains(searchString))
            || (s.Repair_cost != null && s.Repair_cost.ToString()!.Contains(searchString))
            || (s.Repair_description != null && s.Repair_description.ToString()!.Contains(searchString))
            || (s.Cost_of_storage != null && s.Cost_of_storage.ToString()!.Contains(searchString))
            || (s.Other != null && s.Other.ToString()!.Contains(searchString))
            || (s.Total_amount != null && s.Total_amount.ToString()!.Contains(searchString))
            ).ToList();
            }

            return data;
        }

        public async Task<List<Expenses>> GetAsync(int page, int pageSize, string sortOrder, bool desc, string? searchString, string? filter, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate)
        {

            var data = await GetDataAsync(searchString, filter, dateFilterStartDate, dateFilterEndDate);

            sortOrder = sortOrder == "Type" ? (desc ? "Type_desc" : "Type") : (sortOrder);
            sortOrder = sortOrder == "Type_id" ? (desc ? "Type_id_desc" : "Type_id") : (sortOrder);
            sortOrder = sortOrder == "Fuel" ? (desc ? "Fuel_desc" : "Fuel") : (sortOrder);
            sortOrder = sortOrder == "Road_fees" ? (desc ? "Road_fees_desc" : "Road_fees") : (sortOrder);
            sortOrder = sortOrder == "Penalty" ? (desc ? "Penalty_desc" : "Penalty") : (sortOrder);
            sortOrder = sortOrder == "Driver_spending" ? (desc ? "Driver_spending_desc" : "Driver_spending") : (sortOrder);
            sortOrder = sortOrder == "Driver_salary" ? (desc ? "Driver_salary_desc" : "Driver_salary") : (sortOrder);
            sortOrder = sortOrder == "Repair_cost" ? (desc ? "Repair_cost_desc" : "Repair_cost") : (sortOrder);
            sortOrder = sortOrder == "Repair_description" ? (desc ? "Repair_description_desc" : "Repair_description") : (sortOrder);
            sortOrder = sortOrder == "Cost_of_storage" ? (desc ? "Cost_of_storage_desc" : "Cost_of_storage") : (sortOrder);
            sortOrder = sortOrder == "Other" ? (desc ? "Other_desc" : "Other") : (sortOrder);
            sortOrder = sortOrder == "Total_amount" ? (desc ? "Total_amount_desc" : "Total_amount") : (sortOrder);
            sortOrder = sortOrder == "Date" || String.IsNullOrEmpty(sortOrder) ? (desc ? "Date_desc" : "") : (sortOrder);

            data = sortOrder switch
            {
                "Type_desc" => data.OrderByDescending(s => s.Type).ToList(),
                "Type" => data.OrderBy(s => s.Type).ToList(),
                "Type_id_desc" => data.OrderByDescending(s => s.Type_id).ToList(),
                "Type_id" => data.OrderBy(s => s.Type_id).ToList(),
                "Fuel_desc" => data.OrderByDescending(s => s.Fuel).ToList(),
                "Fuel" => data.OrderBy(s => s.Fuel).ToList(),
                "Road_fees_desc" => data.OrderByDescending(s => s.Road_fees).ToList(),
                "Road_fees" => data.OrderBy(s => s.Road_fees).ToList(),
                "Penalty_desc" => data.OrderByDescending(s => s.Penalty).ToList(),
                "Penalty" => data.OrderBy(s => s.Penalty).ToList(),
                "Driver_spending_desc" => data.OrderByDescending(s => s.Driver_spending).ToList(),
                "Driver_spending" => data.OrderBy(s => s.Driver_spending).ToList(),
                "Driver_salary_desc" => data.OrderByDescending(s => s.Driver_salary).ToList(),
                "Driver_salary" => data.OrderBy(s => s.Driver_salary).ToList(),
                "Repair_cost_desc" => data.OrderByDescending(s => s.Repair_cost).ToList(),
                "Repair_cost" => data.OrderBy(s => s.Repair_cost).ToList(),
                "Repair_description_desc" => data.OrderByDescending(s => s.Repair_description).ToList(),
                "Repair_description" => data.OrderBy(s => s.Repair_description).ToList(),
                "Cost_of_storage_desc" => data.OrderByDescending(s => s.Cost_of_storage).ToList(),
                "Cost_of_storage" => data.OrderBy(s => s.Cost_of_storage).ToList(),
                "Other_desc" => data.OrderByDescending(s => s.Other).ToList(),
                "Other" => data.OrderBy(s => s.Other).ToList(),
                "Total_amount_desc" => data.OrderByDescending(s => s.Total_amount).ToList(),
                "Total_amount" => data.OrderBy(s => s.Total_amount).ToList(),
                "Date_desc" => data.OrderByDescending(s => s.Date).ToList(),
                _ => data.OrderBy(s => s.Date).ToList(),
            };

            return data.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        }

        public async Task<Expenses?> GetByIdAsync(int id)
        {
            return await _context.Expenses.FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<List<Expenses>> GetExpensesAsync()
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

        public async Task PostAsync(Expenses data)
        {
            data.Total_amount = GetTotalAmount(data);
            _context?.Add(data);
            await _context?.SaveChangesAsync()!;

            if (data.Type.ToString() == Type.repair.ToString())
            {
                var road = _context.Roads.FirstOrDefault(a => a.Id == data.Type_id);
                if (road != null)
                {
                    road.Expenses_id = data.Id;
                    _context.Entry(road).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                }
            }
        }

        public async Task PutAsync(Expenses data)
        {
            data.Total_amount = GetTotalAmount(data);
            _context.Entry(data).State = EntityState.Modified;
            if (data.Type.ToString() == Type.repair.ToString())
            {
                var road = _context.Roads.FirstOrDefault(a => a.Id == data.Type_id);
                if (road != null)
                {
                    road.Expenses_id = data.Id;
                    _context.Entry(road).State = EntityState.Modified;
                }
            }
            await _context.SaveChangesAsync();
        }

        private int? GetTotalAmount(Expenses data)
        {
            var totalAmount = (data.Fuel != null ? data.Fuel : 0) + (data.Road_fees != null ? data.Road_fees : 0) + (data.Penalty != null ? data.Penalty : 0) +
                (data.Cost_of_storage != null ? data.Cost_of_storage : 0) + (data.Driver_salary != null ? data.Driver_salary : 0) +
                (data.Driver_spending != null ? data.Driver_spending : 0) + (data.Other != null ? data.Other : 0) + (data.Repair_cost != null ? data.Repair_cost : 0);
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
                worksheet.Cell(currentRow, 3).Value = expense.Type_id;
                worksheet.Cell(currentRow, 4).Value = expense.Fuel + (expense.Fuel != null ? " HUF" : "");
                worksheet.Cell(currentRow, 5).Value = expense.Road_fees + (expense.Road_fees != null ? " HUF" : "");
                worksheet.Cell(currentRow, 6).Value = expense.Penalty + (expense.Penalty != null ? " HUF" : "");
                worksheet.Cell(currentRow, 7).Value = expense.Driver_spending + (expense.Driver_spending != null ? " HUF" : "");
                worksheet.Cell(currentRow, 8).Value = expense.Driver_salary + (expense.Driver_salary != null ? " HUF" : "");
                worksheet.Cell(currentRow, 9).Value = expense.Repair_cost + (expense.Repair_cost != null ? " HUF" : "");
                worksheet.Cell(currentRow, 10).Value = expense.Repair_description;
                worksheet.Cell(currentRow, 11).Value = expense.Cost_of_storage + (expense.Cost_of_storage != null ? " HUF" : "");
                worksheet.Cell(currentRow, 12).Value = expense.Other + (expense.Other != null ? " HUF" : "");
                worksheet.Cell(currentRow, 13).Value = expense.Total_amount + (expense.Total_amount != null ? " HUF" : "");
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

            System.Type type = typeof(Expenses);
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

                foreach (Expenses expense in expenses)
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
                    if (!string.IsNullOrEmpty(expense.Type_id.ToString())) { s = expense.Type_id.ToString(); }
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
                    if (!string.IsNullOrEmpty(expense.Road_fees.ToString())) { s = expense.Road_fees.ToString() + " HUF"; }
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
                    if (!string.IsNullOrEmpty(expense.Driver_spending.ToString())) { s = expense.Driver_spending.ToString() + " HUF"; }
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


                foreach (Expenses expense in expenses)
                {
                    var s = "";
                    if (!string.IsNullOrEmpty(expense.Id.ToString())) { s = expense.Id.ToString(); }
                    else { s = "-"; }
                    table2.AddCell(new PdfPCell(new Phrase(s?.ToString(), font2))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    });
                    if (!string.IsNullOrEmpty(expense.Driver_salary.ToString())) { s = expense.Driver_salary.ToString() + " HUF"; }
                    else { s = "-"; }
                    table2.AddCell(new PdfPCell(new Phrase(s?.ToString(), font2))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    });
                    if (!string.IsNullOrEmpty(expense.Repair_cost.ToString())) { s = expense.Repair_cost.ToString() + " HUF"; }
                    else { s = "-"; }
                    table2.AddCell(new PdfPCell(new Phrase(s?.ToString(), font2))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    });
                    if (!string.IsNullOrEmpty(expense.Cost_of_storage.ToString())) { s = expense.Cost_of_storage.ToString() + " HUF"; }
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
                    if (!string.IsNullOrEmpty(expense.Other.ToString())) { s = expense.Total_amount.ToString() + " HUF"; }
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
                txt.Write((expense.Type_id != null ? expense.Type_id : ifNull) + separator);
                txt.Write(expense.Fuel + (expense.Fuel != null ? " HUF" : ifNull) + separator);
                txt.Write(expense.Road_fees + (expense.Road_fees != null ? " HUF" : ifNull) + separator);
                txt.Write(expense.Penalty + (expense.Penalty != null ? " HUF" : ifNull) + separator);
                txt.Write(expense.Driver_spending + (expense.Driver_spending != null ? " HUF" : ifNull) + separator);
                txt.Write(expense.Driver_salary + (expense.Driver_salary != null ? " HUF" : ifNull) + separator);
                txt.Write(expense.Repair_cost + (expense.Repair_cost != null ? " HUF" : ifNull) + separator);
                txt.Write((expense.Repair_description != null ? expense.Repair_description : ifNull) + separator);
                txt.Write(expense.Cost_of_storage + (expense.Cost_of_storage != null ? " HUF" : ifNull) + separator);
                txt.Write(expense.Other + (expense.Other != null ? " HUF" : ifNull) + separator);
                txt.Write(expense.Other + (expense.Total_amount != null ? " HUF" : ifNull) + separator);
                txt.Write(expense.Date + separator);
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

            //filestream for download
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
                        foreach (IXLRow row in worksheet.Rows())
                        {
                            //Use the first row to add columns to DataTable with column names check.
                            if (firstRow)
                            {
                                //copy column names to a list
                                CultureInfo.CurrentUICulture = lang;
                                List<string> columnNames = _columnNameLists.GetExpensesColumnNames().Select(x => _localizer[x].Value).ToList();

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
                                else if (columnNames.Count == 1 && columnNames.Contains("Id"))
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
                                    if (cell.Value != null && cell.Value.ToString() != "")
                                    {
                                        list.Add(cell.Value);
                                    }
                                    else
                                    {
                                        list.Add(System.DBNull.Value);
                                        nulls += 1;
                                    }
                                }

                                list[l] = list[l] switch
                                {
                                    "task" => 0,
                                    "repair" => 1,
                                    "storage" => 2,
                                    "salary" => 3,
                                    "other" => 4,
                                    _ => System.DBNull.Value,
                                };

                                var totalAmount = 0;
                                for (int i = l + 2; i < list.Count - 1; i++)
                                {
                                    if (i != (l + 8) && list[i] != null && list[i] != System.DBNull.Value)
                                    {
                                        list[i] = new String(list[i]?.ToString()?.Where(Char.IsDigit).ToArray());
                                        if (list[i] != null && i < l + 11)
                                        {
                                            totalAmount += Int32.Parse(list[i]?.ToString()!);
                                        }
                                    }
                                }


                                try
                                {
                                    if (nulls != list.Count)
                                    {

                                        var sql = @"Insert Into Expenses (User_id,Type,Type_id,Fuel,Road_fees,Penalty,Driver_spending,Driver_salary,Repair_cost,Repair_description,Cost_of_storage,Total_amount,other,Date) 
                                        Values (@User_id,@Type,@Type_id,@Fuel,@Road_fees,@Penalty,@Driver_spending,@Driver_salary,@Repair_cost,@Repair_description,@Cost_of_storage,@Total_amount,@other,@Date)";
                                        var insert = await _context.Database.ExecuteSqlRawAsync(sql,
                                            new SqlParameter("@User_id", "Imported"),
                                            new SqlParameter("@Type", list[l]),
                                            new SqlParameter("@Type_id", list[l + 1]),
                                            new SqlParameter("@Fuel", list[l + 2]),
                                            new SqlParameter("@Road_fees", list[l + 3]),
                                            new SqlParameter("@Penalty", list[l + 4]),
                                            new SqlParameter("@Driver_spending", list[l + 5]),
                                            new SqlParameter("@Driver_salary", list[l + 6]),
                                            new SqlParameter("@Repair_cost", list[l + 7]),
                                            new SqlParameter("@Repair_description", list[l + 8]),
                                            new SqlParameter("@Cost_of_storage", list[l + 9]),
                                            new SqlParameter("@other", list[l + 10]),
                                            new SqlParameter("@Total_amount", totalAmount),
                                            new SqlParameter("@Date", DateTime.Now)
                                            );

                                        if (insert > 0)
                                        {
                                            var lastId = await _context.Expenses.OrderBy(x => x.Date).LastOrDefaultAsync();

                                            if (lastId != null)
                                            {
                                                var WithNewIds = await _context.Expenses.Where(x => x.Type == lastId.Type && x.Type_id == lastId.Type_id && x.Type != Type.other && x.Type != Type.salary).ToListAsync();
                                                Roads? road = await _context.Roads.FirstOrDefaultAsync(x => x.Id == lastId.Type_id && lastId.Type == Type.repair);
                                                Tasks? task = await _context.Tasks.FirstOrDefaultAsync(x => x.Id == lastId.Type_id && lastId.Type == Type.task);
                                                Cargoes? cargo = await _context.Cargoes.FirstOrDefaultAsync(x => x.Id == lastId.Type_id && lastId.Type == Type.storage);

                                                foreach (var item in WithNewIds)
                                                {
                                                    if (item != null)
                                                    {
                                                        if (item.Id == lastId?.Id)
                                                        {
                                                            if (cargo == null && lastId.Type == Type.storage)
                                                            {
                                                                item.Type_id = null;
                                                            }
                                                            if (task == null && lastId.Type == Type.task)
                                                            {
                                                                item.Type_id = null;
                                                            }
                                                            if (road == null && lastId.Type == Type.repair)
                                                            {
                                                                item.Type_id = null;
                                                            }
                                                            if (item.Type_id == null)
                                                            {
                                                                _context.Remove(item);
                                                                await _context.SaveChangesAsync();
                                                                error += "\n" + _localizer["Deleted_wrong_id"] + " " + lastId.Id + ".";
                                                            }
                                                        }
                                                    }
                                                }

                                                if (road != null && lastId.Type == Type.repair)
                                                {
                                                    road.Expenses_id = lastId?.Id;
                                                    _context.Entry(road).State = EntityState.Modified;
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

