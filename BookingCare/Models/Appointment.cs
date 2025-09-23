using System.ComponentModel.DataAnnotations;

namespace BookingCare.Models
{
    public class Appointment
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public DateTime AppointmentDate { get; set; }
        public string Status { get; set; }
        public string? Note { get; set; }
        //Quan hệ 1-1 với bảng DoctorAvailableTime
        public int DoctorAvailableTimeId { get; set; }
        public DoctorAvailableTime DoctorAvailableTime { get; set; }
        //Khóa ngoài với bảng Patient
        public int PatientId { get; set; }
        public Patient Patient { get; set; }
        //Khóa ngoài với bảng Doctor
        public int DoctorId { get; set; }
        public Doctor Doctor { get; set; }
    }
}
