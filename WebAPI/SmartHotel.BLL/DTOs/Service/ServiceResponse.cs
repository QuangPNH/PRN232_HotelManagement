using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHotel.BLL.DTOs.Service
{
    public class ServiceResponse
    {
        public int ServiceId { get; set; }
        public string ServiceName { get; set; } // Điện, Nước
        public decimal UnitPrice { get; set; }  // 3500
        public string Unit { get; set; }        // kWh
        public bool IsMeterBased { get; set; }  // true
        public bool IsActive { get; set; }
    }
}
