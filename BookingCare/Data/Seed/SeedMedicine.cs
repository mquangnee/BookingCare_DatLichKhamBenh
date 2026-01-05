using BookingCare.Models;
using BookingCare.Repository;

namespace BookingCare.Data.Seed
{
    public static class SeedMedicine
    {
        public static async Task SeedAsync(DataContext dbContext)
        {
            return;
            var medicines = new List<Medicine>();

            string[] units = {"Viên", "Hộp", "Chai", "Lọ", "Ống", "Gói", "Tuýp"};

            string[] functions =
            {
                "Giảm đau, hạ sốt",
                "Kháng sinh điều trị nhiễm khuẩn",
                "Chống viêm, dị ứng",
                "Điều trị bệnh dạ dày",
                "Điều trị tiêu chảy",
                "Hỗ trợ tiêu hóa",
                "Bổ sung vitamin và khoáng chất",
                "Điều trị ho, long đờm",
                "Điều trị cảm cúm",
                "Điều trị huyết áp",
                "Điều trị tiểu đường",
                "Điều trị tim mạch",
                "Tăng cường miễn dịch",
                "An thần, dễ ngủ",
                "Bổ gan, giải độc gan"
            };

            // ===== DANH SÁCH HOẠT CHẤT =====
            string[] drugNames =
            {
                "Paracetamol", "Ibuprofen", "Aspirin", "Diclofenac", "Meloxicam",
                "Amoxicillin", "Cefixime", "Cefuroxime", "Ceftriaxone",
                "Azithromycin", "Clarithromycin", "Erythromycin",
                "Omeprazole", "Esomeprazole", "Pantoprazole", "Lansoprazole",
                "Domperidone", "Metoclopramide", "Loperamide", "Berberine",
                "Vitamin C", "Vitamin B1", "Vitamin B6", "Vitamin B12",
                "Vitamin D3", "Vitamin E", "Calcium D3", "Magnesium B6",
                "Zinc Gluconate", "Iron Ferrous",
                "Salbutamol", "Terbutaline", "Bromhexine", "Acetylcysteine",
                "Cetirizine", "Loratadine", "Fexofenadine",
                "Prednisolone", "Dexamethasone", "Hydrocortisone",
                "Atorvastatin", "Rosuvastatin", "Simvastatin",
                "Amlodipine", "Losartan", "Valsartan", "Bisoprolol",
                "Metformin", "Gliclazide", "Insulin",
                "Probiotic", "Men tiêu hóa", "Smecta", "ORS",
                "Alpha Choay", "Tiffy", "Decolgen", "Panadol",
                "Efferalgan", "Neo-Codion", "Prospan", "Astaxanthin",
                "Glucosamine", "Chondroitin", "Omega 3",
                "Bổ gan Boganic", "Actiso", "Silymarin",
                "An thần Rotunda", "Valerian", "Melatonin",
                "Thuốc nhỏ mắt Tobramycin", "Thuốc nhỏ mũi Otrivin"
            };

            int count = 1;

            while (medicines.Count < 200)
            {
                foreach (var name in drugNames)
                {
                    if (medicines.Count >= 200) break;

                    medicines.Add(new Medicine
                    {
                        Name = $"{name} {count * 10}mg",
                        Unit = units[count % units.Length],
                        Function = functions[count % functions.Length],
                        Status = "Đang sử dụng",
                        CreatedAt = DateTime.Now
                    });

                    count++;
                }
            }

            await dbContext.Medicines.AddRangeAsync(medicines);
            await dbContext.SaveChangesAsync();
        }
    }
}
