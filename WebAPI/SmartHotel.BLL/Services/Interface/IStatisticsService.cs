using SmartHotel.BLL.DTOs.Statistics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHotel.BLL.Services.Interface
{
    public interface  IStatisticsService
    {
        Task<List<RevenueStatisticResponse>> GetRevenueStatisticAsync(int year);
        Task<RoomStatusStatisticResponse> GetRoomOccupancyAsync();
    }
}
