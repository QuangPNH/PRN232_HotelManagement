using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SmartHotel.UI.DTOs;
using SmartHotel.UI.DTOs.Room;
using SmartHotel.UI.Models;
using System.Net.Http.Headers;
using System.Text.Json;

namespace SmartHotel.UI.Controllers
{
    public class RoomController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ApiSettings _apiSettings;

        private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        public RoomController(IHttpClientFactory httpClientFactory, IOptions<ApiSettings> apiSettings)
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

        public async Task<IActionResult> Index(
       int pageIndex = 1,
       int pageSize = 6,
       string? status = null,
       int? capacity = null,
       int? floor = null,
       string? roomNumber = null)
        {
            PagedResult<RoomResponse>? result = null;

            try
            {
                var client = await CreateClientAsync();

                var query = $"Room?pageIndex={pageIndex}&pageSize={pageSize}";

                if (!string.IsNullOrWhiteSpace(status))
                    query += $"&status={status}";

                if (capacity.HasValue)
                    query += $"&capacity={capacity.Value}";

                if (floor.HasValue)
                    query += $"&floor={floor.Value}";

                if (!string.IsNullOrWhiteSpace(roomNumber))
                    query += $"&roomNumber={roomNumber}";

                var response = await client.GetAsync(query);

                if (response.IsSuccessStatusCode)
                {
                    result = await response.Content
                        .ReadFromJsonAsync<PagedResult<RoomResponse>>(_jsonOptions);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return RedirectToAction("Login", "Auth");
                }
                else
                {
                    ViewBag.Error = $"Lỗi tải dữ liệu (Code: {response.StatusCode})";
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Lỗi kết nối Server: " + ex.Message;
            }

            return View(result);
        }


        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateRoomRequest request)
        {
            if (!ModelState.IsValid) return View(request);

            try
            {
                var client = await CreateClientAsync();
                var response = await client.PostAsJsonAsync("Room", request);

                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Tạo phòng mới thành công!";
                    return RedirectToAction("Index");
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return RedirectToAction("Login", "Auth");
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    ViewBag.Error = "Lỗi API: " + error;
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Exception: " + ex.Message;
            }

            return View(request);
        }

        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var client = await CreateClientAsync();
                var response = await client.GetAsync($"Room/{id}");

                if (response.IsSuccessStatusCode)
                {
                    var room = await response.Content.ReadFromJsonAsync<RoomResponse>(_jsonOptions);

                    var updateModel = new UpdateRoomRequest
                    {
                        RoomId = room.RoomId,
                        RoomNumber = room.RoomNumber,
                        Floor = room.Floor,
                        Price = room.Price,
                        Capacity = room.Capacity,
                        ImageUrl = room.ImageUrl,
                        Description = room.Description,
                        Status = room.Status
                    };
                    return View(updateModel);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return RedirectToAction("Login", "Auth");
                }
            }
            catch
            {
                return NotFound();
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, UpdateRoomRequest request)
        {
            if (id != request.RoomId) return BadRequest();
            if (!ModelState.IsValid) return View(request);

            try
            {
                var client = await CreateClientAsync();
                var response = await client.PutAsJsonAsync($"Room/{id}", request);

                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Cập nhật phòng thành công!";
                    return RedirectToAction("Index");
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return RedirectToAction("Login", "Auth");
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    ViewBag.Error = "Cập nhật thất bại: " + error;
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Exception: " + ex.Message;
            }

            return View(request);
        }

        // --- 5. DELETE ---
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var client = await CreateClientAsync();
                var response = await client.DeleteAsync($"Room/{id}");

                if (response.IsSuccessStatusCode)
                {
                    // API trả về kết quả xóa (có thể kèm cảnh báo)
                    var result = await response.Content.ReadFromJsonAsync<DeleteRoomResult>(_jsonOptions);

                    if (result != null && !string.IsNullOrEmpty(result.WarningMessage))
                    {
                        TempData["Warning"] = "Đã xóa nhưng có lưu ý: " + result.WarningMessage;
                    }
                    else
                    {
                        TempData["Success"] = "Xóa phòng thành công!";
                    }
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return RedirectToAction("Login", "Auth");
                }
                else
                {
                    var errorStr = await response.Content.ReadAsStringAsync();
                    TempData["Error"] = "Không thể xóa: " + errorStr;
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi hệ thống: " + ex.Message;
            }

            return RedirectToAction("Index");
        }

        // --- 6. DETAILS ---
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var client = await CreateClientAsync();
                var response = await client.GetAsync($"Room/{id}");

                if (response.IsSuccessStatusCode)
                {
                    var room = await response.Content.ReadFromJsonAsync<RoomResponse>(_jsonOptions);
                    return View(room);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return RedirectToAction("Login", "Auth");
                }
            }
            catch
            {
                return NotFound();
            }
            return NotFound();
        }
    }
}