using System.ComponentModel.DataAnnotations;

namespace BookingCare.Models.DTOs
{
    public class PatientAccountManagementDtos
    {
        [Required]
        [StringLength(100)]
        public string FullName { get; set; }

        [Required]
        public string Gender { get; set; }

        [Required]
        public DateOnly DateOfBirth { get; set; }

        [Required]
        [StringLength(200)]
        public string Address { get; set; }

        [Required]
        [Phone]
        public string PhoneNumber { get; set; }

        [StringLength(200)]
        public string? MedicalHistory { get; set; }
    }

    public class DoctorAccountManagementDtos
    {
    }
}
