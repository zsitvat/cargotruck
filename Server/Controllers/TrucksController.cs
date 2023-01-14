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
using DocumentFormat.OpenXml.Office2010.Excel;

namespace Cargotruck.Server.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TrucksController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public TrucksController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Get(int page, int pageSize, string sortOrder, bool desc, string? searchString,Status? filter, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate)
        {
            var data = await _context.Trucks.Where(s => (dateFilterStartDate != null ? (s.Date >= dateFilterStartDate) : true) && (dateFilterEndDate != null ? (s.Date <= dateFilterEndDate) : true)).ToListAsync();

            if (filter != null)
            {
                data = data.Where(data => data.Status == filter).ToList();
            }

            searchString = searchString == null ? null : searchString.ToLower();
            if (searchString != null && searchString != "")
            {
                data = data.Where(s =>
               (s.Vehicle_registration_number.ToString().ToLower()!.Contains(searchString))
            || (s.Brand == null ? false : s.Brand.ToString().ToLower()!.Contains(searchString))
            || (s.Status.ToString()!.Contains(searchString))
            || (s.Road_id == null ? false : s.Road_id.ToString().ToLower()!.Contains(searchString))
            || (s.Max_weight == null ? false : s.Max_weight.ToString()!.Contains(searchString))
            || s.Date.ToString()!.Contains(searchString)
            ).ToList();
            }

            sortOrder = sortOrder == "Vehicle_registration_number" ? (desc ? "Vehicle_registration_number_desc" : "Vehicle_registration_number") : (sortOrder);
            sortOrder = sortOrder == "Brand" ? (desc ? "Brand_desc" : "Brand") : (sortOrder);
            sortOrder = sortOrder == "Status" ? (desc ? "Status_desc" : "Status") : (sortOrder);
            sortOrder = sortOrder == "Road_id" ? (desc ? "Road_id_desc" : "Road_id") : (sortOrder);
            sortOrder = sortOrder == "Max_weight" ? (desc ? "Max_weight_desc" : "Max_weight") : (sortOrder);
            sortOrder = sortOrder == "Date" || String.IsNullOrEmpty(sortOrder) ? (desc ? "Date_desc" : "") : (sortOrder);

            switch (sortOrder)
            {
                case "Vehicle_registration_number_desc":
                    data = data.OrderByDescending(s => s.Vehicle_registration_number).ToList();
                    break;
                case "Vehicle_registration_number":
                    data = data.OrderBy(s => s.Vehicle_registration_number).ToList();
                    break;
                case "Brand_desc":
                    data = data.OrderByDescending(s => s.Brand).ToList();
                    break;
                case "Brand":
                    data = data.OrderBy(s => s.Brand).ToList();
                    break;
                case "Status_desc":
                    data = data.OrderByDescending(s => s.Status).ToList();
                    break;
                case "Status":
                    data = data.OrderBy(s => s.Status).ToList();
                    break;
                case "Road_id_desc":
                    data = data.OrderByDescending(s => s.Road_id).ToList();
                    break;
                case "Road_id":
                    data = data.OrderBy(s => s.Road_id).ToList();
                    break;
                case "Max_weight_desc":
                    data = data.OrderByDescending(s => s.Max_weight).ToList();
                    break;
                case "Max_weight":
                    data = data.OrderBy(s => s.Max_weight).ToList();
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
        public async Task<IActionResult> GetTrucks()
        {
            var data = await _context.Trucks.ToListAsync();
            return Ok(data);
        }

        [HttpGet]
        public async Task<IActionResult> PageCount()
        {
            var data = await _context.Trucks.ToListAsync();
            int PageCount = data.Count();
            return Ok(PageCount);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var data = await _context.Trucks.FirstOrDefaultAsync(a => a.Id == id);
            return Ok(data);
        }

        [HttpPost]
        public async Task<IActionResult> Post(Trucks data)
        {
            data.User_id = _context.Users.FirstOrDefault(a => a.UserName == User.Identity.Name).Id;
            _context.Add(data);
            await _context.SaveChangesAsync();
            return Ok(data.Id);
        }

        [HttpPut]
        public async Task<IActionResult> Put(Trucks data)
        {
            _context.Entry(data).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var data = new Trucks { Id = id };
            _context.Remove(data);
            await _context.SaveChangesAsync();
            return NoContent();
        }


        //closedXML needed !!!
        [HttpGet]
        public async Task<string> Excel(string lang)
        {
            var trucks = from data in _context.Trucks select data;
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Trucks");
                var currentRow = 1;
                worksheet.Cell(currentRow, 1).Value = "Id";
                worksheet.Cell(currentRow, 1).Style.Font.SetBold();
                worksheet.Cell(currentRow, 2).Value = lang == "hu" ? Cargotruck.Shared.Resources.Resource.User_id : "User ID";
                worksheet.Cell(currentRow, 2).Style.Font.SetBold();
                worksheet.Cell(currentRow, 3).Value = lang == "hu" ? Cargotruck.Shared.Resources.Resource.Vehicle_registration_number : "Task ID";
                worksheet.Cell(currentRow, 3).Style.Font.SetBold();
                worksheet.Cell(currentRow, 4).Value = lang == "hu" ? Cargotruck.Shared.Resources.Resource.Brand : "Cargo ID";
                worksheet.Cell(currentRow, 4).Style.Font.SetBold();
                worksheet.Cell(currentRow, 5).Value = lang == "hu" ? Cargotruck.Shared.Resources.Resource.Status : "Status";
                worksheet.Cell(currentRow, 5).Style.Font.SetBold();
                worksheet.Cell(currentRow, 6).Value = lang == "hu" ? Cargotruck.Shared.Resources.Resource.Road_id : "Road ID";
                worksheet.Cell(currentRow, 6).Style.Font.SetBold();
                worksheet.Cell(currentRow, 7).Value = lang == "hu" ? Cargotruck.Shared.Resources.Resource.Max_weight : "Max weight";
                worksheet.Cell(currentRow, 7).Style.Font.SetBold();
                worksheet.Cell(currentRow, 8).Value = lang == "hu" ? Cargotruck.Shared.Resources.Resource.Date : "Date";
                worksheet.Cell(currentRow, 8).Style.Font.SetBold();

                foreach (var truck in trucks)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = truck.Id;
                    worksheet.Cell(currentRow, 2).Value = truck.User_id;
                    worksheet.Cell(currentRow, 3).Value = truck.Vehicle_registration_number;
                    worksheet.Cell(currentRow, 4).Value = truck.Brand;
                    worksheet.Cell(currentRow, 5).Value = truck.Status;
                    worksheet.Cell(currentRow, 6).Value = truck.Road_id;
                    worksheet.Cell(currentRow, 7).Value = truck.Max_weight;
                    worksheet.Cell(currentRow, 8).Value = truck.Date;
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
            var trucks = from data in _context.Trucks select data;

            int pdfRowIndex = 1;
            Random rnd = new Random();
            int random = rnd.Next(1000000, 9999999);
            string filename = "Trucks" + random + "_" + DateTime.Now.ToString("dd-MM-yyyy");
            string filepath = "Files/" + filename + ".pdf";

            Document document = new Document(PageSize.A4, 5f, 5f, 10f, 10f);
            FileStream fs = new FileStream(filepath, FileMode.Create);
            PdfWriter writer = PdfWriter.GetInstance(document, fs);
            document.Open();

            Font font1 = FontFactory.GetFont(FontFactory.TIMES_BOLD, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 12);
            Font font2 = FontFactory.GetFont(FontFactory.TIMES_ROMAN, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 10);

            System.Type type = typeof(Trucks);
            var column_number = (type.GetProperties().Length)-1;
            var columnDefinitionSize = new float[column_number];
            for (int i = 0; i < column_number; i++) columnDefinitionSize[i] = 1F;

            PdfPTable table;
            PdfPCell cell;

            table = new PdfPTable(columnDefinitionSize)
            {
                WidthPercentage = 90
            };


            cell = new PdfPCell
            {
                BackgroundColor = new BaseColor(0xC0, 0xC0, 0xC0),
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_CENTER
            };

            var title = new Paragraph(15, lang == "hu" ? Cargotruck.Shared.Resources.Resource.Trucks : "Trucks");
            title.Alignment = Element.ALIGN_CENTER;


            document.Add(title);
            document.Add(new Paragraph("\n"));

            if (trucks.Count() > 0)
            {
                table.AddCell(new PdfPCell(new Phrase("Id", font1))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                });
                table.AddCell(new PdfPCell(new Phrase(lang == "hu" ? Cargotruck.Shared.Resources.Resource.Vehicle_registration_number : "Vehicle registration number", font1))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                });
                table.AddCell(new PdfPCell(new Phrase(lang == "hu" ? Cargotruck.Shared.Resources.Resource.Brand : "Brand", font1))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                });
                table.AddCell(new PdfPCell(new Phrase(lang == "hu" ? Cargotruck.Shared.Resources.Resource.Status : "Status", font1))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                });
                table.AddCell(new PdfPCell(new Phrase(lang == "hu" ? Cargotruck.Shared.Resources.Resource.Road_id : "Road ID", font1))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                });
                table.AddCell(new PdfPCell(new Phrase(lang == "hu" ? Cargotruck.Shared.Resources.Resource.Max_weight : "Max weight", font1))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                });
                table.AddCell(new PdfPCell(new Phrase(lang == "hu" ? Cargotruck.Shared.Resources.Resource.Date : "Date", font1))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                });

                foreach (Trucks truck in trucks)
                {
                    var s = "";
                    if (!string.IsNullOrEmpty(truck.Id.ToString())) { s = truck.Id.ToString(); }
                    else { s = "-"; }
                    table.AddCell(new PdfPCell(new Phrase(s.ToString(), font2))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    });
                    if (!string.IsNullOrEmpty(truck.Vehicle_registration_number.ToString())) { s = truck.Vehicle_registration_number.ToString(); }
                    else { s = "-"; }
                    table.AddCell(new PdfPCell(new Phrase(s.ToString(), font2))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    });
                    if (!string.IsNullOrEmpty(truck.Brand.ToString())) { s = truck.Brand.ToString(); }
                    else { s = "-"; }
                    table.AddCell(new PdfPCell(new Phrase(s.ToString(), font2))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    });
                    if (!string.IsNullOrEmpty(truck.Status.ToString())) { s = truck.Status.ToString(); }
                    else { s = "-"; }
                    table.AddCell(new PdfPCell(new Phrase(s.ToString(), font2))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    });
                    if (!string.IsNullOrEmpty(truck.Road_id.ToString())) { s = truck.Road_id.ToString(); }
                    else { s = "-"; }
                    table.AddCell(new PdfPCell(new Phrase(s.ToString(), font2))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    });
                    if (!string.IsNullOrEmpty(truck.Max_weight.ToString())) { s = truck.Max_weight.ToString(); }
                    else { s = "-"; }
                    table.AddCell(new PdfPCell(new Phrase(s.ToString(), font2))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    });
                    if (!string.IsNullOrEmpty(truck.Date.ToString())) { s = truck.Date.ToString(); }
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
            var trucks = from data in _context.Trucks select data;

            Random rnd = new Random();
            int random = rnd.Next(1000000, 9999999);
            string filename = "Trucks" + random + "_" + DateTime.Now.ToString("dd-MM-yyyy");
            string filepath = "Files/" + filename + ".csv";

            StreamWriter txt = new StreamWriter(filepath);
            txt.Write("Id" + ";");
            txt.Write((lang == "hu" ? Cargotruck.Shared.Resources.Resource.User_id : "User ID") + ";");
            txt.Write((lang == "hu" ? Cargotruck.Shared.Resources.Resource.Vehicle_registration_number : "Vehicle registration number") + ";");
            txt.Write((lang == "hu" ? Cargotruck.Shared.Resources.Resource.Brand : "Brand") + ";");
            txt.Write((lang == "hu" ? Cargotruck.Shared.Resources.Resource.Status : "Status") + ";");
            txt.Write((lang == "hu" ? Cargotruck.Shared.Resources.Resource.Road_id : "Road ID") + ";");
            txt.Write((lang == "hu" ? Cargotruck.Shared.Resources.Resource.Max_weight : "Max weight") + ";");
            txt.Write((lang == "hu" ? Cargotruck.Shared.Resources.Resource.Date : "Date") + ";");
            txt.Write("\n");

            foreach (var truck in trucks)
            {
                txt.Write(truck.Id + ";");
                txt.Write(truck.User_id + ";");
                txt.Write(truck.Vehicle_registration_number + ";");
                txt.Write(truck.Brand + ";");
                txt.Write(truck.Status + ";");
                txt.Write(truck.Road_id + ";");
                txt.Write(truck.Max_weight + ";");
                txt.Write(truck.Date + ";");
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
                                lang == "hu" ? Cargotruck.Shared.Resources.Resource.Vehicle_registration_number : "Vehicle registration number",
                                lang == "hu" ? Cargotruck.Shared.Resources.Resource.Brand : "Brand",
                                lang == "hu" ? Cargotruck.Shared.Resources.Resource.Status : "Status",
                                lang == "hu" ? Cargotruck.Shared.Resources.Resource.Road_id : "Road ID",
                                lang == "hu" ? Cargotruck.Shared.Resources.Resource.Max_weight : "Max weight",
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

                                switch (list[l + 3])
                                {
                                    case "delivering":
                                        list[l + 3] = 0;
                                        break;
                                    case "on_road":
                                        list[l + 3] = 1;
                                        break;
                                    case "garage":
                                        list[l + 3] = 2;
                                        break;
                                    case "under_repair":
                                        list[l + 3] = 3;
                                        break;
                                    case "loaned":
                                        list[l + 3] = 4;
                                        break;
                                   case "rented":
                                        list[l + 3] = 5;
                                        break;
                                    default:
                                        list[l + 3] = System.DBNull.Value;
                                        break;
                                }

                                var sql = @"Insert Into Trucks (User_id,Vehicle_registration_number,Brand,Status,Road_id,Max_weight,Date) 
                                Values (@User_id,@Vehicle_registration_number,@Brand,@Status,@Road_id,@Max_weight,@Date)";
                                var insert = await _context.Database.ExecuteSqlRawAsync(sql,
                                    new SqlParameter("@User_id", list[l]),
                                    new SqlParameter("@Vehicle_registration_number", list[l + 1]),
                                    new SqlParameter("@Brand", list[l + 2]),
                                    new SqlParameter("@Status", list[l + 3]),
                                    new SqlParameter("@Road_id", list[l + 4]),
                                    new SqlParameter("@Max_weight", list[l + 5]),
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