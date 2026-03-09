using SmartHotel.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHotel.DAL.Repositories.Interface
{
    public interface IInvoiceRepository
    {
        Task<Invoice?> GetByIdAsync(int id);

        Task CreateAsync(Invoice invoice);
        Task UpdateAsync(Invoice invoice);

        Task<(IEnumerable<Invoice> Items, int TotalCount)> GetByFilterAsync(
            int? roomId,
            int? month,
            int? year,
            bool? isPaid,
            int pageIndex,
            int pageSize);

        Task<bool> IsInvoiceExistsAsync(int contractId, int month, int year);

        Task<List<Invoice>> GetInvoicesByTenantIdAsync(int tenantId);
    }
}
