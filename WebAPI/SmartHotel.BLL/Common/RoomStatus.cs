using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHotel.BLL.Common
{
    public static class RoomStatus
    {
        public const string Available = "Available";   // Phòng trống
        public const string Occupied = "Occupied";     // Đang có khách
        public const string Maintenance = "Maintenance"; // Đang bảo trì/sửa chữa
        public const string Cleaning = "Cleaning";     // Đang dọn dẹp
    }
}
