using SmartHotel.BLL.Common;
using SmartHotel.BLL.DTOs.Service;
using SmartHotel.BLL.Services.Interface;
using SmartHotel.DAL.Models;
using SmartHotel.DAL.Repositories.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHotel.BLL.Services
{
    public class ServiceManagementService : IServiceManagementService
    {
        private readonly IServiceRepository _serviceRepo;

        public ServiceManagementService(IServiceRepository serviceRepo)
        {
            _serviceRepo = serviceRepo;
        }

        public async Task<IEnumerable<ServiceResponse>> GetAllActiveServicesAsync()
        {
            var (items, _) = await _serviceRepo.GetByFilterAsync(null, null, 1, 1000);
            return items.Where(x => x.IsActive).Select(s => MapToResponse(s));
        }

        public async Task<PagedResult<ServiceResponse>> GetServicesAsync(string? keyword, int pageIndex, int pageSize)
        {
            var (items, totalCount) = await _serviceRepo.GetByFilterAsync(keyword, null, pageIndex, pageSize);

            return new PagedResult<ServiceResponse>
            {
                Items = items.Select(s => MapToResponse(s)),
                TotalCount = totalCount,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
        }

        public async Task CreateServiceAsync(CreateServiceRequest request)
        {
            if (await _serviceRepo.IsNameExistsAsync(request.ServiceName))
                throw new Exception($"Dịch vụ '{request.ServiceName}' đã tồn tại.");

            var service = new Service
            {
                ServiceName = request.ServiceName,
                UnitPrice = request.UnitPrice,
                Unit = request.Unit,
                IsMeterBased = request.IsMeterBased,
                IsActive = true
            };

            await _serviceRepo.CreateAsync(service);
        }

        public async Task UpdateServiceAsync(int id, CreateServiceRequest request)
        {
            var service = await _serviceRepo.GetByIdAsync(id);
            if (service == null) throw new Exception("Dịch vụ không tồn tại.");

            service.ServiceName = request.ServiceName;
            service.UnitPrice = request.UnitPrice;
            service.Unit = request.Unit;
            service.IsMeterBased = request.IsMeterBased;

            await _serviceRepo.UpdateAsync(service);
        }

        public async Task DeleteServiceAsync(int id)
        {
            await _serviceRepo.DeleteAsync(id);
        }

        private ServiceResponse MapToResponse(Service s)
        {
            return new ServiceResponse
            {
                ServiceId = s.ServiceId,
                ServiceName = s.ServiceName,
                UnitPrice = s.UnitPrice,
                Unit = s.Unit,
                IsMeterBased = s.IsMeterBased,
                IsActive = s.IsActive
            };
        }

        public async Task<ServiceResponse> GetServiceByIdAsync(int id)
        {
            var service = await _serviceRepo.GetByIdAsync(id);
            if (service == null)
                throw new Exception("Dịch vụ không tồn tại.");

            return MapToResponse(service);
        }

    }
}
