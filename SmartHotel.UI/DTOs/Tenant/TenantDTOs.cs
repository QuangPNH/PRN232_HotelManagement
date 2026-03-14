namespace SmartHotel.UI.DTOs.Tenant
{
    public class TenantFilterRequest
    {
        public string? Keyword { get; set; }
        public bool? IsActive { get; set; }

        // Tự định nghĩa phân trang ở đây luôn
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 6;
    }

    // 2. Dùng để tạo mới khách hàng thủ công (nếu cần)
    public class CreateTenantRequest
    {
        public string FullName { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string CCCD { get; set; } = null!;
        public string? Email { get; set; }
        public string? Address { get; set; }
    }

    // 3. Dùng để trả dữ liệu ra ngoài (Bao gồm cả thông tin phòng đang thuê)
    public class TenantResponse
    {
        public int TenantId { get; set; }
        public string FullName { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string CCCD { get; set; } = null!;
        public string? Email { get; set; }
        public string? Address { get; set; }
        public bool IsActive { get; set; }

        // Mở rộng thêm để biết đang thuê phòng nào
        public bool IsRenting { get; set; }
        public string CurrentRoom { get; set; } = "Chưa thuê";
    }
}
