using SmartHotel.BLL.Common;
using SmartHotel.BLL.DTOs.Room;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHotel.BLL.Services.Interface
{
    public interface IRoomService
    {
        Task<PagedResult<RoomResponse>> GetAllRoomsAsync(
              int pageIndex = 1,
              int pageSize = 6,
              string? status = null,
              int? capacity = null,
              int? floor = null,
              string? roomNumber = null);
        Task<RoomResponse> GetRoomByIdAsync(int id);
        Task CreateRoomAsync(CreateRoomRequest request);
        Task UpdateRoomAsync(UpdateRoomRequest request);

        Task<DeleteRoomResult> DeleteRoomAsync(int id);

        Task<List<LookupDto>> GetRoomLookupAsync();
    }
}
