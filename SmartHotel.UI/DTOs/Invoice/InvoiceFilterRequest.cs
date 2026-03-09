using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHotel.UI.DTOs.Invoice
{
    public class InvoiceFilterRequest
    {
        public int? RoomId { get; set; }
        public int? Month { get; set; }
        public int? Year { get; set; }
        public bool? IsPaid { get; set; }

        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
