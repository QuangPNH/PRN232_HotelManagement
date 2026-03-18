using SmartHotel.BLL.DTOs.Statistics;
using SmartHotel.BLL.Services.Interface;
using SmartHotel.DAL.Repositories.Interface;

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
            var rawData = await _repo.GetMonthlyRevenueAsync(year);

            var result = new List<RevenueStatisticResponse>();

            for (int i = 1; i <= 12; i++)
            {
                var monthData = rawData.FirstOrDefault(x => x.Month == i);

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
    
            var (total, rented, maintenance) = await _repo.GetRoomStatusAsync();

            var empty = total - rented - maintenance;

            return new RoomStatusStatisticResponse
            {
                TotalRooms = total,
                RentedRooms = rented,
                MaintenanceRooms = maintenance,
                EmptyRooms = empty
            };
        }
    }

}
