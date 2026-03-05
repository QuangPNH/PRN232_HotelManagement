using SmartHotel.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHotel.DAL.Repositories.Interface
{
    public interface IContractRepository
    {
        Task<IEnumerable<Contract>> GetAllAsync();
        Task<Contract?> GetByIdAsync(int id);
        Task CreateAsync(Contract contract);
        Task UpdateAsync(Contract contract); 
        Task<Contract?> GetActiveContractByRoomIdAsync(int roomId);
        Task<IEnumerable<Contract>> GetContractsByTenantIdAsync(int tenantId);
        Task<(IEnumerable<Contract> Items, int TotalCount)> GetByFilterAsync(string? keyword, bool? isActive, int pageIndex, int pageSize);
        Task<List<Contract>> GetContractsByTenantAsync(int tenantId);
    }
}
