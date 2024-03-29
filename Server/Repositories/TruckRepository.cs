﻿using Cargotruck.Server.Data;
using Cargotruck.Server.Repositories.Interfaces;
using Cargotruck.Server.Services.Interfaces;
using Cargotruck.Shared.Model;
using Cargotruck.Shared.Resources;
using ClosedXML.Excel;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System.Data;
using System.Globalization;
using System.Text;

namespace Cargotruck.Server.Repositories
{
    public class TruckRepository : ITruckRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IStringLocalizer<Resource> _localizer;
        private readonly IColumnNamesService _columnNameLists;
        private readonly IErrorHandlerService _errorHandler;
        public TruckRepository(ApplicationDbContext context, IStringLocalizer<Resource> localizer, IColumnNamesService columnNameLists, IErrorHandlerService errorHandler)
        {
            _context = context;
            _localizer = localizer;
            _columnNameLists = columnNameLists;
            _errorHandler = errorHandler;
        }

        //this method gets the data from db and filter it
        private async Task<List<Truck>> GetDataAsync(string? searchString, Status? filter, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate)
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
               (s.VehicleRegistrationNumber!.ToString().ToLower().Contains(searchString))
            || (s.Brand != null && s.Brand.ToString().ToLower()!.Contains(searchString))
            || (s.Status.ToString()!.Contains(searchString))
            || (s.RoadId != null && s.RoadId.ToString()!.ToLower().Contains(searchString))
            || (s.MaxWeight != null && s.MaxWeight.ToString()!.Contains(searchString))
            ).ToList();
            }

            return data;
        }

        public async Task<List<Truck>> GetAsync(int page, int pageSize, string sortOrder, bool desc, string? searchString, Status? filter, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate)
        {
            var data = await GetDataAsync(searchString, filter, dateFilterStartDate, dateFilterEndDate);

            sortOrder = sortOrder == "VehicleRegistrationNumber" ? (desc ? "VehicleRegistrationNumber_desc" : "VehicleRegistrationNumber") : (sortOrder);
            sortOrder = sortOrder == "Brand" ? (desc ? "Brand_desc" : "Brand") : (sortOrder);
            sortOrder = sortOrder == "Status" ? (desc ? "Status_desc" : "Status") : (sortOrder);
            sortOrder = sortOrder == "RoadId" ? (desc ? "RoadId_desc" : "RoadId") : (sortOrder);
            sortOrder = sortOrder == "MaxWeight" ? (desc ? "MaxWeight_desc" : "MaxWeight") : (sortOrder);
            sortOrder = sortOrder == "Date" || String.IsNullOrEmpty(sortOrder) ? (desc ? "Date_desc" : "") : (sortOrder);

            data = sortOrder switch
            {
                "VehicleRegistrationNumber_desc" => data.OrderByDescending(s => s.VehicleRegistrationNumber).ToList(),
                "VehicleRegistrationNumber" => data.OrderBy(s => s.VehicleRegistrationNumber).ToList(),
                "Brand_desc" => data.OrderByDescending(s => s.Brand).ToList(),
                "Brand" => data.OrderBy(s => s.Brand).ToList(),
                "Status_desc" => data.OrderByDescending(s => s.Status).ToList(),
                "Status" => data.OrderBy(s => s.Status).ToList(),
                "RoadId_desc" => data.OrderByDescending(s => s.RoadId).ToList(),
                "RoadId" => data.OrderBy(s => s.RoadId).ToList(),
                "MaxWeight_desc" => data.OrderByDescending(s => s.MaxWeight).ToList(),
                "MaxWeight" => data.OrderBy(s => s.MaxWeight).ToList(),
                "Date_desc" => data.OrderByDescending(s => s.Date).ToList(),
                _ => data.OrderBy(s => s.Date).ToList(),
            };

            return data.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        }

        public async Task<Truck?> GetByIdAsync(int id)
        {
            return await _context.Trucks.FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<Truck?> GetByVRNAsync(string vehicle_registration_number)
        {
            return await _context.Trucks.FirstOrDefaultAsync(a => a.VehicleRegistrationNumber == vehicle_registration_number);
        }

        public async Task<List<Truck>> GetTrucksAsync()
        {
            return await _context.Trucks.ToListAsync();
        }

        public async Task<int> CountAsync(bool all)
        {
            if (all)
            {
                return await _context.Trucks.CountAsync();
            }
            else
            {
                return await _context.Trucks.Where(x => x.Status != Status.garage).CountAsync();
            }
        }

        public async Task<int> PageCountAsync(string? searchString, Status? filter, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate)
        {
            var data = await GetDataAsync(searchString, filter, dateFilterStartDate, dateFilterEndDate);
            return data.Count;
        }

        public async Task PostAsync(Truck data)
        {
            _context.Add(data);

            var road = _context.Roads.FirstOrDefault(a => a.Id == data.RoadId);
            if (road != null)
            {
                road.VehicleRegistrationNumber = data.VehicleRegistrationNumber;
                _context.Entry(road).State = EntityState.Modified;

            }

            await _context.SaveChangesAsync();
        }

        public async Task PutAsync(Truck data)
        {
            _context.Entry(data).State = EntityState.Modified;

            var road = _context.Roads.FirstOrDefault(a => a.Id == data.RoadId);
            if (road != null)
            {
                road.VehicleRegistrationNumber = data.VehicleRegistrationNumber;
                _context.Entry(road).State = EntityState.Modified;

            }


            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var data = _context.Trucks.FirstOrDefault(x => x.Id == id);
            if (data != null)
            {
                _context.Trucks.Remove(data);
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }

        public string ExportToExcel(CultureInfo lang, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate)
        {
            var trucks = _context.Trucks.Where(s => (dateFilterStartDate != null ? (s.Date >= dateFilterStartDate) : true) && (dateFilterEndDate != null ? (s.Date <= dateFilterEndDate) : true));

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Trucks");
            var currentRow = 1;


            CultureInfo.CurrentUICulture = lang;
            List<string> columnNames = _columnNameLists.GetTrucksColumnNames().Select(x => _localizer[x].Value).ToList();

            for (var i = 0; i < columnNames.Count; i++)
            {
                worksheet.Cell(currentRow, i + 1).Value = columnNames[i];
                worksheet.Cell(currentRow, i + 1).Style.Font.SetBold();
            }

            foreach (var truck in trucks)
            {
                currentRow++;
                worksheet.Cell(currentRow, 1).Value = truck.Id;
                worksheet.Cell(currentRow, 2).Value = truck.VehicleRegistrationNumber;
                worksheet.Cell(currentRow, 3).Value = truck.Brand;
                worksheet.Cell(currentRow, 4).Value = truck.Status;
                worksheet.Cell(currentRow, 5).Value = truck.RoadId;
                worksheet.Cell(currentRow, 6).Value = truck.MaxWeight;
                worksheet.Cell(currentRow, 7).Value = truck.Date;
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            var content = stream.ToArray();

            return Convert.ToBase64String(content);
        }

        public async Task<string> ExportToPdfAsync(CultureInfo lang, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate)
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

            System.Type type = typeof(Truck);
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

            //copy column names to a list based on language
            CultureInfo.CurrentUICulture = lang;
            List<string> columnNames = _columnNameLists.GetTrucksColumnNames().Select(x => _localizer[x].Value).ToList();

            var title = new Paragraph(15, _localizer["Trucks"].Value)
            {
                Alignment = Element.ALIGN_CENTER
            };

            document.Add(title);
            document.Add(new Paragraph("\n"));

            if (trucks.Any())
            {
                foreach (var name in columnNames.Take(column_number))
                {
                    table.AddCell(new PdfPCell(new Phrase(name, font1))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    });
                }

                foreach (Truck truck in trucks)
                {
                    var s = "";
                    if (!string.IsNullOrEmpty(truck.Id.ToString())) { s = truck.Id.ToString(); }
                    else { s = "-"; }
                    table.AddCell(new PdfPCell(new Phrase(s.ToString(), font2))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    });

                    if (!string.IsNullOrEmpty(truck.VehicleRegistrationNumber?.ToString())) { s = truck.VehicleRegistrationNumber.ToString(); }

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
                    if (!string.IsNullOrEmpty(truck.RoadId?.ToString())) { s = truck.RoadId.ToString(); }
                    else { s = "-"; }
                    table.AddCell(new PdfPCell(new Phrase(s?.ToString(), font2))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    });

                    if (!string.IsNullOrEmpty(truck.MaxWeight?.ToString())) { s = truck.MaxWeight.ToString(); }

                    else { s = "-"; }
                    table.AddCell(new PdfPCell(new Phrase(s?.ToString(), font2))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    });
                    if (!string.IsNullOrEmpty(truck.Date.ToString())) { s = truck.Date.ToString(); }
                    else { s = "-"; }
                    table.AddCell(new PdfPCell(new Phrase(s?.ToString(), font2))
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

            byte[] fileBytes = await File.ReadAllBytesAsync(filepath);
            string base64String = Convert.ToBase64String(fileBytes);
            File.Delete(filepath);

            return base64String;
        }

        public async Task<string> ExportToCSVAsync(CultureInfo lang, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate, bool isTextDocument)
        {
            var trucks = _context.Trucks.Where(s => (dateFilterStartDate != null ? (s.Date >= dateFilterStartDate) : true) && (dateFilterEndDate != null ? (s.Date <= dateFilterEndDate) : true));

            Random rnd = new();
            int random = rnd.Next(1000000, 9999999);
            string filename = "Trucks" + random + "_" + DateTime.Now.ToString("dd-MM-yyyy");
            string filepath = "Files/" + filename + ".csv";

            StreamWriter txt = new(filepath);

            //copy column names to a list based on language
            CultureInfo.CurrentUICulture = lang;
            List<string> columnNames = _columnNameLists.GetTrucksColumnNames().Select(x => _localizer[x].Value).ToList();

            string separator = isTextDocument ? "\t" : ";";
            string ifNull = isTextDocument ? " --- " : "";

            foreach (var name in columnNames)
            {
                 txt.Write(name + (isTextDocument ? "  " : separator));
            }

            txt.Write("\n");

            foreach (var truck in trucks)
            {
                txt.Write(truck.Id + separator);
                txt.Write((truck.VehicleRegistrationNumber ?? ifNull) + separator);
                txt.Write((truck.Brand ?? ifNull) + separator);
                txt.Write((truck.Status + separator));
                txt.Write((truck.RoadId != null ? truck.RoadId : ifNull) + separator);
                txt.Write((truck.MaxWeight != null ? truck.MaxWeight : ifNull) + separator);
                txt.Write(truck.Date + separator);
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
                        int rowNumber = 0;
                        foreach (IXLRow row in worksheet.Rows())
                        {
                            //Use the first row to add columns to DataTable with column names check.
                            if (firstRow)
                            {
                                //copy column names to a list
                                CultureInfo.CurrentUICulture = lang;
                                List<string> columnNames = _columnNameLists.GetTrucksColumnNames().Select(x => _localizer[x].Value).ToList();

                                var cellValues = row.Cells().Select(c => c.Value.ToString()).ToList();
                                if (!columnNames.Where(cname => cname != _localizer["Id"]).SequenceEqual(cellValues.Where(cname => cname != _localizer["Id"])))
                                {
                                    error = _localizer["Not_match_col"].Value;
                                    File.Delete(path); // delete the file
                                    return error;
                                }
                                dt.Columns.AddRange(cellValues.Select(cname => new DataColumn(cname)).ToArray());
                                columnNames.RemoveAll(cname => cellValues.Contains(cname));

                                firstRow = false;
                                if (columnNames.Count == 0 || (columnNames.Count == 1 && columnNames.Contains(_localizer["Id"])))
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

                                        list[l + 2] = list[l + 2] switch
                                        {
                                            "delivering" => "0",
                                            "on_road" => "1",
                                            "garage" => "2",
                                            "under_repair" => "3",
                                            "loaned" => "4",
                                            "rented" => "5",
                                            _ => throw new ArgumentNullException(_localizer["Error_type"])
                                        };

                                        ++rowNumber;

                                        var truck = new Truck
                                        {
                                            UserId = "Imported",
                                            VehicleRegistrationNumber = list[l],
                                            Brand = list[l + 1],
                                            Status = !string.IsNullOrWhiteSpace(list[l + 2]) ? (Status)Enum.Parse(typeof(Status), list[l + 2]!) : Status.on_road,
                                            RoadId = !string.IsNullOrWhiteSpace(list[l + 3]) ? int.Parse(list[l + 3] ?? "0") : null,
                                            MaxWeight = !string.IsNullOrWhiteSpace(list[l + 4]) ? int.Parse(list[l + 4] ?? "0") : null,
                                            Date = DateTime.Now
                                        };
   
                                        if (truck != null)
                                        {
                                            bool saveable = true;
                                            Road? road = await _context.Roads.FirstOrDefaultAsync(x => x.VehicleRegistrationNumber == truck.VehicleRegistrationNumber);

                                            if (saveable && truck?.RoadId != null)
                                            {
                                                var withNewId = await _context.Trucks.Where(x => x.RoadId == truck.RoadId).ToListAsync();

                                                if (withNewId.Count != 0)
                                                {
                                                    saveable = false;
                                                    error += "\n " + _localizer["Deleted_duplicate_id"] + " " + rowNumber + ".";
                                                }
                                                else if (road == null)
                                                {
                                                    saveable = false;
                                                    error += "\n " + _localizer["Deleted_wrong_id"] + " " + rowNumber + ".";
                                                }
                                            }

                                            if (saveable)
                                            {
                                                if (truck?.RoadId != null && road != null)
                                                {
                                                    road.VehicleRegistrationNumber = truck.VehicleRegistrationNumber;
                                                    _context.Entry(road).State = EntityState.Modified;
                                                }

                                                await _context.Trucks.AddAsync(truck!);
                                                await _context.SaveChangesAsync();
                                            }
                                        }
                                        else if (truck == null)
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