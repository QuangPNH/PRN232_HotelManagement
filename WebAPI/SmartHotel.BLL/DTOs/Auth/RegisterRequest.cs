using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHotel.BLL.DTOs.Auth
{
    public class RegisterRequest
    {
        [Required, EmailAddress]
        public string Email { get; set; } // Đăng ký bằng Email
        public string Password { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; } // SĐT
        public string CCCD { get; set; } // Nếu là khách thuê
        public string Role { get; set; } = "Tenant"; // Mặc định là khách thuê
    }
}
