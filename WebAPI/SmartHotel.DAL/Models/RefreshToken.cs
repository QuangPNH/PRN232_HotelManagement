namespace SmartHotel.DAL.Models
{
    public class RefreshToken
    {
        public int RefreshTokenId { get; set; }

        public int AccountId { get; set; }

        public string Token { get; set; }

        public DateTime ExpiredAt { get; set; }

        public bool IsRevoked { get; set; }

        public Account Account { get; set; }
    }
}