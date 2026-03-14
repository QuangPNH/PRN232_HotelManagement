namespace SmartHotel.UI.DTOs.Room
{
    public class RoomResponse
    {
        public int RoomId { get; set; }
        public string RoomNumber { get; set; }
        public string ImageUrl { get; set; }
        public int Floor { get; set; }
        public decimal Price { get; set; }
        public int Capacity { get; set; }
        public string Status { get; set; }
        public string StatusVietnamese { get; set; } // Bonus: Trả về tiếng Việt cho FE dễ hiển thị
        public string Description { get; set; }
    }
}
