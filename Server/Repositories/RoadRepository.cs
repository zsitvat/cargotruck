using Cargotruck.Server.Data;
using Cargotruck.Server.Repositories.Interfaces;
using Cargotruck.Server.Services;
using Cargotruck.Server.Services.Interfaces;
using Cargotruck.Shared.Model;
using Cargotruck.Shared.Resources;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Bibliography;
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

        private async Task<List<Road>> GetDataAsync(string? searchString, string? filter, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate)
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
                    (s.TaskId != null && s.TaskId.ToString()!.ToLower().Contains(searchString))
                || (s.TaskId != null && s.TaskId.ToString()!.ToLower().Contains(searchString))
                || (s.VehicleRegistrationNumber != null && s.VehicleRegistrationNumber.ToString()!.ToLower().Contains(searchString))
                || (s.PurposeOfTheTrip != null && s.PurposeOfTheTrip.ToLower()!.Contains(searchString))
                || s.StartingDate.ToString()!.ToLower().Contains(searchString)
                || s.EndingDate.ToString()!.Contains(searchString)
                || s.Fuel!.ToString()!.Contains(searchString)
                || s.Distance!.ToString()!.Contains(searchString)
                || (s.StartingPlace != null && s.StartingPlace.ToLower()!.Contains(searchString))
                || (s.EndingPlace != null && s.EndingPlace.ToLower()!.Contains(searchString))
                || (s.Direction != null && s.Direction.ToString()!.Contains(searchString))
                || (s.ExpensesId != null && s.ExpensesId.ToString()!.Contains(searchString))
                ).ToList();
            }

            return data;
        }

        public async Task<List<Road>> GetAsync(int page, int pageSize, string sortOrder, bool desc, string? searchString, string? filter, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate)
        {
            var data = await GetDataAsync(searchString, filter, dateFilterStartDate, dateFilterEndDate);

            sortOrder = sortOrder == "TaskId" ? (desc ? "TaskId_desc" : "TaskId") : (sortOrder);
            sortOrder = sortOrder == "VehicleRegistrationNumber" ? (desc ? "VehicleRegistrationNumber_desc" : "VehicleRegistrationNumber") : (sortOrder);
            sortOrder = sortOrder == "CargoId" ? (desc ? "CargoId_desc" : "CargoId") : (sortOrder);
            sortOrder = sortOrder == "PurposeOfTheTrip" ? (desc ? "PurposeOfTheTrip_desc" : "PurposeOfTheTrip") : (sortOrder);
            sortOrder = sortOrder == "StartingDate" ? (desc ? "StartingDate_desc" : "StartingDate") : (sortOrder);
            sortOrder = sortOrder == "EndingDate" ? (desc ? "EndingDate_desc" : "EndingDate") : (sortOrder);
            sortOrder = sortOrder == "StartingPlace" ? (desc ? "StartingPlace_desc" : "StartingPlace") : (sortOrder);
            sortOrder = sortOrder == "EndingPlace" ? (desc ? "EndingPlace_desc" : "EndingPlace") : (sortOrder);
            sortOrder = sortOrder == "Direction" ? (desc ? "Direction_desc" : "Direction") : (sortOrder);
            sortOrder = sortOrder == "Distance" ? (desc ? "Distance_desc" : "Distance") : (sortOrder);
            sortOrder = sortOrder == "Fuel" ? (desc ? "Fuel_desc" : "Fuel") : (sortOrder);
            sortOrder = sortOrder == "ExpensesId" ? (desc ? "ExpensesId_desc" : "ExpensesId") : (sortOrder);
            sortOrder = sortOrder == "Date" || String.IsNullOrEmpty(sortOrder) ? (desc ? "Date_desc" : "") : (sortOrder);

            data = sortOrder switch
            {
                "TaskId_desc" => data.OrderByDescending(s => s.TaskId).ToList(),
                "TaskId" => data.OrderBy(s => s.TaskId).ToList(),
                "VehicleRegistrationNumber_desc" => data.OrderByDescending(s => s.VehicleRegistrationNumber).ToList(),
                "VehicleRegistrationNumber" => data.OrderBy(s => s.VehicleRegistrationNumber).ToList(),
                "CargoId_desc" => data.OrderByDescending(s => s.TaskId).ToList(),
                "CargoId" => data.OrderBy(s => s.TaskId).ToList(),
                "PurposeOfTheTrip_desc" => data.OrderByDescending(s => s.PurposeOfTheTrip).ToList(),
                "PurposeOfTheTrip" => data.OrderBy(s => s.PurposeOfTheTrip).ToList(),
                "StartingDate_desc" => data.OrderByDescending(s => s.StartingDate).ToList(),
                "StartingDate" => data.OrderBy(s => s.StartingDate).ToList(),
                "EndingDate_desc" => data.OrderByDescending(s => s.EndingDate).ToList(),
                "EndingDate" => data.OrderBy(s => s.EndingDate).ToList(),
                "StartingPlace_desc" => data.OrderByDescending(s => s.StartingPlace).ToList(),
                "StartingPlace" => data.OrderBy(s => s.StartingPlace).ToList(),
                "EndingPlace_desc" => data.OrderByDescending(s => s.EndingPlace).ToList(),
                "EndingPlace" => data.OrderBy(s => s.EndingPlace).ToList(),
                "Direction_desc" => data.OrderByDescending(s => s.Direction).ToList(),
                "Direction" => data.OrderBy(s => s.Direction).ToList(),
                "Fuel_desc" => data.OrderByDescending(s => s.Fuel).ToList(),
                "Fuel" => data.OrderBy(s => s.Fuel).ToList(),
                "Distance_desc" => data.OrderByDescending(s => s.Distance).ToList(),
                "Distance" => data.OrderBy(s => s.Distance).ToList(),
                "ExpensesId_desc" => data.OrderByDescending(s => s.ExpensesId).ToList(),
                "ExpensesId" => data.OrderBy(s => s.ExpensesId).ToList(),
                "Date_desc" => data.OrderByDescending(s => s.Date).ToList(),
                _ => data.OrderBy(s => s.Date).ToList(),
            };
            data = data.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            return data;
        }

        public async Task<Road?> GetByIdAsync(int id)
        {
            return await _context.Roads.FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<List<Road>> GetRoadsAsync()
        {
            var t = await _context.Roads.ToListAsync();
            return t;
        }

        //gets the data of the charts
        public async Task<int[]> GetChartDataAsync()
        {
            var data = await _context.Roads.ToListAsync();
            var trucksCount = data.Where(x => x.VehicleRegistrationNumber != null && x.VehicleRegistrationNumber != "").DistinctBy(x => x.VehicleRegistrationNumber).Count();
            var trucksVRN = data?.DistinctBy(x => x.VehicleRegistrationNumber).ToList();
            int[] columnsHeight = new int[12 * trucksCount];
            int h = 1;
            for (int i = 0; i < columnsHeight.Length; i++)
            {
                h++;
                if (h == 13) h = 1;
                if (data != null)
                {
                    columnsHeight[i] = data.Where(x => x.Date.Year == DateTime.Now.Year && x.Date.Month == h && x.VehicleRegistrationNumber != null && x.VehicleRegistrationNumber == trucksVRN?[i / 12].VehicleRegistrationNumber).Count();
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

        public async Task PostAsync(Road data)
        {
            _context.Add(data);

            var expense = _context.Expenses.FirstOrDefault(a => a.Id == data.ExpensesId);
            if (expense != null)
            {
                expense.TypeId = data.Id;
                expense.Type = Shared.Model.Type.repair;
                _context.Entry(expense).State = EntityState.Modified;

            }

            var truck = _context.Trucks.FirstOrDefault(a => a.VehicleRegistrationNumber == data.VehicleRegistrationNumber);
            if (truck != null && data.EndingDate > DateTime.Now)
            {
                truck.RoadId = data.Id;
                truck.Status = data.TaskId == null ? Status.on_road : Status.delivering;
                _context.Entry(truck).State = EntityState.Modified;

            }

            await _context.SaveChangesAsync();
        }

        public async Task PutAsync(Road data)
        {
            _context.Entry(data).State = EntityState.Modified;

            var expense = _context.Expenses.FirstOrDefault(a => a.Id == data.ExpensesId);
            if (expense != null)
            {
                expense.TypeId = data.Id;
                expense.Type = Shared.Model.Type.repair;
                _context.Entry(expense).State = EntityState.Modified;
            }

            var truck = _context.Trucks.FirstOrDefault(a => a.VehicleRegistrationNumber == data.VehicleRegistrationNumber);
            if (truck != null && data.EndingDate > DateTime.Now)
            {
                truck.RoadId = data.Id;
                truck.Status = data.TaskId == null ? Status.on_road : Status.delivering;
                _context.Entry(truck).State = EntityState.Modified;

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
            List<string> columnNames = _columnNameLists.GetRoadsColumnNames().Where(x => x != "UserId").Select(x => _localizer[x].Value).ToList();

            for (var i = 0; i < columnNames.Count; i++)
            {
                worksheet.Cell(currentRow, i + 1).Value = columnNames[i];
                worksheet.Cell(currentRow, i + 1).Style.Font.SetBold();
            }

            foreach (var road in roads)
            {
                currentRow++;
                worksheet.Cell(currentRow, 1).Value = road.Id;
                worksheet.Cell(currentRow, 2).Value = road.TaskId;
                worksheet.Cell(currentRow, 3).Value = road.VehicleRegistrationNumber;
                worksheet.Cell(currentRow, 4).Value = road.TaskId;
                worksheet.Cell(currentRow, 5).Value = road.PurposeOfTheTrip;
                worksheet.Cell(currentRow, 6).Value = road.StartingDate;
                worksheet.Cell(currentRow, 7).Value = road.EndingDate;
                worksheet.Cell(currentRow, 8).Value = road.StartingPlace;
                worksheet.Cell(currentRow, 9).Value = road.EndingPlace;
                worksheet.Cell(currentRow, 10).Value = (road.ToString() == "TO" ? _localizer["to"] : _localizer["from"]);
                worksheet.Cell(currentRow, 11).Value = road.Distance;
                worksheet.Cell(currentRow, 12).Value = road.Fuel;
                worksheet.Cell(currentRow, 13).Value = road.ExpensesId;
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

            System.Type type = typeof(Road);
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

                foreach (Road road in roads)
                {
                    var s = "";
                    if (!string.IsNullOrEmpty(road.Id.ToString())) { s = road.Id.ToString(); }
                    else { s = "-"; }
                    table.AddCell(new PdfPCell(new Phrase(s.ToString(), font2))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    });
                    if (!string.IsNullOrEmpty(road.TaskId.ToString())) { s = road.TaskId.ToString(); }
                    else { s = "-"; }
                    table.AddCell(new PdfPCell(new Phrase(s?.ToString(), font2))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    });
                    if (!string.IsNullOrEmpty(road.VehicleRegistrationNumber?.ToString())) { s = road.VehicleRegistrationNumber.ToString(); }
                    else { s = "-"; }
                    table.AddCell(new PdfPCell(new Phrase(s?.ToString(), font2))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    });
                    if (!string.IsNullOrEmpty(road.TaskId.ToString())) { s = road.TaskId.ToString(); }
                    else { s = "-"; }
                    table.AddCell(new PdfPCell(new Phrase(s?.ToString(), font2))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    });
                    if (!string.IsNullOrEmpty(road.PurposeOfTheTrip?.ToString())) { s = road.PurposeOfTheTrip.ToString(); }
                    else { s = "-"; }
                    table.AddCell(new PdfPCell(new Phrase(s?.ToString(), font2))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    });
                    if (!string.IsNullOrEmpty(road.StartingDate.ToString())) { s = road.StartingDate.ToString(); }
                    else { s = "-"; }
                    table.AddCell(new PdfPCell(new Phrase(s?.ToString(), font2))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    });
                    if (!string.IsNullOrEmpty(road.EndingDate.ToString())) { s = road.EndingDate.ToString(); }
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


                foreach (Road road in roads)
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
                    if (!string.IsNullOrEmpty(road.StartingPlace)) { s = road.StartingPlace.ToString(); }
                    else { s = "-"; }
                    table2.AddCell(new PdfPCell(new Phrase(s?.ToString(), font2))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    });
                    if (!string.IsNullOrEmpty(road.EndingPlace)) { s = road.EndingPlace.ToString(); }
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

            byte[] fileBytes = await File.ReadAllBytesAsync(filepath);
            string base64String = Convert.ToBase64String(fileBytes);
            File.Delete(filepath);

            return base64String;
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
            List<string> columnNames = _columnNameLists.GetRoadsColumnNames().Where(x => x != "UserId").Select(x => _localizer[x].Value).ToList();

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
                txt.Write(road.TaskId + separator);
                txt.Write((road.VehicleRegistrationNumber ?? ifNull) + separator);
                txt.Write((road.TaskId != null ? road.TaskId : ifNull) + separator);
                txt.Write((road.PurposeOfTheTrip ?? ifNull) + separator);
                txt.Write((road.StartingDate != null ? road.StartingDate : ifNull) + separator);
                txt.Write((road.EndingDate != null ? road.EndingDate : ifNull) + separator);
                txt.Write((road.StartingPlace ?? ifNull) + separator);
                txt.Write((road.EndingPlace ?? ifNull) + separator);
                txt.Write((road.Direction == "TO" ? _localizer["to"] : _localizer["from"]) + separator);
                txt.Write((road.Distance != null ? road.Distance : ifNull) + separator);
                txt.Write((road.Fuel != null ? road.Fuel : ifNull) + separator);
                txt.Write((road.ExpensesId != null ? road.ExpensesId : ifNull) + separator);
                txt.Write(road.Date + separator);
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

        public async Task<string?> ImportAsync([FromBody] string file, CultureInfo lang)
        {
            CultureInfo.CurrentUICulture = lang;
            var error = "";
            var haveColumns = false;
            int rowNumber = 0;

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
                                List<string> columnNames = _columnNameLists.GetRoadsColumnNames().Where(x => x != "UserId").Select(x => _localizer[x].Value).ToList();

                                var cellValues = row.Cells().Select(c => c.Value.ToString()).ToList();
                                if (!columnNames.SequenceEqual(cellValues))
                                {
                                    error = _localizer["Not_match_col"].Value;
                                    File.Delete(path); // delete the file
                                    return error;
                                }
                                dt.Columns.AddRange(cellValues.Select(c => new DataColumn(c)).ToArray());
                                columnNames.Clear();

                                firstRow = false;
                                if (columnNames.Count == 0 || (columnNames.Count == 1 && columnNames.Contains("Id")))
                                {
                                    haveColumns = true;
                                    if (columnNames.Count == 0)
                                    {
                                        l += 1;
                                    }
                                }
                            }
                            else if (haveColumns)
                            {
                                List<string?> list = new();
                                int nulls = 0;
                                //Add rows to DataTable.
                                dt.Rows.Add();

                                foreach (IXLCell cell in row.Cells(1, dt.Columns.Count))
                                {
                                    if (cell.Value != null && cell.Value.ToString() != "")
                                    {
                                        list.Add(cell.Value.ToString());
                                    }
                                    else
                                    {
                                        list.Add(null);
                                        nulls += 1;
                                    }
                                }

                                try
                                {
                                    if (nulls != list.Count)
                                    {
                                        ++rowNumber;

                                        var road = new Road
                                        {
                                            UserId = "Imported",
                                            TaskId = !string.IsNullOrWhiteSpace(list[l]) ? int.Parse(list[l] ?? "0") : null,
                                            VehicleRegistrationNumber = list[l + 1],
                                            CargoId = !string.IsNullOrWhiteSpace(list[l + 2]) ? int.Parse(list[l + 2] ?? "0") : null,
                                            PurposeOfTheTrip = list[l + 3],
                                            StartingDate = !string.IsNullOrWhiteSpace(list[l + 4]) ? DateTime.Parse(list[l + 4] ?? "0") : null,
                                            EndingDate = !string.IsNullOrWhiteSpace(list[l + 5]) ? DateTime.Parse(list[l + 5] ?? "0") : null,
                                            StartingPlace = list[l + 6],
                                            EndingPlace = list[l + 7],
                                            Direction = (list[l + 8]?.ToString() == _localizer["to"] ? "TO" : "FROM"),
                                            Distance = !string.IsNullOrWhiteSpace(list[l + 9]) ? int.Parse(list[l + 9] ?? "0") : null,
                                            Fuel = !string.IsNullOrWhiteSpace(list[l + 10]) ? int.Parse(list[l + 10] ?? "0") : null,
                                            ExpensesId = !string.IsNullOrWhiteSpace(list[l + 11]) ? int.Parse(list[l + 11] ?? "0") : null,
                                            Date = DateTime.Now
                                        };

                                        if (road != null)
                                        {
                                            bool saveable = true;
                                            DeliveryTask? task = await _context.Tasks.FirstOrDefaultAsync(x => x.Id == road.TaskId);
                                            Cargo? cargo = await _context.Cargoes.FirstOrDefaultAsync(x => x.Id == road.CargoId);
                                            Expense? expense = await _context.Expenses.FirstOrDefaultAsync(x => x.Id == road.ExpensesId);
                                            Truck? truck = await _context.Trucks.FirstOrDefaultAsync(x => x.VehicleRegistrationNumber == road.VehicleRegistrationNumber);


                                            if (saveable && road?.CargoId != null)
                                            {
                                                var withNewId = await _context.Roads.Where(x => x.CargoId == road.CargoId).ToListAsync();

                                                if (withNewId.Count != 0)
                                                {
                                                    saveable = false;
                                                    error += "\n " + _localizer["Deleted_duplicate_id"] + " " + rowNumber + ".";
                                                }
                                                else if (cargo == null)
                                                {
                                                    saveable = false;
                                                    error += "\n " + _localizer["Deleted_wrong_id"] + " " + rowNumber + ".";
                                                }
                                            }

                                            if (saveable)
                                            {
                                                await _context.Roads.AddAsync(road!);
                                                await _context.SaveChangesAsync();
                                            }
                                        }
                                        else if (road == null)
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
            return error.TrimStart('\r', '\n');
        }
    }
}
