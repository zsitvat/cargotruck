﻿using Cargotruck.Shared.Model.Dto;

namespace Cargotruck.Server.Services
{
    public interface IPrivacyService
    {
        Task<List<PrivaciesDto>> GetAsync();
        Task<PrivaciesDto?> GetByIdAsync(int id);
        Task<int> CountAsync();
        Task PostAsync(PrivaciesDto data);
        Task PutAsync(PrivaciesDto data);
        Task<bool> DeleteAsync(int id);
    }
}
