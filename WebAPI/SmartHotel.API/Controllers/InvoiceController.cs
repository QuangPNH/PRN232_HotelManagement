using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartHotel.BLL.DTOs.Invoice;
using SmartHotel.BLL.Services.Interface;

namespace SmartHotel.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] 
    public class InvoiceController : ControllerBase
    {
        private readonly IInvoiceService _invoiceService;

        public InvoiceController(IInvoiceService invoiceService)
        {
            _invoiceService = invoiceService;
        }

  
        [HttpPost]
        [Authorize(Roles = "Admin,Staff")] 
        public async Task<IActionResult> CreateInvoice([FromBody] CreateInvoiceRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _invoiceService.CreateInvoiceAsync(request);
                return StatusCode(201, new { message = $"Đã lập hóa đơn tháng {request.Month}/{request.Year} thành công!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> GetInvoices([FromQuery] InvoiceFilterRequest request)
        {
            try
            {
                var result = await _invoiceService.GetInvoicesAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> GetInvoiceById(int id)
        {
            try
            {
                var invoice = await _invoiceService.GetInvoiceByIdAsync(id);
                return Ok(invoice);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }


        [HttpPut("{id}/pay")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> MarkAsPaid(int id)
        {
            try
            {
                await _invoiceService.MarkAsPaidAsync(id);
                return Ok(new { message = "Xác nhận thanh toán thành công!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
