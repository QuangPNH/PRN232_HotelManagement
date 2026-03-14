using SmartHotel.UI.DTOs.Contract;
using SmartHotel.UI.DTOs.Invoice;

namespace SmartHotel.UI.DTOs.Tenant
{
    public class TenantDashboardViewModel
    {
        // Bạn có thể dùng class TenantResponse của bạn, ở đây mình dùng dynamic/object tượng trưng
        public dynamic Profile { get; set; }

        public List<ContractResponse> Contracts { get; set; } = new List<ContractResponse>();
        public List<InvoiceResponse> Invoices { get; set; } = new List<InvoiceResponse>();
    }
}
