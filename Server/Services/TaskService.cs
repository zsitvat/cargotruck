using AutoMapper;
using Cargotruck.Server.Repositories.Interfaces;
using Cargotruck.Server.Services.Interfaces;
using Cargotruck.Shared.Model;
using Cargotruck.Shared.Model.Dto;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace Cargotruck.Server.Services
{
    public class TaskService : ITaskService
    {
        private readonly ITaskRepository _taskRepository;
        private readonly IMapper _mapper;
        public TaskService(ITaskRepository taskRepository, IMapper mapper)
        {
            _taskRepository = taskRepository;
            _mapper = mapper;
        }
        public async Task<List<TasksDto>> GetAsync(int page, int pageSize, string sortOrder, bool desc, string? searchString, string? filter, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate)
        {
            var tasks = await _taskRepository.GetAsync(page, pageSize, sortOrder, desc, searchString, filter, dateFilterStartDate, dateFilterEndDate);
            return _mapper.Map<List<TasksDto>>(tasks);
        }
        public async Task<List<TasksDto>> GetTasksAsync()
        {
            var tasks = await _taskRepository.GetTasksAsync();

            return _mapper.Map<List<TasksDto>>(tasks);
        }
        public async Task<TasksDto?> GetByIdAsync(int id)
        {
            var task = await _taskRepository.GetByIdAsync(id);

            return _mapper.Map<TasksDto>(task);
        }
        public async Task<int[]> GetChartDataAsync()
        {
            return await _taskRepository.GetChartDataAsync();
        }
        public async Task<int> PageCountAsync(string? searchString, string? filter, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate)
        {
            return await _taskRepository.PageCountAsync(searchString, filter, dateFilterStartDate, dateFilterEndDate);
        }
        public async Task<int> CountAsync(bool all)
        {
            return await _taskRepository.CountAsync(all);
        }
        public async Task ChangeCompletionAsync(TasksDto task)
        {
            await _taskRepository.ChangeCompletionAsync(_mapper.Map<Tasks>(task));
        }
        public async Task PostAsync(TasksDto task)
        {
            await _taskRepository.PostAsync(_mapper.Map<Tasks>(task));
        }
        public async Task PutAsync(TasksDto task)
        {
            await _taskRepository.PutAsync(_mapper.Map<Tasks>(task));
        }
        public async Task<bool> DeleteAsync(int id)
        {
            return await _taskRepository.DeleteAsync(id);
        }
        public async Task<string> ExportToCSVAsync(CultureInfo lang, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate, bool isTextDocument)
        {
            return await _taskRepository.ExportToCSVAsync(lang, dateFilterStartDate, dateFilterEndDate, isTextDocument);
        }
        public string ExportToExcel(CultureInfo lang, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate)
        {
            return _taskRepository.ExportToExcel(lang, dateFilterStartDate, dateFilterEndDate);
        }
        public async Task<string> ExportToPdfAsync(CultureInfo lang, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate)
        {
            return await _taskRepository.ExportToPdfAsync(lang, dateFilterStartDate, dateFilterEndDate);
        }
        public async Task<string?> ImportAsync([FromBody] string file, CultureInfo lang)
        {
            return await _taskRepository.ImportAsync(file, lang);
        }
    }
}
