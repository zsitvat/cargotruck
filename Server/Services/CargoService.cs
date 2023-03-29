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
    public class CargoService : ICargoService
    {
        private readonly ICargoRepository _cargoRepository;
        private readonly IMapper _mapper;
        public CargoService(ICargoRepository cargoRepository, IMapper mapper)
        {
            _cargoRepository = cargoRepository;
            _mapper = mapper;
        }
        public async Task<List<CargoesDto>> GetAsync(int page, int pageSize, string sortOrder, bool desc, string? searchString, string? filter, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate)
        {
            var tasks = await _cargoRepository.GetAsync(page, pageSize, sortOrder, desc, searchString, filter, dateFilterStartDate, dateFilterEndDate);
            return _mapper.Map<List<CargoesDto>>(tasks);
        }
        public async Task<List<CargoesDto>> GetCargoesAsync()
        {
            var tasks = await _cargoRepository.GetCargoesAsync();

            return _mapper.Map<List<CargoesDto>>(tasks);
        }
        public async Task<CargoesDto?> GetByIdAsync(int id)
        {
            var task = await _cargoRepository.GetByIdAsync(id);

            return _mapper.Map<CargoesDto>(task);
        }
        public async Task<int[]> GetChartDataAsync()
        {
            return await _cargoRepository.GetChartDataAsync();
        }
        public async Task<int> PageCountAsync(string? searchString, string? filter, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate)
        {
            return await _cargoRepository.PageCountAsync(searchString, filter, dateFilterStartDate, dateFilterEndDate);
        }
        public async Task<int> CountAsync(bool all)
        {
            return await _cargoRepository.CountAsync(all);
        }
        public async Task PostAsync(CargoesDto task)
        {
            await _cargoRepository.PostAsync(_mapper.Map<Cargoes>(task));
        }
        public async Task PutAsync(CargoesDto task)
        {
            await _cargoRepository.PutAsync(_mapper.Map<Cargoes>(task));
        }
        public async Task<bool> DeleteAsync(int id)
        {
            return await _cargoRepository.DeleteAsync(id);
        }
        public async Task<string> ExportToCSVAsync(CultureInfo lang, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate, bool isTextDocument)
        {
            return await _cargoRepository.ExportToCSVAsync(lang, dateFilterStartDate, dateFilterEndDate, isTextDocument);
        }
        public string ExportToExcel(CultureInfo lang, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate)
        {
            return _cargoRepository.ExportToExcel(lang, dateFilterStartDate, dateFilterEndDate);
        }
        public async Task<string> ExportToPdfAsync(CultureInfo lang, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate)
        {
            return await _cargoRepository.ExportToPdfAsync(lang, dateFilterStartDate, dateFilterEndDate);
        }
        public async Task<string?> ImportAsync([FromBody] string file, CultureInfo lang)
        {
            return await _cargoRepository.ImportAsync(file, lang);
        }
    }
}
