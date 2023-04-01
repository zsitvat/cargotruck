using Cargotruck.Server.Services.Interfaces;
using Cargotruck.Shared.Model.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace Cargotruck.Server.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class TasksController : ControllerBase
    {
        private readonly ITaskService _taskService;

        public TasksController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        [HttpGet]
        public async Task<ActionResult<List<TaskDto>>> GetAsync(int page, int pageSize, string sortOrder, 
            bool desc, string? searchString, string? filter, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate)
        {
            return Ok(await _taskService.GetAsync(page, pageSize, sortOrder, desc, searchString, filter, dateFilterStartDate, dateFilterEndDate));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TaskDto>> GetByIdAsync(int id)
        {
            return Ok(await _taskService.GetByIdAsync(id));
        }

        [HttpGet]
        public async Task<ActionResult<List<TaskDto>>> GetTasksAsync()
        {
            return Ok(await _taskService.GetTasksAsync());
        }

        [HttpGet]
        public async Task<ActionResult<int[]>> GetChartDataAsync()
        {
            return Ok(await _taskService.GetChartDataAsync());
        }

        [HttpGet]
        public async Task<ActionResult<int>> CountAsync(bool all)
        {
            return Ok(await _taskService.CountAsync(all));
        }

        [HttpGet]
        public async Task<ActionResult<int>> PageCountAsync(string? searchString, string? filter, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate)
        {
            return Ok(await _taskService.PageCountAsync(searchString, filter, dateFilterStartDate, dateFilterEndDate));
        }

        [HttpPost]
        public async Task PostAsync(TaskDto task)
        {
            await _taskService.PostAsync(task);
        }

        [HttpPut]
        public async Task PutAsync(TaskDto task)
        {
            await _taskService.PutAsync(task);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> DeleteAsync(int id)
        {
            return Ok(await _taskService.DeleteAsync(id));
        }

        [HttpPut]
        public async Task ChangeCompletionAsync(TaskDto task)
        {
            await _taskService.ChangeCompletionAsync(task);
        }


        //closedXML needed !!!
        [HttpGet]
        public string ExportToExcel(CultureInfo lang, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate)
        {
            return _taskService.ExportToExcel(lang, dateFilterStartDate, dateFilterEndDate);
        }

        //iTextSharp needed !!!
        [HttpGet]
        public async Task<string> ExportToPdfAsync(CultureInfo lang, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate)
        {
            return await _taskService.ExportToPdfAsync(lang, dateFilterStartDate, dateFilterEndDate);
        }

        [HttpGet]
        public async Task<string> ExportToCSVAsync(CultureInfo lang, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate, bool isTextDocument)
        {
            return await _taskService.ExportToCSVAsync(lang, dateFilterStartDate, dateFilterEndDate, isTextDocument);
        }


        [HttpPost]
        public async Task<ActionResult<string?>> ImportAsync([FromBody] string file, CultureInfo lang)
        {
            var result = await _taskService.ImportAsync(file, lang);
            return (result != null ? BadRequest(result) : Ok());
        }
    }
}