using Cargotruck.Server.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace Cargotruck.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class FilesaveController : ControllerBase
    {
        private readonly IFileSaveService _fileSaveService;

        public FilesaveController(IFileSaveService fileSaveService)
        {
            _fileSaveService = fileSaveService;
        }

        [HttpPost("{id}"), HttpPost]
        public async Task<dynamic> PostFileAsync([FromForm] IEnumerable<IFormFile> files, string id, CultureInfo lang)
        {
            return await _fileSaveService.PostFileAsync(files, id, lang);
        }
    }
}