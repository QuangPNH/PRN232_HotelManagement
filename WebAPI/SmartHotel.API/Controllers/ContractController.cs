using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartHotel.BLL.DTOs.Contract;
using SmartHotel.BLL.Services.Interface;

namespace SmartHotel.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] 
    public class ContractController : ControllerBase
    {
        private readonly IContractService _contractService;

        public ContractController(IContractService contractService)
        {
            _contractService = contractService;
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> CreateContract([FromBody] CreateContractRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                await _contractService.CreateContractAsync(request);
                return StatusCode(201, new { message = "Tạo hợp đồng thành công! Phòng đã chuyển sang trạng thái Đang ở." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        [HttpPut("{id}/terminate")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> TerminateContract(int id)
        {
            try
            {
                await _contractService.TerminateContractAsync(id);
                return Ok(new { message = "Thanh lý hợp đồng thành công! Phòng đã được trả." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        [HttpGet]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> GetContracts([FromQuery] ContractFilterRequest request)
        {

            try
            {
                var result = await _contractService.GetContractsAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> GetContractById(int id)
        {
            try
            {
                var contract = await _contractService.GetContractByIdAsync(id);
                return Ok(contract);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}
