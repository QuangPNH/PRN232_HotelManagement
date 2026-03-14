using Azure.Core;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SmartHotel.UI.DTOs;
using SmartHotel.UI.DTOs.Service;
using SmartHotel.UI.Models;
using System.Net.Http.Headers;
using System.Text.Json;

namespace SmartHotel.UI.Controllers
{
    public class ServiceController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ApiSettings _apiSettings;
        private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        public ServiceController(IHttpClientFactory httpClientFactory, IOptions<ApiSettings> apiSettings)
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

        [HttpGet]
        public async Task<IActionResult> Index(string keyword = "", int pageIndex = 1, int pageSize = 6)
        {
            var result = new PagedResult<ServiceResponse>
            {
                PageIndex = pageIndex,
                PageSize = pageSize
            };

            try
            {
                var client = await CreateClientAsync();

                var response = await client.GetAsync($"Service?keyword={keyword}&pageIndex={pageIndex}&pageSize={pageSize}");

                if (response.IsSuccessStatusCode)
                {
                    result = await response.Content.ReadFromJsonAsync<PagedResult<ServiceResponse>>(_jsonOptions);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return RedirectToAction("Login", "Auth");
                }
            }
            catch
            {
                TempData["Error"] = "Lỗi kết nối Server";
            }

            ViewBag.Keyword = keyword;

            return View(result);
        }

        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(CreateServiceRequest request)
        {
            if (!ModelState.IsValid) return View(request);
            try
            {
                var client = await CreateClientAsync();
                var response = await client.PostAsJsonAsync("Service", request);

                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Thêm dịch vụ thành công!";
                    return RedirectToAction("Index");
                }
                ViewBag.Error = await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex) { ViewBag.Error = ex.Message; }
            return View(request);
        }


        public async Task<IActionResult> Edit(int id )
        {


            try
            {
                var client = await CreateClientAsync();
                var response = await client.GetAsync($"Service/{id}");

                if (response.IsSuccessStatusCode)
                {
                    var serviceData = await response.Content.ReadFromJsonAsync<ServiceResponse>(_jsonOptions);

                    var model = new CreateServiceRequest
                    {
                        ServiceName = serviceData.ServiceName,
                        UnitPrice = serviceData.UnitPrice,
                        Unit = serviceData.Unit,
                        IsMeterBased = serviceData.IsMeterBased
                    };

                    ViewBag.ServiceId = id;
                    return View(model);
                }
                else
                {
                    TempData["Error"] = "Không tìm thấy dịch vụ.";
                }
            }
            catch
            {
                TempData["Error"] = "Lỗi kết nối đến server.";
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CreateServiceRequest request)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ServiceId = id; 
                return View(request);
            }

            try
            {
                var client = await CreateClientAsync();
                var response = await client.PutAsJsonAsync($"Service/{id}", request);

                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Cập nhật dịch vụ thành công!";
                    return RedirectToAction("Index");
                }

                ViewBag.Error = await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Lỗi hệ thống: " + ex.Message;
            }

            ViewBag.ServiceId = id;
            return View(request);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var client = await CreateClientAsync();
                await client.DeleteAsync($"Service/{id}");
                TempData["Success"] = "Đã xóa dịch vụ";
            }
            catch { TempData["Error"] = "Lỗi xóa dịch vụ"; }
            return RedirectToAction("Index");
        }
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var client = await CreateClientAsync();
                var response = await client.GetAsync($"Service/{id}");

                if (response.IsSuccessStatusCode)
                {
                    var model = await response.Content.ReadFromJsonAsync<ServiceResponse>(_jsonOptions);
                    return View(model);
                }
                else
                {
                    TempData["Error"] = "Không tìm thấy dịch vụ.";
                }
            }
            catch
            {
                TempData["Error"] = "Lỗi kết nối server.";
            }
            return RedirectToAction("Index");
        }
    }
}