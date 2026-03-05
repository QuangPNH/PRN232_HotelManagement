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
    public class ContractRepository : IContractRepository
    {
        private readonly SmartHotelDbContext _context;

        public ContractRepository(SmartHotelDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Contract>> GetAllAsync()
        {
            return await _context.Contracts
                .Include(c => c.Room)
                .Include(c => c.Tenant)
                .OrderByDescending(c => c.StartDate) 
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Contract?> GetByIdAsync(int id)
        {
            return await _context.Contracts
                .Include(c => c.Room)
                .Include(c => c.Tenant)
                .FirstOrDefaultAsync(c => c.ContractId == id);
        }

        public async Task CreateAsync(Contract contract)
        {
            await _context.Contracts.AddAsync(contract);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Contract contract)
        {
            _context.Contracts.Update(contract);
            await _context.SaveChangesAsync();
        }

        public async Task<Contract?> GetActiveContractByRoomIdAsync(int roomId)
        {
            return await _context.Contracts
                .FirstOrDefaultAsync(c => c.RoomId == roomId && c.IsActive == true);
        }

        public async Task<IEnumerable<Contract>> GetContractsByTenantIdAsync(int tenantId)
        {
            return await _context.Contracts
                .Include(c => c.Room)
                .Where(c => c.TenantId == tenantId)
                .OrderByDescending(c => c.StartDate)
                .ToListAsync();
        }
        public async Task<(IEnumerable<Contract> Items, int TotalCount)> GetByFilterAsync(string? keyword, bool? isActive, int pageIndex, int pageSize)
        {
            var query = _context.Contracts
                .Include(c => c.Room)
                .Include(c => c.Tenant)
                .AsQueryable();

            if (isActive.HasValue)
            {
                query = query.Where(c => c.IsActive == isActive.Value);
            }

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(c => c.Tenant.FullName.Contains(keyword) ||
                                         c.Room.RoomNumber.Contains(keyword));
            }

            int totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(c => c.StartDate) 
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .AsNoTracking()
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<List<Contract>> GetContractsByTenantAsync(int tenantId)
        {
            return await _context.Contracts
                .Include(c => c.Room) 
                .Where(c => c.TenantId == tenantId)
                .OrderByDescending(c => c.StartDate) 
                .ToListAsync();
        }
    }
}
