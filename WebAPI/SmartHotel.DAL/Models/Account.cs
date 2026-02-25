using System.ComponentModel.DataAnnotations;

namespace SmartHotel.DAL.Models
{
    public class Account
    {
        public int AccountId { get; set; }

        [Required, MaxLength(100), EmailAddress]
        public string Email { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public User User { get; set; }
        public Tenant Tenant { get; set; }

        public ICollection<AccountRole> AccountRoles { get; set; }
        public ICollection<RefreshToken> RefreshTokens { get; set; }
    }
}
