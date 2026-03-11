using Microsoft.EntityFrameworkCore;
using SmartHotel.DAL.Data;
using SmartHotel.DAL.Models;
using SmartHotel.DAL.Repositories.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHotel.DAL.Repositories
{
    public class RoomRepository : IRoomRepository
    {
        private readonly SmartHotelDbContext _context;

        public RoomRepository(SmartHotelDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Room>> GetAllRoomsAsync()
        {
            return await _context.Rooms
                .OrderBy(r => r.Floor)
                .ThenBy(r => r.RoomNumber)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Room?> GetRoomByIdAsync(int id)
        {
            return await _context.Rooms.FindAsync(id);
        }

        public async Task CreateRoomAsync(Room room)
        {
            await _context.Rooms.AddAsync(room);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateRoomAsync(Room room)
        {
            _context.Rooms.Update(room);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteRoomAsync(int id)
        {
            var room = await _context.Rooms.FindAsync(id);
            if (room != null)
            {
                _context.Rooms.Remove(room);
                await _context.SaveChangesAsync();
            }
        }


        public async Task<bool> IsRoomNumberExistsAsync(string roomNumber)
        {
            return await _context.Rooms.AnyAsync(r => r.RoomNumber == roomNumber);
        }

        public async Task<bool> IsRoomAvailableAsync(int roomId)
        {

            var room = await _context.Rooms.FindAsync(roomId);
            return room != null && room.Status == "Available";
        }

        public async Task UpdateRoomStatusAsync(int roomId, string newStatus)
        {
            var room = await _context.Rooms.FindAsync(roomId);
            if (room != null)
            {
                room.Status = newStatus;
                _context.Entry(room).Property(x => x.Status).IsModified = true;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Room>> GetRoomsByFilterAsync(int? floor, decimal? minPrice, decimal? maxPrice)
        {
            var query = _context.Rooms.AsQueryable();

            if (floor.HasValue)
            {
                query = query.Where(r => r.Floor == floor.Value);
            }

            if (minPrice.HasValue)
            {
                query = query.Where(r => r.Price >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                query = query.Where(r => r.Price <= maxPrice.Value);
            }

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<Room>> GetAvailableRoomsLookupAsync()
        {

            return await _context.Rooms
                .Where(r => r.Status == "Available")
                .OrderBy(r => r.RoomNumber)
                .ToListAsync();
        }
    }
}
