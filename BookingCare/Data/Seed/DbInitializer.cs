using BookingCare.Models;
using BookingCare.Repository;
using Microsoft.AspNetCore.Identity;

namespace BookingCare.Data.Seed
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();

            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            var dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();

            // Gọi từng Seed
            await SeedRole.SeedAsync(roleManager);
            await SeedRoom.SeedAsync(dbContext);
            await SeedSpecialty.SeedAsync(dbContext);
            await SeedDoctor.SeedAsync(userManager, dbContext);
            await SeedAdmin.SeedAsync(userManager, dbContext);
            await SeedPatient.SeedAsync(userManager, dbContext);
            await SeedAppointment.SeedAsync(dbContext);
            await dbContext.SaveChangesAsync();
            if (!dbContext.Medicines.Any())
            {
                dbContext.Medicines.AddRange(
                    new Medicine { Name = "Paracetamol 500mg", Unit = "Viên", Function = "Giảm đau, hạ sốt" },
                    new Medicine { Name = "Amoxicillin 500mg", Unit = "Viên", Function = "Kháng sinh" },
                    new Medicine { Name = "Vitamin C 500mg", Unit = "Viên", Function = "Tăng sức đề kháng" },
                    new Medicine { Name = "Oresol", Unit = "Gói", Function = "Bù nước điện giải" },
                    new Medicine { Name = "Efferalgan 500mg", Unit = "Viên", Function = "Hạ sốt, giảm đau" }
                );

                await dbContext.SaveChangesAsync();
            }


        }
    }
}
