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
using Microsoft.JSInterop;
using DocumentFormat.OpenXml.Office.CustomUI;
using DocumentFormat.OpenXml.Spreadsheet;
using Font = iTextSharp.text.Font;
using System.Text;
using Type = Cargotruck.Shared.Models.Type;
using DocumentFormat.OpenXml.Office2010.Excel;

namespace Cargotruck.Server.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ExpensesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public ExpensesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Get(int page, int pageSize, string sortOrder, bool desc, string? searchString, Type? filter, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate)
        {
            var data = await _context.Expenses.ToListAsync();

            data = data.Where(s => (dateFilterStartDate != null ? s.Date >= dateFilterStartDate : true) && (dateFilterEndDate != null ? s.Date <= dateFilterEndDate : true)).ToList();

            if (filter != null)
            {
                data = data.Where(data => data.Type == filter).ToList();
            }

            searchString = searchString == null ? null : searchString.ToLower();
            if (searchString != null && searchString != "")
            {
                data = data.Where(s =>
               s.Type.ToString().ToLower()!.Contains(searchString)
            || s.Type_id.ToString().ToLower()!.Contains(searchString)
            || (s.Fuel == null ? false : s.Fuel.ToString()!.Contains(searchString))
            || (s.Road_fees == null ? false : s.Road_fees.ToString()!.Contains(searchString))
            || (s.Penalty == null ? false : s.Fuel.ToString()!.Contains(searchString))
            || (s.Driver_spending == null ? false : s.Driver_spending.ToString()!.Contains(searchString))
            || (s.Driver_salary == null ? false : s.Driver_salary.ToString()!.Contains(searchString))
            || (s.Repair_cost == null ? false : s.Repair_cost.ToString()!.Contains(searchString))
            || (s.Repair_description == null ? false : s.Repair_description.ToString()!.Contains(searchString))
            || (s.Cost_of_storage == null ? false : s.Cost_of_storage.ToString()!.Contains(searchString))
            || (s.other == null ? false : s.other.ToString()!.Contains(searchString))
            || s.Date.ToString()!.Contains(searchString)
            ).ToList();
            }

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
            sortOrder = sortOrder == "other" ? (desc ? "other_desc" : "other") : (sortOrder);
            sortOrder = sortOrder == "Date" || String.IsNullOrEmpty(sortOrder) ? (desc ? "Date_desc" : "") : (sortOrder);

            switch (sortOrder)
            {
                case "Type_desc":
                    data = data.OrderByDescending(s => s.Type).ToList();
                    break;
                case "Type":
                    data = data.OrderBy(s => s.Type).ToList();
                    break;
                case "Type_id_desc":
                    data = data.OrderByDescending(s => s.Type_id).ToList();
                    break;
                case "Type_id":
                    data = data.OrderBy(s => s.Type_id).ToList();
                    break;
                case "Fuel_desc":
                    data = data.OrderByDescending(s => s.Fuel).ToList();
                    break;
                case "Fuel":
                    data = data.OrderBy(s => s.Fuel).ToList();
                    break;
                case "Road_fees_desc":
                    data = data.OrderByDescending(s => s.Road_fees).ToList();
                    break;
                case "Road_fees":
                    data = data.OrderBy(s => s.Road_fees).ToList();
                    break;
                case "Penalty_desc":
                    data = data.OrderByDescending(s => s.Penalty).ToList();
                    break;
                case "Penalty":
                    data = data.OrderBy(s => s.Penalty).ToList();
                    break;
                case "Driver_spending_desc":
                    data = data.OrderByDescending(s => s.Driver_spending).ToList();
                    break;
                case "Driver_spending":
                    data = data.OrderBy(s => s.Driver_spending).ToList();
                    break;
                case "Driver_salary_desc":
                    data = data.OrderByDescending(s => s.Driver_salary).ToList();
                    break;
                case "Driver_salary":
                    data = data.OrderBy(s => s.Driver_salary).ToList();
                    break;
                case "Repair_cost_desc":
                    data = data.OrderByDescending(s => s.Repair_cost).ToList();
                    break;
                case "Repair_cost":
                    data = data.OrderBy(s => s.Repair_cost).ToList();
                    break;
                case "Repair_description_desc":
                    data = data.OrderByDescending(s => s.Repair_description).ToList();
                    break;
                case "Repair_description":
                    data = data.OrderBy(s => s.Repair_description).ToList();
                    break;
                case "Cost_of_storage_desc":
                    data = data.OrderByDescending(s => s.Repair_description).ToList();
                    break;
                case "Cost_of_storage":
                    data = data.OrderBy(s => s.Repair_description).ToList();
                    break;
                case "other_desc":
                    data = data.OrderByDescending(s => s.Repair_description).ToList();
                    break;
                case "other":
                    data = data.OrderBy(s => s.Repair_description).ToList();
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
        public async Task<IActionResult> PageCount()
        {
            var data = await _context.Expenses.ToListAsync();
            int PageCount = data.Count();
            return Ok(PageCount);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var data = await _context.Expenses.FirstOrDefaultAsync(a => a.Id == id);
            return Ok(data);
        }

        [HttpGet]
        public async Task<IActionResult> GetExpenses()
        {
            var data = await _context.Expenses.ToListAsync();
            return Ok(data);
        }

        [HttpPost]
        public async Task<IActionResult> Post(Expenses data)
        {
            data.User_id = _context.Users.FirstOrDefault(a => a.UserName == User.Identity.Name).Id;
            _context.Add(data);
            await _context.SaveChangesAsync();

            if (data.Type.ToString() == Type.repair.ToString())
            {
                var road = _context.Roads.FirstOrDefault(a => a.Id == data.Type_id);
                road.Expenses_id = data.Id;
                _context.Entry(road).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            return Ok(data.Id);
        }

        [HttpPut]
        public async Task<IActionResult> Put(Expenses data)
        {
            _context.Entry(data).State = EntityState.Modified;
            if (data.Type.ToString() == Type.repair.ToString())
            {
                var road = _context.Roads.FirstOrDefault(a => a.Id == data.Type_id);
                road.Expenses_id = data.Id;
                _context.Entry(road).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var data = new Expenses { Id = id };
            _context.Remove(data);
            await _context.SaveChangesAsync();
            return NoContent();
        }


        //closedXML needed !!!
        [HttpGet]
        public async Task<string> Excel(string lang)
        {
            var expenses = from r in _context.Expenses select r;
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Expenses");
                var currentRow = 1;
                worksheet.Cell(currentRow, 1).Value = "Id";
                worksheet.Cell(currentRow, 1).Style.Font.SetBold();
                worksheet.Cell(currentRow, 2).Value = lang == "hu" ? Cargotruck.Shared.Resources.Resource.User_id : "User ID";
                worksheet.Cell(currentRow, 2).Style.Font.SetBold();
                worksheet.Cell(currentRow, 3).Value = lang == "hu" ? Cargotruck.Shared.Resources.Resource.Type : "Type";
                worksheet.Cell(currentRow, 3).Style.Font.SetBold();
                worksheet.Cell(currentRow, 4).Value = lang == "hu" ? Cargotruck.Shared.Resources.Resource.Type_id : "Type ID";
                worksheet.Cell(currentRow, 4).Style.Font.SetBold();
                worksheet.Cell(currentRow, 5).Value = lang == "hu" ? Cargotruck.Shared.Resources.Resource.Fuel : "Fuel";
                worksheet.Cell(currentRow, 5).Style.Font.SetBold();
                worksheet.Cell(currentRow, 6).Value = lang == "hu" ? Cargotruck.Shared.Resources.Resource.Road_fees : "Road fees";
                worksheet.Cell(currentRow, 6).Style.Font.SetBold();
                worksheet.Cell(currentRow, 7).Value = lang == "hu" ? Cargotruck.Shared.Resources.Resource.Penalty : "Penalty";
                worksheet.Cell(currentRow, 7).Style.Font.SetBold();
                worksheet.Cell(currentRow, 8).Value = lang == "hu" ? Cargotruck.Shared.Resources.Resource.Driver_spending : "Driver spending";
                worksheet.Cell(currentRow, 8).Style.Font.SetBold();
                worksheet.Cell(currentRow, 9).Value = lang == "hu" ? Cargotruck.Shared.Resources.Resource.Driver_salary : "Driver salary";
                worksheet.Cell(currentRow, 9).Style.Font.SetBold();
                worksheet.Cell(currentRow, 10).Value = lang == "hu" ? Cargotruck.Shared.Resources.Resource.Repair_cost : "Repair cost";
                worksheet.Cell(currentRow, 10).Style.Font.SetBold();
                worksheet.Cell(currentRow, 11).Value = lang == "hu" ? Cargotruck.Shared.Resources.Resource.Repair_description : "Repair description";
                worksheet.Cell(currentRow, 11).Style.Font.SetBold();
                worksheet.Cell(currentRow, 12).Value = lang == "hu" ? Cargotruck.Shared.Resources.Resource.Cost_of_storage : "Cost of storage";
                worksheet.Cell(currentRow, 12).Style.Font.SetBold();
                worksheet.Cell(currentRow, 13).Value = lang == "hu" ? Cargotruck.Shared.Resources.Resource.other : "other";
                worksheet.Cell(currentRow, 13).Style.Font.SetBold();
                worksheet.Cell(currentRow, 14).Value = lang == "hu" ? Cargotruck.Shared.Resources.Resource.Date : "Date";
                worksheet.Cell(currentRow, 14).Style.Font.SetBold();

                foreach (var expense in expenses)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = expense.Id;
                    worksheet.Cell(currentRow, 2).Value = expense.User_id;
                    worksheet.Cell(currentRow, 3).Value = expense.Type;
                    worksheet.Cell(currentRow, 4).Value = expense.Type_id;
                    worksheet.Cell(currentRow, 5).Value = expense.Fuel + (expense.Fuel != null ? " HUF" : "");
                    worksheet.Cell(currentRow, 6).Value = expense.Road_fees + (expense.Road_fees != null ? " HUF" : "");
                    worksheet.Cell(currentRow, 7).Value = expense.Penalty + (expense.Penalty != null ? " HUF" : "");
                    worksheet.Cell(currentRow, 8).Value = expense.Driver_spending + (expense.Driver_spending != null ? " HUF" : "");
                    worksheet.Cell(currentRow, 9).Value = expense.Driver_salary + (expense.Driver_salary != null ? " HUF" : "");
                    worksheet.Cell(currentRow, 10).Value = expense.Repair_cost + (expense.Repair_cost != null ? " HUF" : "");
                    worksheet.Cell(currentRow, 11).Value = expense.Repair_description;
                    worksheet.Cell(currentRow, 12).Value = expense.Cost_of_storage + (expense.Cost_of_storage != null ? " HUF" : "");
                    worksheet.Cell(currentRow, 13).Value = expense.other + (expense.other != null ? " HUF" : "");
                    worksheet.Cell(currentRow, 14).Value = expense.Date;
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
            var expenses = from r in _context.Expenses select r;

            int pdfRowIndex = 1;
            Random rnd = new Random();
            int random = rnd.Next(1000000, 9999999);
            string filename = "Expenses" + random + "_" + DateTime.Now.ToString("dd-MM-yyyy");
            string filepath = "Files/" + filename + ".pdf";

            Document document = new Document(PageSize.A4, 5f, 5f, 10f, 10f);
            FileStream fs = new FileStream(filepath, FileMode.Create);
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

            var title = new Paragraph(15, lang == "hu" ? Cargotruck.Shared.Resources.Resource.Expenses : "Expenses");
            title.Alignment = Element.ALIGN_CENTER;


            document.Add(title);
            document.Add(new Paragraph("\n"));

            if (expenses.Count() > 0)
            {
                table.AddCell(new PdfPCell(new Phrase("Id", font1))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                });
                table.AddCell(new PdfPCell(new Phrase(lang == "hu" ? Cargotruck.Shared.Resources.Resource.Type : "Type", font1))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                });
                table.AddCell(new PdfPCell(new Phrase(lang == "hu" ? Cargotruck.Shared.Resources.Resource.Type_id : "Type ID", font1))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                });
                table.AddCell(new PdfPCell(new Phrase(lang == "hu" ? Cargotruck.Shared.Resources.Resource.Fuel : "Fuel", font1))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                });
                table.AddCell(new PdfPCell(new Phrase(lang == "hu" ? Cargotruck.Shared.Resources.Resource.Road_fees : "Road fees", font1))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                });
                table.AddCell(new PdfPCell(new Phrase(lang == "hu" ? Cargotruck.Shared.Resources.Resource.Penalty : "Penalty", font1))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                });
                table.AddCell(new PdfPCell(new Phrase(lang == "hu" ? Cargotruck.Shared.Resources.Resource.Driver_spending : "Driver spending", font1))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                });

                foreach (Expenses expense in expenses)
                {
                    var s = "";
                    if (!string.IsNullOrEmpty(expense.Id.ToString())) { s = expense.Id.ToString(); }
                    else { s = "-"; }
                    table.AddCell(new PdfPCell(new Phrase(s.ToString(), font2))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    });
                    if (!string.IsNullOrEmpty(expense.Type.ToString())) { s = expense.Type.ToString(); }
                    else { s = "-"; }
                    table.AddCell(new PdfPCell(new Phrase(s.ToString(), font2))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    });
                    if (!string.IsNullOrEmpty(expense.Type_id.ToString())) { s = expense.Type_id.ToString(); }
                    else { s = "-"; }
                    table.AddCell(new PdfPCell(new Phrase(s.ToString(), font2))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    });
                    if (!string.IsNullOrEmpty(expense.Fuel.ToString())) { s = expense.Fuel.ToString() + " HUF"; }
                    else { s = "-"; }
                    table.AddCell(new PdfPCell(new Phrase(s.ToString(), font2))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    });
                    if (!string.IsNullOrEmpty(expense.Road_fees.ToString())) { s = expense.Road_fees.ToString() + " HUF"; }
                    else { s = "-"; }
                    table.AddCell(new PdfPCell(new Phrase(s.ToString(), font2))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    });
                    if (!string.IsNullOrEmpty(expense.Penalty.ToString())) { s = expense.Penalty.ToString() + " HUF"; }
                    else { s = "-"; }
                    table.AddCell(new PdfPCell(new Phrase(s.ToString(), font2))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    });
                    if (!string.IsNullOrEmpty(expense.Driver_spending.ToString())) { s = expense.Driver_spending.ToString() + " HUF"; }
                    else { s = "-"; }
                    table.AddCell(new PdfPCell(new Phrase(s.ToString(), font2))
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
                table2.AddCell(new PdfPCell(new Phrase(lang == "hu" ? Cargotruck.Shared.Resources.Resource.Driver_salary : "Driver salary", font1))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                });
                table2.AddCell(new PdfPCell(new Phrase(lang == "hu" ? Cargotruck.Shared.Resources.Resource.Repair_cost : "Repair cost", font1))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                });
                table2.AddCell(new PdfPCell(new Phrase(lang == "hu" ? Cargotruck.Shared.Resources.Resource.Repair_description : "Repair description", font1))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                });
                table2.AddCell(new PdfPCell(new Phrase(lang == "hu" ? Cargotruck.Shared.Resources.Resource.Cost_of_storage : "Cost of storage", font1))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                });
                table2.AddCell(new PdfPCell(new Phrase(lang == "hu" ? Cargotruck.Shared.Resources.Resource.other : "other", font1))
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


                foreach (Expenses expense in expenses)
                {
                    var s = "";
                    if (!string.IsNullOrEmpty(expense.Id.ToString())) { s = expense.Id.ToString(); }
                    else { s = "-"; }
                    table2.AddCell(new PdfPCell(new Phrase(s.ToString(), font2))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    });
                    if (!string.IsNullOrEmpty(expense.Driver_salary.ToString())) { s = expense.Driver_salary.ToString() + " HUF"; }
                    else { s = "-"; }
                    table2.AddCell(new PdfPCell(new Phrase(s.ToString(), font2))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    });
                    if (!string.IsNullOrEmpty(expense.Repair_cost.ToString())) { s = expense.Repair_cost.ToString() + " HUF"; }
                    else { s = "-"; }
                    table2.AddCell(new PdfPCell(new Phrase(s.ToString(), font2))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    });
                    if (!string.IsNullOrEmpty(expense.Repair_description)) { s = expense.Repair_description.ToString() + " HUF"; }
                    else { s = "-"; }
                    table2.AddCell(new PdfPCell(new Phrase(s.ToString(), font2))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    });
                    if (!string.IsNullOrEmpty(expense.Cost_of_storage.ToString())) { s = expense.Cost_of_storage.ToString() + " HUF"; }
                    else { s = "-"; }
                    table2.AddCell(new PdfPCell(new Phrase(s.ToString(), font2))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    });
                    if (!string.IsNullOrEmpty(expense.other.ToString())) { s = expense.other.ToString() + " HUF"; }
                    else { s = "-"; }
                    table2.AddCell(new PdfPCell(new Phrase(s.ToString(), font2))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    });
                    if (!string.IsNullOrEmpty(expense.Date.ToString())) { s = expense.Date.ToString(); }
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
            var expenses = from r in _context.Expenses select r;

            Random rnd = new Random();
            int random = rnd.Next(1000000, 9999999);
            string filename = "Expenses" + random + "_" + DateTime.Now.ToString("dd-MM-yyyy");
            string filepath = "Files/" + filename + ".csv";

            StreamWriter txt = new StreamWriter(filepath);
            txt.Write("Id" + ";");
            txt.Write((lang == "hu" ? Cargotruck.Shared.Resources.Resource.User_id : "User ID") + ";");
            txt.Write((lang == "hu" ? Cargotruck.Shared.Resources.Resource.Type : "Type") + ";");
            txt.Write((lang == "hu" ? Cargotruck.Shared.Resources.Resource.Type_id : "Type ID") + ";");
            txt.Write((lang == "hu" ? Cargotruck.Shared.Resources.Resource.Fuel : "Fuel") + ";");
            txt.Write((lang == "hu" ? Cargotruck.Shared.Resources.Resource.Road_fees : "Road fees") + ";");
            txt.Write((lang == "hu" ? Cargotruck.Shared.Resources.Resource.Penalty : "Penalty") + ";");
            txt.Write((lang == "hu" ? Cargotruck.Shared.Resources.Resource.Driver_spending : "Driver spending") + ";");
            txt.Write((lang == "hu" ? Cargotruck.Shared.Resources.Resource.Driver_salary : "Driver salary") + ";");
            txt.Write((lang == "hu" ? Cargotruck.Shared.Resources.Resource.Repair_cost : "Repair cost") + ";");
            txt.Write((lang == "hu" ? Cargotruck.Shared.Resources.Resource.Repair_description : "Repair description") + ";");
            txt.Write((lang == "hu" ? Cargotruck.Shared.Resources.Resource.Cost_of_storage : "Cost of storage") + ";");
            txt.Write((lang == "hu" ? Cargotruck.Shared.Resources.Resource.other : "other") + ";");
            txt.Write((lang == "hu" ? Cargotruck.Shared.Resources.Resource.Date : "Date") + ";");
            txt.Write("\n");

            foreach (var expense in expenses)
            {
                txt.Write(expense.Id + ";");
                txt.Write(expense.User_id + ";");
                txt.Write(expense.Type + ";");
                txt.Write(expense.Type_id + ";");
                txt.Write(expense.Fuel + (expense.Fuel != null ? " HUF" : "") + ";");
                txt.Write(expense.Road_fees + (expense.Road_fees != null ? " HUF" : "") + ";");
                txt.Write(expense.Penalty + (expense.Penalty != null ? " HUF" : "") + ";");
                txt.Write(expense.Driver_spending + (expense.Driver_spending != null ? " HUF" : "") + ";");
                txt.Write(expense.Driver_salary + (expense.Driver_salary != null ? " HUF" : "") + ";");
                txt.Write(expense.Repair_cost + (expense.Repair_cost != null ? " HUF" : "") + ";");
                txt.Write(expense.Repair_description + ";");
                txt.Write(expense.Cost_of_storage + (expense.Cost_of_storage != null ? " HUF" : "") + ";");
                txt.Write(expense.other + (expense.other != null ? " HUF" : "") + ";");
                txt.Write(expense.Date + ";");
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
                                lang == "hu" ? Cargotruck.Shared.Resources.Resource.User_id : "User ID",
                                lang == "hu" ? Cargotruck.Shared.Resources.Resource.Type : "Type",
                                lang == "hu" ? Cargotruck.Shared.Resources.Resource.Type_id : "Type ID",
                                lang == "hu" ? Cargotruck.Shared.Resources.Resource.Fuel : "Fuel",
                                lang == "hu" ? Cargotruck.Shared.Resources.Resource.Road_fees : "Road fees",
                                lang == "hu" ? Cargotruck.Shared.Resources.Resource.Penalty : "Penalty",
                                lang == "hu" ? Cargotruck.Shared.Resources.Resource.Driver_spending : "Driver spending",
                                lang == "hu" ? Cargotruck.Shared.Resources.Resource.Driver_salary : "Driver salary",
                                lang == "hu" ? Cargotruck.Shared.Resources.Resource.Repair_cost : "Repair cost",
                                lang == "hu" ? Cargotruck.Shared.Resources.Resource.Repair_description : "Repair description",
                                lang == "hu" ? Cargotruck.Shared.Resources.Resource.Cost_of_storage : "Cost of storage",
                                lang == "hu" ? Cargotruck.Shared.Resources.Resource.other : "other",
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
                                //Add rows to DataTable.
                                dt.Rows.Add();
                                foreach (IXLCell cell in row.Cells(1, dt.Columns.Count))
                                {
                                    if (cell.Value != null && cell.Value.ToString() != "")
                                    {
                                        list.Add(cell.Value);
                                    }
                                    else { list.Add(System.DBNull.Value); }
                                }

                                switch (list[l + 1])
                                {
                                    case "task":
                                        list[l + 1] = 0;
                                        break;
                                    case "repair":
                                        list[l + 1] = 1;
                                        break;
                                    case "storage":
                                        list[l + 1] = 2;
                                        break;
                                    case "salary":
                                        list[l + 1] = 3;
                                        break;
                                    case "other":
                                        list[l + 1] = 4;
                                        break;
                                    default:
                                        list[l + 1] = System.DBNull.Value;
                                        break;
                                }

                                for (int i=l+3; i < list.Count()-1; i++)
                                {
                                    if (i != (l + 9) && list[i]!= null && list[i] != System.DBNull.Value)
                                    {
                                        list[i] = new String(list[i]?.ToString()?.Where(Char.IsDigit).ToArray());
                                    }
                                }

                                var sql = @"Insert Into Expenses (User_id,Type,Type_id,Fuel,Road_fees,Penalty,Driver_spending,Driver_salary,Repair_cost,Repair_description,Cost_of_storage,other,Date) 
                                Values (@User_id,@Type,@Type_id,@Fuel,@Road_fees,@Penalty,@Driver_spending,@Driver_salary,@Repair_cost,@Repair_description,@Cost_of_storage,@other,@Date)";
                                var insert = await _context.Database.ExecuteSqlRawAsync(sql,
                                    new SqlParameter("@User_id", list[l]),
                                    new SqlParameter("@Type", list[l + 1]),
                                    new SqlParameter("@Type_id", list[l + 2]),
                                    new SqlParameter("@Fuel", list[l + 3]),
                                    new SqlParameter("@Road_fees", list[l + 4]),
                                    new SqlParameter("@Penalty", list[l + 5]),
                                    new SqlParameter("@Driver_spending", list[l + 6]),
                                    new SqlParameter("@Driver_salary", list[l + 7]),
                                    new SqlParameter("@Repair_cost", list[l + 8]),
                                    new SqlParameter("@Repair_description", list[l + 9]),
                                    new SqlParameter("@Cost_of_storage", list[l + 10]),
                                    new SqlParameter("@other", list[l + 11]),
                                    new SqlParameter("@Date", DateTime.Now)
                                    );

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
    }
}