using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;

namespace SmartHotel.DAL.Models
{
    public class Tenant
    {
        public int TenantId { get; set; }

        public int AccountId { get; set; }

        [Required]
        public string FullName { get; set; }

        [Required, MaxLength(20)]
        public string CCCD { get; set; }

        public string Phone { get; set; }

        public Account Account { get; set; }

        public ICollection<Contract> Contracts { get; set; }
    }
}
