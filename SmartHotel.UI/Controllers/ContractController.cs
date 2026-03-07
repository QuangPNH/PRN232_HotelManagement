using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using SmartHotel.UI.DTOs;
using SmartHotel.UI.DTOs.Contract;
using SmartHotel.UI.Models;
using System.Net.Http.Headers;
using System.Text.Json;

namespace SmartHotel.UI.Controllers
{
    public class ContractController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ApiSettings _apiSettings;
        private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        public ContractController(IHttpClientFactory httpClientFactory, IOptions<ApiSettings> apiSettings)
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
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            return client;
        }

        public async Task<IActionResult> Index(string keyword = "", int pageIndex = 1, int pageSize = 6)
        {
            var result = new PagedResult<ContractResponse> { PageIndex = pageIndex, PageSize = pageSize };
            try
            {
                var client = await CreateClientAsync();
                var response = await client.GetAsync($"Contract?keyword={keyword}&pageIndex={pageIndex}&pageSize={pageSize}");

                if (response.IsSuccessStatusCode)
                    result = await response.Content.ReadFromJsonAsync<PagedResult<ContractResponse>>(_jsonOptions);
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    return RedirectToAction("Login", "Auth");
            }
            catch { TempData["Error"] = "Lỗi kết nối API"; }

            ViewBag.Keyword = keyword;
            return View(result);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadComboboxData(); 
            return View(new CreateContractRequest { StartDate = DateTime.Now });
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateContractRequest request)
        {
            if (!ModelState.IsValid)
            {
                await LoadComboboxData();
                return View(request);
            }

            try
            {
                var client = await CreateClientAsync();
                var response = await client.PostAsJsonAsync("Contract", request);

                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Tạo hợp đồng thành công!";
                    return RedirectToAction("Index");
                }

                var errorMsg = await response.Content.ReadAsStringAsync();
                ViewBag.Error = ExtractMessage(errorMsg);
            }
            catch (Exception ex) { ViewBag.Error = ex.Message; }

            await LoadComboboxData();
            return View(request);
        }

        [HttpPost]
        public async Task<IActionResult> Terminate(int id)
        {
            try
            {
                var client = await CreateClientAsync();
                var response = await client.PutAsync($"Contract/{id}/terminate", null);

                if (response.IsSuccessStatusCode)
                    TempData["Success"] = "Đã thanh lý hợp đồng & trả phòng!";
                else
                    TempData["Error"] = "Lỗi: " + ExtractMessage(await response.Content.ReadAsStringAsync());
            }
            catch { TempData["Error"] = "Lỗi kết nối server"; }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var client = await CreateClientAsync();
                var response = await client.GetAsync($"Contract/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadFromJsonAsync<ContractResponse>(_jsonOptions);
                    return View(data);
                }
            }
            catch { }
            return RedirectToAction("Index");
        }

        private async Task LoadComboboxData()
        {
            var client = await CreateClientAsync();

            var roomsList = new List<SelectListItem>();
            try
            {
                var roomResponse = await client.GetAsync("Room/lookup");

                if (roomResponse.IsSuccessStatusCode)
                {
                    var data = await roomResponse.Content.ReadFromJsonAsync<List<LookupDto>>(_jsonOptions);
                    if (data != null)
                    {
                        roomsList = data.Select(r => new SelectListItem
                        {
                            Value = r.Id.ToString(),
                            Text = $"Phòng {r.Name} - {r.ExtraInfo} VNĐ" 
                        }).ToList();
                    }
                }
            }
            catch {  }

          
            if (!roomsList.Any())
            {
                roomsList.Add(new SelectListItem("--- Hết phòng trống ---", ""));
            }

            ViewBag.Rooms = roomsList;


            var tenantsList = new List<SelectListItem>();
            try
            {
                var tenantResponse = await client.GetAsync("Tenant/lookup");

                if (tenantResponse.IsSuccessStatusCode)
                {
                    var data = await tenantResponse.Content.ReadFromJsonAsync<List<LookupDto>>(_jsonOptions);
                    if (data != null)
                    {
                        tenantsList = data.Select(t => new SelectListItem
                        {
                            Value = t.Id.ToString(),
                            Text = $"{t.Name} ({t.ExtraInfo})"
                        }).ToList();
                    }
                }
            }
            catch 
                
                { }

            ViewBag.Tenants = tenantsList;
        }

        private string ExtractMessage(string json)
        {
            try { return JsonDocument.Parse(json).RootElement.GetProperty("message").GetString(); }
            catch { return json; }
        }

        public class LookupDto
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string? ExtraInfo { get; set; } 
        }
    }
}