using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHotel.DAL.Repositories.Interface
{
    public interface IStatisticsRepository
    {
        // Trả về List<(Tháng, Tổng tiền)>
        Task<List<(int Month, decimal TotalAmount)>> GetMonthlyRevenueAsync(int year);

        // Trả về (Tổng số phòng, Số đã thuê, Số bảo trì)
        Task<(int Total, int Rented, int Maintenance)> GetRoomStatusAsync();
    }
}
