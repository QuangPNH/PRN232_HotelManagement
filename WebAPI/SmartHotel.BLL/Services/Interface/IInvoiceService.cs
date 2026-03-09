using SmartHotel.BLL.Common;
using SmartHotel.BLL.DTOs.Invoice;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHotel.BLL.Services.Interface
{
    public interface IInvoiceService
    {
        Task CreateInvoiceAsync(CreateInvoiceRequest request);
        Task<PagedResult<InvoiceResponse>> GetInvoicesAsync(InvoiceFilterRequest request);
        Task<InvoiceResponse> GetInvoiceByIdAsync(int id);
        Task MarkAsPaidAsync(int id);

        Task<List<InvoiceResponse>> GetInvoicesByTenantIdAsync(int tenantId);
    }
}
