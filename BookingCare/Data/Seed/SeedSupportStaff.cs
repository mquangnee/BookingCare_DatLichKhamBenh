using BookingCare.Models;
using BookingCare.Repository;
using Microsoft.AspNetCore.Identity;

namespace BookingCare.Data.Seed
{
    public static class SeedSupportStaff
    {
        public static async Task SeedAsync(UserManager<IdentityUser> userManager, DataContext dbContext)
        {
            var email = "staff1";
            if (await userManager.FindByEmailAsync(email) == null) //Kiểm tra email đã tồn tại chưa
            {
                var staff = new ApplicationUser
                {
                    UserName = email, //Tên đăng nhập = email
                    Email = email,
                    EmailConfirmed = true,
                    FullName = "Phan Văn Trường",
                    BirthOfDate = new DateTime(1995, 05, 15),
                    Gender = "Nam",
                    Address = "Lào Cai"
                };
                var result = await userManager.CreateAsync(staff, "Abcd@123"); //Mật khẩu: Abcd@123
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(staff, "SupportStaff"); //Gán role SupportStaff
                    var staffEntity = new SupportStaff //Tạo bản ghi trong bảng SupportStaffs
                    {
                        UserId = staff.Id,
                        Department = "Phòng tiếp bệnh nhân"
                    };
                    await dbContext.SupportStaffs.AddAsync(staffEntity); //Thêm bản ghi vào bảng SupportStaffs
                    await dbContext.SaveChangesAsync();
                }
            }
        }
    }

}
