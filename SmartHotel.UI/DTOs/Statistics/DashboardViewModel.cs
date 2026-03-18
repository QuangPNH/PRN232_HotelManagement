namespace SmartHotel.UI.DTOs.Statistics
{
    public class DashboardViewModel
    {
        // Hứng API Revenue (List các tháng)
        public List<RevenueStatisticResponse> RevenueData { get; set; } = new List<RevenueStatisticResponse>();

        // Hứng API Occupancy (Trạng thái phòng)
        public RoomStatusStatisticResponse RoomStatus { get; set; } = new RoomStatusStatisticResponse();
    }
}
