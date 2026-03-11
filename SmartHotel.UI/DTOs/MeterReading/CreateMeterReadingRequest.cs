using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHotel.UI.DTOs.MeterReading
{
    public class CreateMeterReadingRequest
    {
        [Required]
        public int ContractId { get; set; }

        [Required]
        public int ServiceId { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Chỉ số mới không hợp lệ")]
        public int NewIndex { get; set; } 

        [Required]
        public DateTime ReadingDate { get; set; } 
    }
}
