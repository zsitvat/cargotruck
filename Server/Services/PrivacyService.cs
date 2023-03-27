using AutoMapper;
using Cargotruck.Server.Repositories;
using Cargotruck.Shared.Model;
using Cargotruck.Shared.Model.Dto;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace Cargotruck.Server.Services
{
    public class PrivacyService : IPrivacyService
    {

        private readonly IPrivacyRepository _privacyRepository;
        private readonly IMapper _mapper;
        public PrivacyService(IPrivacyRepository privacyRepository, IMapper mapper)
        {
            _privacyRepository = privacyRepository;
            _mapper = mapper;
        }
        public async Task<List<PrivaciesDto>> GetAsync()
        {
            var privacies = await _privacyRepository.GetAsync();
            return _mapper.Map<List<PrivaciesDto>>(privacies);
        }
        public async Task<PrivaciesDto?> GetByIdAsync(int id)
        {
            var privacy = await _privacyRepository.GetByIdAsync(id);

            return _mapper.Map<PrivaciesDto>(privacy);
        }
        public async Task<int> CountAsync()
        {
            return await _privacyRepository.CountAsync();
        }

        public async Task PostAsync(PrivaciesDto privacy)
        {
            await _privacyRepository.PostAsync(_mapper.Map<Privacies>(privacy));
        }
        public async Task PutAsync(PrivaciesDto privacy)
        {
            await _privacyRepository.PutAsync(_mapper.Map<Privacies>(privacy));
        }
        public async Task<bool> DeleteAsync(int id)
        {
            return await _privacyRepository.DeleteAsync(id);
        }
    }
}