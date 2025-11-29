using System.ComponentModel.DataAnnotations;

namespace BookingCare.Models.DTOs
{
    public class PatientBookingDtos
    {
        [Required]
        public int SpecialtyId { get; set; }

        [Required]
        public int DoctorId { get; set; }

        [Required]
        public DateOnly AppointmentDate { get; set; }

        [Required]
        [StringLength(20)]
        public string AppointmentTime { get; set; }

        [Required]
        [StringLength(200)]
        public string ReasonForVisit { get; set; }
    }
}
