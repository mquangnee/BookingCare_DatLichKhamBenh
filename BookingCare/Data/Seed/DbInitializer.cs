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
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();

            // Gọi từng Seed
            await SeedRole.SeedAsync(roleManager);
            await SeedRoom.SeedAsync(dbContext);
            await SeedSpecialty.SeedAsync(dbContext);
            await SeedDoctor.SeedAsync(userManager, dbContext);
            await SeedAdmin.SeedAsync(userManager, dbContext);
            await dbContext.SaveChangesAsync();
        }
    }
}
