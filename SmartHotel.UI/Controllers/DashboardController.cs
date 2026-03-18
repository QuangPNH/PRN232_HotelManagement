using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SmartHotel.UI.DTOs.Statistics;
using SmartHotel.UI.Models;
using System.Net.Http.Headers;

namespace SmartHotel.UI.Controllers
{
    [Authorize] 
    public class DashboardController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ApiSettings _apiSettings;

        public DashboardController(
            IHttpClientFactory httpClientFactory,
            IOptions<ApiSettings> apiSettings)
        {
            _httpClientFactory = httpClientFactory;
            _apiSettings = apiSettings.Value;
        }

        public async Task<IActionResult> Index()
        {
            var model = new DashboardViewModel();

            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_apiSettings.AuthBaseUrl);

            var accessToken = await HttpContext.GetTokenAsync("AccessToken");

            if (!string.IsNullOrEmpty(accessToken))
            {
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", accessToken);
            }

            try
            {
                int currentYear = DateTime.Now.Year;
                var resRevenue = await client.GetAsync($"Dashboard/revenue?year={currentYear}");

                if (resRevenue.IsSuccessStatusCode)
                {
                    var data = await resRevenue.Content.ReadFromJsonAsync<List<RevenueStatisticResponse>>();
                    if (data != null) model.RevenueData = data;
                }

                var resOccupancy = await client.GetAsync("Dashboard/occupancy");

                if (resOccupancy.IsSuccessStatusCode)
                {
                    var data = await resOccupancy.Content.ReadFromJsonAsync<RoomStatusStatisticResponse>();
                    if (data != null) model.RoomStatus = data;
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Lỗi tải dữ liệu: " + ex.Message;
            }

            return View(model);
        }
    }
}
