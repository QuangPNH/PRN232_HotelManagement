using SmartHotel.BLL.Common;
using SmartHotel.BLL.DTOs.Tenant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHotel.BLL.Services.Interface
{
    public interface ITenantService
    {

        Task<List<LookupDto>> GetTenantLookupAsync();

        Task<PagedResult<TenantResponse>> GetTenantsAsync(TenantFilterRequest request);
        Task<TenantResponse> GetTenantByIdAsync(int id);
        Task CreateTenantAsync(CreateTenantRequest request);
        Task ToggleTenantStatusAsync(int id);

        Task<TenantResponse> GetTenantByAccountIdAsync(int accountId);
    }
}
