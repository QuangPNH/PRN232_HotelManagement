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
        // CRUD cơ bản
        Task<IEnumerable<Tenant>> GetAllAsync();
        Task<Tenant?> GetByIdAsync(int id);
        Task CreateAsync(Tenant tenant);
        Task UpdateAsync(Tenant tenant);
        Task SaveAsync();
        // Nghiệp vụ: Tìm khách để làm hợp đồng
        Task<Tenant?> GetByCCCDAsync(string cccd); // Tìm theo CMND/CCCD
        Task<Tenant?> GetByAccountIdAsync(int accountId); // Tìm theo tài khoản đăng nhập
        Task<IEnumerable<Tenant>> SearchByNameOrPhoneAsync(string keyword); // Tìm kiếm nhanh

        Task<IEnumerable<Tenant>> GetTenantsLookupAsync();

        // Thêm dòng này vào Interface
        Task<(IEnumerable<Tenant> Items, int TotalCount)> GetByFilterAsync(string? keyword, bool? isActive, int pageIndex, int pageSize);

        Task<Tenant> GetTenantByAccountIdAsync(int accountId);
    }
}
