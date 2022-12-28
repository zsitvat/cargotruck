using Cargotruck.Data;
using iTextSharp.text.pdf;
using iTextSharp.text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using ClosedXML.Excel;
using Document = iTextSharp.text.Document;
using Microsoft.Data.SqlClient;
using Cargotruck.Shared.Models;
using Font = iTextSharp.text.Font;
using System.Text;
using System.Linq.Dynamic.Core;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Reflection;

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
        public async Task<IActionResult> Get(int page, int pageSize, string sortOrder, bool desc, string? searchString)
        {
            var data = await _context.Monthly_Expenses.ToListAsync();
            searchString = searchString == null ? null : searchString.ToLower();

            if (searchString != null && searchString != "")
            {
                data = data.Where(s =>
            (s.Earning.ToString().ToLower()!.Contains(searchString))
            || (s.Profit.ToString().ToLower()!.Contains(searchString))
            || s.Date.ToString()!.Contains(searchString)
            ).ToList();
            }

            sortOrder = sortOrder == "Earning" ? (desc ? "Earning_desc" : "Earning") : (sortOrder);
            sortOrder = sortOrder == "Month" ? (desc ? "Month_desc" : "Month") : (sortOrder);
            sortOrder = sortOrder == "Profit" ? (desc ? "Profit_desc" : "Profit") : (sortOrder);
            sortOrder = sortOrder == "Date" || String.IsNullOrEmpty(sortOrder) ? (desc ? "Date_desc" : "") : (sortOrder);

            switch (sortOrder)
            {
                case "Earning_desc":
                    data = data.OrderByDescending(s => s.Earning).ToList();
                    break;
                case "Earning":
                    data = data.OrderBy(s => s.Earning).ToList();
                    break;
                case "Profit_desc":
                    data = data.OrderByDescending(s => s.Profit).ToList();
                    break;
                case "Profit":
                    data = data.OrderBy(s => s.Profit).ToList();
                    break;
                case "Month_desc":
                    data = data.OrderByDescending(s => s.Date.Month).ToList();
                    break;
                case "Month":
                    data = data.OrderBy(s => s.Date.Month).ToList();
                    break;
                case "Date_desc":
                    data = data.OrderByDescending(s => s.Date).ToList();
                    break;
                default:
                    data = data.OrderBy(s => s.Date).ToList();
                    break;
            }
            data = data.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            return Ok(data);
        }

        [HttpGet]
        public async Task<IActionResult> GetMonthly_Expenses()
        {
            var data = await _context.Monthly_Expenses.ToListAsync();
            return Ok(data);
        }

        [HttpGet]
        public async Task<IActionResult> PageCount()
        {
            var data = await _context.Monthly_Expenses.ToListAsync();
            int PageCount = data.Count();
            return Ok(PageCount);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var data = await _context.Monthly_Expenses.FirstOrDefaultAsync(a => a.Id == id);
            return Ok(data);
        }

        [HttpPost]
        public async Task<IActionResult> Post(Monthly_expenses data)
        {
            data.User_id = _context.Users.FirstOrDefault(a => a.UserName == User.Identity.Name).Id;
            _context.Add(data);
            await _context.SaveChangesAsync();
            return Ok(data.Id);
        }

        [HttpPut]
        public async Task<IActionResult> Put(Monthly_expenses data)
        {
            _context.Entry(data).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var data = new Monthly_expenses { Id = id };
            _context.Remove(data);
            await _context.SaveChangesAsync();
            return NoContent();
        }


        //closedXML needed !!!
        [HttpGet]
        public async Task<string> Excel(string lang)
        {
            var Monthly_Expenses = from data in _context.Monthly_Expenses select data;
            var cargoes = from data in _context.Cargoes select data;
            using (var workbook = new XLWorkbook())
            {
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
                worksheet.Cell(currentRow, 5).Value = lang == "hu" ? Cargotruck.Shared.Resources.Resource.Profit : "Profit";
                worksheet.Cell(currentRow, 5).Style.Font.SetBold();
                worksheet.Cell(currentRow, 6).Value = lang == "hu" ? Cargotruck.Shared.Resources.Resource.Expenses : "Expenses";
                worksheet.Cell(currentRow, 6).Style.Font.SetBold();
                worksheet.Cell(currentRow, 7).Value = lang == "hu" ? Cargotruck.Shared.Resources.Resource.Expenses : "Task_id";
                worksheet.Cell(currentRow, 7).Style.Font.SetBold();
                worksheet.Cell(currentRow, 8).Value = lang == "hu" ? Cargotruck.Shared.Resources.Resource.Date : "Date";
                worksheet.Cell(currentRow, 8).Style.Font.SetBold();

                foreach (var monthly_expense in Monthly_Expenses)
                {
                    var cellValue = "-";
                    currentRow++;

                    worksheet.Cell(currentRow, 1).Value = monthly_expense.Id;
                    worksheet.Cell(currentRow, 2).Value = monthly_expense.Date.Month;
                    worksheet.Cell(currentRow, 3).Value = monthly_expense.User_id;
                    worksheet.Cell(currentRow, 4).Value = monthly_expense.Earning;
                    worksheet.Cell(currentRow, 5).Value = monthly_expense.Profit;
                   /* worksheet.Cell(currentRow, 6).Value = monthly_expense.Expenses;
                    worksheet.Cell(currentRow, 7).Value = monthly_expense.Task_id;*/
                    worksheet.Cell(currentRow, 8).Value = monthly_expense.Date;
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return Convert.ToBase64String(content);
                }
            }
        }

        //iTextSharp needed !!!
        [HttpGet]
        public async Task<string> PDF(string lang)
        {
            var Monthly_Expenses = from data in _context.Monthly_Expenses select data;
            var cargoes = from data in _context.Cargoes select data;

            int pdfRowIndex = 1;
            Random rnd = new Random();
            int random = rnd.Next(1000000, 9999999);
            string filename = "Monthly_Expenses" + random + "_" + DateTime.Now.ToString("dd-MM-yyyy");
            string filepath = "Files/" + filename + ".pdf";

            Document document = new Document(PageSize.A4, 5f, 5f, 10f, 10f);
            FileStream fs = new FileStream(filepath, FileMode.Create);
            PdfWriter writer = PdfWriter.GetInstance(document, fs);
            document.Open();

            Font font1 = FontFactory.GetFont(FontFactory.TIMES_BOLD, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 12);
            Font font2 = FontFactory.GetFont(FontFactory.TIMES_ROMAN, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 10);

            System.Type type = typeof(Monthly_expenses);
            var column_number = (type.GetProperties().Length);
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

            var title = new Paragraph(15, lang == "hu" ? Cargotruck.Shared.Resources.Resource.Monthly_expenses : "Monthly expenses");
            title.Alignment = Element.ALIGN_CENTER;


            document.Add(title);
            document.Add(new Paragraph("\n"));

            if (Monthly_Expenses.Count() > 0)
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
                    if (!string.IsNullOrEmpty(monthly_expense.Id.ToString())) { s = monthly_expense.Id.ToString(); }
                    else { s = "-"; }
                    table.AddCell(new PdfPCell(new Phrase(s.ToString(), font2))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    });
                    if (!string.IsNullOrEmpty(monthly_expense.Date.ToString())) { s = monthly_expense.Date.Month.ToString(); }
                    else { s = "-"; }
                    table.AddCell(new PdfPCell(new Phrase(s.ToString(), font2))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    });
                    if (!string.IsNullOrEmpty(monthly_expense.Earning.ToString())) { s = monthly_expense.Earning.ToString(); }
                    else { s = "-"; }
                    table.AddCell(new PdfPCell(new Phrase(s.ToString(), font2))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    });
                    if (!string.IsNullOrEmpty(monthly_expense.Profit.ToString())) { s = monthly_expense.Profit.ToString(); }
                    else { s = "-"; }
                    table.AddCell(new PdfPCell(new Phrase(s.ToString(), font2))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    });
                    /*if (!string.IsNullOrEmpty(monthly_expense.Profit.ToString())) { s = monthly_expense.Expenses.ToString(); }
                    else { s = "-"; }
                    table.AddCell(new PdfPCell(new Phrase(s.ToString(), font2))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    });
                    if (!string.IsNullOrEmpty(monthly_expense.Profit.ToString())) { s = monthly_expense.Task_id.ToString(); }
                    else { s = "-"; }
                    table.AddCell(new PdfPCell(new Phrase(s.ToString(), font2))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    });*/
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
                var noContent = new Paragraph(lang == "hu" ? "Nem található adat!" : "No content found!");
                noContent.Alignment = Element.ALIGN_CENTER;
                document.Add(noContent);
            }
            document.Close();
            writer.Close();
            fs.Close();
            fs.Dispose();

            FileStream sourceFile = new FileStream(filepath, FileMode.Open);
            MemoryStream memoryStream = new MemoryStream();
            sourceFile.CopyToAsync(memoryStream);
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
            var Monthly_Expenses = from data in _context.Monthly_Expenses select data;
            var cargoes = from data in _context.Cargoes select data;

            Random rnd = new Random();
            int random = rnd.Next(1000000, 9999999);
            string filename = "Monthly_Expenses" + random + "_" + DateTime.Now.ToString("dd-MM-yyyy");
            string filepath = "Files/" + filename + ".csv";

            StreamWriter txt = new StreamWriter(filepath);
            txt.Write("Id" + ";");
            txt.Write((lang == "hu" ? Cargotruck.Shared.Resources.Resource.User_id : "User ID") + ";");
            txt.Write((lang == "hu" ? Cargotruck.Shared.Resources.Resource.Month : "Month") + ";");
            txt.Write((lang == "hu" ? Cargotruck.Shared.Resources.Resource.Earning : "Earning") + ";");
            txt.Write((lang == "hu" ? Cargotruck.Shared.Resources.Resource.Profit : "Profit") + ";");
            txt.Write((lang == "hu" ? Cargotruck.Shared.Resources.Resource.Expenses : "Expenses") + ";");
            txt.Write((lang == "hu" ? Cargotruck.Shared.Resources.Resource.Task_id : "Task ID") + ";");
            txt.Write((lang == "hu" ? Cargotruck.Shared.Resources.Resource.Date : "Date") + ";");
            txt.Write("\n");

            foreach (var monthly_expense in Monthly_Expenses)
            {
                txt.Write(monthly_expense.Id + ";");
                txt.Write(monthly_expense.User_id + ";");
                txt.Write(monthly_expense.Date.Month + ";");
                txt.Write(monthly_expense.Earning + ";");
                txt.Write(monthly_expense.Profit + ";");
                /*txt.Write(monthly_expense.Expenses + ";");
                txt.Write(monthly_expense.Task_id + ";");*/
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
            FileStream sourceFile = new FileStream(filepath, FileMode.Open);
            MemoryStream memoryStream = new MemoryStream();
            sourceFile.CopyToAsync(memoryStream);
            var buffer = memoryStream.ToArray();
            var file = Convert.ToBase64String(buffer);
            sourceFile.Dispose();
            sourceFile.Close();
            System.IO.File.Delete(filepath); // delete the file in the app folder
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
                    XLWorkbook workbook = new XLWorkbook(path);

                    IXLWorksheet worksheet = workbook.Worksheet(1);
                    //Loop through the Worksheet rows.
                    DataTable? dt = new DataTable();
                    bool firstRow = true;
                    if (worksheet.Row(2).CellsUsed().Count() > 1 && worksheet.Row(2).Cell(worksheet.Row(1).CellsUsed().Count()) != null)
                    {
                        int l = 0;
                        foreach (IXLRow row in worksheet.Rows())
                        {
                            //Use the first row to add columns to DataTable with column names check.
                            if (firstRow)
                            {
                                List<string?> titles = new List<string?>() {
                                "Id",
                                lang == "hu" ? Cargotruck.Shared.Resources.Resource.Month : "Month",
                                lang == "hu" ? Cargotruck.Shared.Resources.Resource.User_id : "User ID",
                                lang == "hu" ? Cargotruck.Shared.Resources.Resource.Earning : "Earning",
                                lang == "hu" ? Cargotruck.Shared.Resources.Resource.Profit : "Profit",
                                lang == "hu" ? Cargotruck.Shared.Resources.Resource.Expenses : "Expenses",
                                lang == "hu" ? Cargotruck.Shared.Resources.Resource.Task_id : "Task ID",
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
                                if (titles.Count() == 0)
                                {
                                    haveColumns = true;
                                    l += 1;
                                }
                                else if (titles.Count() == 1 && titles.Contains("Id"))
                                {
                                    haveColumns = true;

                                }
                            }
                            else if (haveColumns)
                            {
                                List<object?> list = new List<object?>();
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
                                if (nulls != list.Count())
                                {
                                    var sql = @"Insert Into Monthly_Expenses (User_id,Earning,Profit,Expenses,Task_id,Date) 
                                    Values (@User_id,@Earning,@Profit,@Expenses,@Task_id,@Date)";
                                    var insert = await _context.Database.ExecuteSqlRawAsync(sql,
                                        new SqlParameter("@User_id", list[l + 1]?.ToString()),
                                        new SqlParameter("@Earning", list[l + 2]),
                                        new SqlParameter("@Profit", list[l + 3]),
                                        new SqlParameter("@Expenses", list[l + 4]),
                                        new SqlParameter("@Task_id", list[l + 5]),
                                        new SqlParameter("@Date", list[l + 6] == System.DBNull.Value ? System.DBNull.Value : DateTime.Parse(list[l + 6].ToString()))
                                        );
                                    
                                    /*
                                    string[] substrings;

                                    substrings = list[l + 3].ToString().Split("]");
                                    
                                    if (substrings != null)
                                    {
                                        for (int s = 0; s < substrings.Length - 1; ++s)
                                        {
                                            var CargoId = substrings[s].Substring(1, substrings[s].IndexOf("/") - 1);
                                            var Monthly_Expensesection = substrings[s].Substring(substrings[s].IndexOf("/") + 1);

                                            var greatestId = _context.Monthly_Expenses.OrderBy(s => s.Id).Last().Id;

                                            var sql2 = @"Update Cargoes 
                                                        Set monthly_expense_id = @monthly_expense_id, monthly_expense_section = @monthly_expense_section
                                                         Where Id = @Id";
                                            var insert2 = await _context.Database.ExecuteSqlRawAsync(sql2,
                                                new SqlParameter("@monthly_expense_id", greatestId),
                                                new SqlParameter("@monthly_expense_section", Monthly_Expensesection),
                                                new SqlParameter("@Id", CargoId)
                                                );
                                        }
                                    }*/

                                    if (insert > 0)
                                    {
                                        error = "";
                                        await _context.SaveChangesAsync();
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
                    error = lang == "hu" ? @Cargotruck.Shared.Resources.Resource.Not_excel : "Bad format! The file is not an excel.";
                    System.IO.File.Delete(path); // delete the file
                    return BadRequest(error);
                }
            }
            else
            {
                error = lang == "hu" ? @Cargotruck.Shared.Resources.Resource.No_excel : "File not found!";
                return BadRequest(error);
            }
            return NoContent();
        }


        [HttpGet]
        public async Task<IActionResult> GetConnectionIds()
        {
            var data = await _context.Monthly_expenses_tasks_expenses.ToListAsync();
            return Ok(data);
        }
    }
}