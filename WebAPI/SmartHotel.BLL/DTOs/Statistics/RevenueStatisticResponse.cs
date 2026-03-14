using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHotel.BLL.DTOs.Statistics
{
    public class RevenueStatisticResponse
    {
        public int Month { get; set; }
        public decimal TotalRevenue { get; set; } // Tổng tiền thực thu (Đã thanh toán)
    }
}
