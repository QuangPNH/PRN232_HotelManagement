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
        public int RentedRooms { get; set; }
        public int EmptyRooms { get; set; } 
        public int MaintenanceRooms { get; set; } 
        public double OccupancyRate => TotalRooms == 0 ? 0 : Math.Round((double)RentedRooms / TotalRooms * 100, 2);
    }
}
