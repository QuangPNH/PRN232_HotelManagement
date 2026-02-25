using System.ComponentModel.DataAnnotations;

namespace SmartHotel.DAL.Models
{
    public class User
    {
        public int UserId { get; set; }

        public int AccountId { get; set; }

        public string FullName { get; set; }

        public Account Account { get; set; }
    }

}