﻿using Cargotruck.Server.Data;
using Cargotruck.Server.Services;
using Cargotruck.Shared.Model;
using Cargotruck.Shared.Resources;
using ClosedXML.Excel;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System.Data;
using System.Globalization;
using System.Text;

namespace Cargotruck.Server.Repositories
{
    public class SettingRepository : ISettingRepository
    {
        private readonly ApplicationDbContext _context;
        public SettingRepository(ApplicationDbContext context)
        {
            _context = context;
        }
  
        public async Task<List<Settings>> GetAsync()
        {
            return await _context.Settings.ToListAsync();
        }

        public async Task<Settings?> GetAsync(int id)
        {
            return await _context.Settings.FirstOrDefaultAsync(a => a.Id == id);
        }

      
        public async Task<Settings> GetWaitTimeAsync()
{
            var waitTime = await _context.Settings.FirstOrDefaultAsync(x => x.SettingName == "CurrencyExchangeWaitTime");

            if (waitTime == null)
            {
                waitTime = new Settings() { SettingName = "CurrencyExchangeWaitTime", SettingValue = "3600" };
                _context.Settings.Add(waitTime);
                await _context.SaveChangesAsync();
            }

            return waitTime;
        }

        public async Task PostAsync(Settings data)
        {
            _context.Add(data);
            await _context.SaveChangesAsync();
        }

        public async Task PutAsync(Settings data)
        {
            _context.Entry(data).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var data = _context.Tasks.FirstOrDefault(x => x.Id == id);

            if (data != null)
            {
                _context.Tasks.Remove(data);
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }
    }
}