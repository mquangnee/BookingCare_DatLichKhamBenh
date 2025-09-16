using BookingCare.Models;
using BookingCare.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Numerics;

namespace BookingCare.Data.Seed
{

    public static class SeedPatient
    {
        public static async Task SeedAsync(UserManager<IdentityUser> userManager, DataContext dbContext)
        {
            var email = "nguyenminhquangg03012004@gmail.com";
            if (await userManager.FindByEmailAsync(email) == null) //Kiểm tra email đã tồn tại chưa
            {
                var patient = new ApplicationUser
                {
                    UserName = email, //Tên đăng nhập = email
                    Email = email,
                    EmailConfirmed = true,
                    FullName = "Nguyễn Minh Quang",
                    BirthOfDate = new DateTime(2003, 10, 10),
                    Gender = "Nam",
                    Address = "Hà Nội"
                };
                var result = await userManager.CreateAsync(patient, "Abcd@123"); //Mật khẩu: Abcd@123
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(patient, "Patient"); //Gán role Patient
                    var patientEntity = new Patient //Tạo bản ghi trong bảng Patient
                    {
                        UserId = patient.Id,
                        MedicalHistory = "Không có tiền sử bệnh"
                    };
                    await dbContext.Patients.AddAsync(patientEntity); //Thêm bản ghi vào bảng Patients
                }
            }
        }
    }
}
