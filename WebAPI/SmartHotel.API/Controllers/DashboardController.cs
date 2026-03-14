using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartHotel.BLL.Services.Interface;

namespace SmartHotel.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class DashboardController : ControllerBase
    {
        private readonly IStatisticsService _statService;

        public DashboardController(IStatisticsService statService)
        {
            _statService = statService;
        }

        [HttpGet("revenue")]
        public async Task<IActionResult> GetRevenue([FromQuery] int year)
        {
            if (year == 0) year = DateTime.Now.Year;
            var result = await _statService.GetRevenueStatisticAsync(year);
            return Ok(result);
        }

        [HttpGet("occupancy")]
        public async Task<IActionResult> GetOccupancy()
        {
            var result = await _statService.GetRoomOccupancyAsync();
            return Ok(result);
        }
    }
}
