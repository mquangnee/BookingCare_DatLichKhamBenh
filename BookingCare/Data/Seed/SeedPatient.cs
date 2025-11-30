using BookingCare.Models;
using BookingCare.Repository;
using Microsoft.AspNetCore.Identity;

namespace BookingCare.Data.Seed
{
    public static class SeedPatient
    {
        public static async Task SeedAsync(UserManager<ApplicationUser> userManager, DataContext dbContext)
        {
            var email = "ngokhactai37@gmail.com";
            if (await userManager.FindByEmailAsync(email) == null) 
            {
                var patient = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true,
                    FullName = "Ngô Khắc Tài",
                    DateOfBirth = new DateOnly(2003, 10, 09),
                    Gender = "Nam",
                    Address = "Nghệ An",
                    PhoneNumber = "0987654123"
                };
                var result = await userManager.CreateAsync(patient, "Abc@123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(patient, "Patient");
                }
               }
            }
        }
}
