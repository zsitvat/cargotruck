using Cargotruck.Server.Data;
using Cargotruck.Server.Repositories.Interfaces;
using Cargotruck.Server.Services.Interfaces;
using Cargotruck.Shared.Model;
using Cargotruck.Shared.Model.Dto;
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
using System.Text;

namespace Cargotruck.Server.Repositories
{
    public class CargoRepository : ICargoRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IStringLocalizer<Resource> _localizer;
        private readonly IColumnNamesService _columnNameLists;
        private readonly IErrorHandlerService _errorHandler;
        public CargoRepository(ApplicationDbContext context, IStringLocalizer<Resource> localizer, IColumnNamesService columnNameLists, IErrorHandlerService errorHandler)
        {
            _context = context;
            _localizer = localizer;
            _columnNameLists = columnNameLists;
            _errorHandler = errorHandler;
        }

        private async Task<List<Cargo>> GetDataAsync(string? searchString, string? filter, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate)
        {
            var data = await _context.Cargoes.Where(s => (dateFilterStartDate != null ? (s.Date >= dateFilterStartDate) : true) && (dateFilterEndDate != null ? (s.Date <= dateFilterEndDate) : true)).ToListAsync();

            if (filter == "InWarehouse")
            {
                data = data.Where(data => data.WarehouseId != null).ToList();
            }
            else if (filter == "NotInWarehouse")
            {
                data = data.Where(data => data.WarehouseId == null).ToList();
            }

            searchString = searchString?.ToLower();
            if (searchString != null && searchString != "")
            {
                data = data.Where(s =>
               (s.Task!.Id.ToString().ToLower().Contains(searchString))
            || (s.Weight != null && s.Weight.ToString()!.ToLower()!.Contains(searchString))
            || (s.Description != null && s.Description.ToLower()!.Contains(searchString))
            || (s.DeliveryRequirements != null && s.DeliveryRequirements.ToString().ToLower()!.Contains(searchString))
            || (s.VehicleRegistrationNumber != null && s.VehicleRegistrationNumber.ToString()!.Contains(searchString))
            || (s.WarehouseId != null && s.WarehouseId.ToString()!.Contains(searchString))
            || (s.WarehouseSection != null && s.WarehouseSection.ToLower()!.Contains(searchString))
            || (s.StorageStartingTime != null && s.StorageStartingTime.ToString()!.Contains(searchString))
            ).ToList();
            }

            return data;
        }

