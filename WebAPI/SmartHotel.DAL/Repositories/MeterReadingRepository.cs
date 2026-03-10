using Microsoft.EntityFrameworkCore;
using SmartHotel.DAL.Data;
using SmartHotel.DAL.Models;
using SmartHotel.DAL.Repositories.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHotel.DAL.Repositories
{
    public class MeterReadingRepository : IMeterReadingRepository
    {
        private readonly SmartHotelDbContext _context;

        public MeterReadingRepository(SmartHotelDbContext context)
        {
            _context = context;
        }

        public async Task<MeterReading?> GetByIdAsync(int id)
        {
            return await _context.MeterReadings
                .Include(m => m.Contract).ThenInclude(c => c.Room) 
                .Include(m => m.Service) 
                .FirstOrDefaultAsync(m => m.MeterReadingId == id);
        }

        public async Task CreateAsync(MeterReading reading)
        {
            await _context.MeterReadings.AddAsync(reading);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(MeterReading reading)
        {
            _context.MeterReadings.Update(reading);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var item = await _context.MeterReadings.FindAsync(id);
            if (item != null)
            {
                _context.MeterReadings.Remove(item);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<(IEnumerable<MeterReading> Items, int TotalCount)> GetByFilterAsync(
            int? contractId, int? month, int? year, int pageIndex, int pageSize)
        {
            var query = _context.MeterReadings
                .Include(m => m.Contract).ThenInclude(c => c.Room) 
                .Include(m => m.Service)
                .AsQueryable();

            if (contractId.HasValue)
            {
                query = query.Where(m => m.ContractId == contractId.Value);
            }

            if (month.HasValue)
            {
                query = query.Where(m => m.ReadingDate.Month == month.Value);
            }

            if (year.HasValue)
            {
                query = query.Where(m => m.ReadingDate.Year == year.Value);
            }

            int totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(m => m.ReadingDate)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .AsNoTracking()
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<bool> IsReadingExistsAsync(int contractId, int serviceId, int month, int year)
        {
            return await _context.MeterReadings.AnyAsync(m =>
                m.ContractId == contractId &&
                m.ServiceId == serviceId &&
                m.ReadingDate.Month == month &&
                m.ReadingDate.Year == year);
        }

        public async Task<MeterReading?> GetPreviousReadingAsync(int contractId, int serviceId)
        {

            return await _context.MeterReadings
                .Where(m => m.ContractId == contractId && m.ServiceId == serviceId)
                .OrderByDescending(m => m.ReadingDate)
                .FirstOrDefaultAsync();
        }
    }
}
