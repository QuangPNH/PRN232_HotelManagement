using SmartHotel.BLL.Common;
using SmartHotel.BLL.DTOs.Invoice;
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
    public class InvoiceService : IInvoiceService
    {
        private readonly IInvoiceRepository _invoiceRepo;
        private readonly IContractRepository _contractRepo;
        private readonly IServiceRepository _serviceRepo;
        private readonly IMeterReadingRepository _meterRepo;

        public InvoiceService(
            IInvoiceRepository invoiceRepo,
            IContractRepository contractRepo,
            IServiceRepository serviceRepo,
            IMeterReadingRepository meterRepo)
        {
            _invoiceRepo = invoiceRepo;
            _contractRepo = contractRepo;
            _serviceRepo = serviceRepo;
            _meterRepo = meterRepo;
        }

        public async Task CreateInvoiceAsync(CreateInvoiceRequest request)
        {
            var contract = await _contractRepo.GetByIdAsync(request.ContractId);
            if (contract == null) throw new Exception("Hợp đồng không tồn tại.");
            if (!contract.IsActive) throw new Exception("Hợp đồng đã kết thúc hoặc không hoạt động.");

            bool exists = await _invoiceRepo.IsInvoiceExistsAsync(request.ContractId, request.Month, request.Year);
            if (exists) throw new Exception($"Hóa đơn tháng {request.Month}/{request.Year} của phòng này đã được tạo.");

            var details = new List<InvoiceDetail>();

   
            var (services, _) = await _serviceRepo.GetByFilterAsync("Tiền phòng", null, 1, 1);
            var roomService = services.FirstOrDefault();

            if (roomService == null)
            {
                throw new Exception("Chưa cấu hình dịch vụ 'Tiền phòng' trong hệ thống. Vui lòng vào mục Dịch vụ và tạo mới dịch vụ tên là 'Tiền phòng' (IsMeterBased=false).");
            }

            details.Add(new InvoiceDetail
            {
                ServiceId = roomService.ServiceId, 
                Quantity = 1,
                UnitPrice = contract.Price, 
                Amount = contract.Price * 1
            });

            var allServicesResponse = await _serviceRepo.GetByFilterAsync(null, null, 1, 1000);
            var otherServices = allServicesResponse.Items.Where(s => s.ServiceId != roomService.ServiceId && s.IsActive);

            foreach (var service in otherServices)
            {
                int quantity = 0;
                decimal amount = 0;

                if (service.IsMeterBased) 
                {
                    var (readings, _) = await _meterRepo.GetByFilterAsync(contract.ContractId, request.Month, request.Year, 1, 100);
                    var reading = readings.FirstOrDefault(r => r.ServiceId == service.ServiceId);

                    if (reading == null)
                    {
                        throw new Exception($"Chưa ghi chỉ số '{service.ServiceName}' tháng {request.Month}/{request.Year}. Vui lòng ghi chỉ số trước.");
                    }

                    quantity = reading.NewIndex - reading.OldIndex;
                    amount = quantity * service.UnitPrice;
                }
                else 
                {
                    quantity = 1;
                    amount = service.UnitPrice * 1;
                }

                details.Add(new InvoiceDetail
                {
                    ServiceId = service.ServiceId,
                    Quantity = quantity,
                    UnitPrice = service.UnitPrice,
                    Amount = amount
                });
            }

            var invoice = new Invoice
            {
                ContractId = request.ContractId,
                InvoiceMonth = new DateTime(request.Year, request.Month, 1), 
                TotalAmount = details.Sum(d => d.Amount),
                IsPaid = false,
                PaidDate = null,
                InvoiceDetails = details 
            };

            await _invoiceRepo.CreateAsync(invoice);
        }

        public async Task<PagedResult<InvoiceResponse>> GetInvoicesAsync(InvoiceFilterRequest request)
        {
            var (items, totalCount) = await _invoiceRepo.GetByFilterAsync(
                request.RoomId,
                request.Month,
                request.Year,
                request.IsPaid,
                request.PageIndex,
                request.PageSize
            );

            var resultItems = items.Select(i => new InvoiceResponse
            {
                InvoiceId = i.InvoiceId,
                ContractId = i.ContractId,
                RoomNumber = i.Contract?.Room?.RoomNumber ?? "N/A",
                TenantName = i.Contract?.Tenant?.FullName ?? "N/A",
                InvoiceMonth = i.InvoiceMonth,
                TotalAmount = i.TotalAmount,
                IsPaid = i.IsPaid,
                PaidDate = i.PaidDate,
                InvoiceDetails = i.InvoiceDetails?.Select(d => new InvoiceDetailResponse
                {
                    InvoiceDetailId = d.InvoiceDetailId,
                    ServiceId = d.ServiceId,
                    ServiceName = d.Service?.ServiceName ?? "Unknown", 
                    Quantity = d.Quantity,
                    UnitPrice = d.UnitPrice,
                    Amount = d.Amount
                }).ToList() ?? new List<InvoiceDetailResponse>()
            });

            return new PagedResult<InvoiceResponse>
            {
                Items = resultItems,
                TotalCount = totalCount,
                PageIndex = request.PageIndex,
                PageSize = request.PageSize
            };
        }

        public async Task<InvoiceResponse> GetInvoiceByIdAsync(int id)
        {
            var i = await _invoiceRepo.GetByIdAsync(id);
            if (i == null) throw new Exception("Không tìm thấy hóa đơn.");

            return new InvoiceResponse
            {
                InvoiceId = i.InvoiceId,
                ContractId = i.ContractId,
                RoomNumber = i.Contract?.Room?.RoomNumber ?? "N/A",
                TenantName = i.Contract?.Tenant?.FullName ?? "N/A",
                InvoiceMonth = i.InvoiceMonth,
                TotalAmount = i.TotalAmount,
                IsPaid = i.IsPaid,
                PaidDate = i.PaidDate,
                InvoiceDetails = i.InvoiceDetails.Select(d => new InvoiceDetailResponse
                {
                    InvoiceDetailId = d.InvoiceDetailId,
                    ServiceId = d.ServiceId,
                    ServiceName = d.Service?.ServiceName ?? "Unknown",
                    Quantity = d.Quantity,
                    UnitPrice = d.UnitPrice,
                    Amount = d.Amount
                }).ToList()
            };
        }

        public async Task MarkAsPaidAsync(int id)
        {
            var invoice = await _invoiceRepo.GetByIdAsync(id);
            if (invoice == null) throw new Exception("Không tìm thấy hóa đơn.");
            if (invoice.IsPaid) throw new Exception("Hóa đơn này đã thanh toán rồi.");

            invoice.IsPaid = true;
            invoice.PaidDate = DateTime.Now;

            await _invoiceRepo.UpdateAsync(invoice);
        }

        public async Task<List<InvoiceResponse>> GetInvoicesByTenantIdAsync(int tenantId)
        {
            var invoices = await _invoiceRepo.GetInvoicesByTenantIdAsync(tenantId);

            return invoices.Select(i => new InvoiceResponse
            {
                InvoiceId = i.InvoiceId,
                ContractId = i.ContractId,
                RoomNumber = i.Contract?.Room?.RoomNumber,
                InvoiceMonth = new DateTime(i.InvoiceMonth.Year, i.InvoiceMonth.Month, 1), 
                TotalAmount = i.TotalAmount,
                IsPaid = i.IsPaid,
                PaidDate = i.PaidDate,
            }).ToList();
        }
    }
}