        public async Task<List<Cargo>> GetAsync(int page, int pageSize, string sortOrder, bool desc, string? searchString, string? filter, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate)
        {
            var data = await GetDataAsync(searchString, filter, dateFilterStartDate, dateFilterEndDate);

            sortOrder = sortOrder == "TaskId" ? (desc ? "TaskId_desc" : "TaskId") : (sortOrder);
            sortOrder = sortOrder == "Weight" ? (desc ? "Weight_desc" : "Weight") : (sortOrder);
            sortOrder = sortOrder == "Description" ? (desc ? "Description_desc" : "Description") : (sortOrder);
            sortOrder = sortOrder == "DeliveryRequirements" ? (desc ? "DeliveryRequirements_desc" : "DeliveryRequirements") : (sortOrder);
            sortOrder = sortOrder == "VehicleRegistrationNumber" ? (desc ? "VehicleRegistrationNumber_desc" : "VehicleRegistrationNumber") : (sortOrder);
            sortOrder = sortOrder == "WarehouseId" ? (desc ? "WarehouseId_desc" : "WarehouseId") : (sortOrder);
            sortOrder = sortOrder == "WarehouseSection" ? (desc ? "WarehouseSection_desc" : "WarehouseSection") : (sortOrder);
            sortOrder = sortOrder == "StorageStartingTime" ? (desc ? "StorageStartingTime_desc" : "StorageStartingTime") : (sortOrder);
            sortOrder = sortOrder == "Date" || String.IsNullOrEmpty(sortOrder) ? (desc ? "Date_desc" : "") : (sortOrder);

            data = sortOrder switch
            {
                "TaskId_desc" => data.OrderByDescending(s => s.TaskId).ToList(),
                "TaskId" => data.OrderBy(s => s.TaskId).ToList(),
                "Weight_desc" => data.OrderByDescending(s => s.Weight).ToList(),
                "Weight" => data.OrderBy(s => s.Weight).ToList(),
                "Description_desc" => data.OrderByDescending(s => s.Description).ToList(),
                "Description" => data.OrderBy(s => s.Description).ToList(),
                "DeliveryRequirements_desc" => data.OrderByDescending(s => s.DeliveryRequirements).ToList(),
                "DeliveryRequirements" => data.OrderBy(s => s.DeliveryRequirements).ToList(),
                "VehicleRegistrationNumber_desc" => data.OrderByDescending(s => s.VehicleRegistrationNumber).ToList(),
                "VehicleRegistrationNumber" => data.OrderBy(s => s.VehicleRegistrationNumber).ToList(),
                "WarehouseId_desc" => data.OrderByDescending(s => s.WarehouseId).ToList(),
                "WarehouseId" => data.OrderBy(s => s.WarehouseId).ToList(),
                "WarehouseSection_desc" => data.OrderByDescending(s => s.WarehouseSection).ToList(),
                "WarehouseSection" => data.OrderBy(s => s.WarehouseSection).ToList(),
                "StorageStartingTime_desc" => data.OrderByDescending(s => s.StorageStartingTime).ToList(),
                "StorageStartingTime" => data.OrderBy(s => s.StorageStartingTime).ToList(),
                "Date_desc" => data.OrderByDescending(s => s.Date).ToList(),
                _ => data.OrderBy(s => s.Date).ToList(),
            };

            return data.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        }
        public async Task<List<Cargo>> GetCargoesAsync()
        {
            return await _context.Cargoes.ToListAsync();
        }
        public async Task<Cargo?> GetByIdAsync(int id)
        {
            return await _context.Cargoes.FirstOrDefaultAsync(a => a.Id == id);
        }
        public async Task<int[]> GetChartDataAsync()
        {
            var data = await _context.Cargoes.ToListAsync();
            var tasks = await _context.Tasks.ToListAsync();
            int[] columnsHeight = new int[36];

            for (int i = 0; i < 12; i++)
            {
                foreach (var task in tasks.Where(x => x.Completed == false))
                {
                    columnsHeight[i] += data.Where(x => x.Date.Year == DateTime.Now.Year && x.Date.Month == i + 1 && x.WarehouseId == null && x.TaskId == task.Id).Count();
                }
            }

            for (int i = 0; i < 12; i++)
            {
                foreach (var task in tasks.Where(x => x.Completed))
                {
                    columnsHeight[i + 12] += data.Where(x => x.Date.Year == DateTime.Now.Year && x.Date.Month == i + 1 && x.TaskId == task.Id).Count();
                }
            }

            for (int i = 0; i < 12; i++)
            {
                columnsHeight[i + 24] = data.Where(x => x.Date.Year == DateTime.Now.Year && x.Date.Month == i + 1 && x.WarehouseId != null).Count();
            }

            return columnsHeight;
        }
        public async Task<int> PageCountAsync(string? searchString, string? filter, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate)
        {
            var data = await GetDataAsync(searchString, filter, dateFilterStartDate, dateFilterEndDate);
            return data.Count;
        }
        public async Task<int> CountAsync(bool all)
        {
            if (all)
            {
                return await _context.Cargoes.CountAsync();
            }
            else
            {
                int count = 0;
                var cargoes = await _context.Cargoes.ToListAsync();
                var tasks = await _context.Tasks.Where(x => x.Completed == false).ToListAsync();
                foreach (var cargo in cargoes)
                {
                    foreach (var task in tasks)
                    {
                        if (cargo.Task?.Id == task.Id)
                        {
                            count++;
                        }
                    }
                }
                return count;
            }
        }
        public async Task PostAsync(Cargo data)
        {
            var task = await _context.Tasks.FirstOrDefaultAsync(a => a.Id == data.TaskId);
            data.Task = task!;
            _context!.Add(data);
            await _context.SaveChangesAsync();

            if (task != null)
            {
                task.CargoId = data.Id;
                task.Cargo = data;
                _context.Entry(task).State = EntityState.Modified;

                var idsToDelete = await _context.Tasks.Where(d => d.CargoId == data.Id).ToListAsync();
                foreach (var row in idsToDelete)
                {
                    row.CargoId = null;
                    row.Cargo = null;
                    _context.Entry(row).State = EntityState.Modified;
                }
            }
            await _context.SaveChangesAsync();
        }
        public async Task PutAsync(Cargo data)
        {
            var task = _context.Tasks.FirstOrDefault(a => a.Id == data.TaskId);
            data.Task = task!;
            _context.Entry(data).State = EntityState.Modified;

            if (task != null)
            {
                task.CargoId = data.Id;
                task.Cargo = data;
                _context.Entry(task).State = EntityState.Modified;

                var idsToDelete = await _context.Tasks.Where(d => d.CargoId == data.Id).ToListAsync();
                foreach (var row in idsToDelete)
                {
                    row.CargoId = null;
                    row.Cargo = null;
                    _context.Entry(row).State = EntityState.Modified;
                }
            }
            await _context.SaveChangesAsync();
        }
        public async Task<bool> DeleteAsync(int id)
        {
            var data = _context.Cargoes.FirstOrDefault(x => x.Id == id);
            if (data != null)
            {
                _context.Cargoes.Remove(data);
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }
        public string ExportToExcel(CultureInfo lang, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate)
        {
            var cargoes = _context.Cargoes.Where(s => (dateFilterStartDate != null ? (s.Date >= dateFilterStartDate) : true) && (dateFilterEndDate != null ? (s.Date <= dateFilterEndDate) : true));
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Cargoes");
            var currentRow = 1;

            CultureInfo.CurrentUICulture = lang;
            List<string> columnNames = _columnNameLists.GetCargoesColumnNames().Select(x => _localizer[x].Value).ToList();

            for (var i = 0; i < columnNames.Count; i++)
            {
                worksheet.Cell(currentRow, i + 1).Value = columnNames[i];
                worksheet.Cell(currentRow, i + 1).Style.Font.SetBold();
            }

            foreach (var cargo in cargoes)
            {
                currentRow++;
                worksheet.Cell(currentRow, 1).Value = cargo.Id;
                worksheet.Cell(currentRow, 2).Value = cargo.TaskId;
                worksheet.Cell(currentRow, 3).Value = cargo.Weight;
                worksheet.Cell(currentRow, 4).Value = cargo.Description;
                worksheet.Cell(currentRow, 5).Value = cargo.DeliveryRequirements;
                worksheet.Cell(currentRow, 6).Value = cargo.VehicleRegistrationNumber;
                worksheet.Cell(currentRow, 7).Value = cargo.WarehouseId;
                worksheet.Cell(currentRow, 8).Value = cargo.WarehouseSection;
                worksheet.Cell(currentRow, 9).Value = cargo.StorageStartingTime;
                worksheet.Cell(currentRow, 10).Value = cargo.Date;
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            var content = stream.ToArray();
            return Convert.ToBase64String(content);
        }
        public async Task<string> ExportToPdfAsync(CultureInfo lang, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate)
        {
            var cargoes = _context.Cargoes.Where(s => (dateFilterStartDate != null ? (s.Date >= dateFilterStartDate) : true) && (dateFilterEndDate != null ? (s.Date <= dateFilterEndDate) : true));

            int pdfRowIndex = 1;
            Random rnd = new();
            int random = rnd.Next(1000000, 9999999);
            string filename = "Cargoes" + random + "_" + DateTime.Now.ToString("dd-MM-yyyy");
            string filepath = "Files/" + filename + ".pdf";

            Document document = new(PageSize.A4, 5f, 5f, 10f, 10f);
            FileStream fs = new(filepath, FileMode.Create);
            PdfWriter writer = PdfWriter.GetInstance(document, fs);
            document.Open();

            Font font1 = FontFactory.GetFont(FontFactory.TIMES_BOLD, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 12);
            Font font2 = FontFactory.GetFont(FontFactory.TIMES_ROMAN, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 10);

            System.Type type = typeof(CargoDto);
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
            List<string> columnNames = _columnNameLists.GetCargoesColumnNames().Select(x => _localizer[x].Value).ToList();

            var title = new Paragraph(15, _localizer["Cargoes"])
            {
                Alignment = Element.ALIGN_CENTER
            };

            document.Add(title);
            document.Add(new Paragraph("\n"));

            if (cargoes.Any())
            {
                foreach (var name in columnNames.Take(column_number))
                {
                    table.AddCell(new PdfPCell(new Phrase(name, font1))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    });
                }

                foreach (Cargo cargo in cargoes)
                {
                    var s = "";
                    if (!string.IsNullOrEmpty(cargo.Id.ToString())) { s = cargo.Id.ToString(); }
                    else { s = "-"; }
                    table.AddCell(new PdfPCell(new Phrase(s?.ToString(), font2))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    });
                    if (!string.IsNullOrEmpty(cargo.TaskId.ToString())) { s = cargo.TaskId.ToString(); }
                    else { s = "-"; }
                    table.AddCell(new PdfPCell(new Phrase(s?.ToString(), font2))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    });
                    if (!string.IsNullOrEmpty(cargo.Weight.ToString())) { s = cargo.Weight.ToString(); }
                    else { s = "-"; }
                    table.AddCell(new PdfPCell(new Phrase(s?.ToString(), font2))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    });
                    if (!string.IsNullOrEmpty(cargo?.Description?.ToString())) { s = cargo.Description.ToString(); }
                    else { s = "-"; }
                    table.AddCell(new PdfPCell(new Phrase(s?.ToString(), font2))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    });
                    if (!string.IsNullOrEmpty(cargo?.DeliveryRequirements?.ToString())) { s = cargo.DeliveryRequirements.ToString(); }
                    else { s = "-"; }
                    table.AddCell(new PdfPCell(new Phrase(s?.ToString(), font2))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    });
                    pdfRowIndex++;
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
                    if (name == _localizer["Warehouse_id"])
                    {
                        table2.AddCell(new PdfPCell(new Phrase(name + "/" + _localizer["W_section"], font1))
                        {
                            HorizontalAlignment = Element.ALIGN_CENTER,
                            VerticalAlignment = Element.ALIGN_MIDDLE
                        });
                    }
                    else if (name != _localizer["Warehouse_section"] && name != _localizer["Warehouse_id"])
                    {
                        table2.AddCell(new PdfPCell(new Phrase(name, font1))
                        {
                            HorizontalAlignment = Element.ALIGN_CENTER,
                            VerticalAlignment = Element.ALIGN_MIDDLE
                        });
                    }
                }

                table2.HeaderRows = 1;


                foreach (Cargo cargo in cargoes)
                {
                    var s = "";
                    if (!string.IsNullOrEmpty(cargo.Id.ToString())) { s = cargo.Id.ToString(); }
                    else { s = "-"; }
                    table2.AddCell(new PdfPCell(new Phrase(s?.ToString(), font2))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    });
                    if (!string.IsNullOrEmpty(cargo?.VehicleRegistrationNumber?.ToString())) { s = cargo.VehicleRegistrationNumber.ToString(); }
                    else { s = "-"; }
                    table2.AddCell(new PdfPCell(new Phrase(s?.ToString(), font2))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    });
                    if (!string.IsNullOrEmpty(cargo?.WarehouseId.ToString())) { s = cargo.WarehouseId.ToString() + "/" + cargo.WarehouseSection; }
                    else { s = "-"; }
                    table2.AddCell(new PdfPCell(new Phrase(s?.ToString(), font2))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    });
                    if (!string.IsNullOrEmpty(cargo?.StorageStartingTime.ToString())) { s = cargo.StorageStartingTime.ToString(); }
                    else { s = "-"; }
                    table2.AddCell(new PdfPCell(new Phrase(s?.ToString() == "TO" ? _localizer["to"] : _localizer["from"], font2))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    });
                    if (!string.IsNullOrEmpty(cargo?.Date.ToString())) { s = cargo.Date.ToString(); }
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
            var cargoes = _context.Cargoes.Where(s => (dateFilterStartDate != null ? (s.Date >= dateFilterStartDate) : true) && (dateFilterEndDate != null ? (s.Date <= dateFilterEndDate) : true));

            Random rnd = new();
            int random = rnd.Next(1000000, 9999999);
            string filename = "Cargoes" + random + "_" + DateTime.Now.ToString("dd-MM-yyyy");
            string filepath = "Files/" + filename + ".csv";

            StreamWriter txt = new(filepath);

            //copy column names to a list based on language
            CultureInfo.CurrentUICulture = lang;
            List<string> columnNames = _columnNameLists.GetCargoesColumnNames().Select(x => _localizer[x].Value).ToList();

            string separator = isTextDocument ? "\t" : ";";
            string ifNull = isTextDocument ? " --- " : "";

            foreach (var name in columnNames)
            {
                txt.Write(name + (isTextDocument ? "  " : separator));
            }
            txt.Write("\n");

            foreach (var cargo in cargoes)
            {
                txt.Write((cargo.Id + separator));
                txt.Write(cargo.Task?.Id + separator);
                txt.Write((cargo.Weight != null ? cargo.Weight :  ifNull) + separator);
                txt.Write((cargo.Description ?? ifNull) + separator);
                txt.Write((cargo.DeliveryRequirements ?? ifNull) + separator);
                txt.Write((cargo.VehicleRegistrationNumber ?? ifNull) + separator);
                txt.Write((cargo.WarehouseId != null ? cargo.WarehouseId : ifNull) + separator);
                txt.Write((cargo.WarehouseSection ?? ifNull) + separator);
                txt.Write((cargo.StorageStartingTime != null ? cargo.StorageStartingTime : ifNull) + separator);
                txt.Write((cargo.Date + separator));

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
                                List<string> columnNames = _columnNameLists.GetCargoesColumnNames().Select(x => _localizer[x].Value).ToList();

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
                                        var sql = @"Insert Into Cargoes (UserId,TaskId,Weight,Description,DeliveryRequirements,VehicleRegistrationNumber,WarehouseId,WarehouseSection,StorageStartingTime,Date) 
                                            Values (@UserId,@TaskId,@Weight,@Description,@DeliveryRequirements,@VehicleRegistrationNumber,@WarehouseId,@WarehouseSection,@StorageStartingTime,@Date)";
                                        var insert = await _context.Database.ExecuteSqlRawAsync(sql,
                                        new SqlParameter("@UserId", "Imported"),
                                        new SqlParameter("@TaskId", list[l]),
                                        new SqlParameter("@Weight", list[l + 1]),
                                        new SqlParameter("@Description", list[l + 2]),
                                        new SqlParameter("@DeliveryRequirements", list[l + 3]),
                                        new SqlParameter("@VehicleRegistrationNumber", list[l + 4]),
                                        new SqlParameter("@WarehouseId", list[l + 5]),
                                        new SqlParameter("@WarehouseSection", list[l + 6]),
                                        new SqlParameter("@StorageStartingTime", list[l + 7] == System.DBNull.Value || list[l + 7] == null ? System.DBNull.Value : DateTime.Parse(list[l + 7]?.ToString()!)),
                                        new SqlParameter("@Date", DateTime.Now)
                                        );

                                        if (insert > 0)
                                        {
                                            var lastId = await _context.Cargoes.OrderBy(x => x.Id).LastOrDefaultAsync();

                                            if (lastId != null)
                                            {
                                                var WithNewIds = await _context.Cargoes.Where(x => x.TaskId == lastId.TaskId || x.WarehouseId == lastId.WarehouseId || x.VehicleRegistrationNumber == lastId.VehicleRegistrationNumber).ToListAsync();
                                                DeliveryTask? task = await _context.Tasks.FirstOrDefaultAsync(x => x.Id == lastId.TaskId);
                                                Warehouse? warehouse = await _context.Warehouses.FirstOrDefaultAsync(x => x.Id == lastId.WarehouseId);
                                                Truck? truck = await _context.Trucks.FirstOrDefaultAsync(x => x.VehicleRegistrationNumber == lastId.VehicleRegistrationNumber);

                                                foreach (var item in WithNewIds)
                                                {
                                                    if (item != null)
                                                    {
                                                        if (item.Id != lastId?.Id)
                                                        {
                                                            if (item.TaskId == lastId?.TaskId)
                                                            {
                                                                error += "\n" + _localizer["Deleted_wrong_id"] + " " + lastId?.Id + ".";
                                                                _context.Cargoes.Remove(lastId!);
                                                                await _context?.SaveChangesAsync()!;
                                                                return error;
                                                            }
                                                            if (item.VehicleRegistrationNumber == lastId?.VehicleRegistrationNumber)
                                                            {
                                                                item.VehicleRegistrationNumber = null;
                                                            }

                                                            _context.Entry(item).State = EntityState.Modified;

                                                            await _context?.SaveChangesAsync()!;
                                                        }
                                                        else
                                                        {
                                                            if (warehouse == null)
                                                            {
                                                                item.WarehouseId = null;
                                                                item.WarehouseSection = null;
                                                            }
                                                            if (task == null)
                                                            {
                                                                item.TaskId = default;
                                                                error += "\n" + _localizer["Deleted_wrong_id_task"] + " " + lastId?.Id + ".";
                                                                _context.Cargoes.Remove(lastId!);
                                                                await _context?.SaveChangesAsync()!;
                                                                return error;
                                                            }
                                                            if (truck == null)
                                                            {
                                                                item.VehicleRegistrationNumber = null;
                                                            }
                                                            _context.Entry(item).State = EntityState.Modified;
                                                            await _context.SaveChangesAsync();
                                                        }
                                                    }

                                                    if (item != null && item?.TaskId == null)
                                                    {
                                                        error += "\n" + _localizer["Deleted_wrong_id_task"] + " " + lastId?.Id + ".";

                                                        _context.Remove(new Cargo() { Id = item!.Id });
                                                        await _context?.SaveChangesAsync()!;
                                                        return error;
                                                    }
                                                }

                                                if (lastId != null) { 

                                                    var SaveCargoToTask = _context.Tasks.FirstOrDefault(a => a.Id == lastId.TaskId);

                                                    if (SaveCargoToTask != null)
                                                    {
                                                        SaveCargoToTask.CargoId = lastId.Id;
                                                        SaveCargoToTask.Cargo = lastId;
                                                        _context.Entry(SaveCargoToTask).State = EntityState.Modified;
                                                        await _context.SaveChangesAsync();
                                                    }
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
