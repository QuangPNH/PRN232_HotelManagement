using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHotel.BLL.DTOs.Room
{
    public class RoomResponse
    {
        public int RoomId { get; set; }
        public string RoomNumber { get; set; }
        public string ImageUrl { get; set; }
        public int Floor { get; set; }
        public decimal Price { get; set; }
        public int Capacity { get; set; }
        public string Status { get; set; }
        public string StatusVietnamese { get; set; } 
        public string Description { get; set; }
    }
}
