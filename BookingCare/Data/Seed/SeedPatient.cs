using BookingCare.Models;
using BookingCare.Repository;
using Microsoft.AspNetCore.Identity;

namespace BookingCare.Data.Seed
{
    public static class SeedPatient
    {
        public static async Task SeedAsync(UserManager<ApplicationUser> userManager, DataContext dbContext)
        {
            var patients = new List<ApplicationUser>
            {
                new ApplicationUser
                {
                    UserName = "ngokhactai09102003@gmail.com",
                    Email = "ngokhactai09102003@gmail.com",
                    EmailConfirmed = true,
                    FullName = "Ngô Khắc Tài",
                    DateOfBirth = new DateOnly(2003, 10, 9),
                    Gender = "Nam",
                    Address = "Nghệ An",
                    PhoneNumber = "0987654123"
                },
                new ApplicationUser
                {
                    UserName = "tranminhtuan12032001@gmail.com",
                    Email = "tranminhtuan12032001@gmail.com",
                    EmailConfirmed = true,
                    FullName = "Trần Minh Tuấn",
                    DateOfBirth = new DateOnly(2001, 3, 12),
                    Gender = "Nam",
                    Address = "Đà Nẵng",
                    PhoneNumber = "0901110001"
                },
                new ApplicationUser
                {
                    UserName = "nguyenngocanh25052002@gmail.com",
                    Email = "nguyenngocanh25052002@gmail.com",
                    EmailConfirmed = true,
                    FullName = "Nguyễn Ngọc Anh",
                    DateOfBirth = new DateOnly(2002, 5, 25),
                    Gender = "Nữ",
                    Address = "Hà Nội",
                    PhoneNumber = "0901110002"
                },
                new ApplicationUser
                {
                    UserName = "phamquanghuy01111999@gmail.com",
                    Email = "phamquanghuy01111999@gmail.com",
                    EmailConfirmed = true,
                    FullName = "Phạm Quang Huy",
                    DateOfBirth = new DateOnly(1999, 11, 1),
                    Gender = "Nam",
                    Address = "Hồ Chí Minh",
                    PhoneNumber = "0901110003"
                },
                new ApplicationUser
                {
                    UserName = "lethutrang09022001@gmail.com",
                    Email = "lethutrang09022001@gmail.com",
                    EmailConfirmed = true,
                    FullName = "Lê Thu Trang",
                    DateOfBirth = new DateOnly(2001, 2, 9),
                    Gender = "Nữ",
                    Address = "Huế",
                    PhoneNumber = "0901110004"
                },
                new ApplicationUser
                {
                    UserName = "ngohongson17102000@gmail.com",
                    Email = "ngohongson17102000@gmail.com",
                    EmailConfirmed = true,
                    FullName = "Ngô Hồng Sơn",
                    DateOfBirth = new DateOnly(2000, 10, 17),
                    Gender = "Nam",
                    Address = "Quảng Nam",
                    PhoneNumber = "0901110005"
                },
                new ApplicationUser
                {
                    UserName = "buithuha06061998@gmail.com",
                    Email = "buithuha06061998@gmail.com",
                    EmailConfirmed = true,
                    FullName = "Bùi Thu Hà",
                    DateOfBirth = new DateOnly(1998, 6, 6),
                    Gender = "Nữ",
                    Address = "Bắc Ninh",
                    PhoneNumber = "0901110006"
                },
                new ApplicationUser
                {
                    UserName = "danganhkhoa20032003@gmail.com",
                    Email = "danganhkhoa20032003@gmail.com",
                    EmailConfirmed = true,
                    FullName = "Đặng Anh Khoa",
                    DateOfBirth = new DateOnly(2003, 3, 20),
                    Gender = "Nam",
                    Address = "Bình Định",
                    PhoneNumber = "0901110007"
                },
                new ApplicationUser
                {
                    UserName = "phanthaovy12122000@gmail.com",
                    Email = "phanthaovy12122000@gmail.com",
                    EmailConfirmed = true,
                    FullName = "Phan Thảo Vy",
                    DateOfBirth = new DateOnly(2000, 12, 12),
                    Gender = "Nữ",
                    Address = "Cần Thơ",
                    PhoneNumber = "0901110008"
                },
                new ApplicationUser
                {
                    UserName = "vutienthanh15091999@gmail.com",
                    Email = "vutienthanh15091999@gmail.com",
                    EmailConfirmed = true,
                    FullName = "Vũ Tiến Thành",
                    DateOfBirth = new DateOnly(1999, 9, 15),
                    Gender = "Nam",
                    Address = "Hải Phòng",
                    PhoneNumber = "0901110009"
                }
            };

            foreach (var patient in patients)
            {
                if (await userManager.FindByEmailAsync(patient.Email) == null)
                {
                    var result = await userManager.CreateAsync(patient, "Abc@123");

                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(patient, "Patient");
                    }
                }
            }
        }
    }
}
