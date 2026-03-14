using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHotel.UI.DTOs.Room
{
    public class UpdateRoomRequest : CreateRoomRequest
    {
        public int RoomId { get; set; }
        public string Status { get; set; } // Cho phép sửa trạng thái
        [Required]
        public string ImageUrl { get; set; }
    }
}
