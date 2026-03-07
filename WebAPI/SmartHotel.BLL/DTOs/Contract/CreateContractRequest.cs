using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHotel.BLL.DTOs.Contract
{
    public class CreateContractRequest
    {
        [Required]
        public int TenantId { get; set; }

        [Required]
        public int RoomId { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        public decimal DepositAmount { get; set; } 

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; } 
    }
}
