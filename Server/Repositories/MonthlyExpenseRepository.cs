using Cargotruck.Server.Data;
using Cargotruck.Server.Repositories.Interfaces;
using Cargotruck.Server.Services.Interfaces;
using Cargotruck.Shared.Model;
using Cargotruck.Shared.Resources;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Office2010.Excel;
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
    public class MonthlyExpenseRepository: IMonthlyExpenseRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IStringLocalizer<Resource> _localizer;
        private readonly IColumnNamesService _columnNameLists;

        public MonthlyExpenseRepository(ApplicationDbContext context, IStringLocalizer<Resource> localizer, IColumnNamesService columnNameLists)
        {
            _context = context;
            _localizer = localizer;
            _columnNameLists = columnNameLists;
        }
        private async Task<List<Monthly_expenses>> GetDataAsync(string? searchString, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate)
        {
            var data = await _context.Monthly_Expenses.Where(s => (dateFilterStartDate != null ? (s.Date >= dateFilterStartDate) : true) && (dateFilterEndDate != null ? (s.Date <= dateFilterEndDate) : true)).ToListAsync();

            searchString = searchString?.ToLower();
            if (searchString != null && searchString != "")
            {
                data = data.Where(s =>
               s.Earning.ToString()!.ToLower().Contains(searchString)
            || (s.Profit.ToString()!.ToLower().Contains(searchString))
            ).ToList();
            }

            return data;
        }
        public async Task<List<Monthly_expenses>> GetAsync(int page, int pageSize, string sortOrder, bool desc, string? searchString, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate) 
        {
            await CreateMonthsAsync(); // checks and create the monthly expenses data for the current month
            var data = await GetDataAsync(searchString, dateFilterStartDate, dateFilterEndDate);

            sortOrder = sortOrder == "Earning" ? (desc ? "Earning_desc" : "Earning") : (sortOrder);
            sortOrder = sortOrder == "Expense" ? (desc ? "Expense_desc" : "Expense") : (sortOrder);
            sortOrder = sortOrder == "Month" ? (desc ? "Month_desc" : "Month") : (sortOrder);
            sortOrder = sortOrder == "Profit" ? (desc ? "Profit_desc" : "Profit") : (sortOrder);
            sortOrder = sortOrder == "Date" || String.IsNullOrEmpty(sortOrder) ? (desc ? "Date_desc" : "") : (sortOrder);

            data = sortOrder switch
            {
                "Earning_desc" => data.OrderByDescending(s => s.Earning).ToList(),
                "Earning" => data.OrderBy(s => s.Earning).ToList(),
                "Expense_desc" => data.OrderByDescending(s => s.Expense).ToList(),
                "Expense" => data.OrderBy(s => s.Expense).ToList(),
                "Profit_desc" => data.OrderByDescending(s => s.Profit).ToList(),
                "Profit" => data.OrderBy(s => s.Profit).ToList(),
                "Month_desc" => data.OrderByDescending(s => s.Date.Month).ToList(),
                "Month" => data.OrderBy(s => s.Date.Month).ToList(),
                "Date_desc" => data.OrderByDescending(s => s.Date).ToList(),
                _ => data.OrderBy(s => s.Date).ToList(),
            };

            return data.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        }
        public async Task<List<Monthly_expenses>> GetMonthlyExpensesAsync()
        {
            return await _context.Monthly_Expenses.ToListAsync();
        }
        public async Task<Monthly_expenses?> GetByIdAsync(int id)
        {
            return await _context.Monthly_Expenses.FirstOrDefaultAsync(a => a.Monthly_expense_id == id);
        }
        public async Task<int[]> GetChartDataAsync()
        {
            var data = await _context.Monthly_Expenses.ToListAsync();
            int[] columnsHeight = new int[36];

            for (int i = 0; i < 12; i++)
            {
                foreach (var item in data.Where(x => x.Date.Year == DateTime.Now.Year && x.Date.Month == i + 1))
                {
                    columnsHeight[i] += item.Profit != null ? (int)item.Profit : 0;
                }
            }

            for (int i = 0; i < 12; i++)
            {
                foreach (var item in data.Where(x => x.Date.Year == DateTime.Now.Year && x.Date.Month == i + 1))
                {
                    columnsHeight[i + 12] += item.Expense != null ? (int)item.Expense : 0;
                }
            }

            for (int i = 0; i < 12; i++)
            {
                foreach (var item in data.Where(x => x.Date.Year == DateTime.Now.Year && x.Date.Month == i + 1))
                {
                    columnsHeight[i + 24] += item.Earning != null ? (int)item.Earning : 0;
                }
            }

            return columnsHeight;
        }
        public async Task<int> PageCountAsync(string? searchString, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate)
        {
            var data = await GetDataAsync(searchString, dateFilterStartDate, dateFilterEndDate);
            return data.Count;
        }
        public async Task<int> CountAsync()
        {
            return await _context.Monthly_Expenses.CountAsync();
        }
        public async Task PostAsync(Monthly_expenses data)
        {
            data.Profit = (data.Earning != null ? data.Earning : 0) - (data.Expense != null ? data.Expense : 0);
            _context?.Add(data);
            await _context!.SaveChangesAsync();
        }
        public async Task PutAsync(Monthly_expenses data)
        {
            _context!.Entry(data).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
        public async Task<bool> DeleteAsync(int id)
        {
            var data = _context.Monthly_Expenses.FirstOrDefault(x => x.Monthly_expense_id == id);
            if (data != null)
            {
                _context.Monthly_Expenses.Remove(data);
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }
        public async Task CheckDataAsync()
        {
            var monthly_Expenses = await _context.Monthly_Expenses.ToListAsync();
            var conIds = await _context.Monthly_expenses_tasks_expenses.ToListAsync();
            foreach (var data in monthly_Expenses)
            {
                if (conIds.Where(x => x.Monthly_expense_id == data.Monthly_expense_id && (x.Task_id != null || x.Expense_id != null)).Any())
                {
                    data.Profit = 0;
                    data.Earning = 0;
                    data.Expense = 0;
                }

                if (data.Monthly_expenses_tasks_expenses != null)
                {
                    foreach (var row in data.Monthly_expenses_tasks_expenses)
                    {

                        var task = await _context.Tasks.FirstOrDefaultAsync(a => a.Id == row.Task_id);
                        var expense = await _context.Expenses.FirstOrDefaultAsync(a => a.Id == row.Expense_id);

                        if (row.Monthly_expense_id == data.Monthly_expense_id)
                        {
                            data.Earning += (task?.Final_Payment != null ? task.Final_Payment : 0);
                            data.Expense = data.Expense
                                + (expense?.Cost_of_storage != null ? expense.Cost_of_storage : 0)
                                + (expense?.Repair_cost != null ? expense.Repair_cost : 0)
                                + (expense?.Driver_salary != null ? expense.Driver_salary : 0)
                                + (expense?.Driver_spending != null ? expense.Driver_spending : 0)
                                + (expense?.Fuel != null ? expense.Fuel : 0)
                                + (expense?.Road_fees != null ? expense.Road_fees : 0)
                                + (expense?.Penalty != null ? expense.Penalty : 0)
                                + (expense?.Other != null ? expense.Other : 0);
                        }
                    }
                }

                data.Profit = (data.Earning != null ? data.Earning : 0) - (data.Expense != null ? data.Expense : 0);

                _context.Update(data);
            }

            await _context.SaveChangesAsync();
        }

        public async Task<List<Monthly_expenses_tasks_expenses>> GetConnectionIdsAsync()
        {
            return await _context.Monthly_expenses_tasks_expenses.ToListAsync();
        }

        public async Task PostConnectionIdsAsync(Monthly_expenses_tasks_expenses connectionIds, bool first)
        {
            if (first)
            {
                var itemsToDelete = _context.Monthly_expenses_tasks_expenses.Where(x => x.Monthly_expense_id == connectionIds.Monthly_expense_id);
                _context.RemoveRange(itemsToDelete);
            }

            _context.Add(connectionIds);
            await _context.SaveChangesAsync();
        }
        public async Task CreateMonthsAsync()
        {
            Monthly_expenses data = new();
            var currentDate = DateTime.Now;
            Monthly_expenses? hasCurrentMonth = _context.Monthly_Expenses.Where(x => x.Date.Year == currentDate.Year && x.Date.Month == currentDate.Month && x.User_id == "Generated").FirstOrDefault();

            if (hasCurrentMonth == null)
            {
                data.User_id = "Generated";
                data.Date = DateTime.Now;

                _context.Add(data);
                await _context.SaveChangesAsync();
            }
        }
        public async Task CreateConTableAsync()
        {
            var monthly_expenses = await _context.Monthly_Expenses.Where(x => x.User_id == "Generated").ToListAsync();

            foreach (var row in monthly_expenses)
            {
                var itemsToDelete = _context.Monthly_expenses_tasks_expenses.Where(x => x.Monthly_expense_id == row.Monthly_expense_id);
                _context.RemoveRange(itemsToDelete);
            }

            foreach (var row in monthly_expenses)
            {
                var tasks = await _context.Tasks.Where(t => t.Date.Year == row.Date.Year && t.Date.Month == row.Date.Month && t.Completed).ToListAsync();
                var expenses = await _context.Expenses.Where(e => e.Date.Year == row.Date.Year && e.Date.Month == row.Date.Month).ToListAsync();
                int lenght = (tasks.Count > expenses.Count) ? tasks.Count : expenses.Count;

                for (int i = 0; i < lenght; ++i)
                {
                    Monthly_expenses_tasks_expenses connectionIds = new()
                    {
                        Monthly_expense_id = row.Monthly_expense_id
                    };

                    if (tasks.Count > i && tasks[i] != null) connectionIds.Task_id = tasks[i].Id;
                    if (expenses.Count > i && expenses[i] != null) connectionIds.Expense_id = expenses[i].Id;

                    _context.Add(connectionIds);
                }

                await _context.SaveChangesAsync();
            }
        }
        public string ExportToExcel(CultureInfo lang, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate)
        {
            var Monthly_Expenses = _context.Monthly_Expenses.Where(s => (dateFilterStartDate != null ? (s.Date >= dateFilterStartDate) : true) && (dateFilterEndDate != null ? (s.Date <= dateFilterEndDate) : true));
            var Monthly_expenses_tasks_expenses = _context.Monthly_expenses_tasks_expenses.OrderBy(x => x.Id);

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Monthly_Expenses");
            var currentRow = 1;

            CultureInfo.CurrentUICulture = lang;
            List<string> columnNames = _columnNameLists.GetMonthlyExpensesColumnNames().Select(x => _localizer[x].Value).ToList();

            for (var i = 0; i < columnNames.Count; i++)
            {
                worksheet.Cell(currentRow, i + 1).Value = columnNames[i];
                worksheet.Cell(currentRow, i + 1).Style.Font.SetBold();
            }

            foreach (var monthly_expense in Monthly_Expenses)
            {
                currentRow++;

                worksheet.Cell(currentRow, 1).Value = monthly_expense.Monthly_expense_id;
                worksheet.Cell(currentRow, 2).Value = monthly_expense.Date.Month;
                worksheet.Cell(currentRow, 3).Value = monthly_expense.Earning;
                worksheet.Cell(currentRow, 4).Value = monthly_expense.Expense;
                worksheet.Cell(currentRow, 5).Value = monthly_expense.Profit;
                foreach (var row in Monthly_expenses_tasks_expenses.Where(x => x.Monthly_expense_id == monthly_expense.Monthly_expense_id && x.Expense_id != null))
                {
                    if (Monthly_expenses_tasks_expenses.Where(x => x.Monthly_expense_id == monthly_expense.Monthly_expense_id && x.Expense_id != null).Last().Id != row.Id)
                    {
                        worksheet.Cell(currentRow, 6).Value = worksheet.Cell(currentRow, 6).Value + (row.Expense_id + "; ");
                    }
                    else
                    {
                        worksheet.Cell(currentRow, 6).Value = worksheet.Cell(currentRow, 6).Value + row.Expense_id.ToString();
                    }
                }
                foreach (var row in Monthly_expenses_tasks_expenses.Where(x => x.Monthly_expense_id == monthly_expense.Monthly_expense_id && x.Task_id != null))
                {
                    if (Monthly_expenses_tasks_expenses.Where(x => x.Monthly_expense_id == monthly_expense.Monthly_expense_id && x.Task_id != null).Last().Id != row.Id)
                    {
                        worksheet.Cell(currentRow, 7).Value = worksheet.Cell(currentRow, 7).Value + (row.Task_id + "; ");
                    }
                    else
                    {
                        worksheet.Cell(currentRow, 7).Value = worksheet.Cell(currentRow, 7).Value + row.Task_id.ToString();
                    }
                }
                worksheet.Cell(currentRow, 8).Value = monthly_expense.Date;
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            var content = stream.ToArray();
            return Convert.ToBase64String(content);
        }
        public async Task<string> ExportToPdfAsync(CultureInfo lang, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate)
        {
            var Monthly_Expenses = _context.Monthly_Expenses.Where(s => (dateFilterStartDate != null ? (s.Date >= dateFilterStartDate) : true) && (dateFilterEndDate != null ? (s.Date <= dateFilterEndDate) : true));
            var Monthly_expenses_tasks_expenses = _context.Monthly_expenses_tasks_expenses.OrderBy(x => x.Id);

            int pdfRowIndex = 1;
            Random rnd = new();
            int random = rnd.Next(1000000, 9999999);
            string filename = "Monthly_Expenses" + random + "_" + DateTime.Now.ToString("dd-MM-yyyy");
            string filepath = "Files/" + filename + ".pdf";

            Document document = new(PageSize.A4, 5f, 5f, 10f, 10f);
            FileStream fs = new(filepath, FileMode.Create);
            PdfWriter writer = PdfWriter.GetInstance(document, fs);
            document.Open();

            Font font1 = FontFactory.GetFont(FontFactory.TIMES_BOLD, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 12);
            Font font2 = FontFactory.GetFont(FontFactory.TIMES_ROMAN, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 10);

            System.Type type = typeof(Monthly_expenses);
            var column_number = (type.GetProperties().Length) + 1;
            var columnDefinitionSize = new float[column_number];
            for (int i = 0; i < column_number; i++) columnDefinitionSize[i] = 1F;

            PdfPTable table;
            PdfPCell cell;

            table = new PdfPTable(columnDefinitionSize)
            {
                WidthPercentage = 80
            };
            cell = new PdfPCell
            {
                BackgroundColor = new BaseColor(0xC0, 0xC0, 0xC0),
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_CENTER
            };

            //copy column names to a list based on language
            CultureInfo.CurrentUICulture = lang;
            List<string> columnNames = _columnNameLists.GetMonthlyExpensesColumnNames().Select(x => _localizer[x].Value).ToList();

            var title = new Paragraph(15, _localizer["Monthly_expenses"].Value)
            {
                Alignment = Element.ALIGN_CENTER
            };



            document.Add(title);
            document.Add(new Paragraph("\n"));

            if (Monthly_Expenses.Any())
            {
                foreach (var name in columnNames.Take(column_number))
                {
                    table.AddCell(new PdfPCell(new Phrase(name, font1))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    });
                }

                foreach (Monthly_expenses monthly_expense in Monthly_Expenses)
                {
                    var s = "";
                    if (!string.IsNullOrEmpty(monthly_expense.Monthly_expense_id.ToString())) { s = monthly_expense.Monthly_expense_id.ToString(); }
                    else { s = "-"; }
                    table.AddCell(new PdfPCell(new Phrase(s?.ToString(), font2))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    });
                    if (!string.IsNullOrEmpty(monthly_expense.Date.ToString())) { s = monthly_expense.Date.Month.ToString(); }
                    else { s = "-"; }
                    table.AddCell(new PdfPCell(new Phrase(s?.ToString(), font2))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    });
                    if (!string.IsNullOrEmpty(monthly_expense.Earning.ToString())) { s = monthly_expense.Earning.ToString(); }
                    else { s = "-"; }
                    table.AddCell(new PdfPCell(new Phrase(s?.ToString(), font2))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    });
                    if (!string.IsNullOrEmpty(monthly_expense.Expense.ToString())) { s = monthly_expense.Expense.ToString(); }
                    else { s = "-"; }
                    table.AddCell(new PdfPCell(new Phrase(s?.ToString(), font2))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    });
                    if (!string.IsNullOrEmpty(monthly_expense.Profit.ToString())) { s = monthly_expense.Profit.ToString(); }
                    else { s = "-"; }
                    table.AddCell(new PdfPCell(new Phrase(s?.ToString(), font2))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    });
                    s = "";
                    foreach (var row in Monthly_expenses_tasks_expenses.Where(x => x.Monthly_expense_id == monthly_expense.Monthly_expense_id && x.Expense_id != null))
                    {
                        if (Monthly_expenses_tasks_expenses.Where(x => x.Monthly_expense_id == monthly_expense.Monthly_expense_id && x.Expense_id != null).Last().Id != row.Id)
                        {
                            s += row.Expense_id.ToString();
                        }
                        else
                        {
                            s += row.Expense_id.ToString();
                        }
                    }
                    table.AddCell(new PdfPCell(new Phrase(s.ToString(), font2))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    });
                    s = "";
                    foreach (var row in Monthly_expenses_tasks_expenses.Where(x => x.Monthly_expense_id == monthly_expense.Monthly_expense_id && x.Task_id != null))
                    {
                        if (Monthly_expenses_tasks_expenses.Where(x => x.Monthly_expense_id == monthly_expense.Monthly_expense_id && x.Task_id != null).Last().Id != row.Id)
                        {
                            s += row.Task_id.ToString();
                        }
                        else
                        {
                            s += row.Task_id.ToString();
                        }
                    }
                    table.AddCell(new PdfPCell(new Phrase(s.ToString(), font2))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    });

                    if (!string.IsNullOrEmpty(monthly_expense.Date.ToString())) { s = monthly_expense.Date.ToString(); }
                    else { s = "-"; }
                    table.AddCell(new PdfPCell(new Phrase(s.ToString(), font2))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    });
                    pdfRowIndex++;
                }
                document.Add(table);
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
            var Monthly_Expenses = _context.Monthly_Expenses.Where(s => (dateFilterStartDate != null ? (s.Date >= dateFilterStartDate) : true) && (dateFilterEndDate != null ? (s.Date <= dateFilterEndDate) : true));
            var Monthly_expenses_tasks_expenses = _context.Monthly_expenses_tasks_expenses.OrderBy(x => x.Id);

            Random rnd = new();
            int random = rnd.Next(1000000, 9999999);
            string filename = "Monthly_Expenses" + random + "_" + DateTime.Now.ToString("dd-MM-yyyy");
            string filepath = "Files/" + filename + ".csv";

            StreamWriter txt = new(filepath);

            //copy column names to a list based on language
            CultureInfo.CurrentUICulture = lang;
            List<string> columnNames = _columnNameLists.GetMonthlyExpensesColumnNames().Select(x => _localizer[x].Value).ToList();

            string separator = isTextDocument ? "\t" : ";";
            string ifNull = isTextDocument ? " --- " : "";

            foreach (var name in columnNames)
            {
                 txt.Write(name + (isTextDocument ? "  " : separator));
            }

            txt.Write("\n");

            foreach (var monthly_expense in Monthly_Expenses)
            {
                var s = "";
                txt.Write(monthly_expense.Monthly_expense_id + separator);
                txt.Write(monthly_expense.Date.Month + separator);
                txt.Write((monthly_expense.Earning != null ? monthly_expense.Earning : ifNull) + separator);
                txt.Write((monthly_expense.Expense != null ? monthly_expense.Expense : ifNull) + separator);
                txt.Write((monthly_expense.Profit != null ? monthly_expense.Profit : ifNull) + separator);
                s = "";
                foreach (var row in Monthly_expenses_tasks_expenses.Where(x => x.Monthly_expense_id == monthly_expense.Monthly_expense_id && x.Expense_id != null))
                {
                    if (Monthly_expenses_tasks_expenses.Where(x => x.Monthly_expense_id == monthly_expense.Monthly_expense_id && x.Expense_id != null).Last().Id != row.Id)
                    {
                        s += (row.Expense_id + ", ");
                    }
                    else
                    {
                        s += row.Expense_id.ToString();
                    }
                }

                if (s == "")
                {
                    s = ifNull;
                }

                txt.Write(s + separator);

                s = "";
                foreach (var row in Monthly_expenses_tasks_expenses.Where(x => x.Monthly_expense_id == monthly_expense.Monthly_expense_id && x.Task_id != null))
                {
                    if (Monthly_expenses_tasks_expenses.Where(x => x.Monthly_expense_id == monthly_expense.Monthly_expense_id && x.Task_id != null).Last().Id != row.Id)
                    {
                        s += (row.Task_id + ", ");
                    }
                    else
                    {
                        s += row.Task_id.ToString();
                    }
                }

                if (s == "")
                {
                    s = ifNull;
                }

                txt.Write(s + separator);
                txt.Write(monthly_expense.Date + separator);

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
    }
}
