using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace BookingCare.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [StringLength(50)]
        public string FullName { get; set; }
        [Required]
        public string Gender { get; set; }
        [Required]
        public DateOnly BirthOfDate { get; set; }
        [Required]
        [StringLength(100)]
        public string Address { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
    }
}
