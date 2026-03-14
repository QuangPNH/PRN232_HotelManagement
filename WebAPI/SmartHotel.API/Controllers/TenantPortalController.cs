using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartHotel.BLL.Services.Interface;

namespace SmartHotel.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Tenant")] // Chỉ cho phép Role là Tenant truy cập
    public class TenantPortalController : ControllerBase
    {
        private readonly ITenantService _tenantService; // Service lấy thông tin Tenant
        private readonly IContractService _contractService; // Service lấy hợp đồng
        private readonly IInvoiceService _invoiceService; // Service lấy hóa đơn

        public TenantPortalController(ITenantService tenantService, IContractService contractService, IInvoiceService invoiceService)
        {
            _tenantService = tenantService;
            _contractService = contractService;
            _invoiceService = invoiceService;
        }

        // --- HELPER: Lấy AccountId từ JWT Token ---
        private int GetCurrentAccountId()
        {
            var accountIdClaim = User.FindFirst("AccountId");
            if (accountIdClaim != null && int.TryParse(accountIdClaim.Value, out int accountId))
            {
                return accountId;
            }
            throw new UnauthorizedAccessException("Không tìm thấy thông tin tài khoản.");
        }

        // ==========================================
        // 1. LẤY HỒ SƠ CÁ NHÂN (MY PROFILE)
        // Method: GET /api/TenantPortal/my-profile
        // ==========================================
        [HttpGet("my-profile")]
        public async Task<IActionResult> GetMyProfile()
        {
            try
            {
                int accountId = GetCurrentAccountId();
                // BẠN CẦN VIẾT HÀM NÀY: Tìm Tenant dựa trên AccountId
                var profile = await _tenantService.GetTenantByAccountIdAsync(accountId);
                return Ok(profile);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // ==========================================
        // 2. LẤY HỢP ĐỒNG CỦA TÔI (MY CONTRACTS)
        // Method: GET /api/TenantPortal/my-contracts
        // ==========================================
        [HttpGet("my-contracts")]
        public async Task<IActionResult> GetMyContracts()
        {
            try
            {
                int accountId = GetCurrentAccountId();
                var tenant = await _tenantService.GetTenantByAccountIdAsync(accountId);

                // BẠN CẦN VIẾT HÀM NÀY: Lấy danh sách hợp đồng theo TenantId
                var contracts = await _contractService.GetContractsByTenantIdAsync(tenant.TenantId);
                return Ok(contracts);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // ==========================================
        // 3. LẤY HÓA ĐƠN CỦA TÔI (MY INVOICES)
        // Method: GET /api/TenantPortal/my-invoices
        // ==========================================
        [HttpGet("my-invoices")]
        public async Task<IActionResult> GetMyInvoices()
        {
            try
            {
                int accountId = GetCurrentAccountId();
                var tenant = await _tenantService.GetTenantByAccountIdAsync(accountId);

                // BẠN CẦN VIẾT HÀM NÀY: Lấy tất cả hóa đơn thuộc các hợp đồng của Tenant này
                var invoices = await _invoiceService.GetInvoicesByTenantIdAsync(tenant.TenantId);
                return Ok(invoices);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // ==========================================
        // 4. LẤY CHI TIẾT 1 HÓA ĐƠN CỦA TÔI
        // Method: GET /api/TenantPortal/my-invoices/{id}
        // ==========================================
        [HttpGet("my-invoices/{id}")]
        public async Task<IActionResult> GetMyInvoiceDetail(int id)
        {
            try
            {
                int accountId = GetCurrentAccountId();
                var tenant = await _tenantService.GetTenantByAccountIdAsync(accountId);

                // Lấy chi tiết hóa đơn (Giả định bạn đã có hàm GetInvoiceByIdAsync trong IInvoiceService)
                var invoice = await _invoiceService.GetInvoiceByIdAsync(id);

                if (invoice == null)
                    return NotFound(new { message = "Không tìm thấy hóa đơn." });

                // BẢO MẬT: Kiểm tra xem hóa đơn này có thuộc về Hợp đồng của vị khách này không?
                // Cần đảm bảo DTO InvoiceResponse hoặc logic của bạn có thông tin ContractId -> TenantId
                // Dưới đây là cách check đơn giản nếu bạn có sẵn danh sách hợp đồng của khách:
                var myContracts = await _contractService.GetContractsByTenantIdAsync(tenant.TenantId);
                if (!myContracts.Any(c => c.ContractId == invoice.ContractId))
                {
                    return Forbid("Bạn không có quyền xem hóa đơn này.");
                }

                return Ok(invoice);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
