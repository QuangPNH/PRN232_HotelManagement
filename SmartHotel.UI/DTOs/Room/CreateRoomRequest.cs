using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHotel.UI.DTOs.Room
{
    public class CreateRoomRequest
    {
        [Required(ErrorMessage = "Số phòng không được để trống")]
        public string RoomNumber { get; set; } // Vd: P.101

        [Range(1, 100, ErrorMessage = "Tầng phải từ 1 đến 100")]
        public int Floor { get; set; }

        [Required]
        public string ImageUrl { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Giá phòng không được âm")]
        public decimal Price { get; set; }

        [Range(1, 20, ErrorMessage = "Sức chứa phải từ 1 người trở lên")]
        public int Capacity { get; set; }

        public string? Description { get; set; }
    }
}
