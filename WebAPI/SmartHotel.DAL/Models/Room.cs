using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHotel.DAL.Models
{
    public class Room
    {
        public int RoomId { get; set; } // PK

        public string RoomNumber { get; set; } // P.101, P.102...

        public string ImageUrl {  get; set; }

        public int Floor { get; set; } // Tầng

        public decimal Price { get; set; } // Giá phòng (Nằm trực tiếp ở đây)

        public int Capacity { get; set; } // Số người tối đa

        public string Status { get; set; } // "Available", "Occupied", "Cleaning" ...

        public string? Description { get; set; }

        public ICollection<Contract> Contracts { get; set; }
    }

}
