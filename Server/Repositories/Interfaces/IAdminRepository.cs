﻿using Cargotruck.Server.Models;
using Microsoft.AspNetCore.Mvc;

namespace Cargotruck.Server.Repositories.Interfaces
{
    public interface IAdminRepository
    {
        Task<List<User>> GetAsync(int page, int pageSize, string? filter);
        Task<User?> GetAsync(string id);
        Task<bool> DeleteAsync(string id);
        Task<int> PageCountAsync(string? filter);
        Task<int> CountAsync();
        Task<int> LoginsCountAsync();
        Task<Dictionary<string, string>?> ClaimsAsync();
        Task<Dictionary<string, string>?> RolesAsync();
    }
}