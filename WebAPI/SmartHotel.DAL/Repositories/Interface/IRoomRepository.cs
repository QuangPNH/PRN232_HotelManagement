using SmartHotel.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHotel.DAL.Repositories.Interface
{
    public interface IRoomRepository
    {

        Task<IEnumerable<Room>> GetAllRoomsAsync();
        Task<Room?> GetRoomByIdAsync(int id);
        Task CreateRoomAsync(Room room);
        Task UpdateRoomAsync(Room room);
        Task DeleteRoomAsync(int id);

        Task<bool> IsRoomNumberExistsAsync(string roomNumber); 

        Task<bool> IsRoomAvailableAsync(int roomId);

        Task UpdateRoomStatusAsync(int roomId, string newStatus);

        Task<IEnumerable<Room>> GetRoomsByFilterAsync(int? floor, decimal? minPrice, decimal? maxPrice);

        Task<IEnumerable<Room>> GetAvailableRoomsLookupAsync();
    }
}
