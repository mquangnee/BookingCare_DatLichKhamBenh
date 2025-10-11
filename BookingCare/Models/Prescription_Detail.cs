namespace BookingCare.Models
{
    public class Prescription_Detail
    {
        public int Prescription_DetailId { get; set; }
        public int Quantity { get; set; }
        public string Dosage { get; set; }
        public string Instructions { get; set; }
        //Bảng liên kết giữa Prescription và Medicine
        public int PrescriptionId { get; set; }
        public Prescription Prescription { get; set; }
        public int MedicineId { get; set; }
        public Medicine Medicine { get; set; }
    }
}
