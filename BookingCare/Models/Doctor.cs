using System.ComponentModel.DataAnnotations;

namespace BookingCare.Models
{
    public class Doctor
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(50)]
        public string Degree { get; set; }
        [Required]
        public int YearsOfExp { get; set; }
        //Khóa ngoài với bảng Specialty
        public int SpecialtyId { get; set; }
        public Specialty Specialty { get; set; }
        //Khóa ngoài với bảng Room
        public int RoomId { get; set; }
        public Room Room { get; set; }
        //Quan hệ 1-N với bảng Appointment
        public ICollection<Appointment> Appointments { get; set; }
        //Khóa ngoài với bảng AspNetUsers
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

    }
}
