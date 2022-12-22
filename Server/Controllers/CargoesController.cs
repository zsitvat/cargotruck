﻿using Cargotruck.Data;
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
    public class CargoesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public CargoesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Get(int page, int pageSize, string sortOrder, bool desc, string? searchString)
        {
            var data = await _context.Cargoes.ToListAsync();
            searchString = searchString == null ? null : searchString.ToLower();

            if (searchString != null && searchString != "")
            {
                    data = data.Where(s =>
                   (s.Task_id.ToString().ToLower()!.Contains(searchString))
                || (s.Weight == null ? false : s.Weight.ToString().ToLower()!.Contains(searchString))
                || (s.Description == null ? false : s.Description.ToLower()!.Contains(searchString))
                || (s.Delivery_requirements == null ? false : s.Delivery_requirements.ToString().ToLower()!.Contains(searchString))
                || (s.Vehicle_registration_number == null ? false : s.Vehicle_registration_number.ToString()!.Contains(searchString))
                || (s.Warehouse_id == null ? false : s.Warehouse_id.ToString()!.Contains(searchString))
                || (s.Warehouse_section == null ? false : s.Warehouse_section.ToLower()!.Contains(searchString))
                || (s.Storage_starting_time == null ? false : s.Storage_starting_time.ToString()!.Contains(searchString))
                || (s.Cost_of_storage == null ? false : s.Cost_of_storage.ToString()!.Contains(searchString))
                || s.Date.ToString()!.Contains(searchString)
                ).ToList();
            }

            sortOrder = sortOrder == "Task_id" ? (desc ? "Task_id_desc" : "Task_id") : (sortOrder);
            sortOrder = sortOrder == "Weight" ? (desc ? "Weight_desc" : "Weight") : (sortOrder);
            sortOrder = sortOrder == "Description" ? (desc ? "Description_desc" : "Description") : (sortOrder);
            sortOrder = sortOrder == "Delivery_requirements" ? (desc ? "Delivery_requirements_desc" : "Delivery_requirements") : (sortOrder);
            sortOrder = sortOrder == "Vehicle_registration_number" ? (desc ? "Vehicle_registration_number_desc" : "Vehicle_registration_number") : (sortOrder);
            sortOrder = sortOrder == "Warehouse_id" ? (desc ? "Warehouse_id_desc" : "Warehouse_id") : (sortOrder);
            sortOrder = sortOrder == "Warehouse_section" ? (desc ? "Warehouse_section_desc" : "Warehouse_section") : (sortOrder);
            sortOrder = sortOrder == "Storage_starting_time" ? (desc ? "Storage_starting_time_desc" : "Storage_starting_time") : (sortOrder);
            sortOrder = sortOrder == "Cost_of_storage" ? (desc ? "Cost_of_storage_desc" : "Cost_of_storage") : (sortOrder);
            sortOrder = sortOrder == "Date" || String.IsNullOrEmpty(sortOrder) ? (desc ? "Date_desc" : "") : (sortOrder);

            switch (sortOrder)
            {
                case "Task_id_desc":
                    data= data.OrderByDescending(s => s.Task_id).ToList();
                    break;
                case "Task_id":
                    data= data.OrderBy(s => s.Task_id).ToList();
                    break;
                case "Weight_desc":
                    data= data.OrderByDescending(s => s.Weight).ToList();
                    break;
                case "Weight":
                    data= data.OrderBy(s => s.Weight).ToList();
                    break;
                case "Description_desc":
                    data= data.OrderByDescending(s => s.Description).ToList();
                    break;
                case "Description":
                    data= data.OrderBy(s => s.Description).ToList();
                    break;
                case "Delivery_requirements_desc":
                    data= data.OrderByDescending(s => s.Delivery_requirements).ToList();
                    break;
                case "Delivery_requirements":
                    data= data.OrderBy(s => s.Delivery_requirements).ToList();
                    break;
                case "Vehicle_registration_number_desc":
                    data= data.OrderByDescending(s => s.Vehicle_registration_number).ToList();
                    break;
                case "Vehicle_registration_number":
                    data= data.OrderBy(s => s.Vehicle_registration_number).ToList();
                    break;
                case "Warehouse_id_desc":
                    data= data.OrderByDescending(s => s.Warehouse_id).ToList();
                    break;
                case "Warehouse_id":
                    data= data.OrderBy(s => s.Warehouse_id).ToList();
                    break;
                case "Warehouse_section_desc":
                    data= data.OrderByDescending(s => s.Warehouse_section).ToList();
                    break;
                case "Warehouse_section":
                    data= data.OrderBy(s => s.Warehouse_section).ToList();
                    break;
                case "Storage_starting_time_desc":
                    data= data.OrderByDescending(s => s.Storage_starting_time).ToList();
                    break;
                case "Storage_starting_time":
                    data= data.OrderBy(s => s.Storage_starting_time).ToList();
                    break;
                case "Cost_of_storage_desc":
                    data= data.OrderByDescending(s => s.Cost_of_storage).ToList();
                    break;
                case "Cost_of_storage":
                    data= data.OrderBy(s => s.Cost_of_storage).ToList();
                    break;
                case "Date_desc":
                    data= data.OrderByDescending(s => s.Date).ToList();
                    break;
                default:
                    data= data.OrderBy(s => s.Date).ToList();
                    break;
            }
            data= data.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            return Ok(data);
        }

        [HttpGet]
        public async Task<IActionResult> GetCargoes()
        {
            var data = await _context.Cargoes.ToListAsync();
            return Ok(data);
        }

        [HttpGet]
        public async Task<IActionResult> PageCount()
        {
            var data= await _context.Cargoes.ToListAsync();
            int PageCount = data.Count();
            return Ok(PageCount);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var data= await _context.Cargoes.FirstOrDefaultAsync(a => a.Id == id);
            return Ok(data);
        }

        [HttpPost]
        public async Task<IActionResult> Post(Cargoes data)
        {
            _context.Add(data);
            await _context.SaveChangesAsync();
            return Ok(data.Id);
        }

        [HttpPut]
        public async Task<IActionResult> Put(Cargoes data)
        {
            _context.Entry(data).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var data = new Cargoes { Id = id };
            _context.Remove(data);
            await _context.SaveChangesAsync();
            return NoContent();
        }


        //closedXML needed !!!
        [HttpGet]
        public async Task<string> Excel(string lang)
        {
            var cargoes = from data in _context.Cargoes select data;
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Cargoes");
                var currentRow = 1;
                worksheet.Cell(currentRow, 1).Value = "Id";
                worksheet.Cell(currentRow, 1).Style.Font.SetBold();
                worksheet.Cell(currentRow, 2).Value = lang == "hu" ? Cargotruck.Shared.Resources.Resource.User_id : "User ID";
                worksheet.Cell(currentRow, 2).Style.Font.SetBold();
                worksheet.Cell(currentRow, 3).Value = lang == "hu" ? Cargotruck.Shared.Resources.Resource.Task_id : "Task ID";
                worksheet.Cell(currentRow, 3).Style.Font.SetBold();
                worksheet.Cell(currentRow, 4).Value = lang == "hu" ? Cargotruck.Shared.Resources.Resource.Weight : "Cargo ID";
                worksheet.Cell(currentRow, 4).Style.Font.SetBold();
                worksheet.Cell(currentRow, 5).Value = lang == "hu" ? Cargotruck.Shared.Resources.Resource.Description : "Description";
                worksheet.Cell(currentRow, 5).Style.Font.SetBold();
                worksheet.Cell(currentRow, 6).Value = lang == "hu" ? Cargotruck.Shared.Resources.Resource.Delivery_requirements : "Delivery_requirements";
                worksheet.Cell(currentRow, 6).Style.Font.SetBold();
                worksheet.Cell(currentRow, 7).Value = lang == "hu" ? Cargotruck.Shared.Resources.Resource.Vehicle_registration_number : "Vehicle_registration_number";
                worksheet.Cell(currentRow, 7).Style.Font.SetBold();
                worksheet.Cell(currentRow, 8).Value = lang == "hu" ? Cargotruck.Shared.Resources.Resource.Warehouse_id : "Warehouse_id";
                worksheet.Cell(currentRow, 8).Style.Font.SetBold();
                worksheet.Cell(currentRow, 9).Value = lang == "hu" ? Cargotruck.Shared.Resources.Resource.Warehouse_section : "Warehouse_section";
                worksheet.Cell(currentRow, 9).Style.Font.SetBold();
                worksheet.Cell(currentRow, 10).Value = lang == "hu" ? Cargotruck.Shared.Resources.Resource.Storage_starting_time : "Storage_starting_time";
                worksheet.Cell(currentRow, 10).Style.Font.SetBold();
                worksheet.Cell(currentRow, 11).Value = lang == "hu" ? Cargotruck.Shared.Resources.Resource.Cost_of_storage : "Cost_of_storage";
                worksheet.Cell(currentRow, 11).Style.Font.SetBold();
                worksheet.Cell(currentRow, 12).Value = lang == "hu" ? Cargotruck.Shared.Resources.Resource.Date : "Date";
                worksheet.Cell(currentRow, 12).Style.Font.SetBold();

                foreach (var cargo in cargoes)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = cargo.Id;
                    worksheet.Cell(currentRow, 2).Value = cargo.User_id;
                    worksheet.Cell(currentRow, 3).Value = cargo.Task_id;
                    worksheet.Cell(currentRow, 4).Value = cargo.Weight;
                    worksheet.Cell(currentRow, 5).Value = cargo.Description;
                    worksheet.Cell(currentRow, 6).Value = cargo.Delivery_requirements;
                    worksheet.Cell(currentRow, 7).Value = cargo.Vehicle_registration_number;
                    worksheet.Cell(currentRow, 8).Value = cargo.Warehouse_id;
                    worksheet.Cell(currentRow, 9).Value = cargo.Warehouse_section;
                    worksheet.Cell(currentRow, 10).Value = cargo.Storage_starting_time;
                    worksheet.Cell(currentRow, 11).Value = cargo.Cost_of_storage;
                    worksheet.Cell(currentRow, 12).Value = cargo.Date;
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
            var cargoes = from data in _context.Cargoes select data;

            int pdfRowIndex = 1;
            Random rnd = new Random();
            int random = rnd.Next(1000000, 9999999);
            string filename = "Cargoes" + random + "_" + DateTime.Now.ToString("dd-MM-yyyy");
            string filepath = "Files/" + filename + ".pdf";

            Document document = new Document(PageSize.A4, 5f, 5f, 10f, 10f);
            FileStream fs = new FileStream(filepath, FileMode.Create);
            PdfWriter writer = PdfWriter.GetInstance(document, fs);
            document.Open();

            Font font1 = FontFactory.GetFont(FontFactory.TIMES_BOLD, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 12);
            Font font2 = FontFactory.GetFont(FontFactory.TIMES_ROMAN, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 10);

            Type type = typeof(Cargoes);
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

            var title = new Paragraph(15, lang == "hu" ? Cargotruck.Shared.Resources.Resource.Cargoes : "Cargoes");
            title.Alignment = Element.ALIGN_CENTER;


            document.Add(title);
            document.Add(new Paragraph("\n"));

            if (cargoes.Count() > 0)
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
                table.AddCell(new PdfPCell(new Phrase(lang == "hu" ? Cargotruck.Shared.Resources.Resource.Weight : "Cargo ID", font1))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                });
                table.AddCell(new PdfPCell(new Phrase(lang == "hu" ? Cargotruck.Shared.Resources.Resource.Description : "Purpose of the trip", font1))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                });
                table.AddCell(new PdfPCell(new Phrase(lang == "hu" ? Cargotruck.Shared.Resources.Resource.Delivery_requirements : "Starting date", font1))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                });
                table.AddCell(new PdfPCell(new Phrase(lang == "hu" ? Cargotruck.Shared.Resources.Resource.Vehicle_registration_number : "Ending date", font1))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                });

                foreach (Cargoes cargo in cargoes)
                {
                    var s = "";
                    if (!string.IsNullOrEmpty(cargo.Id.ToString())) { s = cargo.Id.ToString(); }
                    else { s = "-"; }
                    table.AddCell(new PdfPCell(new Phrase(s.ToString(), font2))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    });
                    if (!string.IsNullOrEmpty(cargo.Task_id.ToString())) { s = cargo.Task_id.ToString(); }
                    else { s = "-"; }
                    table.AddCell(new PdfPCell(new Phrase(s.ToString(), font2))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    });
                    if (!string.IsNullOrEmpty(cargo.Weight.ToString())) { s = cargo.Weight.ToString(); }
                    else { s = "-"; }
                    table.AddCell(new PdfPCell(new Phrase(s.ToString(), font2))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    });
                    if (!string.IsNullOrEmpty(cargo.Description.ToString())) { s = cargo.Description.ToString(); }
                    else { s = "-"; }
                    table.AddCell(new PdfPCell(new Phrase(s.ToString(), font2))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    });
                    if (!string.IsNullOrEmpty(cargo.Delivery_requirements.ToString())) { s = cargo.Delivery_requirements.ToString(); }
                    else { s = "-"; }
                    table.AddCell(new PdfPCell(new Phrase(s.ToString(), font2))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    });
                    if (!string.IsNullOrEmpty(cargo.Vehicle_registration_number)) { s = cargo.Vehicle_registration_number.ToString(); }
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
                table2.AddCell(new PdfPCell(new Phrase(lang == "hu" ? Cargotruck.Shared.Resources.Resource.Warehouse_id : "Starting place", font1))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                });
                table2.AddCell(new PdfPCell(new Phrase(lang == "hu" ? Cargotruck.Shared.Resources.Resource.Warehouse_section : "Ending place", font1))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                });
                table2.AddCell(new PdfPCell(new Phrase(lang == "hu" ? Cargotruck.Shared.Resources.Resource.Storage_starting_time : "Storage_starting_time", font1))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                });
                table2.AddCell(new PdfPCell(new Phrase(lang == "hu" ? Cargotruck.Shared.Resources.Resource.Cost_of_storage : "Expenses ID", font1))
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


                foreach (Cargoes cargo in cargoes)
                {
                    var s = "";
                    if (!string.IsNullOrEmpty(cargo.Id.ToString())) { s = cargo.Id.ToString(); }
                    else { s = "-"; }
                    table2.AddCell(new PdfPCell(new Phrase(s.ToString(), font2))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    });
                    if (!string.IsNullOrEmpty(cargo.Warehouse_id.ToString())) { s = cargo.Warehouse_id.ToString(); }
                    else { s = "-"; }
                    table2.AddCell(new PdfPCell(new Phrase(s.ToString(), font2))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    });
                    if (!string.IsNullOrEmpty(cargo.Warehouse_section)) { s = cargo.Warehouse_section.ToString(); }
                    else { s = "-"; }
                    table2.AddCell(new PdfPCell(new Phrase(s.ToString(), font2))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    });
                    if (!string.IsNullOrEmpty(cargo.Storage_starting_time.ToString())) { s = cargo.Storage_starting_time.ToString(); }
                    else { s = "-"; }
                    table2.AddCell(new PdfPCell(new Phrase(s.ToString() == "TO" ? lang == "hu" ? Cargotruck.Shared.Resources.Resource.to : "Go to the direction" : lang == "hu" ? Cargotruck.Shared.Resources.Resource.from : "Go from the direction", font2))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    });
                    if (!string.IsNullOrEmpty(cargo.Cost_of_storage.ToString())) { s = cargo.Cost_of_storage.ToString(); }
                    else { s = "-"; }
                    table2.AddCell(new PdfPCell(new Phrase(s.ToString(), font2))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    });
                    if (!string.IsNullOrEmpty(cargo.Date.ToString())) { s = cargo.Date.ToString(); }
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
            var cargoes = from data in _context.Cargoes select data;

            Random rnd = new Random();
            int random = rnd.Next(1000000, 9999999);
            string filename = "Cargoes" + random + "_" + DateTime.Now.ToString("dd-MM-yyyy");
            string filepath = "Files/" + filename + ".csv";

            StreamWriter txt = new StreamWriter(filepath);
            txt.Write("Id" + ";");
            txt.Write((lang == "hu" ? Cargotruck.Shared.Resources.Resource.User_id : "User ID") + ";");
            txt.Write((lang == "hu" ? Cargotruck.Shared.Resources.Resource.Task_id : "Task ID") + ";");
            txt.Write((lang == "hu" ? Cargotruck.Shared.Resources.Resource.Weight : "Cargo ID") + ";");
            txt.Write((lang == "hu" ? Cargotruck.Shared.Resources.Resource.Description : "Purpose of the trip") + ";");
            txt.Write((lang == "hu" ? Cargotruck.Shared.Resources.Resource.Delivery_requirements : "Starting date") + ";");
            txt.Write((lang == "hu" ? Cargotruck.Shared.Resources.Resource.Vehicle_registration_number : "Vehicle_registration_number") + ";");
            txt.Write((lang == "hu" ? Cargotruck.Shared.Resources.Resource.Warehouse_id : "Starting place") + ";");
            txt.Write((lang == "hu" ? Cargotruck.Shared.Resources.Resource.Warehouse_section : "Warehouse_section") + ";");
            txt.Write((lang == "hu" ? Cargotruck.Shared.Resources.Resource.Storage_starting_time : "Storage_starting_time") + ";");
            txt.Write((lang == "hu" ? Cargotruck.Shared.Resources.Resource.Cost_of_storage : "Cost_of_storage") + ";");
            txt.Write((lang == "hu" ? Cargotruck.Shared.Resources.Resource.Date : "Date") + ";");
            txt.Write("\n");

            foreach (var cargo in cargoes)
            {
                txt.Write(cargo.Id + ";");
                txt.Write(cargo.User_id + ";");
                txt.Write(cargo.Task_id + ";");
                txt.Write(cargo.Weight + ";");
                txt.Write(cargo.Description + ";");
                txt.Write(cargo.Delivery_requirements + ";");
                txt.Write(cargo.Vehicle_registration_number + ";");
                txt.Write(cargo.Warehouse_id + ";");
                txt.Write(cargo.Warehouse_section + ";");
                txt.Write(cargo.Storage_starting_time + ";");
                txt.Write(cargo.Cost_of_storage + ";");
                txt.Write(cargo.Date + ";");
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
                                lang == "hu" ? Cargotruck.Shared.Resources.Resource.Weight : "Cargo ID",
                                lang == "hu" ? Cargotruck.Shared.Resources.Resource.Description : "Purpose of the trip",
                                lang == "hu" ? Cargotruck.Shared.Resources.Resource.Delivery_requirements : "Starting date",
                                lang == "hu" ? Cargotruck.Shared.Resources.Resource.Vehicle_registration_number : "Vehicle_registration_number",
                                lang == "hu" ? Cargotruck.Shared.Resources.Resource.Warehouse_id : "Starting place",
                                lang == "hu" ? Cargotruck.Shared.Resources.Resource.Warehouse_section : "Warehouse_section",
                                lang == "hu" ? Cargotruck.Shared.Resources.Resource.Storage_starting_time : "Storage_starting_time",
                                lang == "hu" ? Cargotruck.Shared.Resources.Resource.Cost_of_storage : "Cost_of_storage",
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
                                var sql = @"Insert Into Cargoes (User_id,Task_id,Weight,Description,Delivery_requirements,Vehicle_registration_number,Warehouse_id,Warehouse_section,Storage_starting_time,Cost_of_storage,Date) 
                                Values (@User_id,@Task_id,@Weight,@Description,@Delivery_requirements,@Vehicle_registration_number,@Warehouse_id,@Warehouse_section,@Storage_starting_time,@Cost_of_storage,@Date)";
                                var insert = await _context.Database.ExecuteSqlRawAsync(sql,
                                    new SqlParameter("@User_id", list[l]),
                                    new SqlParameter("@Task_id", list[l + 1]),
                                    new SqlParameter("@Weight", list[l + 2]),
                                    new SqlParameter("@Description", list[l + 3]),
                                    new SqlParameter("@Delivery_requirements", list[l + 4]),
                                    new SqlParameter("@Vehicle_registration_number", list[l + 5]),
                                    new SqlParameter("@Warehouse_id", list[l + 6]),
                                    new SqlParameter("@Warehouse_section", list[l + 7]),
                                    new SqlParameter("@Storage_starting_time", list[l + 8] == System.DBNull.Value ? System.DBNull.Value : DateTime.Parse(list[l + 8].ToString())),
                                    new SqlParameter("@Cost_of_storage", list[l + 9]),
                                    new SqlParameter("@Date", list[l + 10] == System.DBNull.Value ? System.DBNull.Value : DateTime.Parse(list[l + 10].ToString()))
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