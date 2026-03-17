using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SmartHotel.UI.DTOs.Contract;
using SmartHotel.UI.DTOs.Invoice;
using SmartHotel.UI.DTOs.Tenant;
using SmartHotel.UI.Models;
using System.Net.Http.Headers;
using System.Text.Json;

namespace SmartHotel.UI.Controllers
{
    [Authorize(Roles = "Tenant")] 
    public class TenantPortalController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ApiSettings _apiSettings;
        private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        public TenantPortalController(IHttpClientFactory httpClientFactory, IOptions<ApiSettings> apiSettings)
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

        public async Task<IActionResult> Dashboard()
        {
            var client = await CreateClientAsync();
            var model = new TenantDashboardViewModel();

            try
            {
                var profileTask = client.GetAsync("TenantPortal/my-profile");
                var contractsTask = client.GetAsync("TenantPortal/my-contracts");
                var invoicesTask = client.GetAsync("TenantPortal/my-invoices");

                await Task.WhenAll(profileTask, contractsTask, invoicesTask);

                if (contractsTask.Result.IsSuccessStatusCode)
                {
                    model.Contracts = await contractsTask.Result.Content.ReadFromJsonAsync<List<ContractResponse>>(_jsonOptions);
                }

                if (invoicesTask.Result.IsSuccessStatusCode)
                {
                    model.Invoices = await invoicesTask.Result.Content.ReadFromJsonAsync<List<InvoiceResponse>>(_jsonOptions);
                }

                if (profileTask.Result.IsSuccessStatusCode)
                {
                    model.Profile = await profileTask.Result.Content.ReadFromJsonAsync<dynamic>(_jsonOptions);
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi khi tải dữ liệu: " + ex.Message;
            }

            return View(model);
        }

        public async Task<IActionResult> InvoiceDetails(int id)
        {
            var client = await CreateClientAsync();
            var response = await client.GetAsync($"TenantPortal/my-invoices/{id}");

            if (response.IsSuccessStatusCode)
            {
                var invoice = await response.Content.ReadFromJsonAsync<InvoiceResponse>(_jsonOptions);
                return View(invoice);
            }

            TempData["Error"] = "Không thể tải chi tiết hóa đơn hoặc bạn không có quyền truy cập.";
            return RedirectToAction("Dashboard");
        }
    }
}
