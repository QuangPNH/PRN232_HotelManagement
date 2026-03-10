using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHotel.BLL.DTOs.MeterReading
{
    public class MeterReadingFilterRequest
    {
        public int? ContractId { get; set; } 
        public int? Month { get; set; }      
        public int? Year { get; set; }       
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
