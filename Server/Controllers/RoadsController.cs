using Cargotruck.Data;
using Cargotruck.Shared.Models;
using ClosedXML.Excel;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Text;
using Document = iTextSharp.text.Document;
using Font = iTextSharp.text.Font;

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
        public async Task<IActionResult> Get(int page, int pageSize, string sortOrder, bool desc, string? searchString, string? filter, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate)
        {
            var data = await GetData(searchString, filter, dateFilterStartDate, dateFilterEndDate);

            sortOrder = sortOrder == "Task_id" ? (desc ? "Task_id_desc" : "Task_id") : (sortOrder);
            sortOrder = sortOrder == "Vehicle_registration_number" ? (desc ? "Vehicle_registration_number_desc" : "Vehicle_registration_number") : (sortOrder);
            sortOrder = sortOrder == "Id_cargo" ? (desc ? "Id_cargo_desc" : "Id_cargo") : (sortOrder);
            sortOrder = sortOrder == "Purpose_of_the_trip" ? (desc ? "Purpose_of_the_trip_desc" : "Purpose_of_the_trip") : (sortOrder);
            sortOrder = sortOrder == "Starting_date" ? (desc ? "Starting_date_desc" : "Starting_date") : (sortOrder);
            sortOrder = sortOrder == "Ending_date" ? (desc ? "Ending_date_desc" : "Ending_date") : (sortOrder);
            sortOrder = sortOrder == "Starting_place" ? (desc ? "Starting_place_desc" : "Starting_place") : (sortOrder);
            sortOrder = sortOrder == "Ending_place" ? (desc ? "Ending_place_desc" : "Ending_place") : (sortOrder);
            sortOrder = sortOrder == "Direction" ? (desc ? "Direction_desc" : "Direction") : (sortOrder);
            sortOrder = sortOrder == "Fuel" ? (desc ? "Fuel_desc" : "Fuel_place") : (sortOrder);
            sortOrder = sortOrder == "Expenses_id" ? (desc ? "Expenses_id_desc" : "Expenses_id") : (sortOrder);
            sortOrder = sortOrder == "Date" || String.IsNullOrEmpty(sortOrder) ? (desc ? "Date_desc" : "") : (sortOrder);

            data = sortOrder switch
            {
                "Task_id_desc" => data.OrderByDescending(s => s.Task_id).ToList(),
                "Task_id" => data.OrderBy(s => s.Task_id).ToList(),
                "Vehicle_registration_number_desc" => data.OrderByDescending(s => s.Vehicle_registration_number).ToList(),
                "Vehicle_registration_number" => data.OrderBy(s => s.Vehicle_registration_number).ToList(),
                "Id_cargo_desc" => data.OrderByDescending(s => s.Id_cargo).ToList(),
                "Id_cargo" => data.OrderBy(s => s.Id_cargo).ToList(),
                "Purpose_of_the_trip_desc" => data.OrderByDescending(s => s.Purpose_of_the_trip).ToList(),
                "Purpose_of_the_trip" => data.OrderBy(s => s.Purpose_of_the_trip).ToList(),
                "Starting_date_desc" => data.OrderByDescending(s => s.Starting_date).ToList(),
                "Starting_date" => data.OrderBy(s => s.Starting_date).ToList(),
                "Ending_date_desc" => data.OrderByDescending(s => s.Ending_date).ToList(),
                "Ending_date" => data.OrderBy(s => s.Ending_date).ToList(),
                "Starting_place_desc" => data.OrderByDescending(s => s.Starting_place).ToList(),
                "Starting_place" => data.OrderBy(s => s.Starting_place).ToList(),
                "Ending_place_desc" => data.OrderByDescending(s => s.Ending_place).ToList(),
                "Ending_place" => data.OrderBy(s => s.Ending_place).ToList(),
                "Direction_desc" => data.OrderByDescending(s => s.Direction).ToList(),
                "Direction" => data.OrderBy(s => s.Direction).ToList(),
                "Fuel_desc" => data.OrderByDescending(s => s.Fuel).ToList(),
                "Fuel" => data.OrderBy(s => s.Fuel).ToList(),
                "Expenses_id_desc" => data.OrderByDescending(s => s.Expenses_id).ToList(),
                "Expenses_id" => data.OrderBy(s => s.Expenses_id).ToList(),
                "Date_desc" => data.OrderByDescending(s => s.Date).ToList(),
                _ => data.OrderBy(s => s.Date).ToList(),
            };
            data = data.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            return Ok(data);
        }

        [HttpGet]
        public async Task<List<Roads>> GetData(string? searchString, string? filter, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate)
        {
            var data = await _context.Roads.Where(s => (dateFilterStartDate != null ? (s.Date >= dateFilterStartDate) : true) && (dateFilterEndDate != null ? (s.Date <= dateFilterEndDate) : true)).ToListAsync();

            if (filter == "to")
            {
                data = data.Where(data => data.Direction == "TO").ToList();
            }
            else if (filter == "from")
            {
                data = data.Where(data => data.Direction == "FROM").ToList();
            }

            searchString = searchString?.ToLower();
            if (searchString != null && searchString != "")
            {
                data = data.Where(s =>
                    (s.Task_id != null && s.Task_id.ToString()!.ToLower().Contains(searchString))
                || (s.Id_cargo != null && s.Id_cargo.ToString()!.ToLower().Contains(searchString))
                || (s.Vehicle_registration_number != null && s.Vehicle_registration_number.ToString()!.ToLower().Contains(searchString))
                || (s.Purpose_of_the_trip != null && s.Purpose_of_the_trip.ToLower()!.Contains(searchString))
                || s.Starting_date.ToString()!.ToLower().Contains(searchString)
                || s.Ending_date.ToString()!.Contains(searchString)
                || s.Fuel!.ToString().Contains(searchString)
                || (s.Starting_place != null && s.Starting_place.ToLower()!.Contains(searchString))
                || (s.Ending_place != null && s.Ending_place.ToLower()!.Contains(searchString))
                || (s.Direction != null && s.Direction.ToString()!.Contains(searchString))
                || (s.Expenses_id != null && s.Expenses_id.ToString()!.Contains(searchString))
                ).ToList();
            }

            return data;
        }

        [HttpGet]
        public async Task<IActionResult> PageCount(string? searchString, string? filter, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate)
        {
            var data = await GetData(searchString, filter, dateFilterStartDate, dateFilterEndDate);
            int PageCount = data.Count;
            return Ok(PageCount);
        }

        [HttpGet]
        public async Task<IActionResult> GetChartData()
        {
            var data = await _context.Roads.ToListAsync();
            var trucksCount = data.Where(x=>x.Vehicle_registration_number != null && x.Vehicle_registration_number != "").DistinctBy(x=>x.Vehicle_registration_number).Count();
            var trucksVRN = data?.DistinctBy(x => x.Vehicle_registration_number).ToList();
            int[] columnsHeight = new int[12 * trucksCount];
            int h = 1;
            for (int i = 0; i < columnsHeight.Length; i++)
            {
                h++;
                if (h==13) h = 1;
                columnsHeight[i] = data.Where(x => x.Date.Year == DateTime.Now.Year && x.Date.Month == h && x.Vehicle_registration_number != null && x.Vehicle_registration_number == trucksVRN?[i/12].Vehicle_registration_number).Count();
            }
            return Ok(columnsHeight);
        }

        [HttpGet]
        public async Task<IActionResult> Count()
        {
            var t = await _context.Roads.CountAsync();
            return Ok(t);
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
            data.User_id = _context.Users.FirstOrDefault(a => a.UserName == User.Identity!.Name)?.Id;
            _context.Add(data);
            await _context.SaveChangesAsync();
            var expense = _context.Expenses.FirstOrDefault(a => a.Id == data.Expenses_id);
            if (expense != null)
            {
                expense.Type_id = data.Id;
                expense.Type = Shared.Models.Type.repair;
                _context.Entry(expense).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            return Ok(data.Id);
        }

        [HttpPut]
        public async Task<IActionResult> Put(Roads data)
        {
            _context.Entry(data).State = EntityState.Modified;
            var expense = _context.Expenses.FirstOrDefault(a => a.Id == data.Expenses_id);
            if (expense != null)
            {
                expense.Type_id = data.Id;
                expense.Type = Shared.Models.Type.repair;
                _context.Entry(expense).State = EntityState.Modified;

            }
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
        public string Excel(string lang, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate)
        {
            var roads = _context.Roads.Where(s => (dateFilterStartDate != null ? (s.Date >= dateFilterStartDate) : true) && (dateFilterEndDate != null ? (s.Date <= dateFilterEndDate) : true));

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Roads");
            var currentRow = 1;
            worksheet.Cell(currentRow, 1).Value = "Id";
            worksheet.Cell(currentRow, 1).Style.Font.SetBold();
            worksheet.Cell(currentRow, 2).Value = lang == "hu" ? Cargotruck.Shared.Resources.Resource.User_id : "User ID";
            worksheet.Cell(currentRow, 2).Style.Font.SetBold();
            worksheet.Cell(currentRow, 3).Value = lang == "hu" ? Cargotruck.Shared.Resources.Resource.Task_id : "Task ID";
            worksheet.Cell(currentRow, 3).Style.Font.SetBold();
            worksheet.Cell(currentRow, 4).Value = lang == "hu" ? Cargotruck.Shared.Resources.Resource.Vehicle_registration_number : "Vehicle registration number";
            worksheet.Cell(currentRow, 4).Style.Font.SetBold();
            worksheet.Cell(currentRow, 5).Value = lang == "hu" ? Cargotruck.Shared.Resources.Resource.Id_cargo : "Cargo ID";
            worksheet.Cell(currentRow, 6).Style.Font.SetBold();
            worksheet.Cell(currentRow, 6).Value = lang == "hu" ? Cargotruck.Shared.Resources.Resource.Purpose_of_the_trip : "Purpose_of_the_trip";
            worksheet.Cell(currentRow, 6).Style.Font.SetBold();
            worksheet.Cell(currentRow, 7).Value = lang == "hu" ? Cargotruck.Shared.Resources.Resource.Starting_date : "Starting_date";
            worksheet.Cell(currentRow, 7).Style.Font.SetBold();
            worksheet.Cell(currentRow, 8).Value = lang == "hu" ? Cargotruck.Shared.Resources.Resource.Ending_date : "Ending_date";
            worksheet.Cell(currentRow, 8).Style.Font.SetBold();
            worksheet.Cell(currentRow, 9).Value = lang == "hu" ? Cargotruck.Shared.Resources.Resource.Starting_place : "Starting_place";
            worksheet.Cell(currentRow, 9).Style.Font.SetBold();
            worksheet.Cell(currentRow, 10).Value = lang == "hu" ? Cargotruck.Shared.Resources.Resource.Ending_place : "Ending_place";
            worksheet.Cell(currentRow, 10).Style.Font.SetBold();
            worksheet.Cell(currentRow, 11).Value = lang == "hu" ? Cargotruck.Shared.Resources.Resource.Direction : "Direction";
            worksheet.Cell(currentRow, 11).Style.Font.SetBold();
            worksheet.Cell(currentRow, 12).Value = lang == "hu" ? Cargotruck.Shared.Resources.Resource.Fuel : "Fuel";
            worksheet.Cell(currentRow, 12).Style.Font.SetBold();
            worksheet.Cell(currentRow, 13).Value = lang == "hu" ? Cargotruck.Shared.Resources.Resource.Expenses_id : "Expenses_id";
            worksheet.Cell(currentRow, 13).Style.Font.SetBold();
            worksheet.Cell(currentRow, 14).Value = lang == "hu" ? Cargotruck.Shared.Resources.Resource.Date : "Date";
            worksheet.Cell(currentRow, 14).Style.Font.SetBold();

            foreach (var road in roads)
            {
                currentRow++;
                worksheet.Cell(currentRow, 1).Value = road.Id;
                worksheet.Cell(currentRow, 2).Value = road.User_id;
                worksheet.Cell(currentRow, 3).Value = road.Task_id;
                worksheet.Cell(currentRow, 4).Value = road.Vehicle_registration_number;
                worksheet.Cell(currentRow, 5).Value = road.Id_cargo;
                worksheet.Cell(currentRow, 6).Value = road.Purpose_of_the_trip;
                worksheet.Cell(currentRow, 7).Value = road.Starting_date;
                worksheet.Cell(currentRow, 8).Value = road.Ending_date;
                worksheet.Cell(currentRow, 9).Value = road.Starting_place;
                worksheet.Cell(currentRow, 10).Value = road.Ending_place;
                worksheet.Cell(currentRow, 11).Value = (road.ToString() == "TO" ? lang == "hu" ? Cargotruck.Shared.Resources.Resource.to : "Go to the direction" : lang == "hu" ? Cargotruck.Shared.Resources.Resource.from : "Go from the direction");
                worksheet.Cell(currentRow, 12).Value = road.Fuel;
                worksheet.Cell(currentRow, 13).Value = road.Expenses_id;
                worksheet.Cell(currentRow, 14).Value = road.Date;
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
            var roads = _context.Roads.Where(s => (dateFilterStartDate != null ? (s.Date >= dateFilterStartDate) : true) && (dateFilterEndDate != null ? (s.Date <= dateFilterEndDate) : true));

            int pdfRowIndex = 1;
            Random rnd = new();
            int random = rnd.Next(1000000, 9999999);
            string filename = "Roads" + random + "_" + DateTime.Now.ToString("dd-MM-yyyy");
            string filepath = "Files/" + filename + ".pdf";

            Document document = new(PageSize.A4, 5f, 5f, 10f, 10f);
            FileStream fs = new(filepath, FileMode.Create);
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

            var title = new Paragraph(15, lang == "hu" ? Cargotruck.Shared.Resources.Resource.Roads : "Roads")
            {
                Alignment = Element.ALIGN_CENTER
            };


            document.Add(title);
            document.Add(new Paragraph("\n"));

            if (roads.Any())
            {
                table.AddCell(new PdfPCell(new Phrase("Id", font1))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                });
                table.AddCell(new PdfPCell(new Phrase(lang == "hu" ? Cargotruck.Shared.Resources.Resource.User_id : "User ID", font1))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                });
                table.AddCell(new PdfPCell(new Phrase(lang == "hu" ? Cargotruck.Shared.Resources.Resource.Task_id : "Task ID", font1))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                });
                table.AddCell(new PdfPCell(new Phrase(lang == "hu" ? Cargotruck.Shared.Resources.Resource.Vehicle_registration_number : "Vehicle registration number", font1))
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
                    if (!string.IsNullOrEmpty(road.User_id?.ToString())) { s = road.User_id.ToString(); }
                    else { s = "-"; }
                    table.AddCell(new PdfPCell(new Phrase(s.ToString(), font2))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    });
                    if (!string.IsNullOrEmpty(road.Task_id.ToString())) { s = road.Task_id.ToString(); }
                    else { s = "-"; }
                    table.AddCell(new PdfPCell(new Phrase(s?.ToString(), font2))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    });
                    if (!string.IsNullOrEmpty(road.Vehicle_registration_number?.ToString())) { s = road.Vehicle_registration_number.ToString(); }
                    else { s = "-"; }
                    table.AddCell(new PdfPCell(new Phrase(s?.ToString(), font2))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    });
                    if (!string.IsNullOrEmpty(road.Id_cargo.ToString())) { s = road.Id_cargo.ToString(); }
                    else { s = "-"; }
                    table.AddCell(new PdfPCell(new Phrase(s?.ToString(), font2))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    });
                    if (!string.IsNullOrEmpty(road.Purpose_of_the_trip?.ToString())) { s = road.Purpose_of_the_trip.ToString(); }
                    else { s = "-"; }
                    table.AddCell(new PdfPCell(new Phrase(s?.ToString(), font2))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    });
                    if (!string.IsNullOrEmpty(road.Starting_date.ToString())) { s = road.Starting_date.ToString(); }
                    else { s = "-"; }
                    table.AddCell(new PdfPCell(new Phrase(s?.ToString(), font2))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    });
                }



                document.Add(table);
                document.Add(new Paragraph("\n"));
                table2.AddCell(new PdfPCell(new Phrase("Id", font1))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                });
                table2.AddCell(new PdfPCell(new Phrase(lang == "hu" ? Cargotruck.Shared.Resources.Resource.Ending_date : "Ending date", font1))
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
                table2.AddCell(new PdfPCell(new Phrase(lang == "hu" ? Cargotruck.Shared.Resources.Resource.Fuel : "Fuel", font1))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                });
                table2.HeaderRows = 1;


                foreach (Roads road in roads)
                {
                    var s = "";
                    pdfRowIndex++;

                    if (!string.IsNullOrEmpty(road.Id.ToString())) { s = road.Id.ToString(); }
                    else { s = "-"; }
                    table2.AddCell(new PdfPCell(new Phrase(s.ToString(), font2))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    });
                    if (!string.IsNullOrEmpty(road.Ending_date.ToString())) { s = road.Ending_date.ToString(); }
                    else { s = "-"; }
                    table2.AddCell(new PdfPCell(new Phrase(s?.ToString(), font2))
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
                    if (!string.IsNullOrEmpty(road.Direction?.ToString())) { s = road.Direction.ToString(); }
                    else { s = "-"; }
                    table2.AddCell(new PdfPCell(new Phrase(s.ToString() == "TO" ? lang == "hu" ? Cargotruck.Shared.Resources.Resource.to : "Go to the direction" : lang == "hu" ? Cargotruck.Shared.Resources.Resource.from : "Go from the direction", font2))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    });
                    if (!string.IsNullOrEmpty(road.Expenses_id.ToString())) { s = road.Expenses_id.ToString(); }
                    else { s = "-"; }
                    table2.AddCell(new PdfPCell(new Phrase(s?.ToString(), font2))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    });
                    if (!string.IsNullOrEmpty(road?.Fuel?.ToString())) { s = road.Fuel.ToString(); }
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
            var roads = _context.Roads.Where(s => (dateFilterStartDate != null ? (s.Date >= dateFilterStartDate) : true) && (dateFilterEndDate != null ? (s.Date <= dateFilterEndDate) : true));

            Random rnd = new();
            int random = rnd.Next(1000000, 9999999);
            string filename = "Roads" + random + "_" + DateTime.Now.ToString("dd-MM-yyyy");
            string filepath = "Files/" + filename + ".csv";

            StreamWriter txt = new(filepath);
            txt.Write("Id" + ";");
            txt.Write((lang == "hu" ? Cargotruck.Shared.Resources.Resource.User_id : "User ID") + ";");
            txt.Write((lang == "hu" ? Cargotruck.Shared.Resources.Resource.Task_id : "Task ID") + ";");
            txt.Write((lang == "hu" ? Cargotruck.Shared.Resources.Resource.Vehicle_registration_number : "Vehicle registration number") + ";");
            txt.Write((lang == "hu" ? Cargotruck.Shared.Resources.Resource.Id_cargo : "Cargo ID") + ";");
            txt.Write((lang == "hu" ? Cargotruck.Shared.Resources.Resource.Purpose_of_the_trip : "Purpose of the trip") + ";");
            txt.Write((lang == "hu" ? Cargotruck.Shared.Resources.Resource.Starting_date : "Starting date") + ";");
            txt.Write((lang == "hu" ? Cargotruck.Shared.Resources.Resource.Ending_date : "Ending_date") + ";");
            txt.Write((lang == "hu" ? Cargotruck.Shared.Resources.Resource.Starting_place : "Starting place") + ";");
            txt.Write((lang == "hu" ? Cargotruck.Shared.Resources.Resource.Ending_place : "Ending_place") + ";");
            txt.Write((lang == "hu" ? Cargotruck.Shared.Resources.Resource.Direction : "Direction") + ";");
            txt.Write((lang == "hu" ? Cargotruck.Shared.Resources.Resource.Fuel : "Fuel") + ";");
            txt.Write((lang == "hu" ? Cargotruck.Shared.Resources.Resource.Expenses_id : "Expenses_id") + ";");
            txt.Write((lang == "hu" ? Cargotruck.Shared.Resources.Resource.Date : "Date") + ";");
            txt.Write("\n");

            foreach (var road in roads)
            {
                txt.Write(road.Id + ";");
                txt.Write(road.User_id + ";");
                txt.Write(road.Task_id + ";");
                txt.Write(road.Vehicle_registration_number + ";");
                txt.Write(road.Id_cargo + ";");
                txt.Write(road.Purpose_of_the_trip + ";");
                txt.Write(road.Starting_date + ";");
                txt.Write(road.Ending_date + ";");
                txt.Write(road.Starting_place + ";");
                txt.Write(road.Ending_place + ";");
                txt.Write((road.Direction == "TO" ? lang == "hu" ? Cargotruck.Shared.Resources.Resource.to : "Go to the direction" : lang == "hu" ? Cargotruck.Shared.Resources.Resource.from : "Go from the direction") + ";");
                txt.Write(road.Fuel + ";");
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
                                lang == "hu" ? Cargotruck.Shared.Resources.Resource.User_id : "User ID",
                                lang == "hu" ? Cargotruck.Shared.Resources.Resource.Task_id : "Task ID",
                                lang == "hu" ? Cargotruck.Shared.Resources.Resource.Vehicle_registration_number : "Vehicle registration number",
                                lang == "hu" ? Cargotruck.Shared.Resources.Resource.Id_cargo : "Cargo ID",
                                lang == "hu" ? Cargotruck.Shared.Resources.Resource.Purpose_of_the_trip : "Purpose of the trip",
                                lang == "hu" ? Cargotruck.Shared.Resources.Resource.Starting_date : "Starting date",
                                lang == "hu" ? Cargotruck.Shared.Resources.Resource.Ending_date : "Ending_date",
                                lang == "hu" ? Cargotruck.Shared.Resources.Resource.Starting_place : "Starting place",
                                lang == "hu" ? Cargotruck.Shared.Resources.Resource.Ending_place : "Ending_place",
                                lang == "hu" ? Cargotruck.Shared.Resources.Resource.Direction : "Direction",
                                lang == "hu" ? Cargotruck.Shared.Resources.Resource.Fuel : "Fuel",
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

                                if (nulls != list.Count)
                                {
                                    var sql = @"Insert Into Roads (User_id,Task_id,Vehicle_registration_number,Id_cargo,Purpose_of_the_trip,Starting_date,Ending_date,Starting_place,Ending_place,Direction,fuel,Expenses_id,Date) 
                                    Values (@User_id,@Task_id,@Vehicle_registration_number,@Id_cargo,@Purpose_of_the_trip,@Starting_date,@Ending_date,@Starting_place,@Ending_place,@Direction,@fuel,@Expenses_id,@Date)";
                                    var insert = await _context.Database.ExecuteSqlRawAsync(sql,
                                        new SqlParameter("@User_id", list[l]),
                                        new SqlParameter("@Task_id", list[l + 1]),
                                        new SqlParameter("@Vehicle_registration_number", list[l + 2]),
                                        new SqlParameter("@Id_cargo", list[l + 3]),
                                        new SqlParameter("@Purpose_of_the_trip", list[l + 4]),
                                        new SqlParameter("@Starting_date", list[l + 5] == System.DBNull.Value ? System.DBNull.Value : DateTime.Parse(list[l + 5]?.ToString()!)),
                                        new SqlParameter("@Ending_date", list[l + 6] == System.DBNull.Value ? System.DBNull.Value : DateTime.Parse(list[l + 6]?.ToString()!)),
                                        new SqlParameter("@Starting_place", list[l + 7]),
                                        new SqlParameter("@Ending_place", list[l + 8]),
                                        new SqlParameter("@Direction", (list[l + 9]?.ToString() == Cargotruck.Shared.Resources.Resource.to || list[l + 9]?.ToString() == "Go to the direction" ? "TO" : "FROM")),
                                        new SqlParameter("@Fuel", list[l + 10]),
                                        new SqlParameter("@Expenses_id", list[l + 11]),
                                        new SqlParameter("@Date", DateTime.Now)
                                        );
                                    if (insert > 0)
                                    {
                                        error = "";
                                        var lastId = await _context.Roads.OrderBy(x => x.Date).LastOrDefaultAsync();

                                        if (lastId != null)
                                        {
                                            var WithNewIds = await _context.Roads.Where(x => x.Task_id == lastId.Task_id || x.Id_cargo == lastId.Id_cargo || x.Expenses_id == lastId.Expenses_id).ToListAsync();
                                            Cargoes? cargo = await _context.Cargoes.FirstOrDefaultAsync(x => x.Id == lastId.Id_cargo);
                                            Tasks? task = await _context.Tasks.FirstOrDefaultAsync(x => x.Id == lastId.Task_id);
                                            Expenses? expense = await _context.Expenses.FirstOrDefaultAsync(x => x.Id == lastId.Expenses_id);
                                            Trucks? truck = await _context.Trucks.FirstOrDefaultAsync(x => x.Vehicle_registration_number == lastId.Vehicle_registration_number);

                                            foreach (var item in WithNewIds)
                                            {
                                                if (item != null)
                                                {
                                                    if (item.Id != lastId?.Id)
                                                    {
                                                        if (item.Id_cargo == lastId?.Id_cargo)
                                                        {
                                                            item.Id_cargo = null;
                                                        }
                                                        if (item.Task_id == lastId?.Task_id)
                                                        {
                                                            item.Task_id = null;
                                                        }
                                                        if (item.Expenses_id == lastId?.Expenses_id)
                                                        {
                                                            item.Expenses_id = null;
                                                        }
                                                        _context.Entry(item).State = EntityState.Modified;
                                                        await _context.SaveChangesAsync();
                                                    }
                                                    else
                                                    {
                                                        if (cargo == null)
                                                        {
                                                            item.Id_cargo = null;
                                                        }
                                                        if (task == null)
                                                        {
                                                            item.Task_id = null;
                                                        }
                                                        if (expense == null)
                                                        {
                                                            item.Expenses_id = null;
                                                        }
                                                        if (truck == null)
                                                        {
                                                            item.Vehicle_registration_number = null;
                                                        }
                                                        _context.Entry(item).State = EntityState.Modified;
                                                        await _context.SaveChangesAsync();
                                                    }
                                                }
                                            }

                                            if (expense != null)
                                            {
                                                expense.Type_id = lastId?.Id;
                                                expense.Type = Shared.Models.Type.repair;
                                                _context.Entry(expense).State = EntityState.Modified;
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