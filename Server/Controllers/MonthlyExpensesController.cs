using Cargotruck.Data;
using Cargotruck.Shared.Models;
using ClosedXML.Excel;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Linq.Dynamic.Core;
using System.Text;
using Document = iTextSharp.text.Document;
using Font = iTextSharp.text.Font;

namespace Cargotruck.Server.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class Monthly_ExpensesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public Monthly_ExpensesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Get(int page, int pageSize, string sortOrder, bool desc, string? searchString, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate)
        {
            await CreateMonths(); // checks and create the monthly expenses data for the current month
            var data = await GetData(searchString, dateFilterStartDate, dateFilterEndDate);

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
            data = data.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            return Ok(data);
        }


        [HttpGet]
        public async Task<List<Monthly_expenses>> GetData(string? searchString, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate)
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

        [HttpGet]
        public async Task<IActionResult> GetChartData()
        {
            var data = await _context.Monthly_Expenses.ToListAsync();
            int[] columnsHeight = new int[24];
            for (int i = 0; i < 12; i++)
            {
                foreach(var item in data.Where(x => x.Date.Year == DateTime.Now.Year && x.Date.Month == i + 1))
                {
                    columnsHeight[i] += item.Expense != null ? (int)item.Expense : 0;
                }
            }
            for (int i = 0; i < 12; i++)
            {
                foreach (var item in data.Where(x => x.Date.Year == DateTime.Now.Year && x.Date.Month == i + 1))
                {
                    columnsHeight[i+12] += item.Profit != null ? (int)item.Profit : 0;
                }
            }
            return Ok(columnsHeight);
        }

        [HttpGet]
        public async Task<IActionResult> GetMonthlyExpenses()
        {
            var data = await _context.Monthly_Expenses.ToListAsync();
            return Ok(data);
        }

        [HttpGet]
        public async Task<IActionResult> Count()
        {
            var t = await _context.Monthly_Expenses.CountAsync();
            return Ok(t);
        }

        [HttpGet]
        public async Task<IActionResult> GetConnectionIds()
        {
            var data = await _context.Monthly_expenses_tasks_expenses.ToListAsync();
            return Ok(data);
        }

        [HttpGet]
        public async Task<IActionResult> PageCount(string? searchString, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate)
        {
            var data = await GetData(searchString, dateFilterStartDate, dateFilterEndDate);
            int PageCount = data.Count;
            return Ok(PageCount);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var data = await _context.Monthly_Expenses.FirstOrDefaultAsync(a => a.Monthly_expense_id == id);
            return Ok(data);
        }

        public async Task CheckData()
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

        [HttpPut]
        public async Task<IActionResult> Put(Monthly_expenses data)
        {
            data.User_id = _context?.Users?.FirstOrDefault(a => a.UserName == User.Identity!.Name)?.Id;
            _context!.Entry(data).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPost]
        public async Task<IActionResult> Post(Monthly_expenses data)
        {
            data.User_id = _context?.Users?.FirstOrDefault(a => a.UserName == User.Identity!.Name)?.Id;
            data.Profit = (data.Earning != null ? data.Earning : 0) - (data.Expense != null ? data.Expense : 0);
            _context?.Add(data);
            await _context!.SaveChangesAsync();
            return Ok(data.Monthly_expense_id);
        }

        [HttpPost]
        public async Task<IActionResult> PostConnectionIds(Monthly_expenses_tasks_expenses connectionIds, bool first)
        {
            //_context.Entry(connectionIds).State = EntityState.Modified;
            if (first)
            {
                var itemsToDelete = _context.Monthly_expenses_tasks_expenses.Where(x => x.Monthly_expense_id == connectionIds.Monthly_expense_id);
                _context.RemoveRange(itemsToDelete);
            }
            _context.Add(connectionIds);
            await _context.SaveChangesAsync();
            return Ok(connectionIds.Id);
        }

        [HttpPost]
        public async Task<IActionResult> CreateMonths()
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
                return Ok(data.Monthly_expense_id);
            }
            return Ok();
        }

        public async Task CreateConTable()
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

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var data = new Monthly_expenses { Monthly_expense_id = id };
            _context.Remove(data);
            await _context.SaveChangesAsync();
            return NoContent();
        }


        //closedXML needed !!!
        [HttpGet]
        public string Excel(string lang, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate)
        {
            var Monthly_Expenses = _context.Monthly_Expenses.Where(s => (dateFilterStartDate != null ? (s.Date >= dateFilterStartDate) : true) && (dateFilterEndDate != null ? (s.Date <= dateFilterEndDate) : true));
            var Monthly_expenses_tasks_expenses = _context.Monthly_expenses_tasks_expenses.OrderBy(x => x.Id);

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Monthly_Expenses");
            var currentRow = 1;

            worksheet.Cell(currentRow, 1).Value = "Id";
            worksheet.Cell(currentRow, 1).Style.Font.SetBold();
            worksheet.Cell(currentRow, 2).Value = lang == "hu" ? Cargotruck.Shared.Resources.Resource.Month : "Month";
            worksheet.Cell(currentRow, 2).Style.Font.SetBold();
            worksheet.Cell(currentRow, 3).Value = lang == "hu" ? Cargotruck.Shared.Resources.Resource.User_id : "User ID";
            worksheet.Cell(currentRow, 3).Style.Font.SetBold();
            worksheet.Cell(currentRow, 4).Value = lang == "hu" ? Cargotruck.Shared.Resources.Resource.Earning : "Earning";
            worksheet.Cell(currentRow, 4).Style.Font.SetBold();
            worksheet.Cell(currentRow, 5).Value = lang == "hu" ? Cargotruck.Shared.Resources.Resource.Expense : "Expense";
            worksheet.Cell(currentRow, 5).Style.Font.SetBold();
            worksheet.Cell(currentRow, 6).Value = lang == "hu" ? Cargotruck.Shared.Resources.Resource.Profit : "Profit";
            worksheet.Cell(currentRow, 6).Style.Font.SetBold();
            worksheet.Cell(currentRow, 7).Value = lang == "hu" ? Cargotruck.Shared.Resources.Resource.Expense_id : "Expenses ID";
            worksheet.Cell(currentRow, 7).Style.Font.SetBold();
            worksheet.Cell(currentRow, 8).Value = lang == "hu" ? Cargotruck.Shared.Resources.Resource.Task_id : "Task_id";
            worksheet.Cell(currentRow, 8).Style.Font.SetBold();
            worksheet.Cell(currentRow, 9).Value = lang == "hu" ? Cargotruck.Shared.Resources.Resource.Date : "Date";
            worksheet.Cell(currentRow, 9).Style.Font.SetBold();

            foreach (var monthly_expense in Monthly_Expenses)
            {
                currentRow++;

                worksheet.Cell(currentRow, 1).Value = monthly_expense.Monthly_expense_id;
                worksheet.Cell(currentRow, 2).Value = monthly_expense.Date.Month;
                worksheet.Cell(currentRow, 3).Value = monthly_expense.User_id;
                worksheet.Cell(currentRow, 4).Value = monthly_expense.Earning;
                worksheet.Cell(currentRow, 5).Value = monthly_expense.Expense;
                worksheet.Cell(currentRow, 6).Value = monthly_expense.Profit;
                foreach (var row in Monthly_expenses_tasks_expenses.Where(x => x.Monthly_expense_id == monthly_expense.Monthly_expense_id && x.Expense_id != null))
                {
                    if (Monthly_expenses_tasks_expenses.Where(x => x.Monthly_expense_id == monthly_expense.Monthly_expense_id && x.Expense_id != null).Last().Id != row.Id)
                    {
                        worksheet.Cell(currentRow, 7).Value = worksheet.Cell(currentRow, 7).Value + (row.Expense_id + "; ");
                    }
                    else
                    {
                        worksheet.Cell(currentRow, 7).Value = worksheet.Cell(currentRow, 7).Value + row.Expense_id.ToString();
                    }
                }
                foreach (var row in Monthly_expenses_tasks_expenses.Where(x => x.Monthly_expense_id == monthly_expense.Monthly_expense_id && x.Task_id != null))
                {
                    if (Monthly_expenses_tasks_expenses.Where(x => x.Monthly_expense_id == monthly_expense.Monthly_expense_id && x.Task_id != null).Last().Id != row.Id)
                    {
                        worksheet.Cell(currentRow, 8).Value = worksheet.Cell(currentRow, 8).Value + (row.Task_id + "; ");
                    }
                    else
                    {
                        worksheet.Cell(currentRow, 8).Value = worksheet.Cell(currentRow, 8).Value + row.Task_id.ToString();
                    }
                }
                worksheet.Cell(currentRow, 9).Value = monthly_expense.Date;
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            var content = stream.ToArray();
            return Convert.ToBase64String(content);
        }

        //iTextSharp needed !!!
        [HttpGet]
        public async Task<string> PDF(string lang, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate)
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

            var title = new Paragraph(15, lang == "hu" ? Cargotruck.Shared.Resources.Resource.Monthly_expenses : "Monthly expenses")
            {
                Alignment = Element.ALIGN_CENTER
            };


            document.Add(title);
            document.Add(new Paragraph("\n"));

            if (Monthly_Expenses.Any())
            {
                table.AddCell(new PdfPCell(new Phrase("Id", font1))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                });
                table.AddCell(new PdfPCell(new Phrase(lang == "hu" ? Cargotruck.Shared.Resources.Resource.Month : "Month", font1))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                });
                table.AddCell(new PdfPCell(new Phrase(lang == "hu" ? Cargotruck.Shared.Resources.Resource.Earning : "Earning", font1))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                });
                table.AddCell(new PdfPCell(new Phrase(lang == "hu" ? Cargotruck.Shared.Resources.Resource.Expense : "Expense", font1))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                });
                table.AddCell(new PdfPCell(new Phrase(lang == "hu" ? Cargotruck.Shared.Resources.Resource.Profit : "Profit", font1))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                });
                table.AddCell(new PdfPCell(new Phrase(lang == "hu" ? Cargotruck.Shared.Resources.Resource.Expenses : "Expenses", font1))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                });
                table.AddCell(new PdfPCell(new Phrase(lang == "hu" ? Cargotruck.Shared.Resources.Resource.Task_id : "Task ID", font1))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                });
                table.AddCell(new PdfPCell(new Phrase(lang == "hu" ? Cargotruck.Shared.Resources.Resource.Date : "Date", font1))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                });

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
                var noContent = new Paragraph(lang == "hu" ? "Nem található adat!" : "No content found!")
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

        //iTextSharp needed !!!
        [HttpGet]
        public async Task<string> CSV(string lang, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate)
        {
            var Monthly_Expenses = _context.Monthly_Expenses.Where(s => (dateFilterStartDate != null ? (s.Date >= dateFilterStartDate) : true) && (dateFilterEndDate != null ? (s.Date <= dateFilterEndDate) : true));
            var Monthly_expenses_tasks_expenses = _context.Monthly_expenses_tasks_expenses.OrderBy(x => x.Id);

            Random rnd = new();
            int random = rnd.Next(1000000, 9999999);
            string filename = "Monthly_Expenses" + random + "_" + DateTime.Now.ToString("dd-MM-yyyy");
            string filepath = "Files/" + filename + ".csv";

            StreamWriter txt = new(filepath);
            txt.Write("Id" + ";");
            txt.Write((lang == "hu" ? Cargotruck.Shared.Resources.Resource.User_id : "User ID") + ";");
            txt.Write((lang == "hu" ? Cargotruck.Shared.Resources.Resource.Month : "Month") + ";");
            txt.Write((lang == "hu" ? Cargotruck.Shared.Resources.Resource.Earning : "Earning") + ";");
            txt.Write((lang == "hu" ? Cargotruck.Shared.Resources.Resource.Expense : "Expense") + ";");
            txt.Write((lang == "hu" ? Cargotruck.Shared.Resources.Resource.Profit : "Profit") + ";");
            txt.Write((lang == "hu" ? Cargotruck.Shared.Resources.Resource.Expenses : "Expenses") + ";");
            txt.Write((lang == "hu" ? Cargotruck.Shared.Resources.Resource.Task_id : "Task ID") + ";");
            txt.Write((lang == "hu" ? Cargotruck.Shared.Resources.Resource.Date : "Date") + ";");
            txt.Write("\n");

            foreach (var monthly_expense in Monthly_Expenses)
            {
                var s = "";
                txt.Write(monthly_expense.Monthly_expense_id + ";");
                txt.Write(monthly_expense.User_id + ";");
                txt.Write(monthly_expense.Date.Month + ";");
                txt.Write(monthly_expense.Earning + ";");
                txt.Write(monthly_expense.Expense + ";");
                txt.Write(monthly_expense.Profit + ";");
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
                txt.Write(s + ";");
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
                txt.Write(s + ";");
                txt.Write(monthly_expense.Date + ";");
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