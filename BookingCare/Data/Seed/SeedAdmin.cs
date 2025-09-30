using BookingCare.Models;
using BookingCare.Repository;
using Microsoft.AspNetCore.Identity;

namespace BookingCare.Data.Seed
{

    public static class SeedAdmin
    {
        public static async Task SeedAsync(UserManager<IdentityUser> userManager, DataContext dbContext)
        {
            var email = "nguyenminhquangg03012004@gmail.com";
            if (await userManager.FindByEmailAsync(email) == null) //Kiểm tra email đã tồn tại chưa
            {
                var admin = new ApplicationUser
                {
                    UserName = email, //Tên đăng nhập = email
                    Email = email,
                    EmailConfirmed = true,
                    FullName = "Nguyễn Minh Quang",
                    BirthOfDate = new DateTime(2003, 10, 10),
                    Gender = "Nam",
                    Address = "Hà Nội"
                };
                var result = await userManager.CreateAsync(admin, "Abcd@123"); //Mật khẩu: Abcd@123
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "Admin"); //Gán role Admin
                }
            }
        }
    }
}
