using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SmartHotel.UI.DTOs.Auth;
using SmartHotel.UI.Models;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;

namespace SmartHotel.UI.Controllers
{
    public class AuthController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ApiSettings _apiSettings;

        public AuthController(IHttpClientFactory httpClientFactory, IOptions<ApiSettings> apiSettings)
        {
            _httpClientFactory = httpClientFactory;
            _apiSettings = apiSettings.Value;
        }

        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            var client = _httpClientFactory.CreateClient("SmartHotelClient");

            var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");

            var url = $"{_apiSettings.AuthBaseUrl}Auth/register";
            var response = await client.PostAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Đăng ký thành công!";
                return RedirectToAction("Login");
            }

            ViewBag.Error = await response.Content.ReadAsStringAsync();
            return View(request);
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            if (!ModelState.IsValid) return View(request);

            var client = _httpClientFactory.CreateClient("SmartHotelClient");

            var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
            var response = await client.PostAsync($"{_apiSettings.AuthBaseUrl}Auth/login", content);

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error = "Tên đăng nhập hoặc mật khẩu không đúng";
                return View(request);
            }

            var json = await response.Content.ReadAsStringAsync();
            var authResult = JsonConvert.DeserializeObject<AuthResponse>(json);

            HttpContext.Session.SetString("AccessToken", authResult.AccessToken);
            HttpContext.Session.SetString("RefreshToken", authResult.RefreshToken);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, authResult.FullName ?? request.Email),
                new Claim(ClaimTypes.Email, request.Email),
                new Claim(ClaimTypes.Role, authResult.Role ?? "User")
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTime.UtcNow.AddHours(2)
            };

            authProperties.StoreTokens(new List<AuthenticationToken>
            {
                new AuthenticationToken { Name = "AccessToken", Value = authResult.AccessToken }
            });

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                authProperties);

            if (authResult.Role == "Tenant")
            {
                return RedirectToAction("Dashboard", "TenantPortal");
            }
            else
            {
                return RedirectToAction("Index", "Room");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            var refreshToken = HttpContext.Session.GetString("RefreshToken");
            if (!string.IsNullOrEmpty(refreshToken))
            {
                var client = _httpClientFactory.CreateClient("SmartHotelClient"); 
                var content = new StringContent(JsonConvert.SerializeObject(refreshToken), Encoding.UTF8, "application/json");
                await client.PostAsync($"{_apiSettings.AuthBaseUrl}auth/logout", content);
            }

            HttpContext.Session.Clear();

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Login");
        }
    }
}