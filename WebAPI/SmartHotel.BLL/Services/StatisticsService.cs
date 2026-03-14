using SmartHotel.BLL.DTOs.Statistics;
using SmartHotel.BLL.Services.Interface;
using SmartHotel.DAL.Repositories.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHotel.BLL.Services
{
    public class StatisticsService : IStatisticsService
    {
        private readonly IStatisticsRepository _repo;

        public StatisticsService(IStatisticsRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<RevenueStatisticResponse>> GetRevenueStatisticAsync(int year)
        {
            // 1. Lấy dữ liệu thô (Tuple) từ DAL
            var rawData = await _repo.GetMonthlyRevenueAsync(year);

            // 2. Xử lý Logic: Fill đầy đủ 12 tháng (Tháng nào ko có trong DB thì = 0)
            var result = new List<RevenueStatisticResponse>();

            for (int i = 1; i <= 12; i++)
            {
                // Tìm trong list rawData xem có tháng i không
                var monthData = rawData.FirstOrDefault(x => x.Month == i);

                // Tuple giá trị mặc định là (0, 0) nếu không tìm thấy, nên check kỹ hơn chút
                decimal amount = 0;
                if (monthData != default)
                {
                    amount = monthData.TotalAmount;
                }

                result.Add(new RevenueStatisticResponse
                {
                    Month = i,
                    TotalRevenue = amount
                });
            }

            return result;
        }

        public async Task<RoomStatusStatisticResponse> GetRoomOccupancyAsync()
        {
            // 1. Lấy dữ liệu thô
            var (total, rented, maintenance) = await _repo.GetRoomStatusAsync();

            // 2. Tính toán số phòng trống
            var empty = total - rented - maintenance;

            // 3. Map sang DTO
            return new RoomStatusStatisticResponse
            {
                TotalRooms = total,
                RentedRooms = rented,
                MaintenanceRooms = maintenance,
                EmptyRooms = empty
                // OccupancyRate tự tính trong property của DTO rồi
            };
        }
    }

}
