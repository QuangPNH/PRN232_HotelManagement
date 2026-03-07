using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHotel.BLL.DTOs.Contract
{
    public class ContractResponse
    {
        public int ContractId { get; set; }

        public int RoomId { get; set; }
        public string RoomNumber { get; set; }

        public int TenantId { get; set; }
        public string TenantName { get; set; }
        public string TenantPhone { get; set; }

        public decimal Price { get; set; }
        public decimal DepositAmount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
    }
}
