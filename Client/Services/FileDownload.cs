using Cargotruck.Client.Services.Interfaces;
using Cargotruck.Shared.Model.Dto;
using Cargotruck.Shared.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using System.Globalization;

namespace Cargotruck.Client.Services
{
    public class FileDownload : IFileDownload
    {
        [Inject] public static IStringLocalizer<Resource>? Localizer { get; set; }
        public string? DocumentError { get; set; } = null;

        public async Task ExportAsync(string page, string documentExtension, DateFilter? dateFilter, HttpClient? client, IJSRuntime? js)
        {
            //get base64 string from web api call
            string action = documentExtension switch
            {
                "xlsx" => "exporttoexcel",
                "txt" => "exporttocsv",
                "csv" => "exporttocsv",
                "pdf" => "exporttopdf",
                _ => throw new ArgumentException(Localizer!["Invalid_document"])
            };

        var Response = await client!.GetAsync($"api/{page.ToLower()}/{action}?lang={CultureInfo.CurrentCulture.Name}&dateFilterStartDate={dateFilter?.StartDate}&dateFilterEndDate={dateFilter?.EndDate}&isTextDocument={(documentExtension == "txt" ? true : false)}");

            if (Response.IsSuccessStatusCode)
            {
                var base64String = await Response.Content.ReadAsStringAsync();

                Random rnd = new();
                int random = rnd.Next(1000000, 9999999);
                string filename = page + random + "_" + DateTime.Now.ToString("dd-MM-yyyy") + "." + documentExtension;

                if (documentExtension == "xlsx")
                {
                    //call javascript function to download the file
                    await js!.InvokeVoidAsync(
                        "downloadFile",
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        base64String,
                        filename);
                }
                else
                {
                    //call javascript function to download the file
                    await js!.InvokeVoidAsync(
                        "downloadFile",
                        "application/" + documentExtension + ";charset=utf-8",
                        base64String,
                        filename);
                }
            }
            else
            {
                DocumentError = Localizer!["Document_failed_to_create"];
            }
        }
    }
}
