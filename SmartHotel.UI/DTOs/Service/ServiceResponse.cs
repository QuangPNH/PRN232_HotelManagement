using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHotel.UI.DTOs.Service
{
    public class ServiceResponse
    {
        public int ServiceId { get; set; }
        public string ServiceName { get; set; } 
        public decimal UnitPrice { get; set; }
        public string Unit { get; set; }        
        public bool IsMeterBased { get; set; }  
        public bool IsActive { get; set; }
        public string Description { get; set; } 
    }
}
