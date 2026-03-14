using Microsoft.EntityFrameworkCore;
using SmartHotel.DAL.Data;
using SmartHotel.DAL.Models;
using SmartHotel.DAL.Repositories.Interface;

namespace SmartHotel.DAL.Repositories
{
    public class TenantRepository : ITenantRepository
    {
        private readonly SmartHotelDbContext _context;

        public TenantRepository(SmartHotelDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Tenant>> GetAllAsync()
        {
            return await _context.Tenants
                .Include(t => t.Account).Include(c => c.Contracts).ThenInclude(c => c.Room)
                .OrderByDescending(t => t.TenantId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Tenant?> GetByIdAsync(int id)
        {
            return await _context.Tenants
                .Include(t => t.Contracts).Include(a=>a.Account)
                .FirstOrDefaultAsync(t => t.TenantId == id);
        }

        public async Task CreateAsync(Tenant tenant)
        {
            await _context.Tenants.AddAsync(tenant);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Tenant tenant)
        {
            _context.Tenants.Update(tenant);
            await _context.SaveChangesAsync();
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
        public async Task<Tenant?> GetByCCCDAsync(string cccd)
        {
            return await _context.Tenants
                .FirstOrDefaultAsync(t => t.CCCD == cccd);
        }

        public async Task<Tenant?> GetByAccountIdAsync(int accountId)
        {
            return await _context.Tenants
                .FirstOrDefaultAsync(t => t.AccountId == accountId);
        }

        public async Task<IEnumerable<Tenant>> SearchByNameOrPhoneAsync(string keyword)
        {
            return await _context.Tenants
                .Where(t => t.FullName.Contains(keyword) || t.Phone.Contains(keyword))
                .ToListAsync();
        }

        public async Task<IEnumerable<Tenant>> GetTenantsLookupAsync()
        {
            return await _context.Tenants
                .OrderBy(t => t.FullName)
                .ToListAsync();
        }

        // Thêm hàm này vào class TenantRepository
        public async Task<(IEnumerable<Tenant> Items, int TotalCount)> GetByFilterAsync(string? keyword, bool? isActive, int pageIndex, int pageSize)
        {
            var query = _context.Tenants.Include(a => a.Account)
                .Include(t => t.Contracts)
                    .ThenInclude(c => c.Room)
                .AsNoTracking()
                .AsQueryable();

            // 1. Lọc theo từ khóa (Tên, SĐT, CCCD)
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(t => t.FullName.Contains(keyword)
                                      || t.Phone.Contains(keyword)
                                      || t.CCCD.Contains(keyword));
            }

            // 2. Lọc theo trạng thái Hoạt động / Vô hiệu hóa
            if (isActive.HasValue)
            {
                query = query.Where(t => t.Account.IsActive == isActive.Value);
            }

            // 3. Đếm tổng số bản ghi
            int totalCount = await query.CountAsync();

            // 4. Phân trang và lấy dữ liệu
            var items = await query
                .OrderByDescending(t => t.TenantId)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<Tenant> GetTenantByAccountIdAsync(int accountId)
        {
            return await _context.Tenants
                .Include(t => t.Account) 
                .FirstOrDefaultAsync(t => t.AccountId == accountId);
        }
    }
}
