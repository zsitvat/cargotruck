using Cargotruck.Server.Data;
using Cargotruck.Server.Services.Interfaces;
using Cargotruck.Shared.Model;
using Cargotruck.Shared.Model.Dto;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cargotruck.Server.Controllers
{

    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class SettingsController : ControllerBase
    {
        private readonly ISettingService _settingService;
        public SettingsController(ISettingService settingService)
        {
            _settingService = settingService;
        }

        [HttpGet]
        public async Task<ActionResult<List<SettingDto>>> GetAsync()
        {
            return Ok(await _settingService.GetAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SettingDto>> GetAsync(int id)
        {
            return Ok(await _settingService.GetAsync(id));
        }

        [HttpGet]
        public async Task<ActionResult<SettingDto>> GetWaitTimeAsync()
        {
            return Ok(await _settingService.GetWaitTimeAsync());
        }

        [HttpPost]
        public async Task PostAsync(SettingDto data)
        {
            await _settingService.PostAsync(data);
        }

        [HttpPut]
        public async Task PutAsync(SettingDto data)
        {
            await _settingService.PostAsync(data);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> DeleteAsync(int id)
        {
            return Ok(await _settingService.DeleteAsync(id));
        }
    }
}
