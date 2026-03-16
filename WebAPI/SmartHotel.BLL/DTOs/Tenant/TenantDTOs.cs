namespace SmartHotel.BLL.DTOs.Tenant
{
    public class TenantFilterRequest
    {
        public string? Keyword { get; set; }
        public bool? IsActive { get; set; }
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 6;
    }

    public class CreateTenantRequest
    {
        public string FullName { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string CCCD { get; set; } = null!;
        public string? Email { get; set; }
        public string? Address { get; set; }
    }

    public class TenantResponse
    {
        public int TenantId { get; set; }
        public string FullName { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string CCCD { get; set; } = null!;
        public string? Email { get; set; }
        public string? Address { get; set; }
        public bool IsActive { get; set; }
        public bool IsRenting { get; set; }
        public string CurrentRoom { get; set; } = "Chưa thuê";
    }
}
