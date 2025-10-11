namespace BookingCare.Models
{
    public class Medicine
    {
        public int MedicineId { get; set; }
        public string Name { get; set; }
        public string Unit { get; set; }
        public String Function { get; set; }
        //Quan hệ N-N với bảng Prescription qua bảng Prescription_Detail
        public ICollection<Prescription_Detail> Prescription_Details { get; set; }
    }
}
