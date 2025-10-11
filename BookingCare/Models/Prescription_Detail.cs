using System.ComponentModel.DataAnnotations;

namespace BookingCare.Models
{
    public class Prescription_Detail
    {
        [Key]
        public int Prescription_DetailId { get; set; }
        [Required]
        public int Quantity { get; set; }
        [Required]
        [StringLength(50)]
        public string Dosage { get; set; }
        [Required]
        [StringLength(200)]
        public string Instructions { get; set; }
        //Bảng liên kết giữa Prescription và Medicine
        public int PrescriptionId { get; set; }
        public Prescription Prescription { get; set; }
        public int MedicineId { get; set; }
        public Medicine Medicine { get; set; }
    }
}
