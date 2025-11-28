using System.ComponentModel.DataAnnotations;

namespace BookingCare.Models.DTOs
{
    public class BookingHistoryDtos
    {
        [Required]
        public int AppointmentId { get; set; }

        [Required]
        public DateOnly AppointmentDate { get; set; }

        [Required]
        public string AppointmentTime { get; set; }

        [Required]
        public string ReasonForVisit { get; set; }

        [Required]
        public string Status { get; set; }

        [Required]
        public int DoctorId { get; set; }

        [Required]
        public string DoctorName { get; set; }

        [Required]
        public int RoomId { get; set; }
        
        [Required]
        public string RoomName { get; set; }
    }
}
