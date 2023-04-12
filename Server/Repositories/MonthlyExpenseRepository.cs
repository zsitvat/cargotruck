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

        //this method gets the data from db and filter it
        private async Task<List<MonthlyExpense>> GetDataAsync(string? searchString, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate)
        {
            var data = await _context.MonthlyExpenses.Where(s => (dateFilterStartDate != null ? (s.Date >= dateFilterStartDate) : true) && (dateFilterEndDate != null ? (s.Date <= dateFilterEndDate) : true)).ToListAsync();

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
        public async Task<List<MonthlyExpense>> GetAsync(int page, int pageSize, string sortOrder, bool desc, string? searchString, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate) 
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

        public async Task<List<MonthlyExpense>> GetMonthlyExpensesAsync()
        {
            return await _context.MonthlyExpenses.ToListAsync();
        }

        public async Task<MonthlyExpense?> GetByIdAsync(int id)
        {
            return await _context.MonthlyExpenses.FirstOrDefaultAsync(a => a.Id == id);
        }

        //gets the data of the charts
        public async Task<int[]> GetChartDataAsync()
        {
            var data = await _context.MonthlyExpenses.ToListAsync();
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
            return await _context.MonthlyExpenses.CountAsync();
        }

        public async Task<int> PostAsync(MonthlyExpense data)
        {
            data.Profit = (data.Earning != null ? data.Earning : 0) - (data.Expense != null ? data.Expense : 0);
            _context?.Add(data);
            await _context!.SaveChangesAsync();

            return data.Id;
        }

        public async Task PutAsync(MonthlyExpense data)
        {
            _context!.Entry(data).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var data = _context.MonthlyExpenses.FirstOrDefault(x => x.Id == id);
            if (data != null)
            {
                _context.MonthlyExpenses.Remove(data);
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }

        //checks task and expenses and sum the expenses, profit and earning from them
        public async Task CheckDataAsync()
        {
            var monthly_Expenses = await _context.MonthlyExpenses.ToListAsync();
            var conIds = await _context.MonthlyExpensesTasksExpenses.ToListAsync();
            foreach (var data in monthly_Expenses)
            {
                if (conIds.Where(x => x.MonthlyExpenseId == data.Id && (x.TaskId != null || x.ExpenseId != null)).Any())
                {
                    data.Profit = 0;
                    data.Earning = 0;
                    data.Expense = 0;
                }

                if (data.Monthly_expenses_tasks_expenses != null)
                {
                    foreach (var row in data.Monthly_expenses_tasks_expenses)
                    {

                        var task = await _context.Tasks.FirstOrDefaultAsync(a => a.Id == row.TaskId);
                        var expense = await _context.Expenses.FirstOrDefaultAsync(a => a.Id == row.ExpenseId);

                        if (row.MonthlyExpenseId == data.Id)
                        {
                            data.Earning += (task?.FinalPayment != null ? task.FinalPayment : 0);
                            data.Expense = data.Expense
                                + (expense?.CostOfStorage != null ? expense.CostOfStorage : 0)
                                + (expense?.RepairCost != null ? expense.RepairCost : 0)
                                + (expense?.DriverSalary != null ? expense.DriverSalary : 0)
                                + (expense?.DriverSpending != null ? expense.DriverSpending : 0)
                                + (expense?.Fuel != null ? expense.Fuel : 0)
                                + (expense?.RoadFees != null ? expense.RoadFees : 0)
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

        //get the data from MonthlyExpensesTasksExpenses table
        public async Task<List<MonthlyExpense_task_expense>> GetConnectionIdsAsync()
        {
            return await _context.MonthlyExpensesTasksExpenses.ToListAsync();
        }

        //save the data to MonthlyExpensesTasksExpenses table
        public async Task PostConnectionIdsAsync(MonthlyExpense_task_expense connectionIds, bool first)
        {
            if (first)
            {
                var itemsToDelete = _context.MonthlyExpensesTasksExpenses.Where(x => x.MonthlyExpenseId == connectionIds.MonthlyExpenseId);
                _context.RemoveRange(itemsToDelete);
            }

            _context.Add(connectionIds);
            await _context.SaveChangesAsync();
        }

        //create the months automatically to the monthly expenses page
        public async Task CreateMonthsAsync()
        {
            MonthlyExpense data = new();
            var currentDate = DateTime.Now;
            MonthlyExpense? hasCurrentMonth = _context.MonthlyExpenses.Where(x => x.Date.Year == currentDate.Year && x.Date.Month == currentDate.Month && x.UserId == "Generated").FirstOrDefault();

            if (hasCurrentMonth == null)
            {
                data.UserId = "Generated";
                data.Date = DateTime.Now;

                _context.Add(data);
                await _context.SaveChangesAsync();
            }
        }

        //recreate the data for MonthlyExpensesTasksExpenses table for every month
        public async Task CreateConTableAsync()
        {
            var monthly_expenses = await _context.MonthlyExpenses.Where(x => x.UserId == "Generated").ToListAsync();

            foreach (var row in monthly_expenses)
            {
                var itemsToDelete = _context.MonthlyExpensesTasksExpenses.Where(x => x.MonthlyExpenseId == row.Id);
                _context.RemoveRange(itemsToDelete);
            }

            foreach (var row in monthly_expenses)
            {
                var tasks = await _context.Tasks.Where(t => t.Date.Year == row.Date.Year && t.Date.Month == row.Date.Month && t.Completed).ToListAsync();
                var expenses = await _context.Expenses.Where(e => e.Date.Year == row.Date.Year && e.Date.Month == row.Date.Month).ToListAsync();
                int lenght = (tasks.Count > expenses.Count) ? tasks.Count : expenses.Count;

                for (int i = 0; i < lenght; ++i)
                {
                    MonthlyExpense_task_expense connectionIds = new()
                    {
                        MonthlyExpenseId = row.Id
                    };

                    if (tasks.Count > i && tasks[i] != null) connectionIds.TaskId = tasks[i].Id;
                    if (expenses.Count > i && expenses[i] != null) connectionIds.ExpenseId = expenses[i].Id;

                    _context.Add(connectionIds);
                }

                await _context.SaveChangesAsync();
            }
        }

        public string ExportToExcel(CultureInfo lang, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate)
        {
            var Monthly_Expenses = _context.MonthlyExpenses.Where(s => (dateFilterStartDate != null ? (s.Date >= dateFilterStartDate) : true) && (dateFilterEndDate != null ? (s.Date <= dateFilterEndDate) : true));
            var Monthly_expenses_tasks_expenses = _context.MonthlyExpensesTasksExpenses.OrderBy(x => x.Id);

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("MonthlyExpenses");
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

                worksheet.Cell(currentRow, 1).Value = monthly_expense.Id;
                worksheet.Cell(currentRow, 2).Value = monthly_expense.Date.Month;
                worksheet.Cell(currentRow, 3).Value = monthly_expense.Earning;
                worksheet.Cell(currentRow, 4).Value = monthly_expense.Expense;
                worksheet.Cell(currentRow, 5).Value = monthly_expense.Profit;
                foreach (var row in Monthly_expenses_tasks_expenses.Where(x => x.MonthlyExpenseId == monthly_expense.Id && x.ExpenseId != null))
                {
                    if (Monthly_expenses_tasks_expenses.Where(x => x.MonthlyExpenseId == monthly_expense.Id && x.ExpenseId != null).Last().Id != row.Id)
                    {
                        worksheet.Cell(currentRow, 6).Value = worksheet.Cell(currentRow, 6).Value + (row.ExpenseId + "; ");
                    }
                    else
                    {
                        worksheet.Cell(currentRow, 6).Value = worksheet.Cell(currentRow, 6).Value + row.ExpenseId.ToString();
                    }
                }
                foreach (var row in Monthly_expenses_tasks_expenses.Where(x => x.MonthlyExpenseId == monthly_expense.Id && x.TaskId != null))
                {
                    if (Monthly_expenses_tasks_expenses.Where(x => x.MonthlyExpenseId == monthly_expense.Id && x.TaskId != null).Last().Id != row.Id)
                    {
                        worksheet.Cell(currentRow, 7).Value = worksheet.Cell(currentRow, 7).Value + (row.TaskId + "; ");
                    }
                    else
                    {
                        worksheet.Cell(currentRow, 7).Value = worksheet.Cell(currentRow, 7).Value + row.TaskId.ToString();
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
            var Monthly_Expenses = _context.MonthlyExpenses.Where(s => (dateFilterStartDate != null ? (s.Date >= dateFilterStartDate) : true) && (dateFilterEndDate != null ? (s.Date <= dateFilterEndDate) : true));
            var Monthly_expenses_tasks_expenses = _context.MonthlyExpensesTasksExpenses.OrderBy(x => x.Id);

            int pdfRowIndex = 1;
            Random rnd = new();
            int random = rnd.Next(1000000, 9999999);
            string filename = "MonthlyExpenses" + random + "_" + DateTime.Now.ToString("dd-MM-yyyy");
            string filepath = "Files/" + filename + ".pdf";

            Document document = new(PageSize.A4, 5f, 5f, 10f, 10f);
            FileStream fs = new(filepath, FileMode.Create);
            PdfWriter writer = PdfWriter.GetInstance(document, fs);
            document.Open();

            Font font1 = FontFactory.GetFont(FontFactory.TIMES_BOLD, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 12);
            Font font2 = FontFactory.GetFont(FontFactory.TIMES_ROMAN, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 10);

            System.Type type = typeof(MonthlyExpense);
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

            var title = new Paragraph(15, _localizer["MonthlyExpense"].Value)
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

                foreach (MonthlyExpense monthly_expense in Monthly_Expenses)
                {
                    var s = "";
                    if (!string.IsNullOrEmpty(monthly_expense.Id.ToString())) { s = monthly_expense.Id.ToString(); }
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
                    foreach (var row in Monthly_expenses_tasks_expenses.Where(x => x.MonthlyExpenseId == monthly_expense.Id && x.ExpenseId != null))
                    {
                        if (Monthly_expenses_tasks_expenses.Where(x => x.MonthlyExpenseId == monthly_expense.Id && x.ExpenseId != null).Last().Id != row.Id)
                        {
                            s += row.ExpenseId.ToString();
                        }
                        else
                        {
                            s += row.ExpenseId.ToString();
                        }
                    }
                    table.AddCell(new PdfPCell(new Phrase(s.ToString(), font2))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    });
                    s = "";
                    foreach (var row in Monthly_expenses_tasks_expenses.Where(x => x.MonthlyExpenseId == monthly_expense.Id && x.TaskId != null))
                    {
                        if (Monthly_expenses_tasks_expenses.Where(x => x.MonthlyExpenseId == monthly_expense.Id && x.TaskId != null).Last().Id != row.Id)
                        {
                            s += row.TaskId.ToString();
                        }
                        else
                        {
                            s += row.TaskId.ToString();
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

            byte[] fileBytes = await File.ReadAllBytesAsync(filepath);
            string base64String = Convert.ToBase64String(fileBytes);
            File.Delete(filepath);

            return base64String;
        }

        public async Task<string> ExportToCSVAsync(CultureInfo lang, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate, bool isTextDocument)
        {
            var Monthly_Expenses = _context.MonthlyExpenses.Where(s => (dateFilterStartDate != null ? (s.Date >= dateFilterStartDate) : true) && (dateFilterEndDate != null ? (s.Date <= dateFilterEndDate) : true));
            var Monthly_expenses_tasks_expenses = _context.MonthlyExpensesTasksExpenses.OrderBy(x => x.Id);

            Random rnd = new();
            int random = rnd.Next(1000000, 9999999);
            string filename = "MonthlyExpenses" + random + "_" + DateTime.Now.ToString("dd-MM-yyyy");
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
                txt.Write(monthly_expense.Id + separator);
                txt.Write(monthly_expense.Date.Month + separator);
                txt.Write((monthly_expense.Earning != null ? monthly_expense.Earning : ifNull) + separator);
                txt.Write((monthly_expense.Expense != null ? monthly_expense.Expense : ifNull) + separator);
                txt.Write((monthly_expense.Profit != null ? monthly_expense.Profit : ifNull) + separator);
                s = "";
                foreach (var row in Monthly_expenses_tasks_expenses.Where(x => x.MonthlyExpenseId == monthly_expense.Id && x.ExpenseId != null))
                {
                    if (Monthly_expenses_tasks_expenses.Where(x => x.MonthlyExpenseId == monthly_expense.Id && x.ExpenseId != null).Last().Id != row.Id)
                    {
                        s += (row.ExpenseId + ", ");
                    }
                    else
                    {
                        s += row.ExpenseId.ToString();
                    }
                }

                if (s == "")
                {
                    s = ifNull;
                }

                txt.Write(s + separator);

                s = "";
                foreach (var row in Monthly_expenses_tasks_expenses.Where(x => x.MonthlyExpenseId == monthly_expense.Id && x.TaskId != null))
                {
                    if (Monthly_expenses_tasks_expenses.Where(x => x.MonthlyExpenseId == monthly_expense.Id && x.TaskId != null).Last().Id != row.Id)
                    {
                        s += (row.TaskId + ", ");
                    }
                    else
                    {
                        s += row.TaskId.ToString();
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
    }
}
