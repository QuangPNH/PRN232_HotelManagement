using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SmartHotel.UI.DTOs;
using SmartHotel.UI.DTOs.Tenant;
using SmartHotel.UI.Models; 
using System.Net.Http.Headers;
using System.Text.Json;


namespace SmartHotel.UI.Controllers
{
    public class TenantController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ApiSettings _apiSettings;
        private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        public TenantController(IHttpClientFactory httpClientFactory, IOptions<ApiSettings> apiSettings)
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
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", accessToken);
            }

            return client;
        }

        public async Task<IActionResult> Index(string? keyword, bool? isActive, int pageIndex = 1, int pageSize = 10)
        {
            var client = await CreateClientAsync();

            var query = $"Tenant?pageIndex={pageIndex}&pageSize={pageSize}";
            if (!string.IsNullOrEmpty(keyword)) query += $"&keyword={Uri.EscapeDataString(keyword)}";
            if (isActive.HasValue) query += $"&isActive={isActive.Value}";

            try
            {
                var response = await client.GetAsync(query);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<PagedResult<TenantResponse>>(_jsonOptions);

                    ViewBag.Keyword = keyword;
                    ViewBag.IsActive = isActive;

                    return View(result);
                }

                TempData["Error"] = "Không thể tải danh sách khách hàng.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi kết nối: {ex.Message}";
            }

            return View(new PagedResult<TenantResponse>());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateTenantRequest request)
        {
            if (!ModelState.IsValid)
            {
                return View(request);
            }

            var client = await CreateClientAsync();

            try
            {
                var response = await client.PostAsJsonAsync("Tenant", request);

                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Thêm mới khách hàng thành công!";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    TempData["Error"] = $"Thêm thất bại: {error}";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi kết nối: {ex.Message}";
            }

            return View(request);
        }

        [HttpPost]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var client = await CreateClientAsync();

            try
            {
                var response = await client.PutAsync($"Tenant/{id}/toggle-status", null); 

                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Cập nhật trạng thái thành công!";
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
    }
}