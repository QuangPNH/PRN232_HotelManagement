using Microsoft.EntityFrameworkCore;
using SmartHotel.DAL.Data;
using SmartHotel.DAL.Repositories.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHotel.DAL.Repositories
{
    public class StatisticsRepository : IStatisticsRepository
    {
        private readonly SmartHotelDbContext _context;

        public StatisticsRepository(SmartHotelDbContext context)
        {
            _context = context;
        }

        public async Task<List<(int Month, decimal TotalAmount)>> GetMonthlyRevenueAsync(int year)
        {
            // Dùng EF Core Select ra Tuple
            var data = await _context.Invoices
                .Where(i => i.InvoiceMonth.Year == year && i.IsPaid == true)
                .GroupBy(i => i.InvoiceMonth.Month)
                .Select(g => new
                {
                    Month = g.Key,
                    Total = g.Sum(i => i.TotalAmount)
                })
                .ToListAsync();

            // Convert Anonymous Object sang ValueTuple để return được
            return data.Select(x => (x.Month, x.Total)).ToList();
        }

        public async Task<(int Total, int Rented, int Maintenance)> GetRoomStatusAsync()
        {
            var total = await _context.Rooms.CountAsync();
            var rented = await _context.Rooms.CountAsync(r => r.Status == "Rented");
            var maintenance = await _context.Rooms.CountAsync(r => r.Status == "Maintenance");

            return (total, rented, maintenance);
        }
    }
}
