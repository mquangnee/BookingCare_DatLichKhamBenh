using System.ComponentModel.DataAnnotations;

namespace BookingCare.Models
{
    public class Appointment
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public DateOnly AppointmentDate { get; set; }
        public string AppointmentTime { get; set; }
        public string ReasonForVisit { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
        //Khóa ngoài với bảng Patient
        public int PatientId { get; set; }
        public Patient Patient { get; set; }
        //Khóa ngoài với bảng Doctor
        public int DoctorId { get; set; }
        public Doctor Doctor { get; set; }
        //Quan hệ 1-N với bảng Prescription
        public ICollection<Prescription> Prescriptions { get; set; }
    }
}
