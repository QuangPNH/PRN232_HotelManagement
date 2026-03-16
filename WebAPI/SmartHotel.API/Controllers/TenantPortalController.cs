using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartHotel.BLL.Services.Interface;

namespace SmartHotel.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Tenant")] 
    public class TenantPortalController : ControllerBase
    {
        private readonly ITenantService _tenantService;
        private readonly IContractService _contractService; 
        private readonly IInvoiceService _invoiceService; 

        public TenantPortalController(ITenantService tenantService, IContractService contractService, IInvoiceService invoiceService)
        {
            _tenantService = tenantService;
            _contractService = contractService;
            _invoiceService = invoiceService;
        }

        private int GetCurrentAccountId()
        {
            var accountIdClaim = User.FindFirst("AccountId");
            if (accountIdClaim != null && int.TryParse(accountIdClaim.Value, out int accountId))
            {
                return accountId;
            }
            throw new UnauthorizedAccessException("Không tìm thấy thông tin tài khoản.");
        }

        [HttpGet("my-profile")]
        public async Task<IActionResult> GetMyProfile()
        {
            try
            {
                int accountId = GetCurrentAccountId();
                var profile = await _tenantService.GetTenantByAccountIdAsync(accountId);
                return Ok(profile);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("my-contracts")]
        public async Task<IActionResult> GetMyContracts()
        {
            try
            {
                int accountId = GetCurrentAccountId();
                var tenant = await _tenantService.GetTenantByAccountIdAsync(accountId);

                var contracts = await _contractService.GetContractsByTenantIdAsync(tenant.TenantId);
                return Ok(contracts);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("my-invoices")]
        public async Task<IActionResult> GetMyInvoices()
        {
            try
            {
                int accountId = GetCurrentAccountId();
                var tenant = await _tenantService.GetTenantByAccountIdAsync(accountId);

                var invoices = await _invoiceService.GetInvoicesByTenantIdAsync(tenant.TenantId);
                return Ok(invoices);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        [HttpGet("my-invoices/{id}")]
        public async Task<IActionResult> GetMyInvoiceDetail(int id)
        {
            try
            {
                int accountId = GetCurrentAccountId();
                var tenant = await _tenantService.GetTenantByAccountIdAsync(accountId);

                var invoice = await _invoiceService.GetInvoiceByIdAsync(id);

                if (invoice == null)
                    return NotFound(new { message = "Không tìm thấy hóa đơn." });

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
