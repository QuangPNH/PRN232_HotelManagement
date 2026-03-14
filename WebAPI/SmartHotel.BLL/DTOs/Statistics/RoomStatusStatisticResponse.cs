using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHotel.BLL.DTOs.Statistics
{
    public class RoomStatusStatisticResponse
    {
        public int TotalRooms { get; set; }
        public int RentedRooms { get; set; } // Đang thuê
        public int EmptyRooms { get; set; }  // Trống
        public int MaintenanceRooms { get; set; } // Đang sửa

        // Tự tính % để Frontend đỡ phải tính
        public double OccupancyRate => TotalRooms == 0 ? 0 : Math.Round((double)RentedRooms / TotalRooms * 100, 2);
    }
}
