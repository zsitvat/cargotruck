using Cargotruck.Server.Data;
using Cargotruck.Server.Services;
using Cargotruck.Shared.Model;
using Cargotruck.Shared.Resources;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Drawing.Diagrams;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System.Data;
using System.Globalization;
using System.Linq.Dynamic.Core;
using System.Text;
using Document = iTextSharp.text.Document;
using Font = iTextSharp.text.Font;

namespace Cargotruck.Server.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class WarehousesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IStringLocalizer<Resource> _localizer;
        private readonly IColumnNamesService _columnNameLists;

        public WarehousesController(ApplicationDbContext context, IStringLocalizer<Resource> localizer, IColumnNamesService columnNameLists)
        {
            _context = context;
            _localizer = localizer;
            _columnNameLists = columnNameLists;
        }

        private async Task<List<Warehouses>> GetDataAsync(string? searchString, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate)
        {
            var data = await _context.Warehouses.Where(s => (dateFilterStartDate != null ? (s.Date >= dateFilterStartDate) : true) && (dateFilterEndDate != null ? (s.Date <= dateFilterEndDate) : true)).ToListAsync();

            searchString = searchString?.ToLower();
            if (searchString != null && searchString != "")
            {
                data = data.Where(s =>
               (s.Address!.ToString().ToLower()!.Contains(searchString))
            || (s.Owner != null && s.Owner.ToString().ToLower()!.Contains(searchString))
            ).ToList();
            }

            return data;
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync(int page, int pageSize, string sortOrder, bool desc, string? searchString, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate)
        {
            var data = await GetDataAsync(searchString, dateFilterStartDate, dateFilterEndDate);

            sortOrder = sortOrder == "Address" ? (desc ? "Address_desc" : "Address") : (sortOrder);
            sortOrder = sortOrder == "Owner" ? (desc ? "Owner_desc" : "Owner") : (sortOrder);
            sortOrder = sortOrder == "Date" || String.IsNullOrEmpty(sortOrder) ? (desc ? "Date_desc" : "") : (sortOrder);

            data = sortOrder switch
            {
                "Address_desc" => data.OrderByDescending(s => s.Address).ToList(),
                "Address" => data.OrderBy(s => s.Address).ToList(),
                "Owner_desc" => data.OrderByDescending(s => s.Owner).ToList(),
                "Owner" => data.OrderBy(s => s.Owner).ToList(),
                "Date_desc" => data.OrderByDescending(s => s.Date).ToList(),
                _ => data.OrderBy(s => s.Date).ToList(),
            };
            data = data.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            return Ok(data);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var data = await _context.Warehouses.FirstOrDefaultAsync(a => a.Id == id);
            return Ok(data);
        }

        [HttpGet]
        public async Task<IActionResult> GetWarehousesAsync()
        {
            var data = await _context.Warehouses.ToListAsync();
            return Ok(data);
        }

        [HttpGet]
        public async Task<IActionResult> CountAsync()
        {
            var t = await _context.Warehouses.CountAsync();
            return Ok(t);
        }

        [HttpGet]
        public async Task<IActionResult> PageCountAsync(string? searchString, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate)
        {
            var data = await GetDataAsync(searchString, dateFilterStartDate, dateFilterEndDate);
            int PageCount = data.Count;
            return Ok(PageCount);
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync(Warehouses data)
        {
            data.User_id = _context.Users.FirstOrDefault(a => a.UserName == User.Identity!.Name)?.Id;
            _context.Add(data);
            await _context.SaveChangesAsync();
            return Ok(data.Id);
        }

        [HttpPut]
        public async Task<IActionResult> PutAsync(Warehouses data)
        {
            _context.Entry(data).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var data = new Warehouses { Id = id };
            _context.Remove(data);
            await _context.SaveChangesAsync();
            return NoContent();
        }


        //closedXML needed !!!
        [HttpGet]
        public string ExportToExcel(CultureInfo lang, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate)
        {
            var warehouses = _context.Warehouses.Where(s => (dateFilterStartDate != null ? (s.Date >= dateFilterStartDate) : true) && (dateFilterEndDate != null ? (s.Date <= dateFilterEndDate) : true));
            var cargoes = _context.Cargoes.Where(s => (dateFilterStartDate != null ? (s.Date >= dateFilterStartDate) : true) && (dateFilterEndDate != null ? (s.Date <= dateFilterEndDate) : true));

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Warehouses");
            var currentRow = 1;


            CultureInfo.CurrentUICulture = lang;
            List<string> columnNames = _columnNameLists.GetWarehousesColumnNames().Select(x => _localizer[x].Value).ToList();

            for (var i = 0; i < columnNames.Count; i++)
            {
                worksheet.Cell(currentRow, i + 1).Value = columnNames[i];
                worksheet.Cell(currentRow, i + 1).Style.Font.SetBold();
            }

            foreach (var warehouse in warehouses)
            {
                var cellValue = "-";
                currentRow++;

                worksheet.Cell(currentRow, 1).Value = warehouse.Id;
                worksheet.Cell(currentRow, 2).Value = warehouse.Address;
                worksheet.Cell(currentRow, 3).Value = warehouse.Owner;

                foreach (Cargoes cargo in cargoes)
                {
                    if (cargo.Warehouse_id == warehouse.Id)
                    {
                        cellValue = "[" + cargo.Id + (cargo.Warehouse_section!=null ? "/" : "") + cargo.Warehouse_section + "]";
                    }
                }

                worksheet.Cell(currentRow, 4).Value = cellValue;
                worksheet.Cell(currentRow, 5).Value = warehouse.Date;
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            var content = stream.ToArray();

            return Convert.ToBase64String(content);
        }

        //iTextSharp needed !!!
        [HttpGet]
        public async Task<string> ExportToPdfAsync(CultureInfo lang, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate)
        {
            var warehouses = _context.Warehouses.Where(s => (dateFilterStartDate != null ? (s.Date >= dateFilterStartDate) : true) && (dateFilterEndDate != null ? (s.Date <= dateFilterEndDate) : true));
            var cargoes = _context.Cargoes.Where(s => (dateFilterStartDate != null ? (s.Date >= dateFilterStartDate) : true) && (dateFilterEndDate != null ? (s.Date <= dateFilterEndDate) : true));

            int pdfRowIndex = 1;
            Random rnd = new();
            int random = rnd.Next(1000000, 9999999);
            string filename = "Warehouses" + random + "_" + DateTime.Now.ToString("dd-MM-yyyy");
            string filepath = "Files/" + filename + ".pdf";

            Document document = new(PageSize.A4, 5f, 5f, 10f, 10f);
            FileStream fs = new(filepath, FileMode.Create);
            PdfWriter writer = PdfWriter.GetInstance(document, fs);
            document.Open();

            Font font1 = FontFactory.GetFont(FontFactory.TIMES_BOLD, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 12);
            Font font2 = FontFactory.GetFont(FontFactory.TIMES_ROMAN, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 10);

            System.Type type = typeof(Warehouses);
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


            //copy column names to a list based on language
            CultureInfo.CurrentUICulture = lang;
            List<string> columnNames = _columnNameLists.GetWarehousesColumnNames().Select(x => _localizer[x].Value).ToList();

            var title = new Paragraph(15, _localizer["Warehouses"].Value)
            {
                Alignment = Element.ALIGN_CENTER
            };

            document.Add(title);
            document.Add(new Paragraph("\n"));

            if (warehouses.Any())
            {
                foreach (var name in columnNames.Take(column_number))
                {
                    table.AddCell(new PdfPCell(new Phrase(name, font1))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    });
                }

                foreach (Warehouses warehouse in warehouses)
                {
                    var s = "";
                    if (!string.IsNullOrEmpty(warehouse.Id.ToString())) { s = warehouse.Id.ToString(); }
                    else { s = "-"; }
                    table.AddCell(new PdfPCell(new Phrase(s.ToString(), font2))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    });
                    if (!string.IsNullOrEmpty(warehouse.Address?.ToString())) { s = warehouse.Address.ToString(); }
                    else { s = "-"; }
                    table.AddCell(new PdfPCell(new Phrase(s.ToString(), font2))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    });
                    if (!string.IsNullOrEmpty(warehouse.Owner?.ToString())) { s = warehouse.Owner.ToString(); }
                    else { s = "-"; }
                    table.AddCell(new PdfPCell(new Phrase(s.ToString(), font2))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    });
                    s = "";
                    if (cargoes != null)
                    {
                        foreach (Cargoes cargo in cargoes)
                        {
                            if (cargo.Warehouse_id == warehouse.Id)
                            {
                                s = (s + "[" + cargo.Id + (cargo.Warehouse_section != null ? "/" : "") + cargo.Warehouse_section + "]");
                            }
                        }
                    }
                    else { s = "-"; }
                    table.AddCell(new PdfPCell(new Phrase(s.ToString(), font2))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    });
                    if (!string.IsNullOrEmpty(warehouse.Date.ToString())) { s = warehouse.Date.ToString(); }
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

        //iTextSharp needed !!!
        [HttpGet]
        public async Task<string> ExportToCSVAsync(CultureInfo lang, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate, bool isTextDocument)
        {
            var warehouses = _context.Warehouses.Where(s => (dateFilterStartDate != null ? (s.Date >= dateFilterStartDate) : true) && (dateFilterEndDate != null ? (s.Date <= dateFilterEndDate) : true));
            var cargoes = _context.Cargoes.Where(s => (dateFilterStartDate != null ? (s.Date >= dateFilterStartDate) : true) && (dateFilterEndDate != null ? (s.Date <= dateFilterEndDate) : true));

            Random rnd = new();
            int random = rnd.Next(1000000, 9999999);
            string filename = "Warehouses" + random + "_" + DateTime.Now.ToString("dd-MM-yyyy");
            string filepath = "Files/" + filename + ".csv";

            StreamWriter txt = new(filepath);

            //copy column names to a list based on language
            CultureInfo.CurrentUICulture = lang;
            List<string> columnNames = _columnNameLists.GetWarehousesColumnNames().Select(x => _localizer[x].Value).ToList();

            string separator = isTextDocument ? "\t" : ";";
            string ifNull = isTextDocument ? " --- " : "";

            foreach (var name in columnNames)
            {
                txt.Write(name + separator);
            }

            txt.Write("\n");

            foreach (var warehouse in warehouses)
            {
                txt.Write(warehouse.Id + separator);
                txt.Write((warehouse.Address ?? ifNull) + separator);
                txt.Write((warehouse.Owner ?? ifNull) + separator);
                if (cargoes.Any()) { 
                    foreach (Cargoes cargo in cargoes)
                    {
                        if (cargo.Warehouse_id == warehouse.Id)
                        {
                            txt.Write("[" + cargo.Id + (cargo.Warehouse_section != null ? "/" : "") + cargo.Warehouse_section + "]");
                        }
                    }
                }
                else {
                    txt.Write(ifNull);
                }
                txt.Write(separator);
                txt.Write(warehouse.Date + separator);
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
        public async Task<IActionResult> ImportAsync([FromBody] string file, CultureInfo lang)
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
                                List<string> columnNames = _columnNameLists.GetWarehousesColumnNames().Select(x => _localizer[x].Value).ToList();

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
                                        return BadRequest(error);
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

                                if (nulls != list.Count)
                                {
                                    var sql = @"Insert Into Warehouses (User_id,Address,Owner,Date) 
                                    Values (@User_id,@Address,@Owner,@Date)";
                                    var insert = await _context.Database.ExecuteSqlRawAsync(sql,
                                        new SqlParameter("@User_id", "Imported"),
                                        new SqlParameter("@Address", list[l]),
                                        new SqlParameter("@Owner", list[l + 1]),
                                        new SqlParameter("@Date", DateTime.Now)
                                        );

                                    string[]? substrings = list[l + 2]?.ToString()?.Split("]");

                                    if (substrings != null)
                                    {
                                        for (int s = 0; s < (substrings.Length > 0 ? substrings.Length - 1 : 0); ++s)
                                        {
                                            var CargoId = substrings[s][1..substrings[s].IndexOf("/")];
                                            var warehouseSection = substrings[s].Substring(substrings[s].IndexOf("/") + 1);

                                            var greatestId = _context.Warehouses.OrderBy(s => s.Id).Last().Id;

                                            var sql2 = @"Update Cargoes 
                                                        Set Warehouse_id = @Warehouse_id, Warehouse_section = @Warehouse_section
                                                         Where Id = @Id";
                                            var insert2 = await _context.Database.ExecuteSqlRawAsync(sql2,
                                                new SqlParameter("@Warehouse_id", greatestId),
                                                new SqlParameter("@Warehouse_section", warehouseSection),
                                                new SqlParameter("@Id", CargoId)
                                                );
                                        }
                                    }

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
                                error = _localizer["Not_match_col_count"];
                                return BadRequest(error);
                            }
                            //If no data in ExportToExcel file  
                            if (firstRow)
                            {
                                error = _localizer["Empty_excel"];
                                System.IO.File.Delete(path); // delete the file
                                return BadRequest(error);
                            }
                        }
                    }
                    else
                    {
                        error = _localizer["Missing_data_rows"];
                        System.IO.File.Delete(path); // delete the file
                        return BadRequest(error);
                    }
                }
                else
                {
                    //If file extension of the uploaded file is different then .xlsx  
                    error = _localizer["Not_excel"];
                    System.IO.File.Delete(path); // delete the file
                    return BadRequest(error);
                }
            }
            else
            {
                error = _localizer["No_excel"];
                return BadRequest(error);
            }
            return NoContent();
        }
    }
}