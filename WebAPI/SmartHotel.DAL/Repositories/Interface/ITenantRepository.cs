using SmartHotel.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHotel.DAL.Repositories.Interface
{
    public interface ITenantRepository
    {
        Task<IEnumerable<Tenant>> GetAllAsync();
        Task<Tenant?> GetByIdAsync(int id);
        Task CreateAsync(Tenant tenant);
        Task UpdateAsync(Tenant tenant);
        Task SaveAsync();
        Task<Tenant?> GetByCCCDAsync(string cccd); 
        Task<Tenant?> GetByAccountIdAsync(int accountId); 
        Task<IEnumerable<Tenant>> SearchByNameOrPhoneAsync(string keyword);

        Task<IEnumerable<Tenant>> GetTenantsLookupAsync();

        Task<(IEnumerable<Tenant> Items, int TotalCount)> GetByFilterAsync(string? keyword, bool? isActive, int pageIndex, int pageSize);

        Task<Tenant> GetTenantByAccountIdAsync(int accountId);
    }
}
