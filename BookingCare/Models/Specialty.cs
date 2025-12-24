using System.ComponentModel.DataAnnotations;

namespace BookingCare.Models
{
    public class Specialty
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(50)]
        public string Name { get; set; }
        [Required]
        [StringLength(200)]
        public string Description { get; set; }
        public string ImageUrl { set; get; } = "/images/specialties/specialty_default.jpg";
        //Quan hệ 1-N với bảng Doctor
        public ICollection<Doctor> Doctors { get; set; }
    }
}
