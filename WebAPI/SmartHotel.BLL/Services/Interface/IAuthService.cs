using SmartHotel.BLL.DTOs.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHotel.BLL.Services.Interface
{
    public interface IAuthService
    {
        Task<AuthResponse> LoginAsync(LoginRequest request);
        Task<bool> RegisterAsync(RegisterRequest request);
        Task<AuthResponse> RefreshTokenAsync(RefreshTokenRequest request);
        Task LogoutAsync(string refreshToken); 
        Task<bool> ChangePasswordAsync(int accountId, string oldPass, string newPass);
    }
}
