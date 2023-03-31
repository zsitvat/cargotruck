using Cargotruck.Server.Data;
using Cargotruck.Server.Repositories.Interfaces;
using Cargotruck.Server.Services;
using Cargotruck.Server.Services.Interfaces;
using Cargotruck.Shared.Model;
using Cargotruck.Shared.Resources;
using ClosedXML.Excel;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System.Data;
using System.Globalization;
using System.Linq.Expressions;
using System.Text;

namespace Cargotruck.Server.Repositories
{
    public class RoadRepository:IRoadRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IStringLocalizer<Resource> _localizer;
        private readonly IColumnNamesService _columnNameLists;
        private readonly IErrorHandlerService _errorHandler;

        public RoadRepository(ApplicationDbContext context, IStringLocalizer<Resource> localizer, IColumnNamesService columnNameLists, IErrorHandlerService errorHandler)
        {
            _context = context;
            _localizer = localizer;
            _columnNameLists = columnNameLists;
            _errorHandler = errorHandler;
        }

        private async Task<List<Roads>> GetDataAsync(string? searchString, string? filter, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate)
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
                || s.Fuel!.ToString()!.Contains(searchString)
                || s.Distance!.ToString()!.Contains(searchString)
                || (s.Starting_place != null && s.Starting_place.ToLower()!.Contains(searchString))
                || (s.Ending_place != null && s.Ending_place.ToLower()!.Contains(searchString))
                || (s.Direction != null && s.Direction.ToString()!.Contains(searchString))
                || (s.Expenses_id != null && s.Expenses_id.ToString()!.Contains(searchString))
                ).ToList();
            }

            return data;
        }

        public async Task<List<Roads>> GetAsync(int page, int pageSize, string sortOrder, bool desc, string? searchString, string? filter, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate)
        {
            var data = await GetDataAsync(searchString, filter, dateFilterStartDate, dateFilterEndDate);

            sortOrder = sortOrder == "Task_id" ? (desc ? "Task_id_desc" : "Task_id") : (sortOrder);
            sortOrder = sortOrder == "Vehicle_registration_number" ? (desc ? "Vehicle_registration_number_desc" : "Vehicle_registration_number") : (sortOrder);
            sortOrder = sortOrder == "Id_cargo" ? (desc ? "Id_cargo_desc" : "Id_cargo") : (sortOrder);
            sortOrder = sortOrder == "Purpose_of_the_trip" ? (desc ? "Purpose_of_the_trip_desc" : "Purpose_of_the_trip") : (sortOrder);
            sortOrder = sortOrder == "Starting_date" ? (desc ? "Starting_date_desc" : "Starting_date") : (sortOrder);
            sortOrder = sortOrder == "Ending_date" ? (desc ? "Ending_date_desc" : "Ending_date") : (sortOrder);
            sortOrder = sortOrder == "Starting_place" ? (desc ? "Starting_place_desc" : "Starting_place") : (sortOrder);
            sortOrder = sortOrder == "Ending_place" ? (desc ? "Ending_place_desc" : "Ending_place") : (sortOrder);
            sortOrder = sortOrder == "Direction" ? (desc ? "Direction_desc" : "Direction") : (sortOrder);
            sortOrder = sortOrder == "Distance" ? (desc ? "Distance_desc" : "Distance") : (sortOrder);
            sortOrder = sortOrder == "Fuel" ? (desc ? "Fuel_desc" : "Fuel") : (sortOrder);
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
                "Distance_desc" => data.OrderByDescending(s => s.Distance).ToList(),
                "Distance" => data.OrderBy(s => s.Distance).ToList(),
                "Expenses_id_desc" => data.OrderByDescending(s => s.Expenses_id).ToList(),
                "Expenses_id" => data.OrderBy(s => s.Expenses_id).ToList(),
                "Date_desc" => data.OrderByDescending(s => s.Date).ToList(),
                _ => data.OrderBy(s => s.Date).ToList(),
            };
            data = data.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            return data;
        }

        public async Task<Roads?> GetByIdAsync(int id)
        {
            return await _context.Roads.FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<List<Roads>> GetRoadsAsync()
        {
            var t = await _context.Roads.ToListAsync();
            return t;
        }

        public async Task<int[]> GetChartDataAsync()
        {
            var data = await _context.Roads.ToListAsync();
            var trucksCount = data.Where(x => x.Vehicle_registration_number != null && x.Vehicle_registration_number != "").DistinctBy(x => x.Vehicle_registration_number).Count();
            var trucksVRN = data?.DistinctBy(x => x.Vehicle_registration_number).ToList();
            int[] columnsHeight = new int[12 * trucksCount];
            int h = 1;
            for (int i = 0; i < columnsHeight.Length; i++)
            {
                h++;
                if (h == 13) h = 1;
                if (data != null)
                {
                    columnsHeight[i] = data.Where(x => x.Date.Year == DateTime.Now.Year && x.Date.Month == h && x.Vehicle_registration_number != null && x.Vehicle_registration_number == trucksVRN?[i / 12].Vehicle_registration_number).Count();
                }
                else
                {
                    columnsHeight[i] = 0;
                }
            }
            return columnsHeight;
        }

        public async Task<int> PageCountAsync(string? searchString, string? filter, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate)
        {
            var t = await GetDataAsync(searchString, filter, dateFilterStartDate, dateFilterEndDate);
            return t.Count;
        }

        public async Task<int> CountAsync()
        {
            return await _context.Roads.CountAsync();
        }

        public async Task PostAsync(Roads data)
        {
            _context.Add(data);

            var expense = _context.Expenses.FirstOrDefault(a => a.Id == data.Expenses_id);
            if (expense != null)
            {
                expense.Type_id = data.Id;
                expense.Type = Shared.Model.Type.repair;
                _context.Entry(expense).State = EntityState.Modified;
               
            }

            await _context.SaveChangesAsync();
        }

        public async Task PutAsync(Roads data)
        {
            _context.Entry(data).State = EntityState.Modified;

            var expense = _context.Expenses.FirstOrDefault(a => a.Id == data.Expenses_id);
            if (expense != null)
            {
                expense.Type_id = data.Id;
                expense.Type = Shared.Model.Type.repair;
                _context.Entry(expense).State = EntityState.Modified;
            }

            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var data = _context.Roads.FirstOrDefault(x => x.Id == id);
            if (data != null)
            {
                _context.Roads.Remove(data);
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }

        public string ExportToExcel(CultureInfo lang, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate)
        {
            var roads = _context.Roads.Where(s => (dateFilterStartDate != null ? (s.Date >= dateFilterStartDate) : true) && (dateFilterEndDate != null ? (s.Date <= dateFilterEndDate) : true));

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Roads");
            var currentRow = 1;

            CultureInfo.CurrentUICulture = lang;
            List<string> columnNames = _columnNameLists.GetRoadsColumnNames().Where(x => x != "User_id").Select(x => _localizer[x].Value).ToList();

            for (var i = 0; i < columnNames.Count; i++)
            {
                worksheet.Cell(currentRow, i + 1).Value = columnNames[i];
                worksheet.Cell(currentRow, i + 1).Style.Font.SetBold();
            }

            foreach (var road in roads)
            {
                currentRow++;
                worksheet.Cell(currentRow, 1).Value = road.Id;
                worksheet.Cell(currentRow, 2).Value = road.Task_id;
                worksheet.Cell(currentRow, 3).Value = road.Vehicle_registration_number;
                worksheet.Cell(currentRow, 4).Value = road.Id_cargo;
                worksheet.Cell(currentRow, 5).Value = road.Purpose_of_the_trip;
                worksheet.Cell(currentRow, 6).Value = road.Starting_date;
                worksheet.Cell(currentRow, 7).Value = road.Ending_date;
                worksheet.Cell(currentRow, 8).Value = road.Starting_place;
                worksheet.Cell(currentRow, 9).Value = road.Ending_place;
                worksheet.Cell(currentRow, 10).Value = (road.ToString() == "TO" ? _localizer["to"] : _localizer["from"]);
                worksheet.Cell(currentRow, 11).Value = road.Distance;
                worksheet.Cell(currentRow, 12).Value = road.Fuel;
                worksheet.Cell(currentRow, 13).Value = road.Expenses_id;
                worksheet.Cell(currentRow, 14).Value = road.Date;
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            var content = stream.ToArray();
            return Convert.ToBase64String(content);
        }

        public async Task<string> ExportToPdfAsync(CultureInfo lang, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate)
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


            //copy column names to a list based on language
            CultureInfo.CurrentUICulture = lang;
            List<string> columnNames = _columnNameLists.GetRoadsColumnNames().Where(x => x != "Expenses_id").Select(x => _localizer[x].Value).ToList();

            var title = new Paragraph(15, _localizer["Roads"].Value)
            {
                Alignment = Element.ALIGN_CENTER
            };

            document.Add(title);
            document.Add(new Paragraph("\n"));

            if (roads.Any())
            {
                foreach (var name in columnNames.Take(column_number))
                {
                    table.AddCell(new PdfPCell(new Phrase(name, font1))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    });
                }

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
                    if (!string.IsNullOrEmpty(road.Ending_date.ToString())) { s = road.Ending_date.ToString(); }
                    else { s = "-"; }
                    table.AddCell(new PdfPCell(new Phrase(s?.ToString(), font2))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    });
                }

                document.Add(table);
                document.Add(new Paragraph("\n"));

                table2.AddCell(new PdfPCell(new Phrase(_localizer["Id"].Value, font1))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                });

                foreach (var name in columnNames.Skip(column_number).Take(column_number))
                {
                    table2.AddCell(new PdfPCell(new Phrase(name, font1))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    });
                }

                table2.HeaderRows = 1;


                foreach (Roads road in roads)
                {
                    var s = "";
                    pdfRowIndex++;

                    if (!string.IsNullOrEmpty(road.Id.ToString())) { s = road.Id.ToString(); }
                    else { s = "-"; }
                    table2.AddCell(new PdfPCell(new Phrase(s?.ToString(), font2))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    });
                    if (!string.IsNullOrEmpty(road.Starting_place)) { s = road.Starting_place.ToString(); }
                    else { s = "-"; }
                    table2.AddCell(new PdfPCell(new Phrase(s?.ToString(), font2))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    });
                    if (!string.IsNullOrEmpty(road.Ending_place)) { s = road.Ending_place.ToString(); }
                    else { s = "-"; }
                    table2.AddCell(new PdfPCell(new Phrase(s?.ToString(), font2))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    });
                    if (!string.IsNullOrEmpty(road.Direction?.ToString())) { s = road.Direction.ToString(); }
                    else { s = "-"; }
                    table2.AddCell(new PdfPCell(new Phrase(s.ToString() == "TO" ? _localizer["to"] : _localizer["from"], font2))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    });
                    if (!string.IsNullOrEmpty(road?.Distance?.ToString())) { s = road.Distance.ToString(); }
                    else { s = "-"; }
                    table2.AddCell(new PdfPCell(new Phrase(s?.ToString(), font2))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    });
                    if (!string.IsNullOrEmpty(road?.Fuel?.ToString())) { s = road.Fuel.ToString(); }
                    else { s = "-"; }
                    table2.AddCell(new PdfPCell(new Phrase(s?.ToString(), font2))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    });
                    if (!string.IsNullOrEmpty(road?.Date.ToString())) { s = road?.Date.ToString(); }
                    else { s = "-"; }
                    table2.AddCell(new PdfPCell(new Phrase(s?.ToString(), font2))
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
            var roads = _context.Roads.Where(s => (dateFilterStartDate != null ? (s.Date >= dateFilterStartDate) : true) && (dateFilterEndDate != null ? (s.Date <= dateFilterEndDate) : true));

            Random rnd = new();
            int random = rnd.Next(1000000, 9999999);
            string filename = "Roads" + random + "_" + DateTime.Now.ToString("dd-MM-yyyy");
            string filepath = "Files/" + filename + ".csv";

            StreamWriter txt = new(filepath);
            //copy column names to a list based on language
            CultureInfo.CurrentUICulture = lang;
            List<string> columnNames = _columnNameLists.GetRoadsColumnNames().Where(x => x != "User_id").Select(x => _localizer[x].Value).ToList();

            string separator = isTextDocument ? "\t" : ";";
            string ifNull = isTextDocument ? " --- " : "";

            foreach (var name in columnNames)
            {
                 txt.Write(name + (isTextDocument ? "  " : separator));
            }

            txt.Write("\n");

            foreach (var road in roads)
            {
                txt.Write(road.Id + separator);
                txt.Write(road.Task_id + separator);
                txt.Write((road.Vehicle_registration_number ?? ifNull) + separator);
                txt.Write((road.Id_cargo != null ? road.Id_cargo : ifNull) + separator);
                txt.Write((road.Purpose_of_the_trip ?? ifNull) + separator);
                txt.Write((road.Starting_date != null ? road.Starting_date : ifNull) + separator);
                txt.Write((road.Ending_date != null ? road.Ending_date : ifNull) + separator);
                txt.Write((road.Starting_place ?? ifNull) + separator);
                txt.Write((road.Ending_place ?? ifNull) + separator);
                txt.Write((road.Direction == "TO" ? _localizer["to"] : _localizer["from"]) + separator);
                txt.Write((road.Distance != null ? road.Distance : ifNull) + separator);
                txt.Write((road.Fuel != null ? road.Fuel : ifNull) + separator);
                txt.Write((road.Expenses_id != null ? road.Expenses_id : ifNull) + separator);
                txt.Write(road.Date + separator);
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

        public async Task<string?> ImportAsync([FromBody] string file, CultureInfo lang)
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
                                List<string> columnNames = _columnNameLists.GetRoadsColumnNames().Where(x => x != "User_id").Select(x => _localizer[x].Value).ToList();

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
                                        return error;
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

                                try
                                {
                                    if (nulls != list.Count)
                                    {

                                        var sql = @"Insert Into Roads (User_id,Task_id,Vehicle_registration_number,Id_cargo,Purpose_of_the_trip,Starting_date,Ending_date,Starting_place,Ending_place,Direction,Distance,Fuel,Expenses_id,Date) 
                                        Values (@User_id,@Task_id,@Vehicle_registration_number,@Id_cargo,@Purpose_of_the_trip,@Starting_date,@Ending_date,@Starting_place,@Ending_place,@Direction,@Distance,@Fuel,@Expenses_id,@Date)";
                                        var insert = await _context.Database.ExecuteSqlRawAsync(sql,
                                            new SqlParameter("@User_id", "Imported"),
                                            new SqlParameter("@Task_id", list[l]),
                                            new SqlParameter("@Vehicle_registration_number", list[l + 1]),
                                            new SqlParameter("@Id_cargo", list[l + 2]),
                                            new SqlParameter("@Purpose_of_the_trip", list[l + 3]),
                                            new SqlParameter("@Starting_date", list[l + 4] == System.DBNull.Value ? System.DBNull.Value : DateTime.Parse(list[l + 4]?.ToString()!)),
                                            new SqlParameter("@Ending_date", list[l + 5] == System.DBNull.Value ? System.DBNull.Value : DateTime.Parse(list[l + 5]?.ToString()!)),
                                            new SqlParameter("@Starting_place", list[l + 6]),
                                            new SqlParameter("@Ending_place", list[l + 7]),
                                            new SqlParameter("@Direction", (list[l + 8]?.ToString() == _localizer["to"] ? "TO" : "FROM")),
                                            new SqlParameter("@Distance", list[l + 9]),
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
                                                    expense.Type = Shared.Model.Type.repair;
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
                                catch (Exception ex)
                                {
                                    return _errorHandler.GetErrorMessageAsString(ex);
                                }

                            }
                            else
                            {
                                error = _localizer["Not_match_col_count"];
                                return error;
                            }
                            //If no data in ExportToExcel file  
                            if (firstRow)
                            {
                                error = _localizer["Empty_excel"];
                                System.IO.File.Delete(path); // delete the file
                                return error;
                            }
                        }
                    }
                    else
                    {
                        error = _localizer["Missing_data_rows"];
                        System.IO.File.Delete(path); // delete the file
                        return error;
                    }
                }
                else
                {
                    //If file extension of the uploaded file is different then .xlsx  
                    error = _localizer["Not_excel"];
                    System.IO.File.Delete(path); // delete the file
                    return error;
                }
            }
            else
            {
                error = _localizer["No_excel"];
                return error;
            }
            return null;
        }
    }
}
