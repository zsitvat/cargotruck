using Cargotruck.Shared.Models.Request;
using Microsoft.JSInterop;

namespace Cargotruck.Client.Services
{
    public interface IFileDownload
    {
        string? DocumentError { get; set; }

        Task ExportAsync(string page, string documentExtension, DateFilter? dateFilter, HttpClient? client, IJSRuntime? js);
    }
}