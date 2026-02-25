using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHotel.DAL.Models
{
    public class Service
    {
        public int ServiceId { get; set; }

        [Required, MaxLength(100)]
        public string ServiceName { get; set; } 

        public decimal UnitPrice { get; set; }

        [Required, MaxLength(20)]
        public string Unit { get; set; }          

        public bool IsMeterBased { get; set; }     

        public bool IsActive { get; set; } = true;

        public ICollection<MeterReading> MeterReadings { get; set; }
        public ICollection<InvoiceDetail> InvoiceDetails { get; set; }
    }
}
