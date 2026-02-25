using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHotel.DAL.Models
{
    public class Invoice
    {
        public int InvoiceId { get; set; }

        public int ContractId { get; set; }

        public DateTime InvoiceMonth { get; set; }   // VD: 2025-01-01

        public decimal TotalAmount { get; set; }

        public bool IsPaid { get; set; }
        public DateTime? PaidDate { get; set; }

        public Contract Contract { get; set; }

        public ICollection<InvoiceDetail> InvoiceDetails { get; set; }
    }

}
