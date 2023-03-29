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
        public async Task<List<SettingsDto>> GetAsync()
        {
            var settings = await _settingRepository.GetAsync();
            return _mapper.Map<List<SettingsDto>>(settings);
        }
        public async Task<SettingsDto?> GetAsync(int id)
        {
            var setting = await _settingRepository.GetAsync(id);
            return _mapper.Map<SettingsDto>(setting);
        }
        public async Task<SettingsDto> GetWaitTimeAsync()
        {
            var setting = await _settingRepository.GetWaitTimeAsync();
            return _mapper.Map<SettingsDto>(setting);
        }
        public async Task PostAsync(SettingsDto data)
        {
            await _settingRepository.PostAsync(_mapper.Map<Settings>(data));
        }
        public async Task PutAsync(SettingsDto data)
        {
            await _settingRepository.PutAsync(_mapper.Map<Settings>(data));
        }
        public async Task<bool> DeleteAsync(int id)
        {
            return await _settingRepository.DeleteAsync(id);
        }
    }
}
