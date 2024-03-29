﻿using Cargotruck.Server.Services.Interfaces;
using Cargotruck.Shared.Model.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cargotruck.Server.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class PrivaciesController : ControllerBase
    {
        private readonly IPrivacyService _privacyService;

        public PrivaciesController(IPrivacyService privacyService)
        {
            _privacyService = privacyService;
        }

        [HttpGet]
        public async Task<ActionResult<List<PrivacyDto>>> GetAsync(string lang)
        {
            return Ok(await _privacyService.GetAsync(lang));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PrivacyDto>> GetByIdAsync(int id)
        {
            return Ok(await _privacyService.GetByIdAsync(id));
        }

        [HttpGet]
        public async Task<ActionResult<int>> CountAsync()
        {
            return Ok(await _privacyService.CountAsync());
        }

        [HttpPost]
        public async Task PostAsync(PrivacyDto privacy)
        {
            await _privacyService.PostAsync(privacy);
        }

        [HttpPut]
        public async Task PutAsync(PrivacyDto privacy)
        {
            await _privacyService.PutAsync(privacy);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> DeleteAsync(int id)
        {
            return Ok(await _privacyService.DeleteAsync(id));
        }
    }
}