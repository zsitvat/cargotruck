using Cargotruck.Shared.Models.Request;
using Cargotruck.Shared.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using System.Globalization;

namespace Cargotruck.Client.Services
{
    public class FileDownload
    {
        [Inject] public static IStringLocalizer<Resource>? Localizer { get; set; }
        public static string? DocumentError { get; set; } = null;

        public static async Task Export(string page, string documentExtension, DateFilter? dateFilter, HttpClient? client, IJSRuntime? js)
        {
            //get base64 string from web api call
            string action;
            switch (documentExtension)
            {
                case "xlsx":
                    action = "excel";
                    break;
                case "txt":
                    action = "csv";
                    break;
                case "csv":
                    action = "csv";
                    break;
                case "pdf":
                    action = "pdf";
                    break;
                default:
                    return;
            }
            var Response = await client!.GetAsync($"api/{page.ToLower()}/{action}?lang={CultureInfo.CurrentCulture.Name}&dateFilterStartDate={dateFilter?.StartDate}&dateFilterEndDate={dateFilter?.EndDate}");

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
                DocumentError = Localizer["Document_failed_to_create"];
            }
        }
    }
}
