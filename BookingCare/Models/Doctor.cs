using System.ComponentModel.DataAnnotations;

namespace BookingCare.Models
{
    public class Doctor
    {
        [Key]
        public int DoctorId { get; set; }
        [Required]
        [StringLength(20)]
        public string Degree { get; set; }
        [Required]
        public int YearsOfExp { get; set; }
        [Required]
        public bool IsActive { get; set; }
        //Khóa ngoài với bảng Specialty
        public int SpecialtyId { get; set; }
        public Specialty Specialty { get; set; }
        //Khóa ngoài với bảng AspNetUsers
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

    }
}
