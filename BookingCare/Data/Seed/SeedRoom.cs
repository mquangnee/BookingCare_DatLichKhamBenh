using BookingCare.Models;
using BookingCare.Repository;
using Microsoft.EntityFrameworkCore;

namespace BookingCare.Data.Seed
{
    public static class SeedRoom
    {
        public static async Task SeedAsync(DataContext dbContext)
        {
            // Danh sách phòng khám
            string[] roomNames = {
                "P101", "P102", "P103", "P104", "P105", "P106", "P107", "P108",
                "P201", "P202", "P203", "P204", "P205", "P206", "P207", "P208",
                "P301", "P302", "P303", "P304", "P305", "P306", "P307", "P308"}; 
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
