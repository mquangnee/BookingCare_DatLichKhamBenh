using System.ComponentModel.DataAnnotations;

namespace BookingCare.Models.DTOs
{
    public class AppointmentScheduleDtos
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
        public int PatientId { get; set; }

        [Required]
        public string PatientName { get; set; }
    }

    public class AppointmentScheduleDetailDtos
    {
        [Required]
        public int AppointmentId { get; set; }
        
        [Required]        
        public string Status { get; set; }

        [Required]
        public DateOnly AppointmentDate { get; set; }

        [Required]
        public string AppointmentTime { get; set; }

        [Required]
        public string ReasonForVisit { get; set; }

        [Required]
        public int PatientId { get; set; }

        [Required]
        public string PatientName { get; set; }

        [Required]
        public DateOnly DateOfBirth { get; set; }

        [Required]
        public string Gender { get; set; }

        [Required]
        public string MedicalHistory { get; set; }
    }
}
