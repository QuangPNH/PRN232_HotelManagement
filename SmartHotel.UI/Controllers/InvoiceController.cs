using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using SmartHotel.UI.DTOs;
using SmartHotel.UI.DTOs.Contract; 
using SmartHotel.UI.DTOs.Invoice;
using SmartHotel.UI.Models;
using System.Net.Http.Headers;
using System.Text.Json;

namespace SmartHotel.UI.Controllers
{
    public class InvoiceController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ApiSettings _apiSettings;
        private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        public InvoiceController(IHttpClientFactory httpClientFactory, IOptions<ApiSettings> apiSettings)
        {
            _httpClientFactory = httpClientFactory;
            _apiSettings = apiSettings.Value;
        }

        private async Task<HttpClient> CreateClientAsync()
        {
            var client = _httpClientFactory.CreateClient("SmartHotelClient");
            client.BaseAddress = new Uri(_apiSettings.AuthBaseUrl);

            var accessToken = await HttpContext.GetTokenAsync("AccessToken");
            if (!string.IsNullOrEmpty(accessToken))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            }
            return client;
        }

        public async Task<IActionResult> Index(int? roomId, int? month, int? year, bool? isPaid, int pageIndex = 1, int pageSize = 10)
        {
            var client = await CreateClientAsync();

            var query = $"Invoice?pageIndex={pageIndex}&pageSize={pageSize}";
            if (roomId.HasValue) query += $"&roomId={roomId.Value}";
            if (month.HasValue) query += $"&month={month.Value}";
            if (year.HasValue) query += $"&year={year.Value}";
            if (isPaid.HasValue) query += $"&isPaid={isPaid.Value}";

            try
            {
                var response = await client.GetAsync(query);
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<PagedResult<InvoiceResponse>>(_jsonOptions);

                    ViewBag.Month = month;
                    ViewBag.Year = year;
                    ViewBag.IsPaid = isPaid;

                    return View(result);
                }
                TempData["Error"] = "Không thể tải danh sách hóa đơn.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi kết nối: {ex.Message}";
            }

            return View(new PagedResult<InvoiceResponse>());
        }


        public async Task<IActionResult> Details(int id)
        {
            var client = await CreateClientAsync();
            try
            {
                var response = await client.GetAsync($"Invoice/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var invoice = await response.Content.ReadFromJsonAsync<InvoiceResponse>(_jsonOptions);
                    return View(invoice);
                }
                TempData["Error"] = "Không tìm thấy chi tiết hóa đơn.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi kết nối: {ex.Message}";
            }
            return RedirectToAction(nameof(Index));
        }


        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadActiveContractsDropdown();

            var defaultRequest = new CreateInvoiceRequest
            {
                Month = DateTime.Now.Month,
                Year = DateTime.Now.Year
            };
            return View(defaultRequest);
        }


        [HttpPost]
        public async Task<IActionResult> Create(CreateInvoiceRequest request)
        {
            if (!ModelState.IsValid)
            {
                await LoadActiveContractsDropdown();
                return View(request);
            }

            var client = await CreateClientAsync();
            try
            {
                var response = await client.PostAsJsonAsync("Invoice", request);
                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = $"Lập hóa đơn tháng {request.Month}/{request.Year} thành công!";
                    return RedirectToAction(nameof(Index));
                }

                var error = await response.Content.ReadAsStringAsync();
                TempData["Error"] = $"Lỗi khi lập hóa đơn: {error}";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi kết nối: {ex.Message}";
            }

            await LoadActiveContractsDropdown();
            return View(request);
        }


        [HttpPost]
        public async Task<IActionResult> MarkAsPaid(int id)
        {
            var client = await CreateClientAsync();
            try
            {
                var response = await client.PutAsync($"Invoice/{id}/pay", null);
                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Xác nhận thanh toán thành công!";
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    TempData["Error"] = $"Lỗi: {error}";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi kết nối: {ex.Message}";
            }

            var referer = Request.Headers["Referer"].ToString();
            return string.IsNullOrEmpty(referer) ? RedirectToAction(nameof(Index)) : Redirect(referer);
        }


        private async Task LoadActiveContractsDropdown()
        {
            var client = await CreateClientAsync();
            var response = await client.GetAsync("Contract?isActive=true&pageIndex=1&pageSize=200");
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<PagedResult<ContractResponse>>(_jsonOptions);
                ViewBag.Contracts = result?.Items?.Select(c => new SelectListItem
                {
                    Value = c.ContractId.ToString(),
                    Text = $"Phòng {c.RoomNumber} - Khách: {c.TenantName}"
                });
            }
        }
    }
}