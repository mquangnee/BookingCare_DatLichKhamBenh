using BookingCare.Models;
using BookingCare.Repository;

namespace BookingCare.Data.Seed
{
    public static class SeedSpecialty
    {
        public static async Task SeedAsync(DataContext dbContext)
        {
            string[] specialties = { "Nội khoa", "Ngoại khoa", "Sản khoa", "Nhi khoa", "Răng hàm mặt", "Mắt", "Tai mũi họng" }; //Danh sách chuyên khoa
            foreach (var specialty in specialties)
            {
                if (!dbContext.Specialties.Any(s => s.Name == specialty)) //Kiểm tra chuyên khoa đã tồn tại chưa
                {
                    var specialtyEntity = new Specialty
                    {
                        Name = specialty
                    };
                    await dbContext.Specialties.AddAsync(specialtyEntity); //Thêm chuyên khoa mới vào DB
                }
            }
        }
    }
}
