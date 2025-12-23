using BookingCare.Models;
using BookingCare.Repository;
using System.Reflection.Metadata;

namespace BookingCare.Data.Seed
{
    public static class SeedSpecialty
    {
        public record SpecialtySeed(
       string Name,
       string Description,
       string ImageUrl
);

        public static async Task SeedAsync(DataContext dbContext)
        {
            var specialties = new List<SpecialtySeed>
{
    new(
        "Cơ xương khớp",
        "Chẩn đoán và điều trị các bệnh lý về xương, khớp, cơ, dây chằng và cột sống.",
        "/images/specialties/KhoaCoXuongKhop.png"
    ),
    new(
        "Thần kinh",
        "Khám và điều trị các bệnh lý liên quan đến hệ thần kinh trung ương và ngoại biên.",
        "/images/specialties/KhoaThanKinh.png"
    ),
    new(
        "Tiêu hóa",
        "Chẩn đoán và điều trị các bệnh lý về dạ dày, ruột, gan, mật và hệ tiêu hóa.",
        "/images/specialties/KhoaTieuHoa.png"
    ),
    new(
        "Tim mạch",
        "Khám và điều trị các bệnh lý liên quan đến tim và hệ tuần hoàn.",
        "/images/specialties/KhoaTimMach.png"
    ),
    new(
        "Tai mũi họng",
        "Khám và điều trị các bệnh lý về tai, mũi, họng và đường hô hấp trên.",
        "/images/specialties/KhoaTaiMuiHong"
    ),
    new(
        "Nhi khoa",
        "Chăm sóc, khám và điều trị các bệnh lý ở trẻ em từ sơ sinh đến tuổi vị thành niên.",
        "/images/specialties/NhiKhoa.png"
    ),
    new(
        "Da liễu",
        "Khám và điều trị các bệnh lý về da, tóc, móng và các bệnh da liễu.",
        "/images/specialties/KhoaDaLieu.png"
    ),
    new(
        "Nội khoa",
        "Khám và điều trị các bệnh lý nội khoa tổng quát mà không cần can thiệp phẫu thuật.",
        "/images/specialties/KhoaNoiKhoa.png"
    ),
    new(
        "Nha khoa",
        "Khám, điều trị và chăm sóc các bệnh lý về răng, hàm và khoang miệng.",
        "/images/specialties/KhoaNhaKhoa.png"
    )
};


            foreach (var item in specialties)
            {
                if (!dbContext.Specialties.Any(s => s.Name == item.Name))
                {
                    await dbContext.Specialties.AddAsync(new Specialty
                    {
                        Name = item.Name,
                        Description = item.Description,
                        AvatarUrl = item.ImageUrl
                    });
                }
            }

            await dbContext.SaveChangesAsync();
        }
    }

}
