using SmartHotel.BLL.Common;
using SmartHotel.BLL.DTOs.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHotel.BLL.Services.Interface
{
    public interface IServiceManagementService
    {
        Task<IEnumerable<ServiceResponse>> GetAllActiveServicesAsync();
        Task<PagedResult<ServiceResponse>> GetServicesAsync(string? keyword, int pageIndex, int pageSize);
        Task CreateServiceAsync(CreateServiceRequest request);
        Task UpdateServiceAsync(int id, CreateServiceRequest request);
        Task DeleteServiceAsync(int id);

        Task<ServiceResponse> GetServiceByIdAsync(int id);

    }
}
