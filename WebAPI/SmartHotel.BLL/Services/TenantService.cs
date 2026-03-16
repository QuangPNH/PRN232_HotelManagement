using SmartHotel.BLL.Common;
using SmartHotel.BLL.DTOs.Tenant;
using SmartHotel.BLL.Services.Interface;
using SmartHotel.DAL.Models;
using SmartHotel.DAL.Repositories.Interface;

namespace SmartHotel.BLL.Services
{
    public class TenantService : ITenantService
    {
        private readonly  ITenantRepository _tenantRepo;

        public TenantService(ITenantRepository tenantRepo)
        {
            _tenantRepo = tenantRepo;
        }

        public async Task<List<LookupDto>> GetTenantLookupAsync()
        {
            var tenants = await _tenantRepo.GetTenantsLookupAsync();
            return tenants.Select(t => new LookupDto
            {
                Id = t.TenantId,
                Name = t.FullName,
                ExtraInfo = t.Phone 
            }).ToList();
        }

        public async Task<PagedResult<TenantResponse>> GetTenantsAsync(TenantFilterRequest request)
        {
            var (items, totalCount) = await _tenantRepo.GetByFilterAsync(
                request.Keyword,
                request.IsActive,
                request.PageIndex,
                request.PageSize);

            var dtos = items.Select(t =>
            {
                var activeContract = t.Contracts?.FirstOrDefault(c => c.IsActive);

                return new TenantResponse
                {
                    TenantId = t.TenantId,
                    FullName = t.FullName,
                    Phone = t.Phone,
                    CCCD = t.CCCD,
                    IsActive = t.Account.IsActive,
                    IsRenting = activeContract != null,
                    CurrentRoom = activeContract?.Room?.RoomNumber ?? "Chưa thuê"
                };
            }).ToList();

            return new PagedResult<TenantResponse>
            {
                Items = dtos,
                TotalCount = totalCount,
                PageIndex = request.PageIndex,
                PageSize = request.PageSize
            };
        }

        public async Task<TenantResponse> GetTenantByIdAsync(int id)
        {
            var t = await _tenantRepo.GetByIdAsync(id);
            if (t == null) throw new Exception("Không tìm thấy khách hàng.");

            var activeContract = t.Contracts?.FirstOrDefault(c => c.IsActive);

            return new TenantResponse
            {
                TenantId = t.TenantId,
                FullName = t.FullName,
                Phone = t.Phone,
                CCCD = t.CCCD,
                IsActive = t.Account.IsActive,
                IsRenting = activeContract != null,
                CurrentRoom = activeContract?.Room?.RoomNumber ?? "Chưa thuê"
            };
        }

        public async Task CreateTenantAsync(CreateTenantRequest request)
        {
            var existingTenant = await _tenantRepo.GetByCCCDAsync(request.CCCD);
            if (existingTenant != null)
                throw new Exception($"Khách hàng với CCCD {request.CCCD} đã tồn tại trong hệ thống.");

            var tenant = new Tenant
            {
                FullName = request.FullName,
                Phone = request.Phone,
                CCCD = request.CCCD,
            };

            await _tenantRepo.CreateAsync(tenant);
        }


        public async Task ToggleTenantStatusAsync(int id)
        {
            var tenant = await _tenantRepo.GetByIdAsync(id);
            if (tenant == null)
                throw new Exception("Không tìm thấy khách hàng.");

            tenant.Account.IsActive = !tenant.Account.IsActive;

            await _tenantRepo.SaveAsync();
        }

        public async Task<TenantResponse> GetTenantByAccountIdAsync(int accountId)
        {
            var tenant = await _tenantRepo.GetTenantByAccountIdAsync(accountId);
            if (tenant == null)
                throw new Exception("Không tìm thấy hồ sơ khách thuê cho tài khoản này.");

            return new TenantResponse
            {
                TenantId = tenant.TenantId,
                FullName = tenant.FullName,
                CCCD = tenant.CCCD,
                Phone = tenant.Phone,
                Email = tenant.Account?.Email
            };
        }
    }
}

