using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartHotel.BLL.DTOs.Tenant;
using SmartHotel.BLL.Services.Interface;

namespace SmartHotel.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TenantController : ControllerBase
    {
        private readonly ITenantService _tenantService;
   
        public TenantController(ITenantService tenantService)
        {
            _tenantService = tenantService;
        }

        [HttpGet("lookup")]
        public async Task<IActionResult> GetTenantLookup()
        {
            var data = await _tenantService.GetTenantLookupAsync();
            return Ok(data);
        }

        [HttpGet]
        public async Task<IActionResult> GetTenants([FromQuery] TenantFilterRequest request)
        {
            try
            {
                var result = await _tenantService.GetTenantsAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTenantById(int id)
        {
            try
            {
                var result = await _tenantService.GetTenantByIdAsync(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateTenant([FromBody] CreateTenantRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                await _tenantService.CreateTenantAsync(request);
                return StatusCode(201, new { message = "Thêm khách hàng thành công." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}/toggle-status")]
        [Authorize(Roles = "Admin")] 
        public async Task<IActionResult> ToggleTenantStatus(int id)
        {
            try
            {
                await _tenantService.ToggleTenantStatusAsync(id);
                return Ok(new { message = "Đã cập nhật trạng thái khách hàng." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
