using Cargotruck.Server.Data;
using Cargotruck.Server.Repositories.Interfaces;
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
  
        public async Task<List<Setting>> GetAsync()
        {
            return await _context.Settings.ToListAsync();
        }

        public async Task<Setting?> GetAsync(int id)
        {
            return await _context.Settings.FirstOrDefaultAsync(a => a.Id == id);
        }

        // gets the wait time for the next currency exchange rate request
        public async Task<Setting> GetWaitTimeAsync()
{
            var waitTime = await _context.Settings.FirstOrDefaultAsync(x => x.SettingName == "CurrencyExchangeWaitTime");

            if (waitTime == null)
            {
                waitTime = new Setting() { SettingName = "CurrencyExchangeWaitTime", SettingValue = "3600" };
                _context.Settings.Add(waitTime);
                await _context.SaveChangesAsync();
            }

            return waitTime;
        }

        public async Task PostAsync(Setting data)
        {
            _context.Add(data);
            await _context.SaveChangesAsync();
        }

        public async Task PutAsync(Setting data)
        {
            _context.Entry(data).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var data = _context.Settings.FirstOrDefault(x => x.Id == id);

            if (data != null)
            {
                _context.Settings.Remove(data);
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }
    }
}
