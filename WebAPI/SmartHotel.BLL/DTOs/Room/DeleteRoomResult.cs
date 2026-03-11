using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHotel.BLL.DTOs.Room
{
    public class DeleteRoomResult
    {
        public bool Success { get; set; }
        public string? WarningMessage { get; set; }
    }

}
