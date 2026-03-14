using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SmartHotel.UI.DTOs.Statistics;
using SmartHotel.UI.Models;
using System.Net.Http.Headers;

namespace SmartHotel.UI.Controllers
{
    [Authorize] // Bắt buộc đăng nhập mới vào được Dashboard
    public class DashboardController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ApiSettings _apiSettings; // Class chứa BaseUrl API của bạn

        public DashboardController(
            IHttpClientFactory httpClientFactory,
            IOptions<ApiSettings> apiSettings)
        {
            _httpClientFactory = httpClientFactory;
            _apiSettings = apiSettings.Value;
        }

        // GET: /Dashboard/Index
        public async Task<IActionResult> Index()
        {
            var model = new DashboardViewModel();

            // 1. Tạo Client
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_apiSettings.AuthBaseUrl); // Lấy URL từ appsettings.json

            // 2. Lấy Token từ Cookie (lúc Login đã lưu)
            var accessToken = await HttpContext.GetTokenAsync("AccessToken");

            // 3. Gắn Token vào Header (Quan trọng: Không có cái này API sẽ trả về 401 Unauthorized)
            if (!string.IsNullOrEmpty(accessToken))
            {
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", accessToken);
            }

            try
            {
                // --- GỌI API 1: DOANH THU ---
                int currentYear = DateTime.Now.Year;
                var resRevenue = await client.GetAsync($"Dashboard/revenue?year={currentYear}");

                if (resRevenue.IsSuccessStatusCode)
                {
                    // Đọc JSON và map vào List<RevenueItem>
                    var data = await resRevenue.Content.ReadFromJsonAsync<List<RevenueStatisticResponse>>();
                    if (data != null) model.RevenueData = data;
                }

                // --- GỌI API 2: TRẠNG THÁI PHÒNG ---
                var resOccupancy = await client.GetAsync("Dashboard/occupancy");

                if (resOccupancy.IsSuccessStatusCode)
                {
                    // Đọc JSON và map vào RoomStatusItem
                    var data = await resOccupancy.Content.ReadFromJsonAsync<RoomStatusStatisticResponse>();
                    if (data != null) model.RoomStatus = data;
                }
            }
            catch (Exception ex)
            {
                // Nếu API lỗi, ghi log hoặc hiện thông báo nhẹ, không crash trang
                ViewBag.Error = "Lỗi tải dữ liệu: " + ex.Message;
            }

            // Trả ViewModel về cho View (Razor)
            return View(model);
        }
    }
}
