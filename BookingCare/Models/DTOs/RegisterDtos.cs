using System.ComponentModel.DataAnnotations;

namespace BookingCare.Models.DTOs
{
    // DTOs cho bước 1 đăng ký
    public class RegisterStep1Dtos
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        [Compare("Password", ErrorMessage = "Mật khẩu xác nhận không khớp!")]
        public string ConfirmedPassword { get; set; }
    }
    // DTOs cho bước 2 đăng ký
    public class RegisterStep2Dtos
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
            
        [Required]
        public string Otp { get; set; }
    }
    // DTOs cho bước 3 đăng ký
    public class RegisterStep3Dtos
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(50)]
        public string FullName { get; set; }

        [Required]
        public string Gender { get; set; }

        [Required]
        public DateOnly DateOfBirth { get; set; }

        [Required]
        [StringLength(100)]
        public string Address { get; set; }

        [Required]
        [Phone]
        public string PhoneNumber { get; set; }
    }
}
