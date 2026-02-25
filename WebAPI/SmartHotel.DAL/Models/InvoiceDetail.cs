using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHotel.DAL.Models
{
    public class InvoiceDetail
    {
        public int InvoiceDetailId { get; set; }

        public int InvoiceId { get; set; }
        public int ServiceId { get; set; }

        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }

        public decimal Amount { get; set; }

        public Invoice Invoice { get; set; }
        public Service Service { get; set; }
    }

}
