using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHotel.DAL.Models
{
    public class Contract
    {
        public int ContractId { get; set; }

        public int TenantId { get; set; }
        public int RoomId { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public decimal DepositAmount { get; set; }

        public bool IsActive { get; set; } = true;
        public decimal Price { get; set; } // Giá chốt lúc ký

        public DateTime CreatedAt { get; set; }
        public Tenant Tenant { get; set; }
        public Room Room { get; set; }

        public ICollection<Invoice> Invoices { get; set; }
        public ICollection<MeterReading> MeterReadings { get; set; }
    }
}
