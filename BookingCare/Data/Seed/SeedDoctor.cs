using BookingCare.Models;
using BookingCare.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BookingCare.Data.Seed
{
    // Khởi tạo tài khoản bác sĩ trong hệ thống
    public static class SeedDoctor
    {
        public static async Task SeedAsync(UserManager<ApplicationUser> _userManager, DataContext _dbContext)
        {
            var doctors = new List<(string Email, string FullName, string Gender, DateOnly Dob, string SpecialtyName, string RoomName, int YearsExp)>
            {
                // ✅ Bác sĩ cũ (Nội khoa)
                ("ngokhactai03011970@gmail.com", "Ngô Khắc Tài", "Nam", new DateOnly(1970, 1, 3), "Nội khoa", "P101", 20),

                // ✅ 5 bác sĩ mới – TÊN THẬT
                ("nguyenvanthang12061978@gmail.com", "Nguyễn Văn Thắng", "Nam", new DateOnly(1978, 6, 12), "Ngoại khoa", "P102", 18),
                ("tranthilan15031982@gmail.com", "Trần Thị Lan", "Nữ", new DateOnly(1982, 3, 15), "Sản khoa", "P103", 16),
                ("levanhung22091985@gmail.com", "Lê Văn Hùng", "Nam", new DateOnly(1985, 9, 22), "Nhi khoa", "P104", 14),
                ("phamthimai08111980@gmail.com", "Phạm Thị Mai", "Nữ", new DateOnly(1980, 11, 8), "Răng hàm mặt", "P105", 15),
                ("hoangvanphuc04041977@gmail.com", "Hoàng Văn Phúc", "Nam", new DateOnly(1977, 4, 4), "Mắt", "P106", 17)
            };

            foreach (var item in doctors)
            {
                if (await _userManager.FindByEmailAsync(item.Email) == null)
                {
                    var doctorUser = new ApplicationUser
                    {
                        UserName = item.Email,
                        Email = item.Email,
                        EmailConfirmed = true,
                        FullName = item.FullName,
                        DateOfBirth = item.Dob,
                        Gender = item.Gender,
                        Address = "Việt Nam",
                        PhoneNumber = "0909000000"
                    };

                    var result = await _userManager.CreateAsync(doctorUser, "Abcd@123");

                    if (result.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(doctorUser, "Doctor");

                        var specialty = await _dbContext.Specialties
                            .FirstOrDefaultAsync(s => s.Name == item.SpecialtyName);

                        var room = await _dbContext.Rooms
                            .FirstOrDefaultAsync(r => r.Name == item.RoomName);

                        if (specialty != null && room != null)
                        {
                            var doctorEntity = new Doctor
                            {
                                UserId = doctorUser.Id,
                                Degree = "Bác sĩ chuyên khoa (BSCK)",
                                YearsOfExp = item.YearsExp,
                                SpecialtyId = specialty.Id,
                                RoomId = room.Id
                            };

                            await _dbContext.Doctors.AddAsync(doctorEntity);
                        }
                    }
                }
            }

            await _dbContext.SaveChangesAsync();
        }
    }
}
