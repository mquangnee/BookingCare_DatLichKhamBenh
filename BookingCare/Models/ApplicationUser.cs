using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace BookingCare.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [StringLength(100)]
        public string FullName { get; set; }
        [Required]
        [StringLength(15)]
        public string Gender { get; set; }
        [Required]
        public DateOnly BirthOfDate { get; set; }
        [Required]
        [StringLength(200)]
        public string Address { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
        // Quan hệ 1–1 (tùy chọn): Một User có thể là Doctor
        public Doctor? Doctor { get; set; }

        // Quan hệ 1–1 (tùy chọn): Một User có thể là Patient
        public Patient? Patient { get; set; }
    }
}
