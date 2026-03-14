using Microsoft.EntityFrameworkCore;
using SmartHotel.DAL.Data;
using SmartHotel.DAL.Models;
using SmartHotel.DAL.Repositories.Interface;

namespace SmartHotel.DAL.Repositories
{
    public class ServiceRepository : IServiceRepository
    {
        private readonly SmartHotelDbContext _context;

        public ServiceRepository(SmartHotelDbContext context)
        {
            _context = context;
        }

        public async Task<Service?> GetByIdAsync(int id)
        {
            return await _context.Services.FindAsync(id);
        }

        public async Task CreateAsync(Service service)
        {
            await _context.Services.AddAsync(service);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Service service)
        {
            _context.Services.Update(service);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var service = await _context.Services.FindAsync(id);
            if (service != null)
            {
                service.IsActive = false;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<(IEnumerable<Service> Items, int TotalCount)> GetByFilterAsync(string? keyword, bool? isMeterBased, int pageIndex, int pageSize)
        {
            var query = _context.Services.AsQueryable();

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(s => s.ServiceName.Contains(keyword));
            }

            if (isMeterBased.HasValue)
            {
                query = query.Where(s => s.IsMeterBased == isMeterBased.Value);
            }

            int totalCount = await query.CountAsync();

            var items = await query
                .OrderBy(s => s.ServiceName)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .AsNoTracking()
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<bool> IsNameExistsAsync(string name)
        {
            return await _context.Services.AnyAsync(s => s.ServiceName == name && s.IsActive);
        }
    }
}
