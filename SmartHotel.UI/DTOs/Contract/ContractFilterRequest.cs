using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHotel.UI.DTOs.Contract
{
    public class ContractFilterRequest
    {
        public string? Keyword { get; set; } 
        public bool? IsActive { get; set; }  
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
