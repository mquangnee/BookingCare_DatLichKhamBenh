using System.ComponentModel.DataAnnotations;

namespace BookingCare.Models
{
    public class Medicine
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(150)]
        public string Name { get; set; }
        [Required]
        [StringLength(20)]
        public string Unit { get; set; }
        [Required]
        [StringLength(200)]
        public string Function { get; set; }
        [Required]
        [StringLength(50)]
        public string Status { get; set; } = "Đang sử dụng";
        //Quan hệ N-N với bảng Prescription qua bảng Prescription_Detail
        public ICollection<Prescription_Detail> Prescription_Details { get; set; }
    }
}
