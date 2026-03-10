using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHotel.UI.DTOs.Invoice
{
    public class InvoiceResponse
    {
        public int InvoiceId { get; set; }
        public int ContractId { get; set; }

        // Thông tin hiển thị thêm
        public string RoomNumber { get; set; }
        public string TenantName { get; set; }

        public DateTime InvoiceMonth { get; set; }
        public decimal TotalAmount { get; set; }

        public bool IsPaid { get; set; }
        public DateTime? PaidDate { get; set; }

        public List<InvoiceDetailResponse> InvoiceDetails { get; set; }
    }

    public class InvoiceDetailResponse
    {
        public int InvoiceDetailId { get; set; }
        public int ServiceId { get; set; }
        public string ServiceName { get; set; } // Tên dịch vụ lấy từ bảng Service
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Amount { get; set; }
    }
}
