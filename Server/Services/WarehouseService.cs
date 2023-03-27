using AutoMapper;
using Cargotruck.Server.Repositories;
using Cargotruck.Shared.Model;
using Cargotruck.Shared.Model.Dto;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace Cargotruck.Server.Services
{
    public class WarehouseService:IWarehouseService
    {
        private readonly IWarehouseRepository _warehouseRepository;
        private readonly IMapper _mapper;
        public WarehouseService(IWarehouseRepository warehouseRepository, IMapper mapper)
        {
            _warehouseRepository = warehouseRepository;
            _mapper = mapper;
        }
        public async Task<List<WarehousesDto>> GetAsync(int page, int pageSize, string sortOrder, bool desc, string? searchString, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate)
        {
            var warehouses = await _warehouseRepository.GetAsync(page, pageSize, sortOrder, desc, searchString, dateFilterStartDate, dateFilterEndDate);
            return _mapper.Map<List<WarehousesDto>>(warehouses);
        }
        public async Task<List<WarehousesDto>> GetWarehousesAsync()
        {
            var warehouses = await _warehouseRepository.GetWarehousesAsync();

            return _mapper.Map<List<WarehousesDto>>(warehouses);
        }
        public async Task<WarehousesDto?> GetByIdAsync(int id)
        {
            var warehouse = await _warehouseRepository.GetByIdAsync(id);

            return _mapper.Map<WarehousesDto>(warehouse);
        }
        public async Task<int> PageCountAsync(string? searchString, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate)
        {
            var count = await _warehouseRepository.PageCountAsync(searchString, dateFilterStartDate, dateFilterEndDate);

            return count;
        }
        public async Task<int> CountAsync()
        {
            var count = await _warehouseRepository.CountAsync();

            return count;
        }
        public async Task PostAsync(WarehousesDto warehouse)
        {
            await _warehouseRepository.PostAsync(_mapper.Map<Warehouses>(warehouse));
        }
        public async Task PutAsync(WarehousesDto warehouse)
        {
            await _warehouseRepository.PutAsync(_mapper.Map<Warehouses>(warehouse));
        }
        public async Task<bool> DeleteAsync(int id)
        {
            return await _warehouseRepository.DeleteAsync(id);
        }
        public async Task<string> ExportToCSVAsync(CultureInfo lang, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate, bool isTextDocument)
        {
            return await _warehouseRepository.ExportToCSVAsync(lang, dateFilterStartDate, dateFilterEndDate, isTextDocument);
        }
        public string ExportToExcel(CultureInfo lang, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate)
        {
            return _warehouseRepository.ExportToExcel(lang, dateFilterStartDate, dateFilterEndDate);
        }
        public async Task<string> ExportToPdfAsync(CultureInfo lang, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate)
        {
            return await _warehouseRepository.ExportToPdfAsync(lang, dateFilterStartDate, dateFilterEndDate);
        }
        public async Task<string?> ImportAsync([FromBody] string file, CultureInfo lang)
        {
            return await _warehouseRepository.ImportAsync(file, lang);
        }
    }
}
