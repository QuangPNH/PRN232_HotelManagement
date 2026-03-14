using SmartHotel.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHotel.DAL.Repositories.Interface
{
    public interface IServiceRepository
    {
        Task<Service?> GetByIdAsync(int id);
        Task CreateAsync(Service service);
        Task UpdateAsync(Service service);
        Task DeleteAsync(int id); 

        Task<(IEnumerable<Service> Items, int TotalCount)> GetByFilterAsync(
            string? keyword,
            bool? isMeterBased,
            int pageIndex,
            int pageSize);

        Task<bool> IsNameExistsAsync(string name);
    }
}
