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

namespace Cargotruck.Server.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class RoadsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public RoadsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Get(int page, int pageSize, string sortOrder, bool desc, string? searchString)
        {
            var r = await _context.Roads.ToListAsync();
            searchString = searchString == null ? null : searchString.ToLower();

            if (searchString != null && searchString != "")
            {
                    r = r.Where(s => 
                    (s.Task_id == null ? false : s.Task_id.ToString().ToLower()!.Contains(searchString))
                || (s.Id_cargo == null ? false : s.Id_cargo.ToString().ToLower()!.Contains(searchString))
                || (s.Purpose_of_the_trip == null ? false : s.Purpose_of_the_trip.ToLower()!.Contains(searchString))
                || s.Starting_date.ToString().ToLower()!.Contains(searchString)
                || s.Ending_date.ToString()!.Contains(searchString)
                || (s.Starting_place == null ? false : s.Starting_place.ToLower()!.Contains(searchString))
                || (s.Ending_place == null ? false : s.Ending_place.ToLower()!.Contains(searchString))
                || (s.Direction == null ? false : s.Direction.ToString()!.Contains(searchString))
                || (s.Expenses_id == null ? false : s.Expenses_id.ToString()!.Contains(searchString))
                || s.Date.ToString()!.Contains(searchString)
                ).ToList();
            }

            sortOrder = sortOrder == "Task_id" ? (desc ? "Task_id_desc" : "Task_id") : (sortOrder);
            sortOrder = sortOrder == "Id_cargo" ? (desc ? "Id_cargo_desc" : "Id_cargo") : (sortOrder);
            sortOrder = sortOrder == "Purpose_of_the_trip" ? (desc ? "Purpose_of_the_trip_desc" : "Purpose_of_the_trip") : (sortOrder);
            sortOrder = sortOrder == "Starting_date" ? (desc ? "Starting_date_desc" : "Starting_date") : (sortOrder);
            sortOrder = sortOrder == "Ending_date" ? (desc ? "Ending_date_desc" : "Ending_date") : (sortOrder);
            sortOrder = sortOrder == "Starting_place" ? (desc ? "Starting_place_desc" : "Starting_place") : (sortOrder);
            sortOrder = sortOrder == "Ending_place" ? (desc ? "Ending_place_desc" : "Ending_place") : (sortOrder);
            sortOrder = sortOrder == "Direction" ? (desc ? "Direction_desc" : "Direction") : (sortOrder);
            sortOrder = sortOrder == "Expenses_id" ? (desc ? "Expenses_id_desc" : "Expenses_id") : (sortOrder);
            sortOrder = sortOrder == "Date" || String.IsNullOrEmpty(sortOrder) ? (desc ? "Date_desc" : "") : (sortOrder);

            switch (sortOrder)
            {
                case "Task_id_desc":
                    r = r.OrderByDescending(s => s.Task_id).ToList();
                    break;
                case "Task_id":
                    r = r.OrderBy(s => s.Task_id).ToList();
                    break;
                case "Id_cargo_desc":
                    r = r.OrderByDescending(s => s.Id_cargo).ToList();
                    break;
                case "Id_cargo":
                    r = r.OrderBy(s => s.Id_cargo).ToList();
                    break;
                case "Purpose_of_the_trip_desc":
                    r = r.OrderByDescending(s => s.Purpose_of_the_trip).ToList();
                    break;
                case "Purpose_of_the_trip":
                    r = r.OrderBy(s => s.Purpose_of_the_trip).ToList();
                    break;
                case "Starting_date_desc":
                    r = r.OrderByDescending(s => s.Starting_date).ToList();
                    break;
                case "Starting_date":
                    r = r.OrderBy(s => s.Starting_date).ToList();
                    break;
                case "Ending_date_desc":
                    r = r.OrderByDescending(s => s.Ending_date).ToList();
                    break;
                case "Ending_date":
                    r = r.OrderBy(s => s.Ending_date).ToList();
                    break;
                case "Starting_place_desc":
                    r = r.OrderByDescending(s => s.Starting_place).ToList();
                    break;
                case "Starting_place":
                    r = r.OrderBy(s => s.Starting_place).ToList();
                    break;
                case "Ending_place_desc":
                    r = r.OrderByDescending(s => s.Ending_place).ToList();
                    break;
                case "Ending_place":
                    r = r.OrderBy(s => s.Ending_place).ToList();
                    break;
                case "Direction_desc":
                    r = r.OrderByDescending(s => s.Direction).ToList();
                    break;
                case "Direction":
                    r = r.OrderBy(s => s.Direction).ToList();
                    break;
                case "Expenses_id_desc":
                    r = r.OrderByDescending(s => s.Expenses_id).ToList();
                    break;
                case "Expenses_id":
                    r = r.OrderBy(s => s.Expenses_id).ToList();
                    break;
                case "Date_desc":
                    r = r.OrderByDescending(s => s.Date).ToList();
                    break;
                default:
                    r = r.OrderBy(s => s.Date).ToList();
                    break;
            }
            r = r.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            return Ok(r);
        }

        [HttpGet]
        public async Task<IActionResult> PageCount()
        {
            var data = await _context.Roads.ToListAsync();
            int PageCount = data.Count();
            return Ok(PageCount);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var data = await _context.Roads.FirstOrDefaultAsync(a => a.Id == id);
            return Ok(data);
        }

        [HttpGet]
        public async Task<IActionResult> GetRoads()
        {
            var data = await _context.Roads.ToListAsync();
            return Ok(data);
        }

        [HttpPost]
        public async Task<IActionResult> Post(Roads data)
        {
            data.User_id = _context.Users.FirstOrDefault(a => a.UserName == User.Identity.Name).Id;
            _context.Add(data);
            await _context.SaveChangesAsync();
            var expense = _context.Expenses.FirstOrDefault(a => a.Id == data.Expenses_id);
            expense.Type_id = data.Id;
            expense.Type = Shared.Models.Type.repair;
            _context.Entry(expense).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return Ok(data.Id);
        }

        [HttpPut]
        public async Task<IActionResult> Put(Roads data)
        {
            _context.Entry(data).State = EntityState.Modified;
            var expense = _context.Expenses.FirstOrDefault(a => a.Id == data.Expenses_id);
            expense.Type_id = data.Id;
            expense.Type = Shared.Models.Type.repair;
            _context.Entry(expense).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var data = new Roads { Id = id };
            _context.Remove(data);
            await _context.SaveChangesAsync();
            return NoContent();
        }


        //closedXML needed !!!
        [HttpGet]
        public async Task<string> Excel(string lang)
        {
            var roads = from data in _context.Roads select data;
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Roads");
                var currentRow = 1;
                worksheet.Cell(currentRow, 1).Value = "Id";
                worksheet.Cell(currentRow, 1).Style.Font.SetBold();
                worksheet.Cell(currentRow, 2).Value = lang == "hu" ? Cargotruck.Shared.Resources.Resource.User_id : "User ID";
                worksheet.Cell(currentRow, 2).Style.Font.SetBold();
                worksheet.Cell(currentRow, 3).Value = lang == "hu" ? Cargotruck.Shared.Resources.Resource.Task_id : "Task ID";
                worksheet.Cell(currentRow, 3).Style.Font.SetBold();
                worksheet.Cell(currentRow, 4).Value = lang == "hu" ? Cargotruck.Shared.Resources.Resource.Id_cargo : "Cargo ID";
                worksheet.Cell(currentRow, 4).Style.Font.SetBold();
                worksheet.Cell(currentRow, 5).Value = lang == "hu" ? Cargotruck.Shared.Resources.Resource.Purpose_of_the_trip : "Purpose_of_the_trip";
                worksheet.Cell(currentRow, 5).Style.Font.SetBold();
                worksheet.Cell(currentRow, 6).Value = lang == "hu" ? Cargotruck.Shared.Resources.Resource.Starting_date : "Starting_date";
                worksheet.Cell(currentRow, 6).Style.Font.SetBold();
                worksheet.Cell(currentRow, 7).Value = lang == "hu" ? Cargotruck.Shared.Resources.Resource.Ending_date : "Ending_date";
                worksheet.Cell(currentRow, 7).Style.Font.SetBold();
                worksheet.Cell(currentRow, 8).Value = lang == "hu" ? Cargotruck.Shared.Resources.Resource.Starting_place : "Starting_place";
                worksheet.Cell(currentRow, 8).Style.Font.SetBold();
                worksheet.Cell(currentRow, 9).Value = lang == "hu" ? Cargotruck.Shared.Resources.Resource.Ending_place : "Ending_place";
                worksheet.Cell(currentRow, 9).Style.Font.SetBold();
                worksheet.Cell(currentRow, 10).Value = lang == "hu" ? Cargotruck.Shared.Resources.Resource.Direction : "Direction";
                worksheet.Cell(currentRow, 10).Style.Font.SetBold();
                worksheet.Cell(currentRow, 11).Value = lang == "hu" ? Cargotruck.Shared.Resources.Resource.Expenses_id : "Expenses_id";
                worksheet.Cell(currentRow, 11).Style.Font.SetBold();
                worksheet.Cell(currentRow, 12).Value = lang == "hu" ? Cargotruck.Shared.Resources.Resource.Date : "Date";
                worksheet.Cell(currentRow, 12).Style.Font.SetBold();

                foreach (var road in roads)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = road.Id;
                    worksheet.Cell(currentRow, 2).Value = road.User_id;
                    worksheet.Cell(currentRow, 3).Value = road.Task_id;
                    worksheet.Cell(currentRow, 4).Value = road.Id_cargo;
                    worksheet.Cell(currentRow, 5).Value = road.Purpose_of_the_trip;
                    worksheet.Cell(currentRow, 6).Value = road.Starting_date;
                    worksheet.Cell(currentRow, 7).Value = road.Ending_date;
                    worksheet.Cell(currentRow, 8).Value = road.Starting_place;
                    worksheet.Cell(currentRow, 9).Value = road.Ending_place;
                    worksheet.Cell(currentRow, 10).Value = (road.ToString() == "TO" ? lang == "hu" ? Cargotruck.Shared.Resources.Resource.to : "Go to the direction" : lang == "hu" ? Cargotruck.Shared.Resources.Resource.from : "Go from the direction");
                    worksheet.Cell(currentRow, 11).Value = road.Expenses_id;
                    worksheet.Cell(currentRow, 12).Value = road.Date;
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
            var roads = from r in _context.Roads select r;

            int pdfRowIndex = 1;
            Random rnd = new Random();
            int random = rnd.Next(1000000, 9999999);
            string filename = "Roads" + random + "_" + DateTime.Now.ToString("dd-MM-yyyy");
            string filepath = "Files/" + filename + ".pdf";

            Document document = new Document(PageSize.A4, 5f, 5f, 10f, 10f);
            FileStream fs = new FileStream(filepath, FileMode.Create);
            PdfWriter writer = PdfWriter.GetInstance(document, fs);
            document.Open();

            Font font1 = FontFactory.GetFont(FontFactory.TIMES_BOLD, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 12);
            Font font2 = FontFactory.GetFont(FontFactory.TIMES_ROMAN, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 10);

            System.Type type = typeof(Roads);
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

            var title = new Paragraph(15, lang == "hu" ? Cargotruck.Shared.Resources.Resource.Roads : "Roads");
            title.Alignment = Element.ALIGN_CENTER;


            document.Add(title);
            document.Add(new Paragraph("\n"));

            if (roads.Count() > 0)
            {
                table.AddCell(new PdfPCell(new Phrase("Id", font1))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                });
                table.AddCell(new PdfPCell(new Phrase(lang == "hu" ? Cargotruck.Shared.Resources.Resource.Task_id : "Task ID", font1))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                });
                table.AddCell(new PdfPCell(new Phrase(lang == "hu" ? Cargotruck.Shared.Resources.Resource.Id_cargo : "Cargo ID", font1))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                });
                table.AddCell(new PdfPCell(new Phrase(lang == "hu" ? Cargotruck.Shared.Resources.Resource.Purpose_of_the_trip : "Purpose of the trip", font1))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                });
                table.AddCell(new PdfPCell(new Phrase(lang == "hu" ? Cargotruck.Shared.Resources.Resource.Starting_date : "Starting date", font1))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                });
                table.AddCell(new PdfPCell(new Phrase(lang == "hu" ? Cargotruck.Shared.Resources.Resource.Ending_date : "Ending date", font1))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                });

                foreach (Roads road in roads)
                {
                    var s = "";
                    if (!string.IsNullOrEmpty(road.Id.ToString())) { s = road.Id.ToString(); }
                    else { s = "-"; }
                    table.AddCell(new PdfPCell(new Phrase(s.ToString(), font2))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    });
                    if (!string.IsNullOrEmpty(road.Task_id.ToString())) { s = road.Task_id.ToString(); }
                    else { s = "-"; }
                    table.AddCell(new PdfPCell(new Phrase(s.ToString(), font2))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    });
                    if (!string.IsNullOrEmpty(road.Id_cargo.ToString())) { s = road.Id_cargo.ToString(); }
                    else { s = "-"; }
                    table.AddCell(new PdfPCell(new Phrase(s.ToString(), font2))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    });
                    if (!string.IsNullOrEmpty(road.Purpose_of_the_trip.ToString())) { s = road.Purpose_of_the_trip.ToString(); }
                    else { s = "-"; }
                    table.AddCell(new PdfPCell(new Phrase(s.ToString(), font2))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    });
                    if (!string.IsNullOrEmpty(road.Starting_date.ToString())) { s = road.Starting_date.ToString(); }
                    else { s = "-"; }
                    table.AddCell(new PdfPCell(new Phrase(s.ToString(), font2))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    });
                    if (!string.IsNullOrEmpty(road.Ending_date.ToString())) { s = road.Ending_date.ToString(); }
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
                table2.AddCell(new PdfPCell(new Phrase(lang == "hu" ? Cargotruck.Shared.Resources.Resource.Starting_place : "Starting place", font1))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                });
                table2.AddCell(new PdfPCell(new Phrase(lang == "hu" ? Cargotruck.Shared.Resources.Resource.Ending_place : "Ending place", font1))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                });
                table2.AddCell(new PdfPCell(new Phrase(lang == "hu" ? Cargotruck.Shared.Resources.Resource.Direction : "Direction", font1))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                });
                table2.AddCell(new PdfPCell(new Phrase(lang == "hu" ? Cargotruck.Shared.Resources.Resource.Expenses_id : "Expenses ID", font1))
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


                foreach (Roads road in roads)
                {
                    var s = "";
                    if (!string.IsNullOrEmpty(road.Id.ToString())) { s = road.Id.ToString(); }
                    else { s = "-"; }
                    table2.AddCell(new PdfPCell(new Phrase(s.ToString(), font2))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    });
                    if (!string.IsNullOrEmpty(road.Starting_place)) { s = road.Starting_place.ToString(); }
                    else { s = "-"; }
                    table2.AddCell(new PdfPCell(new Phrase(s.ToString(), font2))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    });
                    if (!string.IsNullOrEmpty(road.Ending_place)) { s = road.Ending_place.ToString(); }
                    else { s = "-"; }
                    table2.AddCell(new PdfPCell(new Phrase(s.ToString(), font2))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    });
                    if (!string.IsNullOrEmpty(road.Direction.ToString())) { s = road.Direction.ToString(); }
                    else { s = "-"; }
                    table2.AddCell(new PdfPCell(new Phrase( s.ToString()=="TO" ? lang=="hu" ? Cargotruck.Shared.Resources.Resource.to: "Go to the direction" : lang == "hu" ? Cargotruck.Shared.Resources.Resource.from : "Go from the direction", font2))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    });
                    if (!string.IsNullOrEmpty(road.Expenses_id.ToString())) { s = road.Expenses_id.ToString(); }
                    else { s = "-"; }
                    table2.AddCell(new PdfPCell(new Phrase(s.ToString(), font2))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    });
                    if (!string.IsNullOrEmpty(road.Date.ToString())) { s = road.Date.ToString(); }
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
            var roads = from r in _context.Roads select r;

            Random rnd = new Random();
            int random = rnd.Next(1000000, 9999999);
            string filename = "Roads" + random + "_" + DateTime.Now.ToString("dd-MM-yyyy");
            string filepath = "Files/" + filename + ".csv";

            StreamWriter txt = new StreamWriter(filepath);
            txt.Write("Id" + ";");
            txt.Write((lang == "hu" ? Cargotruck.Shared.Resources.Resource.User_id : "User ID") + ";");
            txt.Write((lang == "hu" ? Cargotruck.Shared.Resources.Resource.Task_id : "Task ID") + ";");
            txt.Write((lang == "hu" ? Cargotruck.Shared.Resources.Resource.Id_cargo : "Cargo ID") + ";");
            txt.Write((lang == "hu" ? Cargotruck.Shared.Resources.Resource.Purpose_of_the_trip : "Purpose of the trip") + ";");
            txt.Write((lang == "hu" ? Cargotruck.Shared.Resources.Resource.Starting_date : "Starting date") + ";");
            txt.Write((lang == "hu" ? Cargotruck.Shared.Resources.Resource.Ending_date : "Ending_date") + ";");
            txt.Write((lang == "hu" ? Cargotruck.Shared.Resources.Resource.Starting_place : "Starting place") + ";");
            txt.Write((lang == "hu" ? Cargotruck.Shared.Resources.Resource.Ending_place : "Ending_place") + ";");
            txt.Write((lang == "hu" ? Cargotruck.Shared.Resources.Resource.Direction : "Direction") + ";" );
            txt.Write((lang == "hu" ? Cargotruck.Shared.Resources.Resource.Expenses_id : "Expenses_id") + ";");
            txt.Write((lang == "hu" ? Cargotruck.Shared.Resources.Resource.Date : "Date") + ";");
            txt.Write("\n");

            foreach (var road in roads)
            {
                txt.Write(road.Id + ";");
                txt.Write(road.User_id + ";");
                txt.Write(road.Task_id + ";");
                txt.Write(road.Id_cargo + ";");
                txt.Write(road.Purpose_of_the_trip + ";"); 
                txt.Write(road.Starting_date + ";");
                txt.Write(road.Ending_date + ";");
                txt.Write(road.Starting_place + ";");
                txt.Write(road.Ending_place + ";");
                txt.Write((road.Direction == "TO" ? lang == "hu" ? Cargotruck.Shared.Resources.Resource.to : "Go to the direction" : lang == "hu" ? Cargotruck.Shared.Resources.Resource.from : "Go from the direction") + ";");
                txt.Write(road.Expenses_id + ";");
                txt.Write(road.Date + ";");
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
                                lang == "hu" ? Cargotruck.Shared.Resources.Resource.Task_id : "Task ID",
                                lang == "hu" ? Cargotruck.Shared.Resources.Resource.Id_cargo : "Cargo ID",
                                lang == "hu" ? Cargotruck.Shared.Resources.Resource.Purpose_of_the_trip : "Purpose of the trip",
                                lang == "hu" ? Cargotruck.Shared.Resources.Resource.Starting_date : "Starting date",
                                lang == "hu" ? Cargotruck.Shared.Resources.Resource.Ending_date : "Ending_date",
                                lang == "hu" ? Cargotruck.Shared.Resources.Resource.Starting_place : "Starting place",
                                lang == "hu" ? Cargotruck.Shared.Resources.Resource.Ending_place : "Ending_place",
                                lang == "hu" ? Cargotruck.Shared.Resources.Resource.Direction : "Direction",
                                lang == "hu" ? Cargotruck.Shared.Resources.Resource.Expenses_id : "Expenses_id",
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
                                var sql = @"Insert Into Roads (User_id,Task_id,Id_cargo,Purpose_of_the_trip,Starting_date,Ending_date,Starting_place,Ending_place,Direction,Expenses_id,Date) 
                                Values (@User_id,@Task_id,@Id_cargo,@Purpose_of_the_trip,@Starting_date,@Ending_date,@Starting_place,@Ending_place,@Direction,@Expenses_id,@Date)";
                                var insert = await _context.Database.ExecuteSqlRawAsync(sql,
                                    new SqlParameter("@User_id", list[l]),
                                    new SqlParameter("@Task_id", list[l + 1]),
                                    new SqlParameter("@Id_cargo", list[l + 2]),
                                    new SqlParameter("@Purpose_of_the_trip", list[l + 3]),
                                    new SqlParameter("@Starting_date", list[l + 4] == System.DBNull.Value ? System.DBNull.Value : DateTime.Parse(list[l + 4].ToString())),
                                    new SqlParameter("@Ending_date", list[l + 5] == System.DBNull.Value ? System.DBNull.Value : DateTime.Parse(list[l + 5].ToString())),
                                    new SqlParameter("@Starting_place", list[l + 6]),
                                    new SqlParameter("@Ending_place", list[l + 7]),
                                    new SqlParameter("@Direction", list[l + 8]),
                                    new SqlParameter("@Expenses_id", list[l + 9]),
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