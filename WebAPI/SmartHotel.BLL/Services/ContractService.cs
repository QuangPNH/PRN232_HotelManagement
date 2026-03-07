using SmartHotel.BLL.Common;
using SmartHotel.BLL.DTOs.Contract;
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
    public class ContractService : IContractService
    {
        private readonly IContractRepository _contractRepo;
        private readonly IRoomRepository _roomRepo;
        private readonly ITenantRepository _tenantRepo;

        public ContractService(
            IContractRepository contractRepo,
            IRoomRepository roomRepo,
            ITenantRepository tenantRepo)
        {
            _contractRepo = contractRepo;
            _roomRepo = roomRepo;
            _tenantRepo = tenantRepo;
        }


        public async Task CreateContractAsync(CreateContractRequest request)
        {
            if (request.StartDate >= request.EndDate)
                throw new Exception("Ngày kết thúc phải sau ngày bắt đầu.");

            var room = await _roomRepo.GetRoomByIdAsync(request.RoomId);
            if (room == null) throw new Exception("Phòng không tồn tại.");


            if (room.Status == RoomStatus.Occupied)
                throw new Exception($"Phòng {room.RoomNumber} đang có người ở.");

            var activeContract = await _contractRepo.GetActiveContractByRoomIdAsync(request.RoomId);
            if (activeContract != null)
                throw new Exception($"Phòng {room.RoomNumber} đang có hợp đồng hiệu lực (ID: {activeContract.ContractId}).");

            var tenant = await _tenantRepo.GetByIdAsync(request.TenantId);
            if (tenant == null) throw new Exception("Khách thuê không tồn tại.");

            var contract = new Contract
            {
                TenantId = request.TenantId,
                RoomId = request.RoomId,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                Price = request.Price,
                DepositAmount = request.DepositAmount,
                IsActive = true,
                CreatedAt = DateTime.Now
            };

            await _contractRepo.CreateAsync(contract);


            await _roomRepo.UpdateRoomStatusAsync(room.RoomId, RoomStatus.Occupied);
        }


        public async Task TerminateContractAsync(int contractId)
        {
            var contract = await _contractRepo.GetByIdAsync(contractId);
            if (contract == null) throw new Exception("Hợp đồng không tồn tại.");

            if (!contract.IsActive) throw new Exception("Hợp đồng này đã kết thúc rồi.");

            contract.IsActive = false;
            contract.EndDate = DateTime.Now; 

            await _contractRepo.UpdateAsync(contract);


            await _roomRepo.UpdateRoomStatusAsync(contract.RoomId, RoomStatus.Available);
        }


        public async Task<PagedResult<ContractResponse>> GetContractsAsync(ContractFilterRequest request)
        {
            var (items, totalCount) = await _contractRepo.GetByFilterAsync(
                request.Keyword,
                request.IsActive,
                request.PageIndex,
                request.PageSize
            );

            var contractDtos = items.Select(c => new ContractResponse
            {
                ContractId = c.ContractId,
                RoomId = c.RoomId,
                RoomNumber = c.Room?.RoomNumber ?? "Unknown",
                TenantId = c.TenantId,
                TenantName = c.Tenant?.FullName ?? "Unknown",
                TenantPhone = c.Tenant?.Phone,
                Price = c.Price,
                DepositAmount = c.DepositAmount,
                StartDate = c.StartDate,
                EndDate = c.EndDate,
                IsActive = c.IsActive
            });

            return new PagedResult<ContractResponse>
            {
                Items = contractDtos,
                TotalCount = totalCount,
                PageIndex = request.PageIndex,
                PageSize = request.PageSize
            };
        }

        public async Task<ContractResponse> GetContractByIdAsync(int id)
        {
            var c = await _contractRepo.GetByIdAsync(id);
            if (c == null) throw new Exception("Không tìm thấy hợp đồng.");

            return new ContractResponse
            {
                ContractId = c.ContractId,
                RoomId = c.RoomId,
                RoomNumber = c.Room?.RoomNumber ?? "Unknown",
                TenantId = c.TenantId,
                TenantName = c.Tenant?.FullName ?? "Unknown",
                TenantPhone = c.Tenant?.Phone,
                Price = c.Price,
                DepositAmount = c.DepositAmount,
                StartDate = c.StartDate,
                EndDate = c.EndDate,
                IsActive = c.IsActive
            };
        }
        public async Task<List<ContractResponse>> GetContractsByTenantIdAsync(int tenantId)
        {
            var contracts = await _contractRepo.GetContractsByTenantAsync(tenantId);

            return contracts.Select(c => new ContractResponse
            {
                ContractId = c.ContractId,
                RoomId = c.RoomId,
                RoomNumber = c.Room?.RoomNumber, 
                TenantId = c.TenantId,
                StartDate = c.StartDate,
                EndDate = c.EndDate,
                Price = c.Price,
                DepositAmount = c.DepositAmount,
                IsActive = c.IsActive
            }).ToList();
        }
    }
}
