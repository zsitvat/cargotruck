using AutoMapper;
using Cargotruck.Server.Repositories.Interfaces;
using Cargotruck.Server.Services.Interfaces;
using Cargotruck.Shared.Model.Dto;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Spreadsheet;

namespace Cargotruck.Server.Services
{
    public class AdminService : IAdminService
    {
        private readonly IAdminRepository _adminRepository;
        private readonly IMapper _mapper;

        public AdminService(IAdminRepository adminRepository, IMapper mapper)
        {
            _adminRepository = adminRepository;
            _mapper = mapper;
        }

        public async Task<List<UserDto>> GetAsync(int page, int pageSize, string? filter)
        {
            var users = await _adminRepository.GetAsync(page, pageSize, filter);
            return _mapper.Map<List<UserDto>>(users);
        }
        public async Task<UserDto?> GetAsync(string id)
        {
            var users = await _adminRepository.GetAsync(id);
            return _mapper.Map<UserDto>(users);
        }
        public async Task<bool> DeleteAsync(string id)
        {
            return await _adminRepository.DeleteAsync(id);
        }
        public async Task<int> PageCountAsync(string? filter)
        {
            return await _adminRepository.PageCountAsync(filter);
        }
        public async Task<int> CountAsync()
        {
            return await _adminRepository.CountAsync();
        }
        public async Task<int> LoginsCountAsync()
        {
            return await _adminRepository.LoginsCountAsync();
        }
        public async Task<Dictionary<string, string>?> ClaimsAsync()
        {
            return await _adminRepository.ClaimsAsync();
        }
        public async Task<Dictionary<string, string>?> RolesAsync()
        {
            return await _adminRepository.RolesAsync();
        }
    }
}
