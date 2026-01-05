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
                ("ngokhactai03011970@gmail.com", "Ngô Khắc Tài", "Nam", new DateOnly(1970, 1, 3), "Nội khoa", "P101", 20),
                ("nguyenvanthang12061978@gmail.com", "Nguyễn Văn Thắng", "Nam", new DateOnly(1978, 6, 12), "Cơ xương khớp", "P102", 18),
                ("tranthilan15031982@gmail.com", "Trần Thị Lan", "Nữ", new DateOnly(1982, 3, 15), "Tiêu hóa", "P103", 16),
                ("levanhung22091985@gmail.com", "Lê Văn Hùng", "Nam", new DateOnly(1985, 9, 22), "Nhi khoa", "P104", 14),
                ("phamthimai08111980@gmail.com", "Phạm Thị Mai", "Nữ", new DateOnly(1980, 11, 8), "Tim mạch", "P105", 15),
                ("hoangvanphuc04041977@gmail.com", "Hoàng Văn Phúc", "Nam", new DateOnly(1977, 4, 4), "Da liễu", "P106", 17),
                ("dangthiminh05051975@gmail.com", "Đặng Thị Minh", "Nữ", new DateOnly(1975, 5, 5), "Tai mũi họng", "P107", 19),
                ("nguyenhuutuan20021980@gmail.com", "Nguyễn Hữu Tuấn", "Nam", new DateOnly(1980, 2, 20), "Thần kinh", "P108", 15),
                ("phamthithu16071983@gmail.com", "Phạm Thị Thu", "Nữ", new DateOnly(1983, 7, 16), "Nha khoa", "P201", 14),
                ("tranvantruong30091976@gmail.com", "Trần Văn Trường", "Nam", new DateOnly(1976, 9, 30), "Thần kinh", "P202", 18),
                ("nguyenthihong12031979@gmail.com", "Nguyễn Thị Hồng", "Nữ", new DateOnly(1979, 3, 12), "Tim mạch", "P203", 17),
                ("lethanhdat21061981@gmail.com", "Lê Thành Đạt", "Nam", new DateOnly(1981, 6, 21), "Nội khoa", "P204", 16),
                ("hoangthithao07071984@gmail.com", "Hoàng Thị Thảo", "Nữ", new DateOnly(1984, 7, 7), "Da liễu", "P205", 13),
                ("nguyenquanghuy15011977@gmail.com", "Nguyễn Quang Huy", "Nam", new DateOnly(1977, 1, 15), "Tiêu hóa", "P206", 19),
                ("phamthithanh25051982@gmail.com", "Phạm Thị Thanh", "Nữ", new DateOnly(1982, 5, 25), "Nhi khoa", "P207", 14),
                ("tranminhquan09091980@gmail.com", "Trần Minh Quân", "Nam", new DateOnly(1980, 9, 9), "Cơ xương khớp", "P208", 15),
                ("ngothithu22041985@gmail.com", "Ngô Thị Thu", "Nữ", new DateOnly(1985, 4, 22), "Nha khoa", "P301", 12),
                ("dangvanlong31031978@gmail.com", "Đặng Văn Long", "Nam", new DateOnly(1978, 3, 31), "Nội khoa", "P302", 17),
                ("nguyenthithanhhoa10101982@gmail.com", "Nguyễn Thị Thanh Hoa", "Nữ", new DateOnly(1982, 10, 10), "Tim mạch", "P303", 14),
                ("phamvanmanh20021979@gmail.com", "Phạm Văn Mạnh", "Nam", new DateOnly(1979, 2, 20), "Thần kinh", "P304", 16),
                ("tranthithuy05051981@gmail.com", "Trần Thị Thúy", "Nữ", new DateOnly(1981, 5, 5), "Da liễu", "P305", 13),
                ("nguyenminhduc15081984@gmail.com", "Nguyễn Minh Đức", "Nam", new DateOnly(1984, 8, 15), "Tiêu hóa", "P306", 12),
                ("hoangthithanh27021983@gmail.com", "Hoàng Thị Thanh", "Nữ", new DateOnly(1983, 2, 27), "Nhi khoa", "P307", 14),
                ("lequangvinh30061978@gmail.com", "Lê Quang Vinh", "Nam", new DateOnly(1978, 6, 30), "Cơ xương khớp", "P308", 17)
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