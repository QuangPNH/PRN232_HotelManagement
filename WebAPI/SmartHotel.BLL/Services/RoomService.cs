using SmartHotel.BLL.Common;
using SmartHotel.BLL.DTOs.Room;
using SmartHotel.BLL.Services.Interface;
using SmartHotel.DAL.Models;
using SmartHotel.DAL.Repositories.Interface;

namespace SmartHotel.BLL.Services
{
    public class RoomService : IRoomService
    {
        private readonly IRoomRepository _roomRepo;

        public RoomService(IRoomRepository roomRepo)
        {
            _roomRepo = roomRepo;
        }

        public async Task<PagedResult<RoomResponse>> GetAllRoomsAsync(
      int pageIndex = 1,
      int pageSize = 6,
      string? status = null,
      int? capacity = null,
      int? floor = null,
      string? roomNumber = null)
        {
            var rooms = await _roomRepo.GetAllRoomsAsync();

            var query = rooms.AsQueryable();

            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(r =>
                    r.Status
                        .ToLower()
                        .Contains(status.ToLower()));
            }

            if (capacity.HasValue)
            {
                query = query.Where(r => r.Capacity == capacity.Value);
            }

            if (floor.HasValue)
            {
                query = query.Where(r => r.Floor == floor.Value);
            }

            if (!string.IsNullOrWhiteSpace(roomNumber))
            {
                query = query.Where(r =>
                    r.RoomNumber.ToLower().Contains(roomNumber.ToLower()));
            }

            var totalCount = query.Count();

            var pagedRooms = query
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var result = pagedRooms.Select(r => new RoomResponse
            {
                RoomId = r.RoomId,
                RoomNumber = r.RoomNumber,
                Floor = r.Floor,
                ImageUrl = r.ImageUrl,
                Price = r.Price,
                Capacity = r.Capacity,
                Status = r.Status,
                StatusVietnamese = MapStatusToVietnamese(r.Status),
                Description = r.Description
            });

            return new PagedResult<RoomResponse>
            {
                Items = result,
                TotalCount = totalCount,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
        }
        public async Task<RoomResponse> GetRoomByIdAsync(int id)
        {
            var room = await _roomRepo.GetRoomByIdAsync(id);
            if (room == null) throw new Exception("Không tìm thấy phòng.");

            return new RoomResponse
            {
                RoomId = room.RoomId,
                RoomNumber = room.RoomNumber,
                Floor = room.Floor,
                ImageUrl = room.ImageUrl,
                Price = room.Price,
                Capacity = room.Capacity,
                Status = room.Status,
                StatusVietnamese = MapStatusToVietnamese(room.Status),
                Description = room.Description
            };
        }

        public async Task CreateRoomAsync(CreateRoomRequest request)
        {
            var cleanRoomNumber = request.RoomNumber.Trim();

            if (await _roomRepo.IsRoomNumberExistsAsync(cleanRoomNumber))
            {
                throw new Exception($"Số phòng '{cleanRoomNumber}' đã tồn tại trong hệ thống.");
            }

            if (request.Price < 0) throw new Exception("Giá phòng không được là số âm.");

            var newRoom = new Room
            {
                RoomNumber = cleanRoomNumber,
                Floor = request.Floor,
                Price = request.Price,
                Capacity = request.Capacity,
                Description = request.Description,
                ImageUrl = request.ImageUrl,
                Status = RoomStatus.Available
            };

            await _roomRepo.CreateRoomAsync(newRoom);
        }


        public async Task UpdateRoomAsync(UpdateRoomRequest request)
        {
            var room = await _roomRepo.GetRoomByIdAsync(request.RoomId);
            if (room == null) throw new Exception("Phòng không tồn tại.");

            var cleanRoomNumber = request.RoomNumber.Trim();
            if (cleanRoomNumber != room.RoomNumber) 
            {
                if (await _roomRepo.IsRoomNumberExistsAsync(cleanRoomNumber))
                    throw new Exception($"Số phòng '{cleanRoomNumber}' đã tồn tại.");
            }

            room.RoomNumber = cleanRoomNumber;
            room.Floor = request.Floor;
            room.Price = request.Price;
            room.ImageUrl = request.ImageUrl;
            room.Capacity = request.Capacity;
            room.Description = request.Description;

            if (!string.IsNullOrEmpty(request.Status))
            {
                if (IsValidStatus(request.Status))
                {
                    room.Status = request.Status;
                }
                else
                {
                    throw new Exception("Trạng thái phòng không hợp lệ.");
                }
            }

            await _roomRepo.UpdateRoomAsync(room);
        }

        public async Task<DeleteRoomResult> DeleteRoomAsync(int id)
        {
            var room = await _roomRepo.GetRoomByIdAsync(id);
            if (room == null)
                throw new Exception("Phòng không tồn tại.");

            if (room.Status == RoomStatus.Occupied)
                throw new Exception("Không thể xóa phòng đang có khách thuê. Vui lòng thanh lý hợp đồng trước.");

            string? warning = null;

            if (room.Status == RoomStatus.Cleaning || room.Status == RoomStatus.Maintenance)
            {
                warning = $"Phòng đang ở trạng thái {MapStatusToVietnamese(room.Status)}.";
            }

            await _roomRepo.DeleteRoomAsync(id);

            return new DeleteRoomResult
            {
                Success = true,
                WarningMessage = warning
            };
        }



        private string MapStatusToVietnamese(string status)
        {
            return status switch
            {
                RoomStatus.Available => "Phòng trống",
                RoomStatus.Occupied => "Đang thuê",
                RoomStatus.Maintenance => "Đang sửa chữa",
                RoomStatus.Cleaning => "Đang dọn dẹp",
                _ => "Không xác định"
            };
        }

        private bool IsValidStatus(string status)
        {
            return status == RoomStatus.Available ||
                   status == RoomStatus.Occupied ||
                   status == RoomStatus.Maintenance ||
                   status == RoomStatus.Cleaning;
        }

        public async Task<List<LookupDto>> GetRoomLookupAsync()
        {
            var rooms = await _roomRepo.GetAvailableRoomsLookupAsync();
            return rooms.Select(r => new LookupDto
            {
                Id = r.RoomId,
                Name = r.RoomNumber,
                ExtraInfo = r.Price.ToString("N0")
            }).ToList();
        }
    }
}
