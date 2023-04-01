using AutoMapper;
using Cargotruck.Server.Repositories.Interfaces;
using Cargotruck.Server.Services.Interfaces;
using Cargotruck.Shared.Model;
using Cargotruck.Shared.Model.Dto;
using DocumentFormat.OpenXml.Office2010.Excel;

namespace Cargotruck.Server.Services
{
    public class SettingService : ISettingService
    {
        private readonly ISettingRepository _settingRepository;
        private readonly IMapper _mapper;
        public SettingService(ISettingRepository settingRepository, IMapper mapper)
        {
            _settingRepository = settingRepository;
            _mapper = mapper;
        }
        public async Task<List<SettingDto>> GetAsync()
        {
            var settings = await _settingRepository.GetAsync();
            return _mapper.Map<List<SettingDto>>(settings);
        }
        public async Task<SettingDto?> GetAsync(int id)
        {
            var setting = await _settingRepository.GetAsync(id);
            return _mapper.Map<SettingDto>(setting);
        }
        public async Task<SettingDto> GetWaitTimeAsync()
        {
            var setting = await _settingRepository.GetWaitTimeAsync();
            return _mapper.Map<SettingDto>(setting);
        }
        public async Task PostAsync(SettingDto data)
        {
            await _settingRepository.PostAsync(_mapper.Map<Setting>(data));
        }
        public async Task PutAsync(SettingDto data)
        {
            await _settingRepository.PutAsync(_mapper.Map<Setting>(data));
        }
        public async Task<bool> DeleteAsync(int id)
        {
            return await _settingRepository.DeleteAsync(id);
        }
    }
}
