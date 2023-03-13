﻿using Cargotruck.Data;
using Cargotruck.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cargotruck.Server.Controllers
{

    [Route("api/[controller]/[action]")]
    [ApiController]
    public class SettingsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public SettingsController(ApplicationDbContext context)
        {
            this._context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var data = await _context.Settings.ToListAsync();
            return Ok(data);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var data = await _context.Settings.FirstOrDefaultAsync(a => a.Id == id);
            return Ok(data);
        }

        [HttpGet]
        public async Task<IActionResult> GetWaitTime()
        {
            var waitTime = await _context.Settings.FirstOrDefaultAsync(x => x.SettingName == "CurrencyExchangeWaitTime");
            
            if (waitTime == null)
            {
                waitTime = new Settings() { SettingName = "CurrencyExchangeWaitTime", SettingValue = "3600" };
                _context.Settings.Add(waitTime);
                await _context.SaveChangesAsync();
            }

            return Ok(waitTime);
        }

        [HttpPost]
        public async Task<IActionResult> Post(Settings data)
        {
            _context.Add(data);
            await _context.SaveChangesAsync();
            return Ok(data.Id);
        }

        [HttpPut]
        public async Task<IActionResult> Put(Settings data)
        {
            _context.Entry(data).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var data = new Settings { Id = id };
            _context.Remove(data);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
