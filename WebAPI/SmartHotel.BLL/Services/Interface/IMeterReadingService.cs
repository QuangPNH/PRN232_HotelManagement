using SmartHotel.BLL.Common;
using SmartHotel.BLL.DTOs.MeterReading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHotel.BLL.Services.Interface
{
    public interface IMeterReadingService
    {
        Task RecordReadingAsync(CreateMeterReadingRequest request);

        Task<PagedResult<MeterReadingResponse>> GetReadingsAsync(MeterReadingFilterRequest request);
        Task DeleteReadingAsync(int id);
    }
}
