using AutoMapper;
using Cargotruck.Shared.Model.Dto;
using Cargotruck.Shared.Model;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using DocumentFormat.OpenXml.Office2010.Excel;
using Cargotruck.Server.Repositories.Interfaces;
using Cargotruck.Server.Services.Interfaces;

namespace Cargotruck.Server.Services
{
    public class TruckService : ITruckService
    {
        private readonly ITruckRepository _truckRepository;
        private readonly IMapper _mapper;
        public TruckService(ITruckRepository truckRepository, IMapper mapper)
        {
            _truckRepository = truckRepository;
            _mapper = mapper;
        }
        public async Task<List<TruckDto>> GetAsync(int page, int pageSize, string sortOrder, bool desc, string? searchString, Status? filter, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate)
        {
            var trucks = await _truckRepository.GetAsync(page, pageSize, sortOrder, desc, searchString, filter, dateFilterStartDate, dateFilterEndDate);
            return _mapper.Map<List<TruckDto>>(trucks);
        }
        public async Task<List<TruckDto>> GetTrucksAsync()
        {
            var trucks = await _truckRepository.GetTrucksAsync();

            return _mapper.Map<List<TruckDto>>(trucks);
        }
        public async Task<TruckDto?> GetByIdAsync(int id)
        {
            var truck = await _truckRepository.GetByIdAsync(id);

            return _mapper.Map<TruckDto>(truck);
        }

        public async Task<TruckDto> GetByVRNAsync(string vehicle_registration_number)
        {
            var truck = await _truckRepository.GetByVRNAsync(vehicle_registration_number);

            return _mapper.Map<TruckDto>(truck);
        }
        public async Task<int> PageCountAsync(string? searchString, Status? filter, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate)
        {
            var count = await _truckRepository.PageCountAsync(searchString, filter, dateFilterStartDate, dateFilterEndDate);

            return count;
        }
        public async Task<int> CountAsync(bool all)
        {
            var count = await _truckRepository.CountAsync(all);

            return count;
        }
        public async Task PostAsync(TruckDto truck)
        {
            await _truckRepository.PostAsync(_mapper.Map<Truck>(truck));
        }
        public async Task PutAsync(TruckDto truck)
        {
            await _truckRepository.PutAsync(_mapper.Map<Truck>(truck));
        }
        public async Task<bool> DeleteAsync(int id)
        {
            return await _truckRepository.DeleteAsync(id);
        }
        public async Task<string> ExportToCSVAsync(CultureInfo lang, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate, bool isTextDocument)
        {
            return await _truckRepository.ExportToCSVAsync(lang, dateFilterStartDate, dateFilterEndDate, isTextDocument);
        }
        public string ExportToExcel(CultureInfo lang, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate)
        {
            return _truckRepository.ExportToExcel(lang, dateFilterStartDate, dateFilterEndDate);
        }
        public async Task<string> ExportToPdfAsync(CultureInfo lang, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate)
        {
            return await _truckRepository.ExportToPdfAsync(lang, dateFilterStartDate, dateFilterEndDate);
        }
        public async Task<string?> ImportAsync([FromBody] string file, CultureInfo lang)
        {
            return await _truckRepository.ImportAsync(file, lang);
        }
    }
}
