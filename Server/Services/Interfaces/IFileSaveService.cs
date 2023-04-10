using Cargotruck.Shared.Model.Dto;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace Cargotruck.Server.Services.Interfaces
{
    public interface IFileSaveService
    {
        Task<dynamic> PostFileAsync([FromForm] IEnumerable<IFormFile> files, string id, CultureInfo lang);
    }
}