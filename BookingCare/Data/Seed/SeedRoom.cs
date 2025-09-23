using BookingCare.Models;
using BookingCare.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BookingCare.Data.Seed
{
    public static class SeedRoom
    {
        public static async Task SeedAsync(DataContext dbContext)
        {
            string[] roomNames = { "P101", "P102", "P103" }; // Danh sách phòng khám
            foreach (var roomName in roomNames)
            {
                //Kiểm tra xem phòng đã tồn tại chưa
                var exists = await dbContext.Rooms.AnyAsync(r => r.Name == roomName);
                if (!exists)
                {
                    var room = new Room
                    {
                        Name = roomName,
                        Capacity = 2 //Số ghế khám mặc định
                    };

                    await dbContext.Rooms.AddAsync(room);
                }
            }
            await dbContext.SaveChangesAsync();
        }
    }
}
