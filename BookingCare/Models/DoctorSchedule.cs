using System.ComponentModel.DataAnnotations;

namespace BookingCare.Models
{
    public class DoctorSchedule
    {
        [Key]
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Shift { get; set; }
        public bool Status { get; set; }
        //Khóa ngoài với bảng Doctor
        public int DoctorId { get; set; }
        public Doctor Doctor { get; set; }
    }
}
