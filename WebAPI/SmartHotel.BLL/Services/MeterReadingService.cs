using SmartHotel.BLL.Common;
using SmartHotel.BLL.DTOs.MeterReading;
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
    public class MeterReadingService : IMeterReadingService
    {
        private readonly IMeterReadingRepository _readingRepo;
        private readonly IContractRepository _contractRepo;
        private readonly IServiceRepository _serviceRepo;

        public MeterReadingService(
            IMeterReadingRepository readingRepo,
            IContractRepository contractRepo,
            IServiceRepository serviceRepo)
        {
            _readingRepo = readingRepo;
            _contractRepo = contractRepo;
            _serviceRepo = serviceRepo;
        }

        public async Task RecordReadingAsync(CreateMeterReadingRequest request)
        {
            var contract = await _contractRepo.GetByIdAsync(request.ContractId);
            if (contract == null) throw new Exception("Hợp đồng không tồn tại.");
            if (!contract.IsActive) throw new Exception("Hợp đồng này đã kết thúc, không thể ghi chỉ số.");

            var service = await _serviceRepo.GetByIdAsync(request.ServiceId);
            if (service == null) throw new Exception("Dịch vụ không tồn tại.");
            if (!service.IsMeterBased) throw new Exception($"Dịch vụ '{service.ServiceName}' không tính theo đồng hồ (MeterBased = false).");

            bool isExists = await _readingRepo.IsReadingExistsAsync(
                request.ContractId,
                request.ServiceId,
                request.ReadingDate.Month,
                request.ReadingDate.Year);

            if (isExists)
            {
                throw new Exception($"Dịch vụ {service.ServiceName} tháng {request.ReadingDate.Month}/{request.ReadingDate.Year} đã được ghi chỉ số rồi.");
            }

            var previousReading = await _readingRepo.GetPreviousReadingAsync(request.ContractId, request.ServiceId);

            int oldIndex = 0;
            if (previousReading != null)
            {
                oldIndex = previousReading.NewIndex;
            }

            if (request.NewIndex < oldIndex)
            {
                throw new Exception($"Chỉ số mới ({request.NewIndex}) không được nhỏ hơn chỉ số cũ ({oldIndex}).");
            }
            if (request.ReadingDate > DateTime.Now)
            {
                throw new Exception("Không được ghi chỉ số cho ngày trong tương lai.");
            }

            var reading = new MeterReading
            {
                ContractId = request.ContractId,
                ServiceId = request.ServiceId,
                OldIndex = oldIndex,
                NewIndex = request.NewIndex,
                ReadingDate = request.ReadingDate
            };

            await _readingRepo.CreateAsync(reading);
        }


        public async Task<PagedResult<MeterReadingResponse>> GetReadingsAsync(MeterReadingFilterRequest request)
        {
            var (items, totalCount) = await _readingRepo.GetByFilterAsync(
                request.ContractId,
                request.Month,
                request.Year,
                request.PageIndex,
                request.PageSize);

            var readingDtos = items.Select(m => new MeterReadingResponse
            {
                MeterReadingId = m.MeterReadingId,
                ContractId = m.ContractId,
                RoomNumber = m.Contract?.Room?.RoomNumber ?? "Unknown",
                ServiceName = m.Service?.ServiceName ?? "Unknown",
                OldIndex = m.OldIndex,
                NewIndex = m.NewIndex,
                ReadingDate = m.ReadingDate
            });

            return new PagedResult<MeterReadingResponse>
            {
                Items = readingDtos,
                TotalCount = totalCount,
                PageIndex = request.PageIndex,
                PageSize = request.PageSize
            };
        }

        public async Task DeleteReadingAsync(int id)
        {

            await _readingRepo.DeleteAsync(id);
        }
    }
}
