using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartHotel.BLL.DTOs.MeterReading;
using SmartHotel.BLL.Services.Interface;

namespace SmartHotel.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MeterReadingController : ControllerBase
    {
        private readonly IMeterReadingService _meterService;

        public MeterReadingController(IMeterReadingService meterService)
        {
            _meterService = meterService;
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> RecordReading([FromBody] CreateMeterReadingRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                await _meterService.RecordReadingAsync(request);
                return StatusCode(201, new { message = "Ghi chỉ số thành công!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        [HttpGet]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> GetReadings([FromQuery] MeterReadingFilterRequest request)
        {

            try
            {
                var result = await _meterService.GetReadingsAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteReading(int id)
        {
            try
            {
                await _meterService.DeleteReadingAsync(id);
                return Ok(new { message = "Xóa chỉ số thành công!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
