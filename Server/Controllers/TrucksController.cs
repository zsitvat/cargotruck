﻿using Cargotruck.Server.Data;
using Cargotruck.Shared.Models;
using Cargotruck.Shared.Resources;
using ClosedXML.Excel;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System.Data;
using System.Globalization;
using System.Text;
using Document = iTextSharp.text.Document;
using Font = iTextSharp.text.Font;

namespace Cargotruck.Server.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TrucksController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IStringLocalizer<Resource> _localizer;

        public TrucksController(ApplicationDbContext context, IStringLocalizer<Resource> localizer)
        {
            _context = context;
            _localizer = localizer;
        }

        private async Task<List<Trucks>> GetData(string? searchString, Status? filter, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate)
        {
            var data = await _context.Trucks.Where(s => (dateFilterStartDate != null ? (s.Date >= dateFilterStartDate) : true) && (dateFilterEndDate != null ? (s.Date <= dateFilterEndDate) : true)).ToListAsync();

            if (filter != null)
            {
                data = data.Where(data => data.Status == filter).ToList();
            }

            searchString = searchString?.ToLower();
            if (searchString != null && searchString != "")
            {
                data = data.Where(s =>
               (s.Vehicle_registration_number!.ToString().ToLower().Contains(searchString))
            || (s.Brand != null && s.Brand.ToString().ToLower()!.Contains(searchString))
            || (s.Status.ToString()!.Contains(searchString))
            || (s.Road_id != null && s.Road_id.ToString()!.ToLower().Contains(searchString))
            || (s.Max_weight != null && s.Max_weight.ToString()!.Contains(searchString))
            ).ToList();
            }

            return data;
        }

        [HttpGet]
        public async Task<IActionResult> Get(int page, int pageSize, string sortOrder, bool desc, string? searchString, Status? filter, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate)
        {
            var data = await GetData(searchString, filter, dateFilterStartDate, dateFilterEndDate);

            sortOrder = sortOrder == "Vehicle_registration_number" ? (desc ? "Vehicle_registration_number_desc" : "Vehicle_registration_number") : (sortOrder);
            sortOrder = sortOrder == "Brand" ? (desc ? "Brand_desc" : "Brand") : (sortOrder);
            sortOrder = sortOrder == "Status" ? (desc ? "Status_desc" : "Status") : (sortOrder);
            sortOrder = sortOrder == "Road_id" ? (desc ? "Road_id_desc" : "Road_id") : (sortOrder);
            sortOrder = sortOrder == "Max_weight" ? (desc ? "Max_weight_desc" : "Max_weight") : (sortOrder);
            sortOrder = sortOrder == "Date" || String.IsNullOrEmpty(sortOrder) ? (desc ? "Date_desc" : "") : (sortOrder);

            data = sortOrder switch
            {
                "Vehicle_registration_number_desc" => data.OrderByDescending(s => s.Vehicle_registration_number).ToList(),
                "Vehicle_registration_number" => data.OrderBy(s => s.Vehicle_registration_number).ToList(),
                "Brand_desc" => data.OrderByDescending(s => s.Brand).ToList(),
                "Brand" => data.OrderBy(s => s.Brand).ToList(),
                "Status_desc" => data.OrderByDescending(s => s.Status).ToList(),
                "Status" => data.OrderBy(s => s.Status).ToList(),
                "Road_id_desc" => data.OrderByDescending(s => s.Road_id).ToList(),
                "Road_id" => data.OrderBy(s => s.Road_id).ToList(),
                "Max_weight_desc" => data.OrderByDescending(s => s.Max_weight).ToList(),
                "Max_weight" => data.OrderBy(s => s.Max_weight).ToList(),
                "Date_desc" => data.OrderByDescending(s => s.Date).ToList(),
                _ => data.OrderBy(s => s.Date).ToList(),
            };
            data = data.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            return Ok(data);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var data = await _context.Trucks.FirstOrDefaultAsync(a => a.Id == id);
            return Ok(data);
        }

        [HttpGet]
        public async Task<IActionResult> GetTrucks()
        {
            var data = await _context.Trucks.ToListAsync();
            return Ok(data);
        }

        [HttpGet]
        public async Task<IActionResult> Count(bool all)
        {
            if (all)
            {
                return Ok(await _context.Trucks.CountAsync());
            }
            else
            {
                return Ok(await _context.Trucks.Where(x => x.Status != Status.garage).CountAsync());
            }
        }

        [HttpGet]
        public async Task<IActionResult> PageCount(string? searchString, Status? filter, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate)
        {
            var data = await GetData(searchString, filter, dateFilterStartDate, dateFilterEndDate);
            int PageCount = data.Count;
            return Ok(PageCount);
        }

        [HttpGet("{vehicle_registration_number}")]
        public async Task<IActionResult> GetByVRN(string vehicle_registration_number)
        {
            var data = await _context.Trucks.FirstOrDefaultAsync(a => a.Vehicle_registration_number == vehicle_registration_number);
            return Ok(data);
        }

        [HttpPost]
        public async Task<IActionResult> Post(Trucks data)
        {
            data.User_id = _context.Users.FirstOrDefault(a => a.UserName == User.Identity!.Name)?.Id;
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
        public string Excel(string lang, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate)
        {
            var trucks = _context.Trucks.Where(s => (dateFilterStartDate != null ? (s.Date >= dateFilterStartDate) : true) && (dateFilterEndDate != null ? (s.Date <= dateFilterEndDate) : true));

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Trucks");
            var currentRow = 1;

            List<string> columnNames = new() {
                "Id",
                lang == "hu" ? Cargotruck.Shared.Resources.Resource.User_id : "User ID",
                lang == "hu" ? Cargotruck.Shared.Resources.Resource.Vehicle_registration_number : "Task ID",
                lang == "hu" ? Cargotruck.Shared.Resources.Resource.Brand : "Cargo ID",
                lang == "hu" ? Cargotruck.Shared.Resources.Resource.Status : "Status",
                lang == "hu" ? Cargotruck.Shared.Resources.Resource.Road_id : "Road ID",
                lang == "hu" ? Cargotruck.Shared.Resources.Resource.Max_weight : "Max weight",
                lang == "hu" ? Cargotruck.Shared.Resources.Resource.Date : "Date"
            };

            for (var i = 0; i < columnNames.Count; i++)
            {
                worksheet.Cell(currentRow, i + 1).Value = columnNames[i];
                worksheet.Cell(currentRow, i + 1).Style.Font.SetBold();
            }

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

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            var content = stream.ToArray();
            return Convert.ToBase64String(content);
        }

        //iTextSharp needed !!!
        [HttpGet]
        public async Task<string> PDF(string lang, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate)
        {
            var trucks = _context.Trucks.Where(s => (dateFilterStartDate != null ? (s.Date >= dateFilterStartDate) : true) && (dateFilterEndDate != null ? (s.Date <= dateFilterEndDate) : true));

            int pdfRowIndex = 1;
            Random rnd = new();
            int random = rnd.Next(1000000, 9999999);
            string filename = "Trucks" + random + "_" + DateTime.Now.ToString("dd-MM-yyyy");
            string filepath = "Files/" + filename + ".pdf";

            Document document = new(PageSize.A4, 5f, 5f, 10f, 10f);
            FileStream fs = new(filepath, FileMode.Create);
            PdfWriter writer = PdfWriter.GetInstance(document, fs);
            document.Open();

            Font font1 = FontFactory.GetFont(FontFactory.TIMES_BOLD, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 12);
            Font font2 = FontFactory.GetFont(FontFactory.TIMES_ROMAN, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 10);

            System.Type type = typeof(Trucks);
            var column_number = (type.GetProperties().Length) - 1;
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

            var title = new Paragraph(15, lang == "hu" ? Cargotruck.Shared.Resources.Resource.Trucks : "Trucks")
            {
                Alignment = Element.ALIGN_CENTER
            };


            document.Add(title);
            document.Add(new Paragraph("\n"));

            if (trucks.Any())
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

                    if (!string.IsNullOrEmpty(truck.Vehicle_registration_number?.ToString())) { s = truck.Vehicle_registration_number.ToString(); }

                    else { s = "-"; }
                    table.AddCell(new PdfPCell(new Phrase(s.ToString(), font2))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    });

                    if (!string.IsNullOrEmpty(truck.Brand?.ToString())) { s = truck.Brand.ToString(); }

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
                    if (!string.IsNullOrEmpty(truck.Road_id?.ToString())) { s = truck.Road_id.ToString(); }
                    else { s = "-"; }
                    table.AddCell(new PdfPCell(new Phrase(s?.ToString(), font2))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    });

                    if (!string.IsNullOrEmpty(truck.Max_weight?.ToString())) { s = truck.Max_weight.ToString(); }

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
            var trucks = _context.Trucks.Where(s => (dateFilterStartDate != null ? (s.Date >= dateFilterStartDate) : true) && (dateFilterEndDate != null ? (s.Date <= dateFilterEndDate) : true));

            Random rnd = new();
            int random = rnd.Next(1000000, 9999999);
            string filename = "Trucks" + random + "_" + DateTime.Now.ToString("dd-MM-yyyy");
            string filepath = "Files/" + filename + ".csv";

            StreamWriter txt = new(filepath);
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
        public async Task<IActionResult> Import([FromBody] string file, CultureInfo lang)
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

                    //Started reading the Excel file.  
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
                                List<string?> titles = new() {
                                "Id",
                                 _localizer["User_id"].Value,
                                 _localizer["Vehicle_registration_number"].Value,
                                 _localizer["Brand"].Value,
                                 _localizer["Status"].Value,
                                 _localizer["Road_id"].Value,
                                 _localizer["Max_weight"].Value,
                                 _localizer["Date"].Value
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
                                        error = _localizer["Not_match_col"].Value;
                                        System.IO.File.Delete(path); // delete the file
                                        return BadRequest(error);
                                    }

                                }
                                firstRow = false;
                                if (titles.Count == 0)
                                {
                                    haveColumns = true;
                                    l += 1;
                                }
                                else if (titles.Count == 1 && titles.Contains("Id"))
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

                                list[l + 3] = list[l + 3] switch
                                {
                                    "delivering" => 0,
                                    "on_road" => 1,
                                    "garage" => 2,
                                    "under_repair" => 3,
                                    "loaned" => 4,
                                    "rented" => 5,
                                    _ => System.DBNull.Value,
                                };

                                if (nulls != list.Count)
                                {
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
                                        var lastId = await _context.Trucks.OrderBy(x => x.Id).LastOrDefaultAsync();

                                        if (lastId != null)
                                        {
                                            var WithNewIds = await _context.Trucks.Where(x => x.Road_id == lastId.Road_id).ToListAsync();
                                            Roads? road = await _context.Roads.FirstOrDefaultAsync(x => x.Vehicle_registration_number == lastId.Vehicle_registration_number);

                                            foreach (var item in WithNewIds)
                                            {
                                                if (item != null)
                                                {
                                                    if (item.Id == lastId?.Id)
                                                    {

                                                        if (road == null)
                                                        {
                                                            item.Road_id = null;
                                                        }

                                                        _context.Entry(item).State = EntityState.Modified;
                                                        await _context.SaveChangesAsync();
                                                    }
                                                }
                                            }

                                            if (road != null)
                                            {
                                                road.Vehicle_registration_number = lastId?.Vehicle_registration_number;
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
                            else
                            {
                                error = _localizer["Not_match_col_count"];
                                return BadRequest(error);
                            }
                            //If no data in Excel file  
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