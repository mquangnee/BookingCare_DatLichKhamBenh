using Microsoft.AspNetCore.Identity;

namespace BookingCare.Data.Seed
{
    //Khởi tạo role trong hệ thống
    public static class SeedRole
    {
        public static async Task SeedAsync(RoleManager<IdentityRole> roleManager)
        {
            string[] roles = {"Admin", "Doctor", "Patient" }; //Danh sách role
            foreach (var role in roles)
            {
                if(!await roleManager.RoleExistsAsync(role)) //Kiểm tra role đã tồn tại chưa
                {
                    await roleManager.CreateAsync(new IdentityRole(role)); //Thêm role mới vào DB
                }
            }
        }
    }
}
