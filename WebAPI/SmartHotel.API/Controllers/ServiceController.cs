using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartHotel.BLL.DTOs.Service;
using SmartHotel.BLL.Services.Interface;

namespace SmartHotel.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Yêu cầu đăng nhập
    public class ServiceController : ControllerBase
    {
        private readonly IServiceManagementService _service;

        public ServiceController(IServiceManagementService service)
        {
            _service = service;
        }

        // ==========================================
        // 1. LẤY DANH SÁCH (CÓ PHÂN TRANG & SEARCH)
        // Dùng cho màn hình Quản lý danh mục
        // ==========================================
        [HttpGet]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> GetServices([FromQuery] string? keyword, [FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 6)
        {
            try
            {
                var result = await _service.GetServicesAsync(keyword, pageIndex, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // ==========================================
        // 2. LẤY TẤT CẢ DỊCH VỤ ĐANG HOẠT ĐỘNG
        // Dùng cho Dropdownlist khi Tạo Hợp đồng hoặc Ghi điện nước
        // ==========================================
        [HttpGet("active")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> GetAllActive()
        {
            try
            {
                var result = await _service.GetAllActiveServicesAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // ==========================================
        // 3. TẠO DỊCH VỤ MỚI
        // Chỉ Admin mới được quyền định giá
        // ==========================================
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CreateServiceRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                await _service.CreateServiceAsync(request);
                return StatusCode(201, new { message = "Thêm dịch vụ thành công!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message }); // Lỗi trùng tên...
            }
        }

        // ==========================================
        // 4. CẬP NHẬT GIÁ/TÊN DỊCH VỤ
        // Chỉ Admin
        // ==========================================
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] CreateServiceRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                await _service.UpdateServiceAsync(id, request);
                return Ok(new { message = "Cập nhật dịch vụ thành công!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // ==========================================
        // 5. XÓA DỊCH VỤ
        // Chỉ Admin
        // ==========================================
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _service.DeleteServiceAsync(id);
                return Ok(new { message = "Xóa dịch vụ thành công!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var service = await _service.GetServiceByIdAsync(id);
                return Ok(service);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

    }
}
