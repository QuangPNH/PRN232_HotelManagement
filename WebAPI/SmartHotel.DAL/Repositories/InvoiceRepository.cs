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
    public class InvoiceRepository : IInvoiceRepository
    {
        private readonly SmartHotelDbContext _context;

        public InvoiceRepository(SmartHotelDbContext context)
        {
            _context = context;
        }

        public async Task<Invoice?> GetByIdAsync(int id)
        {

            return await _context.Invoices
                .Include(i => i.Contract).ThenInclude(c => c.Room)
                .Include(i => i.Contract).ThenInclude(c => c.Tenant)
                .Include(i => i.InvoiceDetails).ThenInclude(d => d.Service)
                .FirstOrDefaultAsync(i => i.InvoiceId == id);
        }

        public async Task CreateAsync(Invoice invoice)
        {
            await _context.Invoices.AddAsync(invoice);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Invoice invoice)
        {
            _context.Invoices.Update(invoice);
            await _context.SaveChangesAsync();
        }

        public async Task<(IEnumerable<Invoice> Items, int TotalCount)> GetByFilterAsync(
            int? roomId, int? month, int? year, bool? isPaid, int pageIndex, int pageSize)
        {
            var query = _context.Invoices
                .Include(i => i.Contract).ThenInclude(c => c.Room)
                .Include(i => i.Contract).ThenInclude(c => c.Tenant)
                .AsQueryable();

            if (roomId.HasValue)
            {
                query = query.Where(i => i.Contract.RoomId == roomId.Value);
            }

            if (month.HasValue)
            {
                query = query.Where(i => i.InvoiceMonth.Month == month.Value);
            }

            if (year.HasValue)
            {
                query = query.Where(i => i.InvoiceMonth.Year == year.Value);
            }

            if (isPaid.HasValue)
            {
                query = query.Where(i => i.IsPaid == isPaid.Value);
            }

            int totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(i => i.InvoiceMonth)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .AsNoTracking()
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<bool> IsInvoiceExistsAsync(int contractId, int month, int year)
        {
            return await _context.Invoices.AnyAsync(i =>
                i.ContractId == contractId &&
                i.InvoiceMonth.Month == month &&
                i.InvoiceMonth.Year == year);
        }

        public async Task<List<Invoice>> GetInvoicesByTenantIdAsync(int tenantId)
        {
            return await _context.Invoices
                .Include(i => i.Contract)
                    .ThenInclude(c => c.Room)
                .Where(i => i.Contract.TenantId == tenantId)
                .OrderByDescending(i => i.InvoiceMonth.Year)
                .ThenByDescending(i => i.InvoiceMonth) 
                .ToListAsync();
        }
    }
}
