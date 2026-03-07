using SmartHotel.BLL.Common;
using SmartHotel.BLL.DTOs.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHotel.BLL.Services.Interface
{
    public interface IContractService
    {
        Task CreateContractAsync(CreateContractRequest request);

        Task TerminateContractAsync(int contractId);

        Task<PagedResult<ContractResponse>> GetContractsAsync(ContractFilterRequest request);

        Task<ContractResponse> GetContractByIdAsync(int id);

        Task<List<ContractResponse>> GetContractsByTenantIdAsync(int tenantId);
    }
}
