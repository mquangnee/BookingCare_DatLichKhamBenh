using BookingCare.Models;
using BookingCare.Repository;

namespace BookingCare.Data.Seed
{
    public static class SeedSpecialty
    {
        public static async Task SeedAsync(DataContext dbContext)
        {
            // Danh sách chuyên khoa kèm mô tả
            var specialties = new Dictionary<string, string>
            {
                {
                    "Nội khoa",
                    "Chuyên khoa Nội khoa chuyên khám, chẩn đoán và điều trị các bệnh lý liên quan đến các cơ quan nội tạng mà không cần can thiệp phẫu thuật."
                },
                {
                    "Ngoại khoa",
                    "Chuyên khoa Ngoại khoa tập trung điều trị các bệnh lý cần can thiệp phẫu thuật, bao gồm chấn thương và các bệnh lý ngoại khoa."
                },
                {
                    "Sản khoa",
                    "Chuyên khoa Sản khoa chuyên theo dõi và chăm sóc sức khỏe phụ nữ trong thời kỳ mang thai, sinh nở và sau sinh."
                },
                {
                    "Nhi khoa",
                    "Chuyên khoa Nhi khoa chuyên khám và điều trị các bệnh lý ở trẻ em từ sơ sinh đến tuổi vị thành niên."
                },
                {
                    "Răng hàm mặt",
                    "Chuyên khoa Răng Hàm Mặt chuyên khám và điều trị các bệnh lý liên quan đến răng, hàm và khoang miệng."
                },
                {
                    "Mắt",
                    "Chuyên khoa Mắt chuyên khám và điều trị các bệnh lý về thị giác và nhãn khoa."
                },
                {
                    "Tai mũi họng",
                    "Chuyên khoa Tai Mũi Họng chuyên khám và điều trị các bệnh lý liên quan đến tai, mũi, họng và đường hô hấp trên."
                }
            };

            foreach (var specialty in specialties)
            {
                // Kiểm tra chuyên khoa đã tồn tại chưa
                if (!dbContext.Specialties.Any(s => s.Name == specialty.Key))
                {
                    var specialtyEntity = new Specialty
                    {
                        Name = specialty.Key,
                        Description = specialty.Value
                    };

                    await dbContext.Specialties.AddAsync(specialtyEntity);
                }
            }
        }
    }
}
