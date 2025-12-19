using System.ComponentModel.DataAnnotations;

namespace BookingCare.Models.DTOs
{
    public class MedicalReportDtos
    {
        [Required]
        public int AppointmentId { get; set; }

        [Required]
        public string Diagnosis { get; set; }

        [Required]
        public string Instructions { get; set; }

        [Required]
        public List<MedPrescriptionDtos> Medicines { get; set; }
    }
}
