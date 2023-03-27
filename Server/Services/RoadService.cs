using AutoMapper;
using Cargotruck.Server.Repositories;
using Cargotruck.Shared.Model;
using Cargotruck.Shared.Model.Dto;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace Cargotruck.Server.Services
{
    public class RoadService: IRoadService
    {

        private readonly IRoadRepository _roadRepository;
        private readonly IMapper _mapper;
        public RoadService(IRoadRepository roadRepository, IMapper mapper)
        {
            _roadRepository = roadRepository;
            _mapper = mapper;
        }
        public async Task<List<RoadsDto>> GetAsync(int page, int pageSize, string sortOrder, bool desc, string? searchString, string? filter, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate)
        {
            var roads = await _roadRepository.GetAsync(page, pageSize, sortOrder, desc, searchString, filter, dateFilterStartDate, dateFilterEndDate);
            return _mapper.Map<List<RoadsDto>>(roads);
        }
        public async Task<List<RoadsDto>> GetRoadsAsync()
        {
            var roads = await _roadRepository.GetRoadsAsync();

            return _mapper.Map<List<RoadsDto>>(roads);
        }
        public async Task<RoadsDto?> GetByIdAsync(int id)
        {
            var road = await _roadRepository.GetByIdAsync(id);

            return _mapper.Map<RoadsDto>(road);
        }
        public async Task<int[]> GetChartDataAsync()
        {
            var chartdata = await _roadRepository.GetChartDataAsync();

            return chartdata;
        }
        public async Task<int> PageCountAsync(string? searchString, string? filter, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate)
        {
            var count = await _roadRepository.PageCountAsync(searchString, filter, dateFilterStartDate, dateFilterEndDate);

            return count;
        }
        public async Task<int> CountAsync()
        {
            var count = await _roadRepository.CountAsync();

            return count;
        }
        public async Task PostAsync(RoadsDto road)
        {
            await _roadRepository.PostAsync(_mapper.Map<Roads>(road));
        }
        public async Task PutAsync(RoadsDto road)
        {
            await _roadRepository.PutAsync(_mapper.Map<Roads>(road));
        }
        public async Task<bool> DeleteAsync(int id)
        {
            return await _roadRepository.DeleteAsync(id);
        }
        public async Task<string> ExportToCSVAsync(CultureInfo lang, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate, bool isTextDocument)
        {
            return await _roadRepository.ExportToCSVAsync(lang, dateFilterStartDate, dateFilterEndDate, isTextDocument);
        }
        public string ExportToExcel(CultureInfo lang, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate)
        {
            return _roadRepository.ExportToExcel(lang, dateFilterStartDate, dateFilterEndDate);
        }
        public async Task<string> ExportToPdfAsync(CultureInfo lang, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate)
        {
            return await _roadRepository.ExportToPdfAsync(lang, dateFilterStartDate, dateFilterEndDate);
        }
        public async Task<string?> ImportAsync([FromBody] string file, CultureInfo lang)
        {
            return await _roadRepository.ImportAsync(file, lang);
        }
    }
}
