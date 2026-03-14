using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHotel.UI.DTOs.Service
{
    public class CreateServiceRequest
    {
        public int ServiceId { get; set; }

        [Required(ErrorMessage = "Tên dịch vụ không được để trống")]
        public string ServiceName { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Đơn giá không được âm")]
        public decimal UnitPrice { get; set; }

        [Required(ErrorMessage = "Đơn vị tính không được để trống")]
        public string Unit { get; set; } // kWh, Khối, Người, Tháng

        public bool IsMeterBased { get; set; } // True: Điện/Nước, False: Wifi/Rác
    }
}
