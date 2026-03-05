using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHotel.BLL.DTOs.Auth
{
    public class AuthResponse
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime ExpiresAt { get; set; } // Thời gian hết hạn AccessToken
        public string FullName { get; set; }
        public string Role { get; set; }
    }
}
