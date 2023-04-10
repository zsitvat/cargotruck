using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System.Globalization;
using System.Net.Http.Headers;
using RestSharp;
using Cargotruck.Shared.Model.Dto;

namespace Cargotruck.Client.Components
{
    public partial class UploadFiles
    {
        [Parameter]
        public string? Page { get; set; }

        [Parameter]
        public EventCallback StateChanged { get; set; }

        string? error = "";
        private readonly List<File> files = new();
        private List<UploadResult> uploadResults = new();
        private readonly int maxAllowedFiles = 1;
        private async Task OnInputFileChangeAsync(InputFileChangeEventArgs e)
        {
            long maxFileSize = 1024 * 5000;
            var upload = false;
            using var content = new MultipartFormDataContent();
            foreach (var file in e.GetMultipleFiles(maxAllowedFiles))
            {
                if (uploadResults.SingleOrDefault(f => f.FileName == file.Name)is null)
                {
                    try
                    {
                        var fileContent = new StreamContent(file.OpenReadStream(maxFileSize));
                        fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
                        files.Add(new() { Name = file.Name });
                        content.Add(content: fileContent, name: "\"files\"", fileName: file.Name);
                        upload = true;
                    }
                    catch (Exception ex)
                    {
                        error = file.Name + " " + localizer["Not_uploaded"] + " " + ex.Message;
                    }
                }
            }

            if (upload)
            {
                HttpResponseMessage response = await Http.PostAsync($"api/filesave/page?lang={CultureInfo.CurrentCulture.Name}", content);
                var newUploadResults = await response.Content.ReadFromJsonAsync<IList<UploadResult>>();
                if (newUploadResults is not null)
                {
                    uploadResults = uploadResults.Concat(newUploadResults).ToList();
                    foreach (var file in uploadResults)
                    {
                        var datasaved = await Http.PostAsJsonAsync<string?>($"api/{Page}/import?&lang={CultureInfo.CurrentCulture.Name}", file.StoredFileName);
                        error = await datasaved.Content.ReadAsStringAsync();
                        if (datasaved.IsSuccessStatusCode && error == "")
                        {
                            navigationManager.NavigateTo($"/{Page}", true);
                        }
                        else
                        {
                            await OnInitializedAsync();
                            await StateChanged.InvokeAsync();
                        }
                    }
                }
            }
        }

        private class File
        {
            public string? Name { get; set; }
        }
    }
}