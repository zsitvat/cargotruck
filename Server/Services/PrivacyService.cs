﻿using AutoMapper;
using Cargotruck.Server.Repositories.Interfaces;
using Cargotruck.Server.Services.Interfaces;
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
        public async Task<List<PrivacyDto>> GetAsync(string lang)
        {
            var privacies = await _privacyRepository.GetAsync(lang);
            return _mapper.Map<List<PrivacyDto>>(privacies);
        }
        public async Task<PrivacyDto?> GetByIdAsync(int id)
        {
            var privacy = await _privacyRepository.GetByIdAsync(id);

            return _mapper.Map<PrivacyDto>(privacy);
        }
        public async Task<int> CountAsync()
        {
            return await _privacyRepository.CountAsync();
        }

        public async Task PostAsync(PrivacyDto privacy)
        {
            await _privacyRepository.PostAsync(_mapper.Map<Privacies>(privacy));
        }
        public async Task PutAsync(PrivacyDto privacy)
        {
            await _privacyRepository.PutAsync(_mapper.Map<Privacies>(privacy));
        }
        public async Task<bool> DeleteAsync(int id)
        {
            return await _privacyRepository.DeleteAsync(id);
        }
    }
}