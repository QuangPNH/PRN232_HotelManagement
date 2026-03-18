using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHotel.DAL.Repositories.Interface
{
    public interface IStatisticsRepository
    {
        Task<List<(int Month, decimal TotalAmount)>> GetMonthlyRevenueAsync(int year);

        Task<(int Total, int Rented, int Maintenance)> GetRoomStatusAsync();
    }
}
