namespace BookingCare.DTOs
{
    // ✅ DTO tổng khi trả kết quả khám
    public class ExamResultRequest
    {
        public string Diagnosis { get; set; }   // Chẩn đoán
        public string? Advice { get; set; }     // Dặn dò bệnh nhân (nếu có)
        public List<PrescriptionItemRequest> Medicines { get; set; }
    }

    // ✅ DTO cho từng thuốc trong đơn
    public class PrescriptionItemRequest
    {
        public int MedicineId { get; set; }     // ID thuốc
        public int Quantity { get; set; }       // Số lượng
        public string Dosage { get; set; }      // Liều dùng
        public string Instructions { get; set; } // Cách dùng
    }
}
