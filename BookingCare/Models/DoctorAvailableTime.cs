using System.ComponentModel.DataAnnotations;

namespace BookingCare.Models
{
    public class DoctorAvailableTime
    {
        [Key]
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public bool IsBooked { get; set; }
        //Khóa ngoài với bảng Doctor
        public int DoctorId { get; set; }
        public Doctor Doctor { get; set; }
        //Quan hệ 1-1 với bảng Appointment
        public Appointment? Appointment { get; set; }
    }
}
