using SmartHotel.DAL.Models;

namespace SmartHotel.DAL.Repositories.Interface
{
    public interface IMeterReadingRepository
    {
        Task<MeterReading?> GetByIdAsync(int id);
        Task CreateAsync(MeterReading reading);
        Task UpdateAsync(MeterReading reading);
        Task DeleteAsync(int id);

        Task<(IEnumerable<MeterReading> Items, int TotalCount)> GetByFilterAsync(
            int? contractId,
            int? month,
            int? year,
            int pageIndex,
            int pageSize);

        Task<bool> IsReadingExistsAsync(int contractId, int serviceId, int month, int year);

        Task<MeterReading?> GetPreviousReadingAsync(int contractId, int serviceId);
    }
}
