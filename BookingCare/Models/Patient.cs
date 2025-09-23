using System.ComponentModel.DataAnnotations;

namespace BookingCare.Models
{
    public class Patient
    {
        [Key]
        public int PatientId { get; set; }
        public string? MedicalHistory { get; set; }
        //Quan hệ 1-N với bảng Appointment
        public ICollection<Appointment> Appointments { get; set; }
        //Khóa ngoài với bảng AspNetUsers
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
    }
}
