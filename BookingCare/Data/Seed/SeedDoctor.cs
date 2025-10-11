using BookingCare.Models;
using BookingCare.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BookingCare.Data.Seed
{
    //Khởi tạo tài khoản bác sĩ trong hệ thống
    public static class SeedDoctor
    {
        public static async Task SeedAsync(UserManager<IdentityUser> userManager, DataContext dbContext)
        {
            var email = "doctor1";
            if (await userManager.FindByEmailAsync(email) == null) //Kiểm tra email đã tồn tại chưa
            {
                var doctor = new ApplicationUser
                {
                    UserName = email, //Tên đăng nhập = email
                    Email = email,
                    EmailConfirmed = true,
                    FullName = "Ngô Khắc Tài",
                    BirthOfDate = new DateOnly(1970, 01, 03),
                    Gender = "Nam",
                    Address = "Nghệ An",
                    PhoneNumber = "0123456789"
                };
                var result = await userManager.CreateAsync(doctor, "Abcd@123"); //Mật khẩu: Abcd@123
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(doctor, "Doctor"); //Gán role Doctor
                    var specialty = await dbContext.Specialties.FirstOrDefaultAsync(s => s.Name == "Nội khoa"); //Lấy chuyên khoa Nội khoa
                    var room = await dbContext.Rooms.FirstOrDefaultAsync(r => r.Name == "P101"); //Lấy phòng khám P101
                    var doctorEntity = new Doctor //Tạo bản ghi trong bảng Doctors
                    {
                        UserId = doctor.Id,
                        Degree = "Tiến sĩ",
                        YearsOfExp = 20,
                        SpecialtyId = specialty.Id,
                        RoomId = room.Id
                    };
                    await dbContext.Doctors.AddAsync(doctorEntity); //Thêm bản ghi vào bảng Doctors
                }
            }
        }
    }
}
